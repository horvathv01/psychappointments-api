using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class SessionService : ISessionService
{
    private readonly PsychAppointmentContext _context;
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;
    private readonly ISlotService _slotService;
    
    public SessionService(
        PsychAppointmentContext context,
        IUserService userService,
        ILocationService locationService,
        ISlotService slotService
    )
    {
        _context = context;
        _userService = userService;
        _locationService = locationService;
        _slotService = slotService;
    }
    
    public async Task<bool> AddSession(SessionDTO session)
    {
        if (session.PsychologistId == null || session.LocationId == null)
        {
            Console.WriteLine($"Psychologist id, location id and slot id in a sessionDTO are required for creating a session.");
            return false;
        }
        
        session = SpecifyDateKindForSessionDTO(session);
        try
        {
            var psychologist = (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            var location = await _locationService.GetLocationById((long)session.LocationId);
            
            if (psychologist == null || location == null)
            {
                Console.WriteLine($"Psychologist or location not found when trying to add a session.");
                return false;
            }
            
            //check for overlapping
            if (!await SessionDoesNotOverlap(session, location, psychologist)) return false;
            
            //if slot id is not provided, we create a slot just for this session
            if (session.SlotId == null)
            {
                if (!await AddSlotForOneSession(session, psychologist, location))
                    return false;
            }

            Slot? slot = await GetSlotForSessionAddition(session, psychologist, location);
            
            //if slot is null, something went wrong --> should be handled
            if (slot == null)
            {
                Console.WriteLine($"Slot not found when trying to add a session.");
                return false;
            }

            Psychologist? partnerPsychologist = null;
            if (session.PartnerPsychologistId != null)
            {
                partnerPsychologist = (Psychologist?)await _userService.GetUserById((long)session.PartnerPsychologistId);
            }
            
            int price = session.Price ?? 0;

            SessionFrequency frequency = SessionFrequency.None; 
            Enum.TryParse(session.Frequency, out frequency);
            Client? client = null;
            if (session.ClientId != null)
            {
                client = (Client?)await _userService.GetUserById((long)session.ClientId);
                session.Blank = false;
            }
            else
            {
                session.Blank = true;
            }

            Session newSession = new Session(psychologist, location, session.Date, session.Start, session.End, slot, price, session.Blank,session.Description, frequency, client, partnerPsychologist);
            Console.WriteLine(newSession);
            await _context.Sessions.AddAsync(newSession);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session could not be added.");
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Session?> GetSessionById(long id)
    {
        var result = await _context.Sessions
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .FirstOrDefaultAsync(ses => ses.Id == id);

        
        if (result != null)
        {
            result.Date = TimeZoneConverter.ConvertToCET(result.Date);
            result.Start = TimeZoneConverter.ConvertToCET(result.Start);
            result.End = TimeZoneConverter.ConvertToCET(result.End);
        }
        
        return result;
    }

    public async Task<IEnumerable<Session>> GetAllSessions()
    {
        var result = await _context.Sessions
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<IEnumerable<Session>> GetNonBlankSessions()
    {
        var result = await _context.Sessions
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .Where(ses => !ses.Blank)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Session>> GetListOfSessions(List<long> ids)
    {
        var result = await _context.Sessions
            .Where(ses => ids.Contains(ses.Id))
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<IEnumerable<Session>> GetSessionsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Session> result;
        if (startOfRange == null || endOfRange == null)
        {
            
            result = await _context.Sessions
                .Where(ses => ses.Location.Equals(location))
                .Include(ses => ses.Psychologist)
                .Include(ses => ses.PartnerPsychologist)
                .Include(ses => ses.Location)
                .Include(ses => ses.Slot)
                .Include(ses => ses.Client)
                .ToListAsync();
        }
        
        result = await _context.Sessions
            .Where(ses => ses.Location.Equals(location) && ses.Start >= startOfRange &&
                          ses.End.AddDays(-1) <= endOfRange)
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<IEnumerable<Session>> GetSessionsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Session> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Sessions
                .Where(ses => ses.Psychologist.Equals(psychologist))
                .Include(ses => ses.Psychologist)
                .Include(ses => ses.PartnerPsychologist)
                .Include(ses => ses.Location)
                .Include(ses => ses.Slot)
                .Include(ses => ses.Client)
                .ToListAsync();
        }
        
        result = await _context.Sessions
            .Where(ses => ses.Psychologist.Equals(psychologist) && ses.Start >= startOfRange &&
                          ses.End.AddDays(-1) <= endOfRange)
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }
    
    public async Task<IEnumerable<Session>> GetSessionsByPsychologistLocationAndDates(Psychologist psychologist, Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Session> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Sessions
                .Where(ses => ses.Location.Equals(location) &&  ses.Psychologist.Equals(psychologist))
                .Include(ses => ses.Psychologist)
                .Include(ses => ses.PartnerPsychologist)
                .Include(ses => ses.Location)
                .Include(ses => ses.Slot)
                .Include(ses => ses.Client)
                .ToListAsync();
        }
        
        result = await _context.Sessions
            .Where(ses => ses.Location.Equals(location) &&  ses.Psychologist.Equals(psychologist) && ses.Start >= startOfRange &&
                          ses.End.AddDays(-1) <= endOfRange)
            .Include(ses => ses.Psychologist)
            .Include(ses => ses.PartnerPsychologist)
            .Include(ses => ses.Location)
            .Include(ses => ses.Slot)
            .Include(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Session>> GetSessionsByClient(Client client, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Session> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Sessions
                .Where(ses => ses.Client.Equals(client))
                .ToListAsync();
        }

        result = await _context.Sessions
            .Where(ses => ses.Client.Equals(client) && ses.Start >= startOfRange &&
                          ses.End.AddDays(-1) <= endOfRange)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Session>> GetSessionsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Session> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Sessions
                .Where(ses => ses.Location.Managers.Contains(manager))
                .ToListAsync();
        }

        result = await _context.Sessions
            .Where(ses => ses.Location.Managers.Contains(manager) && ses.Start >= startOfRange &&
                         ses.End.AddDays(-1) <= endOfRange)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Session>> GetSessionsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        var result = await _context.Sessions
            .Where(ses => ses.Date >= startOfRange && ses.Date <= endOfRange)
            .ToListAsync();

        return ConvertToCET(result);
    }

    public async Task<bool> UpdateSession(long id, SessionDTO session)
    {
        try
        {
            session.Id = id;
            //get original session
            var original = await GetSessionById(id);
            //null check for updatable session
            if (original == null)
            {
                Console.WriteLine($"Session {id} not found in DB.");
                return false;
            }
            //check for psychologist ID, location ID and slot ID in DTO (crucial parameters)
            if (!ValidateLocationSlotAndPsychologistInSessionDto(session)) return false;
            
            //specify date kind
            session = SpecifyDateKindForSessionDTO(session);
            
            //get crucial parameters
            Psychologist? psychologist = (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            Location? location = await _locationService.GetLocationById((long)session.LocationId);
            Slot? slot = await _slotService.GetSlotById((long)session.SlotId);

            if (psychologist == null || location == null || slot == null)
            {
                Console.WriteLine($"Psychologist or location or slot not found in DB.");
                return false;
            }
            
            //check for overlapping
            if (!await SessionDoesNotOverlap(session, location, psychologist)) return false;
            
            //non-crucial parameters
            Psychologist? partnerPsychologist = null;
            if (session.PartnerPsychologistId != null)
            { 
             partnerPsychologist = (Psychologist?)await _userService.GetUserById((long)session.PartnerPsychologistId);
            }

            Client? client = null;
            if (session.ClientId != null)
            {
                client = (Client?)await _userService.GetUserById((long)session.ClientId);
                session.Blank = false;
            }
            else
            {
                session.Blank = true;
            }

            int price = session.Price ?? 0;
            SessionFrequency frequency = SessionFrequency.None; 
            Enum.TryParse(session.Frequency, out frequency);

            original.PsychologistId = psychologist.Id;
            original.Psychologist = psychologist;
            original.PartnerPsychologistId = partnerPsychologist == null ? null : partnerPsychologist.Id;
            original.PartnerPsychologist = partnerPsychologist;
            original.Blank = session.Blank;
            original.LocationId = location.Id;
            original.Location = location;
            original.Date = session.Date;
            original.Start = session.Start;
            original.End = session.End;
            original.ClientId = client == null ? null : client.Id;
            original.Client = client;
            original.Price = price;
            original.Frequency = frequency;
            original.SlotId = slot.Id;
            original.Slot = slot;
            original.SlotId = slot.Id;
            original.Description = session.Description;

            _context.Update(original);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> DeleteSession(long id)
    {
        try
        {
            var session = await GetSessionById(id);
            if (session == null) return false;
            //see if slot only has the session we are deleting --> delete slot too   
            var slot = await _slotService.GetSlotById(session.SlotId);
            bool removeSlot = slot != null && slot.Sessions.Count == 1 && slot.Sessions.FirstOrDefault().Id == session.Id;
            
            _context.Remove(session);
            if(slot != null && removeSlot) await _slotService.DeleteSlot(slot.Id);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public bool Overlap(SessionDTO session1, SessionDTO session2)
    {
        if (session1.Date != session2.Date)
        {
            return false;
        }

        List<SessionDTO> sessions = new List<SessionDTO>(){ session1, session2 };  
        sessions.Sort((ses1, ses2) => ses1.Start.CompareTo(ses2.Start));
        
        if (sessions[0].End > sessions[1].Start)
        {
            return true;
        }

        return false;
    }

    private List<Session> ConvertToCET(List<Session> queryResult)
    {
        
        if (queryResult.Count > 0)
        {
            queryResult = queryResult.Select(ses =>
                {
                    ses.Date = TimeZoneConverter.ConvertToCET(ses.Date);
                    ses.Start = TimeZoneConverter.ConvertToCET(ses.Start);
                    ses.End = TimeZoneConverter.ConvertToCET(ses.End);
                    return ses;
                })
                .ToList();
        }
        
        return queryResult;
    }

    private async Task<bool> SessionDoesNotOverlap(SessionDTO session, Location location, Psychologist psychologist)
    {
        
        var sameDaysSessions = await GetSessionsByPsychologistLocationAndDates(psychologist, location, session.Start, session.End);
        
        foreach (var sameDaysSession in sameDaysSessions)
        {
            //if slot overlaps with another, it cannot be added
            if (session.Id != sameDaysSession.Id && Overlap(session, new SessionDTO(sameDaysSession)))
            {
                Console.WriteLine($"Adding/updating session is not possible because it would overlap with session {sameDaysSession.Id}.");
                return false;
            }
        }
        return true;
    }

    private bool ValidateLocationSlotAndPsychologistInSessionDto(SessionDTO session)
    {
        if (session.PsychologistId == null || session.LocationId == null || session.SlotId == null)
        {
            Console.WriteLine($"Psychologist id, location id and slot id in a sessionDTO are required for adding/updating session.");
            return false;
        }

        return true;
    }

    private async Task<bool> AddSlotForOneSession(SessionDTO session, Psychologist psychologist, Location location)
    {
        //create new slot && calculate slot length
        int slotLength = (int)(session.End - session.Start).TotalMinutes;
        var newSlot = new SlotDTO(psychologist, location, session.Date, session.Start, session.End, slotLength, 0, false, new List<Session>());

        //add new slot to DB
        bool addSlotResult = await _slotService.AddSlot(newSlot, false);
        if (!addSlotResult)
        {
            Console.WriteLine($"Session could not be added because slot could not be added.");
            return false;
        }

        return true;
    }

    private async Task<Slot?> GetSlotForSessionAddition(SessionDTO session, Psychologist psychologist, Location location)
    {
        if (session.SlotId == null)
        {
            //gets slot that we just added by start&end
            var slots = await _slotService.GetSlotsByPsychologistLocationAndDates(psychologist,
                location, session.Start, session.End);
            return slots.FirstOrDefault();
        }
        
        return await _slotService.GetSlotById((long)session.SlotId);
    }

    private SessionDTO SpecifyDateKindForSessionDTO(SessionDTO session)
    {
        session.Date = DateTime.SpecifyKind(session.Date, DateTimeKind.Utc);
        session.Start = new DateTime(session.Date.Year, session.Date.Month, session.Date.Day, session.Start.Hour, session.Start.Minute, session.Start.Second);
        session.End = new DateTime(session.Date.Year, session.Date.Month, session.Date.Day, session.End.Hour, session.End.Minute, session.End.Second);
        
        session.Start = DateTime.SpecifyKind(session.Start, DateTimeKind.Utc);
        session.End = DateTime.SpecifyKind(session.End, DateTimeKind.Utc);

        return session;
    }
}