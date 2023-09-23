using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface ILocationService
{
    Task<bool> AddLocation(LocationDTO location);
    
    Task<Location?> GetLocationById(long id);
    Task<IEnumerable<Location>> GetAllLocations();
    Task<List<Location>> GetListOfLocations(List<long> ids);
    
    IEnumerable<Location> GetLocationsByPsychologist(Psychologist psychologist);
    IEnumerable<Location> GetLocationByClient(Client client);
    Task<Location?> GetLocationByAddress(Address address); 
    
    Task<bool> UpdateLocation(long id, Location location);
    Task<bool> UpdateLocation(long id, LocationDTO location);
    Task<bool> DeleteLocation(long id);
}