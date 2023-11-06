using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class SlotService : ISlotService
{
    private readonly PsychAppointmentContext _context;
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;
    
    public SlotService(
    PsychAppointmentContext context,
    IUserService userService,
    ILocationService locationService
        )
    {
        _context = context;
        _userService = userService;
        _locationService = locationService;
    }
    
    
    public async Task<bool> AddSlot(SlotDTO slot, bool prepopulate)
    {
        try
        {
            ValidateLocationAndPsychologistIDsInSlotDto(slot);
            slot = SpecifyDateKindForSessionDto(slot);
        
            var psychologist = (Psychologist?)await _userService.GetUserById(slot.PsychologistId);
            var location = await _locationService.GetLocationById(slot.LocationId);
            if (psychologist == null || location == null)
                throw new InvalidOperationException("Psychologist or location not found when trying to add a slot.");
        
            await SlotDoesNotOverlap(slot, location, psychologist); 
            
            Slot newSlot = new Slot(psychologist, location, slot.Date, slot.SlotStart, slot.SlotEnd, slot.SessionLength, slot.Rest, slot.Weekly, new List<Session>());

            if (prepopulate) await PrepopulateSlotWithBlankSessions(newSlot);    
            
            await _context.Slots.AddAsync(newSlot);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Slot could not be added. See inner exception for details.");
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<Slot?> GetSlotById(long id)
    {
        var result = await _context.Slots
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
                .ThenInclude(ses => ses.Client)
            .FirstOrDefaultAsync(sl => sl.Id == id);

        if (result != null)
        {
            result.Date = TimeZoneConverter.ConvertToCET(result.Date);
            result.SlotStart = TimeZoneConverter.ConvertToCET(result.SlotStart);
            result.SlotEnd = TimeZoneConverter.ConvertToCET(result.SlotEnd);    
        }

        return result;
    }

    public async Task<IEnumerable<Slot>> GetAllSlots()
    {
        var result = await _context.Slots
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();
        return ConvertToCET(result);
    }

    public async Task<List<Slot>> GetListOfSlots(List<long> ids)
    {
        var result = await _context.Slots
            .Where(sl => ids.Contains(sl.Id))
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<IEnumerable<Slot>> GetSlotsByPsychologistAndDates(Psychologist psychologist, DateTime? startOfRange = null,
        DateTime? endOfRange = null)
    {
        List<Slot> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Slots
                .Where(sl => sl.Psychologist.Equals(psychologist))
                .Include(sl => sl.Location)
                .Include(sl => sl.Psychologist)
                .Include(sl => sl.Sessions)
                .ThenInclude(ses => ses.Client)
                .ToListAsync();
        }
        result = await _context.Slots
            .Where(sl => sl.Psychologist.Equals(psychologist) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd.AddDays(-1) <= endOfRange)
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Slot>> GetSlotsByLocationAndDates(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Slot> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Slots
                .Where(sl => sl.Location.Equals(location))
                .Include(sl => sl.Location)
                .Include(sl => sl.Psychologist)
                .Include(sl => sl.Sessions)
                .ThenInclude(ses => ses.Client)
                .ToListAsync();
        }
        result = await _context.Slots
            .Where(sl => sl.Location.Equals(location) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd.AddDays(-1) <= endOfRange)
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }
    
    public async Task<IEnumerable<Slot>> GetSlotsByPsychologistLocationAndDates(Psychologist psychologist, Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Slot> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Slots
                .Where(sl => sl.Location.Equals(location) &&  sl.Psychologist.Equals(psychologist))
                .Include(sl => sl.Location)
                .Include(sl => sl.Psychologist)
                .Include(sl => sl.Sessions)
                .ThenInclude(ses => ses.Client)
                .ToListAsync();
        }
        
        result = await _context.Slots
            .Where(sl => sl.Location.Equals(location) &&  sl.Psychologist.Equals(psychologist) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd.AddDays(-1) <= endOfRange)
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        List<Slot> result;
        if (startOfRange == null || endOfRange == null)
        {
            result = await _context.Slots
                .Where(sl => sl.Location.Managers.Contains(manager))
                .Include(sl => sl.Location)
                .Include(sl => sl.Psychologist)
                .Include(sl => sl.Sessions)
                .ThenInclude(ses => ses.Client)
                .ToListAsync();
        }

        result = await _context.Slots
            .Where(sl => sl.Location.Managers.Contains(manager) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd.AddDays(-1) <= endOfRange)
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();
        
        return ConvertToCET(result);
    }

    public async Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        var result = await _context.Slots
            .Where(sl => sl.Date >= startOfRange && sl.Date <= endOfRange)
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
            .ThenInclude(ses => ses.Client)
            .ToListAsync();

        return ConvertToCET(result);
    }

    public async Task<bool> UpdateSlot(long id, SlotDTO slot)
    {
        try
        {
            slot.Id = id;
            var original = await GetSlotById(id);
            if (original == null)
                throw new InvalidOperationException($"Slot {id} not found in DB.");

            bool hasBlankSessionsOnly = original.Sessions.All(ses => ses.Blank);
            if (!hasBlankSessionsOnly)
                throw new InvalidOperationException("Slot cannot be updated because it has sessions that are not blank. In this case you should add a new one instead of modifying an existing one.");
            
            slot = SpecifyDateKindForSessionDto(slot);
            
            var psychologist = (Psychologist?)await _userService.GetUserById(slot.PsychologistId);
            var location = await _locationService.GetLocationById(slot.LocationId);
            if (psychologist == null || location == null)
                throw new InvalidOperationException("Psychologist or location not found when trying to add a slot.");
            await SlotDoesNotOverlap(slot, location, psychologist); 
            
            original.Psychologist = psychologist;
            original.Location = location;
            original.Date = slot.Date;
            original.SlotStart = slot.SlotStart;
            original.SlotEnd = slot.SlotEnd;
            original.SessionLength = slot.SessionLength;
            original.Rest = slot.Rest;
            original.Weekly = slot.Weekly;
            
            _context.RemoveRange(original.Sessions);

            var newSessions = await PrepopulateSlotWithBlankSessions(original);
            await _context.Sessions.AddRangeAsync(newSessions);
            
            _context.Update(original);
            await _context.SaveChangesAsync();
            
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Slot could not be updated. See inner exception for details.");
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<bool> DeleteSlot(long id)
    {
        try
        {
            var slot = await GetSlotById(id);
            if (slot == null) throw new InvalidOperationException($"Slot with id {id} was not found in DB.");
            _context.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Slot could not be deleted. See inner exception for details.");
            Console.WriteLine(e);
            throw;
        }
    }

    public bool Overlap(SlotDTO slot1, SlotDTO slot2)
    {
        if (slot1.Date != slot2.Date)
        {
            return false;
        }

        List<SlotDTO> slots = new List<SlotDTO>(){ slot1, slot2 };  
        slots.Sort((sl1, sl2) => sl1.SlotStart.CompareTo(sl2.SlotStart));
        
        if (slots[0].SlotEnd > slots[1].SlotStart)
        {
            return true;
        }

        return false;
    }
    
    private async Task<List<Session>> PrepopulateSlotWithBlankSessions(Slot slot)
    {
        List<Session> sessions = new List<Session>();
        double ts = (slot.SlotEnd - slot.SlotStart).TotalMinutes;
        

        if (ts < slot.SessionLength)
        {
            throw new ArgumentException("Slot is too short to fit a session with the provided length.");
        }
        //how many (session + rest) fits in the slot?
        int sessionCount = Convert.ToInt32(Math.Floor(ts / (slot.SessionLength + slot.Rest)));
        //would giving up the last rest result in an extra session? 
        int plusOneSessionTotalLength = (sessionCount + 1) * (slot.SessionLength + slot.Rest) - slot.Rest;
        if (plusOneSessionTotalLength <= ts)
        {
            //plus one session fits
            sessionCount += 1;
        }

        DateTime start = new DateTime(slot.Date.Year, slot.Date.Month, slot.Date.Day, slot.SlotStart.Hour, slot.SlotStart.Minute, slot.SlotStart.Second);
        Console.WriteLine("This is start");
        
        start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        
        //long sessionId = await _context.Sessions.CountAsync() + 1;
        for (int i = 0; i < sessionCount; i++)
        {
            DateTime end = start.AddMinutes(slot.SessionLength);
            end = DateTime.SpecifyKind(end, DateTimeKind.Utc);
            SessionFrequency frequency;
            if (slot.Weekly)
            {
                frequency = SessionFrequency.Weekly;
            }
            else
            {
                frequency = SessionFrequency.None;
            }
            Session ses = new Session(slot.Psychologist, slot.Location, slot.Date, start, end, slot, 0, true, "", frequency, null, null);
            //sessionId += 1;
            //start = end.AddMinutes(slot.SessionLength + slot.Rest);
            sessions.Add(ses);
            start = end.AddMinutes(slot.Rest);
            
        }
        
        slot.Sessions.AddRange(sessions);
        return sessions;
    }
    
    private List<Slot> ConvertToCET(List<Slot> queryResult)
    {
        if (queryResult.Count > 0)
        {
            queryResult = queryResult.Select(slot =>
                {
                    slot.Date = TimeZoneConverter.ConvertToCET(slot.Date);
                    slot.SlotStart = TimeZoneConverter.ConvertToCET(slot.SlotStart);
                    slot.SlotEnd = TimeZoneConverter.ConvertToCET(slot.SlotEnd);
                    return slot;
                })
                .ToList();
        }
        return queryResult;
    }
    
    private async Task<bool> SlotDoesNotOverlap(SlotDTO slot, Location location, Psychologist psychologist)
    {
        
        var sameDaysSlots = await GetSlotsByPsychologistLocationAndDates(psychologist, location, slot.SlotStart, slot.SlotEnd);
        
        foreach (var sameDaysSlot in sameDaysSlots)
        {
            //if slot overlaps with another, it cannot be added
            if (slot.Id != sameDaysSlot.Id && Overlap(slot, new SlotDTO(sameDaysSlot)))
            {
                throw new InvalidOperationException($"Adding/updating slot is not possible because it would overlap with session {sameDaysSlot.Id}.");
            }
        }
        return true;
    }
    
        private void ValidateLocationAndPsychologistIDsInSlotDto(SlotDTO slot)
    {
        if (slot.PsychologistId == null || slot.LocationId == null)
            throw new InvalidOperationException($"Psychologist id and location id a slotDTO are required for adding/updating session.");
    }
    

    private SlotDTO SpecifyDateKindForSessionDto(SlotDTO slot)
    {
        slot.Date = DateTime.SpecifyKind(slot.Date, DateTimeKind.Utc);
        slot.SlotStart = new DateTime(slot.Date.Year, slot.Date.Month, slot.Date.Day, slot.SlotStart.Hour, slot.SlotStart.Minute, slot.SlotStart.Second);
        slot.SlotEnd = new DateTime(slot.Date.Year, slot.Date.Month, slot.Date.Day, slot.SlotEnd.Hour, slot.SlotEnd.Minute, slot.SlotEnd.Second);
        
        slot.SlotStart = DateTime.SpecifyKind(slot.SlotStart, DateTimeKind.Utc);
        slot.SlotEnd = DateTime.SpecifyKind(slot.SlotEnd, DateTimeKind.Utc);

        return slot;
    }
}