using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface ISlotService
{
    Task<bool> AddSlot(Slot slot);
    
    Task<Slot?> GetSlotById(long id);
    Task<List<Slot>> GetAllSlots();
    
    Task<List<Slot>> GetListOfSlots(List<long> ids);
    
    List<Slot> GetSlotsByPsychologistAndDates(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByLocationAndDates(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange);
    
    Task<bool> UpdateSlot(long id, Slot slot);
    Task<bool> UpdateSlot(long id, SlotDTO slot);
    Task<bool> DeleteSlot(long id);
}