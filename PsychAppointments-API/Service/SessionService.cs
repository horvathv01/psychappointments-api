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
        try
        {
            ValidateLocationAndPsychologistIDsInSessionDto(session);
            session = SpecifyDateKindForSessionDto(session);
            
            var psychologist = (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            var location = await _locationService.GetLocationById((long)session.LocationId);
            if (psychologist == null || location == null)
                throw new InvalidOperationException("Psychologist or location not found when trying to add a session.");
            
            await SessionDoesNotOverlap(session, location, psychologist); 
            
            Client? client = await GetClientForSession(session);
            session.Blank = client == null;
            
            Psychologist? partnerPsychologist = null;
            if (session.PartnerPsychologistId != null)
            {
                partnerPsychologist = (Psychologist?)await _userService.GetUserById((long)session.PartnerPsychologistId);
                if (partnerPsychologist == null) throw new InvalidOperationException("Partner psychologist with provided ID was not found.");
            }
            
            //if slot id is not provided, a new slot is created just for this session
            if (session.SlotId == null) await AddSlotForOneSession(session, psychologist, location);
            Slot? slot = await GetSlotForSessionAddition(session, psychologist, location);
            if (slot == null) throw new InvalidOperationException("Slot not found when trying to add a session.");
            
            int price = session.Price ?? 0;
            SessionFrequency frequency = SessionFrequency.None; 
            Enum.TryParse(session.Frequency, out frequency);
            
            Session newSession = new Session(psychologist, location, session.Date, session.Start, session.End, slot, price, session.Blank,session.Description, frequency, client, partnerPsychologist);
            await _context.Sessions.AddAsync(newSession);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Session could not be added. See the inner exception for details.");
            Console.WriteLine(e);
            throw;
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
            var original = await GetSessionById(id);
            if (original == null) 
                throw new InvalidOperationException($"Session {id} not found in DB.");
            ValidateLocationAndPsychologistIDsInSessionDto(session);
            if (session.SlotId == null)
                throw new InvalidOperationException($"Slot id a sessionDTO is required for adding/updating session.");
            
            session = SpecifyDateKindForSessionDto(session);
            
            Psychologist? psychologist = (Psychologist?)await _userService.GetUserById((long)session.PsychologistId);
            Location? location = await _locationService.GetLocationById((long)session.LocationId);
            Slot? slot = await _slotService.GetSlotById((long)session.SlotId);

            if (psychologist == null || location == null || slot == null) 
                throw new InvalidOperationException($"Psychologist or location or slot not found in DB.");
            
            //check for overlapping
            if (!await SessionDoesNotOverlap(session, location, psychologist)) return false;
            
            //non-crucial parameters
            Client? client = await GetClientForSession(session);
            session.Blank = client == null;
            
            Psychologist? partnerPsychologist = null;
            if (session.PartnerPsychologistId != null)
            {
                partnerPsychologist = (Psychologist?)await _userService.GetUserById((long)session.PartnerPsychologistId);
                if (partnerPsychologist == null) throw new InvalidOperationException("Partner psychologist with provided ID was not found.");
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
            Console.WriteLine($"Session could not be updated. See the inner exception for details.");
            Console.WriteLine(e);
            throw;
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
                throw new InvalidOperationException($"Adding/updating session is not possible because it would overlap with session {sameDaysSession.Id}.");
            }
        }
        return true;
    }

    private void ValidateLocationAndPsychologistIDsInSessionDto(SessionDTO session)
    {
        if (session.PsychologistId == null || session.LocationId == null)
            throw new InvalidOperationException($"Psychologist id and location id a sessionDTO are required for adding/updating session.");
    }

    private async Task AddSlotForOneSession(SessionDTO session, Psychologist psychologist, Location location)
    {
        //create new slot && calculate slot length
        int slotLength = (int)(session.End - session.Start).TotalMinutes;
        var newSlot = new SlotDTO(psychologist, location, session.Date, session.Start, session.End, slotLength, 0, false, new List<Session>());

        //add new slot to DB
        bool addSlotResult = await _slotService.AddSlot(newSlot, false);
        if (!addSlotResult)
            throw new InvalidOperationException($"Session could not be added because slot could not be added.");
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

    private SessionDTO SpecifyDateKindForSessionDto(SessionDTO session)
    {
        session.Date = DateTime.SpecifyKind(session.Date, DateTimeKind.Utc);
        session.Start = new DateTime(session.Date.Year, session.Date.Month, session.Date.Day, session.Start.Hour, session.Start.Minute, session.Start.Second);
        session.End = new DateTime(session.Date.Year, session.Date.Month, session.Date.Day, session.End.Hour, session.End.Minute, session.End.Second);
        
        session.Start = DateTime.SpecifyKind(session.Start, DateTimeKind.Utc);
        session.End = DateTime.SpecifyKind(session.End, DateTimeKind.Utc);

        return session;
    }

    private async Task<Client?> GetClientForSession(SessionDTO session)
    {
        if (session.ClientId != null)
        {
            var client = (Client?)await _userService.GetUserById((long)session.ClientId);
            if (client == null) throw new InvalidOperationException("Client with provided ClientID was not found.");
            return client;
        }
        return null;
    }
}