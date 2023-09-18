using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Auth;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class ClientService : IClientService
{
    private readonly PsychAppointmentContext _context;
    private readonly IUserService _userService;
    private readonly IAccessUtilities _hasher;
    private readonly IPsychologistService _psychologistService;
    private readonly ISessionService _sessionService;
    
    public ClientService(
    PsychAppointmentContext context,
    IUserService userService,
    IPsychologistService psychologistService,
    ISessionService sessionService,
    IAccessUtilities hasher
    )
    {
        _context = context;
        _userService = userService;
        _psychologistService = psychologistService;
        _sessionService = sessionService;
        _hasher = hasher;
    }
    
    
    public async Task<bool> AddClient(Client client)
    {
        //id exists?
        var alreadyExisting = await _userService.GetUserById(client.Id);
        if (alreadyExisting != null && client.Email == alreadyExisting.Email)
        { 
            Console.WriteLine($"Client {client.Name} is already present in the DB.");
            return false;
        }
        try
        {
            await _userService.AddUser(new UserDTO(client));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Client?> GetClientById(long id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            if (user != null)
            {
                return (Client)user;    
            }
            return null;
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
            var user = await _userService.GetUserByEmail(email);
            if (user != null)
            {
                return (Client)user;    
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<Client>> GetAllClients()
    {
        return await _context.Clients.ToListAsync();
    }

    public async Task<List<Client>> GetListOfClients(List<long> ids)
    {
        return await _context.Clients
            .Where(cli => ids.Contains(cli.Id))
            .ToListAsync();
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
        return await _userService.UpdateUser(id, new UserDTO(client));
    }

    public async Task<bool> UpdateClient(long id, UserDTO client)
    {
        if (client.Type != Enum.GetName(typeof(UserType), UserType.Client))
        {
            return false;
        }

        try
        {
            var original = await _userService.GetUserById(client.Id);
            if (original == null)
            {
                Console.WriteLine($"Client {client.Name} not found in DB.");
                return false;
            }

            DateTime birthDay = DateTime.MinValue;
            DateTime.TryParse(client.DateOfBirth, out birthDay);
            string password = _hasher.HashPassword(client.Password, client.Email);

            List<Session> sessions = client.SessionIds == null
                ? new List<Session>()
                : await _sessionService.GetListOfSessions(client.SessionIds);

            List<Psychologist> psychologists = client.PsychologistIds == null
                ? new List<Psychologist>()
                : await _psychologistService.GetListOfPsychologists(client.PsychologistIds);
            
            
            original.Name = client.Name;
            original.Email = client.Email;
            original.Phone = original.Phone;
            original.DateOfBirth = DateTime.SpecifyKind(birthDay, DateTimeKind.Utc);
            original.Address = client.Address;
            original.Password = password;
            ((Client)original).Sessions = sessions;
            ((Client)original).Psychologists = psychologists;
            _context.Update(original);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}