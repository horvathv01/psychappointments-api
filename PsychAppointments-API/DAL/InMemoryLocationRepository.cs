using PsychAppointments_API.Models;

namespace PsychAppointments_API.DAL;

public class InMemoryLocationRepository : IRepository<Location>
{
    private readonly List<Location> _locations;
    
    public InMemoryLocationRepository()
    {
        _locations = new List<Location>();
        //Prepopulate();
    }

    private void Prepopulate()
    {
        //prepopulate with data
        Console.WriteLine("InMemoryLocationRepository has been prepopulated.");
    }
    
    public async Task<Location> GetById(long id)
    {
        var location = _locations.FirstOrDefault(loc => loc.Id == id);
        return await Task.FromResult(location);
    }

    public async Task<Location> GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Location>> GetAll()
    {
        return await Task.FromResult<IEnumerable<Location>>(_locations);
    }
    
    public async Task<IEnumerable<Location>> GetList(List<long> ids)
    {
        return await Task.FromResult<IEnumerable<Location>>(_locations.Where(us => ids.Contains(us.Id)));
    }

    public async Task<bool> Add(Location entity)
    {
        try
        {
            _locations.Add(entity);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> Update(long id, Location entity)
    {
        try
        {
            var location = _locations.FirstOrDefault(loc => loc.Id == id);
            location.Name = entity.Name;
            location.Address = entity.Address;
            location.Managers = entity.Managers;
            location.Psychologists = entity.Psychologists;
        
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
            var location = _locations.FirstOrDefault(loc => loc.Id == id);
            if (location != null)
            {
                _locations.Remove(location);
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