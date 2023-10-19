using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface ISlotService
{
    Task<bool> AddSlot(SlotDTO slot, bool prepopulate);
    
    Task<Slot?> GetSlotById(long id);
    Task<IEnumerable<Slot>> GetAllSlots();
    
    Task<List<Slot>> GetListOfSlots(List<long> ids);
    
    Task<IEnumerable<Slot>> GetSlotsByPsychologistAndDates(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByLocationAndDates(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null);

    Task<IEnumerable<Slot>> GetSlotsByPsychologistLocationAndDates(Psychologist psychologist, Location location,
        DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange);
    Task<bool> UpdateSlot(long id, SlotDTO slot);
    Task<bool> DeleteSlot(long id);

    bool Overlap(SlotDTO slot1, SlotDTO slot2);
}