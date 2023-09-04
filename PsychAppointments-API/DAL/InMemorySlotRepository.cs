using PsychAppointments_API.Models;

namespace PsychAppointments_API.DAL;

public class InMemorySlotRepository : IRepository<Slot>
{
       private readonly List<Slot> _slots;
    
    public InMemorySlotRepository()
    {
        _slots = new List<Slot>();
        //Prepopulate();
    }

    private void Prepopulate()
    {
        //prepopulate with data
        Console.WriteLine("InMemorySlotRepository has been prepopulated.");
    }
    
    public async Task<Slot> GetById(long id)
    {
        var slot = _slots.FirstOrDefault(slot => slot.Id == id);
        return await Task.FromResult(slot);
    }

    public async Task<Slot> GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Slot>> GetAll()
    {
        return await Task.FromResult<IEnumerable<Slot>>(_slots);
    }

    public async Task<bool> Add(Slot entity)
    {
        try
        {
            _slots.Add(entity);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> Update(long id, Slot entity)
    {
        try
        {
            var slot = _slots.FirstOrDefault(slot => slot.Id == id);
            slot.Psychologist = entity.Psychologist;
            slot.Location = entity.Location;
            slot.Date = entity.Date;
            slot.SlotStart = entity.SlotStart;
            slot.SlotEnd = entity.SlotEnd;
            slot.SessionLength = entity.SessionLength;
            slot.Rest = entity.Rest;
            slot.Weekly = entity.Weekly;
            slot.Sessions = entity.Sessions; 
        
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> Delete(long id)
    {
        try
        {
            var slot = _slots.FirstOrDefault(slot => slot.Id == id);
            if (slot != null)
            {
                _slots.Remove(slot);
                return await Task.FromResult(true);    
            }
            return await Task.FromResult(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }
}