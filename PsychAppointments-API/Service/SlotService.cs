using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class SlotService : ISlotService
{
    private readonly DbContext _context;
    
    public SlotService(DbContext context)
    {
        _context = context;
    }
    
    
    public Task<bool> AddSlot(Slot slot)
    {
        throw new NotImplementedException();
    }

    public Task<Slot> GetSlotById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Slot>> GetAllSessions()
    {
        throw new NotImplementedException();
    }

    public Task<List<Slot>> GetSlotsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Slot>> GetSlotsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Slot>> GetSlotsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Slot>> GetSlotsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSlot(long id, Slot session)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSlot(long id)
    {
        throw new NotImplementedException();
    }
}