using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface ISlotService
{
    Task<bool> AddSlot(Slot slot);
    
    Task<Slot> GetSlotById(long id);
    Task<List<Slot>> GetAllSessions();
    Task<List<Slot>> GetSlotsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange);
    
    Task<bool> UpdateSlot(long id, Slot session);
    Task<bool> DeleteSlot(long id);
}