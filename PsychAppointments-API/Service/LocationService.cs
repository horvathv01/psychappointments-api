using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class LocationService : ILocationService
{
    
    private readonly PsychAppointmentContext _context;
    
    public LocationService(
    PsychAppointmentContext context
    )
    {
        _context = context;
    }
    
    
    public async Task<bool> AddLocation(LocationDTO location)
    {
        try
        {
            location.Id = await _context.Locations.CountAsync();
            var newLocation = new Location(location.Name, location.Address, new List<Manager>(),
                new List<Psychologist>(), location.Id);
            if (location.ManagerIds.Count > 0)
            {
                var managers = await _context.Managers.Where(man => location.ManagerIds.Contains(man.Id)).ToListAsync();
                newLocation.Managers = managers;
            }

            if (location.PsychologistIds.Count > 0)
            {
                var psychologists = await _context.Psychologists.Where(psy => location.PsychologistIds.Contains(psy.Id)).ToListAsync();
                newLocation.Psychologists = psychologists;
            }
            
            await _context.Locations.AddAsync(newLocation);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Location?> GetLocationById(long id)
    {
        return await _context.Locations
            .Include(loc => loc.Psychologists)
            .Include(loc => loc.Managers)
            .FirstOrDefaultAsync(loc => loc.Id == id);
    }

    public async Task<IEnumerable<Location>> GetAllLocations()
    {
        return await _context.Locations.ToListAsync();
    }

    public async Task<List<Location>> GetListOfLocations(List<long> ids)
    {
        return await _context.Locations
            .Where(loc => ids.Contains(loc.Id))
            .Include(loc => loc.Psychologists)
            .Include(loc => loc.Managers)
            .ToListAsync();
    }

    public IEnumerable<Location> GetLocationsByPsychologist(Psychologist psychologist)
    {
        return psychologist.Sessions.Select(ses => ses.Location).Distinct().ToList();
    }

    public IEnumerable<Location> GetLocationByClient(Client client)
    {
        return client.Sessions.Select(ses => ses.Location).Distinct().ToList();
    }

    public async Task<Location?> GetLocationByAddress(Address address)
    {
        return await _context.Locations
            .Include(loc => loc.Psychologists)
            .Include(loc => loc.Managers)
            .FirstOrDefaultAsync(loc => loc.Address.Equals(address));
    }

    public async Task<bool> UpdateLocation(long id, Location location)
    {
    
    var originalLocation = await GetLocationById(id);
    if (originalLocation == null)
    {
        return false;
    }
    originalLocation.Name = location.Name;
    originalLocation.Address = location.Address;
    originalLocation.Managers = location.Managers;
    originalLocation.Psychologists = location.Psychologists;
    
    _context.Update(originalLocation);
    await _context.SaveChangesAsync();
    return true;
    }

    public async Task<bool> UpdateLocation(long id, LocationDTO location)
    {
        var originalLocation = await GetLocationById(id);
        if (originalLocation == null)
        {
            return false;
        }
        originalLocation.Name = location.Name;
        originalLocation.Address = location.Address;
        originalLocation.Managers = await _context.Managers
            .Where(man => location.ManagerIds.Contains(man.Id))
            .ToListAsync();
        originalLocation.Psychologists = await _context.Psychologists
            .Where(psy => location.PsychologistIds.Contains(psy.Id))
            .ToListAsync();
    
        _context.Update(originalLocation);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteLocation(long id)
    {
        try
        {
            var location = await GetLocationById(id);
            if (location != null)
            {
                _context.Remove(location);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}