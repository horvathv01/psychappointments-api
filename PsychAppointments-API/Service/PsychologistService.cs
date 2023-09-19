using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Auth;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class PsychologistService : IPsychologistService
{
    private readonly PsychAppointmentContext _context;
    private readonly IUserService _userService;
    private readonly IClientService _clientService;
    private readonly ISessionService _sessionService;
    private readonly ISlotService _slotService;
    private readonly IAccessUtilities _hasher;
    
    public PsychologistService(
    PsychAppointmentContext context,
    IUserService userService,
    IClientService clientService,
    ISessionService sessionService,
    ISlotService slotService,
    IAccessUtilities hasher
    )
    {
        _context = context;
        _userService = userService;
        _clientService = clientService;
        _sessionService = sessionService;
        _slotService = slotService;
        _hasher = hasher;
    }
    public async Task<bool> AddPsychologist(Psychologist psychologist)
    {
        var alreadyExisting = await _userService.GetUserById(psychologist.Id);
        if (alreadyExisting != null && psychologist.Email == alreadyExisting.Email)
        {
            Console.WriteLine($"Psychologist {psychologist.Name} is already present in the DB.");
            return false;
        }
        try
        {
            await _userService.AddUser(new UserDTO(psychologist));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Psychologist?> GetPsychologistById(long id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            if (user != null)
            {
                return (Psychologist)user;    
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Psychologist?> GetPsychologistByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmail(email);
            if (user != null)
            {
                return (Psychologist)user;    
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<Psychologist>> GetAllPsychologists()
    {
        return await _context.Psychologists.ToListAsync();
    }

    public async Task<List<Psychologist>> GetListOfPsychologists(List<long> ids)
    {
        return await _context.Psychologists
            .Where(man => ids.Contains(man.Id))
            .ToListAsync();
    }

    public async Task<bool> UpdatePsychologist(long id, Psychologist psychologist)
    {
        return await _userService.UpdateUser(id, new UserDTO(psychologist));
    }

    public async Task<bool> UpdatePsychologist(long id, UserDTO psychologist)
    {
        if (psychologist.Type != Enum.GetName(typeof(UserType), UserType.Psychologist))
        {
            return false;
        }

        try
        {
            var original = await _userService.GetUserById(psychologist.Id);
            if (original == null)
            {
                Console.WriteLine($"Psychologist {psychologist.Name} not found in DB.");
                return false;
            }

            DateTime birthDay = DateTime.MinValue;
            DateTime.TryParse(psychologist.DateOfBirth, out birthDay);
            string password = _hasher.HashPassword(psychologist.Password, psychologist.Email);
            //clients
            List<Client> clients = psychologist.ClientIds == null
                ? new List<Client>()
                : await _clientService.GetListOfClients(psychologist.ClientIds);
            //slots
            List<Slot> slots = psychologist.SlotIds == null
                ? new List<Slot>()
                : await _slotService.GetListOfSlots(psychologist.SlotIds);
            //sessions
            List<Session> sessions = psychologist.SessionIds == null
                ? new List<Session>()
                : await _sessionService.GetListOfSessions(psychologist.SessionIds);
        
            original.Name = psychologist.Name;
            original.Email = psychologist.Email;
            original.Phone = psychologist.Phone;
            original.DateOfBirth = DateTime.SpecifyKind(birthDay, DateTimeKind.Utc);
            original.Address = psychologist.Address;
            original.Password = password;
            ((Psychologist)original).Clients = clients;
            ((Psychologist)original).Slots = slots;
            ((Psychologist)original).Sessions = sessions;
            
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