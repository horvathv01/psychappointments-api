using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class ClientService : IClientService
{
    private readonly IRepository<User> _userRepository;
    
    public ClientService(
        //DbContext context
        IRepository<User> userRepository
    )
    {
        //_context = context;
        _userRepository = userRepository;
    }
    
    
    public async Task<bool> AddClient(Client client)
    {
        return await _userRepository.Add(client);
    }

    public async Task<Client?> GetClientById(long id)
    {
        try
        {
            var user = await _userRepository.GetById(id);
            return (Client)user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Client?> GetClientByEmail(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmail(email);
            return (Client)user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<Client>> GetAllClients()
    {
        var allUsers = await _userRepository.GetAll();
        return allUsers.Where(us => us.Type == UserType.Client).Select(us => (Client)us).ToList();
    }

    public async Task<List<Client>> GetListOfClients(List<long> ids)
    {
        var allUsers = await _userRepository.GetList(ids);
        return allUsers.Select(us => (Client)us).ToList();
    }

    public List<Client?> GetClientsByLocation(Location location)
    {
        var result = location.Psychologists.SelectMany(psy => psy.Sessions)
            .Where(ses => ses.Location.Equals(location))
            .Select(ses => ses.Client)
            .ToList();

        return result;
    }

    public List<Client?> GetClientsBySlot(Slot slot)
    {
        return slot.Sessions.Select(ses => ses.Client).ToList();
    }

    public async Task<bool> UpdateClient(long id, Client client)
    {
        return await _userRepository.Update(id, client);
    }
}