using PsychAppointments_API.Auth;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class LocationService : ILocationService
{
    //private readonly DbContext _context;
    private readonly IRepository<Location> _locationRepository;
    //private readonly IPsychologistService _psychologistService;
    //private readonly IClientService _clientService;
    //private readonly ISessionService _sessionService;
    //private readonly ISlotService _slotService;
    //private readonly IAccessUtilities _hasher;
    
    public LocationService(
        //DbContext context
        IRepository<Location> locationRepository 
        //IAccessUtilities hasher, 
        //IPsychologistService psychologistService, 
        //IClientService clientService, 
        //ISessionService sessionService, 
        //ISlotService slotService
    )
    {
        //_context = context;
        _locationRepository = locationRepository;
        //_hasher = hasher;

        //_psychologistService = psychologistService;
        //_clientService = clientService;
        //_sessionService = sessionService;
        //_slotService = slotService;
    }
    
    
    public async Task<bool> AddLocation(Location location)
    {
        return await _locationRepository.Add(location);
    }

    public async Task<Location?> GetLocationById(long id)
    {
        return await _locationRepository.GetById(id);
    }

    public async Task<List<Location>> GetAllLocations()
    {
        var allLocations = await _locationRepository.GetAll();
        return allLocations.ToList();
    }

    public async Task<List<Location>> GetListOfLocations(List<long> ids)
    {
        var result = await _locationRepository.GetList(ids);
        return result.ToList();
    }

    public List<Location> GetLocationsByPsychologist(Psychologist psychologist)
    {
        return psychologist.Sessions.Select(ses => ses.Location).Distinct().ToList();
    }

    public List<Location> GetLocationByClient(Client client)
    {
        return client.Sessions.Select(ses => ses.Location).Distinct().ToList();
    }

    public async Task<Location?> GetLocationByAddress(Address address)
    {
        var allLocations = await _locationRepository.GetAll();
        return allLocations.FirstOrDefault(loc => loc.Address.Equals(address));
    }

    public async Task<bool> UpdateLocation(long id, Location location)
    {
        return await _locationRepository.Update(id, location);
    }

    public async Task<bool> DeleteLocation(long id)
    {
        return await _locationRepository.Delete(id);
    }
}