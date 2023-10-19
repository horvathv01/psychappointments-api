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
        long id = await _context.Slots.CountAsync() + 1;
        slot.Id = id;

        //find slots from same day, location and psychologist to avoid overlaps
        var sameDaysSlots = await _context.Slots
            .Where(sl => sl.Date == slot.Date && sl.Location.Id == slot.LocationId && sl.Psychologist.Id == slot.PsychologistId)
            .ToListAsync();
        
        foreach (var sameDaysSlot in sameDaysSlots)
        {
            //if slot overlaps with another, it cannot be added
            if (Overlap(slot, new SlotDTO(sameDaysSlot))) return false;
        }

        try
        {
            var psychologist = (Psychologist?)await _userService.GetUserById(slot.PsychologistId);
            var location = await _locationService.GetLocationById(slot.LocationId);
            if (psychologist == null || location == null)
            {
                return false;
            }
            slot.Date = DateTime.SpecifyKind(slot.Date, DateTimeKind.Utc);
            slot.SlotStart = DateTime.SpecifyKind(slot.SlotStart, DateTimeKind.Utc);
            slot.SlotEnd = DateTime.SpecifyKind(slot.SlotEnd, DateTimeKind.Utc);
            Slot newSlot = new Slot(psychologist, location, slot.Date, slot.SlotStart, slot.SlotEnd, slot.SessionLength, slot.Rest, slot.Weekly, new List<Session>(), id);
            
            //new slots are prepopulated with empty sessions. 
            //newSlot.Sessions = await PrepopulateSlotWithBlankSessions(newSlot);

            if (prepopulate)
            {
                await PrepopulateSlotWithBlankSessions(newSlot);    
            }
            await _context.Slots.AddAsync(newSlot);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Slot could not be added.");
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Slot?> GetSlotById(long id)
    {
        return await _context.Slots
            .Include(sl => sl.Location)
            .Include(sl => sl.Psychologist)
            .Include(sl => sl.Sessions)
                .ThenInclude(ses => ses.Client)
            .FirstOrDefaultAsync(sl => sl.Id == id);
    }

    public async Task<IEnumerable<Slot>> GetAllSlots()
    {
        return await _context.Slots.ToListAsync();
    }

    public async Task<List<Slot>> GetListOfSlots(List<long> ids)
    {
        return await _context.Slots
            .Where(sl => ids.Contains(sl.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Slot>> GetSlotsByPsychologistAndDates(Psychologist psychologist, DateTime? startOfRange = null,
        DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Slots
                .Where(sl => sl.Psychologist.Equals(psychologist))
                .ToListAsync();
        }
        return await _context.Slots
            .Where(sl => sl.Psychologist.Equals(psychologist) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd <= endOfRange)
            .ToListAsync();
    }

    public async Task<List<Slot>> GetSlotsByLocationAndDates(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Slots
                .Where(sl => sl.Location.Equals(location))
                .ToListAsync();
        }
        return await _context.Slots
            .Where(sl => sl.Location.Equals(location) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd <= endOfRange)
            .ToListAsync();
        
    }
    
    public async Task<IEnumerable<Slot>> GetSlotsByPsychologistLocationAndDates(Psychologist psychologist, Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Slots
                .Where(sl => sl.Location.Equals(location) &&  sl.Psychologist.Equals(psychologist))
                .Include(sl => sl.Sessions)
                .ToListAsync();
        }
        
        return await _context.Slots
            .Where(sl => sl.Location.Equals(location) &&  sl.Psychologist.Equals(psychologist) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd <= endOfRange)
            .Include(sl => sl.Sessions)
            .ToListAsync();
        
    }

    public async Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        if (startOfRange == null || endOfRange == null)
        {
            return await _context.Slots
                .Where(sl => sl.Location.Managers.Contains(manager))
                .ToListAsync();
        }

        return await _context.Slots
            .Where(sl => sl.Location.Managers.Contains(manager) && sl.SlotStart >= startOfRange &&
                         sl.SlotEnd <= endOfRange)
            .ToListAsync();
    }

    public async Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        return await _context.Slots
            .Where(sl => sl.Date >= startOfRange && sl.Date <= endOfRange)
            .ToListAsync();
    }

    public async Task<bool> UpdateSlot(long id, SlotDTO slot)
    {
        try
        {
            var original = await GetSlotById(id);
            if (original == null)
            {
                Console.WriteLine($"Slot {id} not found in DB.");
                return false;
            }

            bool hasBlankSessionsOnly = original.Sessions.All(ses => ses.Blank);

            if (!hasBlankSessionsOnly)
            {
                Console.WriteLine("Slot cannot be updated because it has sessions that are not blank. In this case you should add a new one instead of modifying an existing one.");
                return false;
            }
            
            var slotDate = DateTime.SpecifyKind(slot.Date, DateTimeKind.Utc);
            
            //find slots from same day, location and psychologist to avoid overlaps
            var sameDaysSlots = await _context.Slots
                .Where(sl => sl.Date == slotDate && sl.Location.Id == slot.LocationId && sl.Psychologist.Id == slot.PsychologistId)
                .ToListAsync();
        
            foreach (var sameDaysSlot in sameDaysSlots)
            {
                //if slot overlaps with another, it cannot be added
                if (Overlap(slot, new SlotDTO(sameDaysSlot)))
                {
                    Console.WriteLine($"Updating slot {id} is not possible because it would overlap with slot {sameDaysSlot.Id}.");
                    return false;
                }
            }
            
            Psychologist? psychologist = (Psychologist?)await _userService.GetUserById(slot.PsychologistId);
            Location? location = await _locationService.GetLocationById(slot.LocationId);
            
            if (psychologist == null || location == null)
            {
                Console.WriteLine($"Psychologist and/or location in slot cannot be null.");
                return false;
            }
            
            List<Session> sessions = slot.SessionIds == null
                ? new List<Session>()
                : await _context.Sessions.Where(ses => slot.SessionIds.Contains(ses.Id)).ToListAsync();
            
            original.Psychologist = psychologist;
            original.Location = location;
            original.Date = DateTime.SpecifyKind(slot.Date, DateTimeKind.Utc);
            original.SlotStart = DateTime.SpecifyKind(slot.SlotStart, DateTimeKind.Utc);
            original.SlotEnd = DateTime.SpecifyKind(slot.SlotEnd, DateTimeKind.Utc);
            original.SessionLength = slot.SessionLength;
            original.Rest = slot.Rest;
            original.Weekly = slot.Weekly;
            //original.Sessions = await PrepopulateSlotWithBlankSessions(original);
            
            _context.RemoveRange(original.Sessions);

            var newSessions = await PrepopulateSlotWithBlankSessions(original);
            await _context.Sessions.AddRangeAsync(newSessions);
            
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

    public async Task<bool> DeleteSlot(long id)
    {
        try
        {
            var slot = await GetSlotById(id);
            if (slot == null) return false;
            _context.Remove(slot);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
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
}