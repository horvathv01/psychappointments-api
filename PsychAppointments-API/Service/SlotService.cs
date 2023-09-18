using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class SlotService : ISlotService
{
    //private readonly DbContext _context;
    private readonly IRepository<Slot> _slotRepository;
    
    public SlotService(
        //DbContext context, 
        IRepository<Slot> slotRepository
        )
    {
        //_context = context;
        _slotRepository = slotRepository;
    }
    
    
    public async Task<bool> AddSlot(Slot slot)
    {
        return await _slotRepository.Add(slot);
    }

    public async Task<Slot?> GetSlotById(long id)
    {
        return await _slotRepository.GetById(id);
    }

    public async Task<List<Slot>> GetAllSlots()
    {
        var allSlots = await _slotRepository.GetAll();
        return allSlots.ToList();
    }

    public async Task<List<Slot>> GetListOfSlots(List<long> ids)
    {
        var slots = await _slotRepository.GetList(ids);
        return slots.ToList();
    }

    public List<Slot> GetSlotsByPsychologistAndDates(Psychologist psychologist, DateTime? startOfRange = null,
        DateTime? endOfRange = null)
    {
        return psychologist.Slots.Where(slot =>
                slot.SlotStart >= startOfRange
                && slot.SlotEnd <= endOfRange
                )
            .ToList();
    }

    public async Task<List<Slot>> GetSlotsByLocationAndDates(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        var allSlots = await _slotRepository.GetAll();
        var locationSlots = allSlots.Where(slot => slot.Location.Equals(location)).ToList(); 
        if (startOfRange == null || endOfRange == null)
        {
            return locationSlots;
        }
        return locationSlots.Where(slot => 
                slot.SlotStart >= startOfRange
                && slot.SlotEnd <= endOfRange
            ).ToList();
    }

    public async Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        var allSlots = await _slotRepository.GetAll();
        var managersSlots = allSlots.Where(slot => manager.Locations.Contains(slot.Location)).ToList();
        if (startOfRange == null || endOfRange == null)
        {
            return managersSlots;
        }
        return managersSlots.Where(slot => 
                slot.SlotStart >= startOfRange
                && slot.SlotEnd <= endOfRange
            ).ToList();
    }

    public async Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        var allSlots = await _slotRepository.GetAll();
        return allSlots.Where(slot =>
                slot.SlotStart >= startOfRange
                && slot.SlotEnd <= endOfRange
            ).ToList();
    }

    public async Task<bool> UpdateSlot(long id, Slot slot)
    {
        try
        {
            var oldSlot = await _slotRepository.GetById(id);
            oldSlot.Psychologist = slot.Psychologist;
            oldSlot.Location = slot.Location;
            oldSlot.Date = slot.Date;
            oldSlot.SlotStart = slot.SlotStart;
            oldSlot.SlotEnd = slot.SlotEnd;
            oldSlot.SessionLength = slot.SessionLength;
            oldSlot.Rest = slot.Rest;
            oldSlot.Weekly = slot.Weekly;
            oldSlot.Sessions = slot.Sessions;
            
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public Task<bool> UpdateSlot(long id, SlotDTO slot)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteSlot(long id)
    {
        return await _slotRepository.Delete(id);
    }
}