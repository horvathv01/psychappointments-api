using System.Data;
using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Auth;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class UserService : IUserService
{
    private readonly PsychAppointmentContext _context;
    private readonly IAccessUtilities _hasher;
    /*
    private readonly IRepository<User> _userRepository;
    private readonly IPsychologistService _psychologistService;
    private readonly IClientService _clientService;
    private readonly ISessionService _sessionService;
    private readonly ISlotService _slotService;
    private readonly ILocationService _locationService;
    private readonly IManagerService _managerService;
    
    */
    
    public UserService(
        PsychAppointmentContext context,
        IAccessUtilities hasher
        /*
        IRepository<User> userRepository, 
        
        IPsychologistService psychologistService, 
        IClientService clientService, 
        ISessionService sessionService, 
        ISlotService slotService,
        ILocationService locationService,
        IManagerService managerService
        */
        )
    {
        _context = context;
        _hasher = hasher;
        /*
        _userRepository = userRepository;
    
        _psychologistService = psychologistService;
        _clientService = clientService;
        _sessionService = sessionService;
        _slotService = slotService;
        _locationService = locationService;
        _managerService = managerService;
        */
    }
    
    
    public async Task<bool> AddUser(UserDTO user)
    {
        int userQuantity = _context.Psychologists.Count() + _context.Clients.Count() + _context.Managers.Count() +
                           _context.Admins.Count();
        //var allUsers = await _userRepository.GetAll();
        long id = userQuantity + 1; //userId should never be 0
        string password = _hasher.HashPassword(user.Password, user.Email);
        long registeredById = user.RegisteredBy ?? 0;
        User? registeredBy = null;
        if (user.RegisteredBy != null)
        {
            registeredBy = await GetUserById(registeredById);    
        }
        DateTime birthDate = DateTime.MinValue;
        DateTime.TryParse(user.DateOfBirth, out birthDate);

        try
        {
            if (user.Type == Enum.GetName(typeof(UserType), UserType.Admin))
            {
                var newAdmin = new Admin(user.Name, user.Email, user.Phone, birthDate, user.Address, password,
                    registeredBy, id);
                await _context.Admins.AddAsync(newAdmin);
                return true;
            } else if (user.Type == Enum.GetName(typeof(UserType), UserType.Psychologist))
            {
                var newPsychologist = new Psychologist(user.Name, user.Email, user.Phone, birthDate, user.Address,
                    password, null, null, null, registeredBy, id);
                await _context.Psychologists.AddAsync(newPsychologist);
                return true;
            } else if (user.Type == Enum.GetName(typeof(UserType), UserType.Manager))
            {
                var newManager = new Manager(user.Name, user.Email, user.Phone, birthDate, user.Address, password, null, registeredBy, id);
                await _context.Managers.AddAsync(newManager);
                return true;
            }
            else
            {
                //meaning: client
                var newClient = new Client(user.Name, user.Email, user.Phone, birthDate, user.Address, password, null, null, registeredBy, id);
                await _context.Clients.AddAsync(newClient);
                return true;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Adding new user failed.");
            Console.WriteLine(e);
            return false;
        }
        
        
    }

    public async Task<User?> GetUserById(long id)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(cli => cli.Id == id);
        if (client != null)
        {
            return client;
        }
        var psychologist = await _context.Psychologists.FirstOrDefaultAsync(psy => psy.Id == id);
        if (psychologist != null)
        {
            return psychologist;
        }
        var manager = await _context.Managers.FirstOrDefaultAsync(man => man.Id == id);
        if (manager != null)
        {
            return manager;
        }
        var admin = await _context.Admins.FirstOrDefaultAsync(adm => adm.Id == id);
        if (admin != null)
        {
            return admin;
        }

        return null;
        //return await _userRepository.GetById(id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var client = await _context.Clients.FirstOrDefaultAsync(cli => cli.Email == email);
        if (client != null)
        {
            return client;
        }
        var psychologist = await _context.Psychologists.FirstOrDefaultAsync(psy => psy.Email == email);
        if (psychologist != null)
        {
            return psychologist;
        }
        var manager = await _context.Managers.FirstOrDefaultAsync(man => man.Email == email);
        if (manager != null)
        {
            return manager;
        }
        var admin = await _context.Admins.FirstOrDefaultAsync(adm => adm.Email == email);
        if (admin != null)
        {
            return admin;
        }

        return null;
        
        //return await _userRepository.GetByEmail(email);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var clients = await _context.Clients.ToListAsync();
        var psychologists = await _context.Psychologists.ToListAsync();
        var managers = await _context.Managers.ToListAsync();
        var admins = await _context.Admins.ToListAsync();

        return clients.Cast<User>()
            .Union(psychologists)
            .Union(managers)
            .Union(admins)
            .ToList();
        //return await _userRepository.GetAll();
    }

    public async Task<IEnumerable<User>> GetAllPsychologists()
    {
        return await _context.Psychologists.ToListAsync();
        //var allPsychologists = await _psychologistService.GetAllPsychologists();
        //return allPsychologists.Cast<User>();
    }

    public async Task<IEnumerable<User>> GetAllClients()
    {
        return await _context.Clients.ToListAsync();
        //var allClients = await _clientService.GetAllClients();
        //return allClients.Cast<User>().ToList();
    }

    public async Task<IEnumerable<User>> GetAllManagers()
    {
        return await _context.Managers.ToListAsync();
        //var allManagers = await _managerService.GetAllManagers();
        //return allManagers.Cast<User>().ToList();
    }

    public async Task<IEnumerable<User>> GetListOfUsers(List<long> ids)
    {
        var clients = await _context.Clients.Where(cli => ids.Contains(cli.Id)).ToListAsync();
        var psychologists = await _context.Psychologists.Where(psy => ids.Contains(psy.Id)).ToListAsync();
        var managers = await _context.Managers.Where(man => ids.Contains(man.Id)).ToListAsync();
        var admins = await _context.Admins.Where(ad => ids.Contains(ad.Id)).ToListAsync();

        return clients.Cast<User>()
            .Union(psychologists)
            .Union(managers)
            .Union(admins)
            .ToList();
    }

    public async Task<bool> UpdateUser(long id, UserDTO newUser)
    {
        var user = await GetUserById(id);
        if (user == null)
        {
            return false;
        }
        string password = _hasher.HashPassword(newUser.Password, newUser.Email);
        
        DateTime newBirthDay = user.DateOfBirth;
        DateTime.TryParse(newUser.DateOfBirth, out newBirthDay);

        if (Enum.GetName(typeof(UserType), user.Type) != newUser.Type)
        {
            //USER TYPE CONVERSION
            switch (newUser.Type)
            {
                case "Admin":
                    var newAdmin = new Admin(newUser.Name, newUser.Email, newUser.Phone, newBirthDay, newUser.Address,
                        password,
                        user.RegisteredBy, id);
                    //return await _userRepository.Update(id, newAdmin);
                    await _context.Admins.AddAsync(newAdmin);
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                case "Psychologist":
                    var newPsychologist = new Psychologist(newUser.Name, newUser.Email, newUser.Phone, newBirthDay,
                        newUser.Address,
                        password, null, null, null, user.RegisteredBy, id);
                    if (newUser.SessionIds != null)
                    {
                        newPsychologist.Sessions = await _context.Sessions
                            .Where(ses => newUser.SessionIds.Contains(ses.Id))
                            .ToListAsync();
                        //_sessionService.GetListOfSessions(newUser.SessionIds);
                    }

                    if (newUser.SlotIds != null)
                    {
                        newPsychologist.Slots = await _context.Slots.Where(slot => newUser.SlotIds.Contains(slot.Id))
                            .ToListAsync();
                        //_slotService.GetListOfSlots(newUser.SlotIds);
                    }

                    if (newUser.ClientIds != null)
                    {
                        newPsychologist.Clients = await _context.Clients
                            .Where(cli => newUser.ClientIds.Contains(cli.Id))
                            .ToListAsync();
                        //_clientService.GetListOfClients(newUser.ClientIds);
                    }

                    //return await _psychologistService.UpdatePsychologist(user.Id, newPsychologist);
                    await _context.Psychologists.AddAsync(newPsychologist);
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                case "Manager":
                    var newManager = new Manager(newUser.Name, newUser.Email, newUser.Phone, newBirthDay, newUser.Address,
                        password, null, user.RegisteredBy, id);

                    if (newUser.LocationIds != null)
                    {
                        newManager.Locations = await _context.Locations
                            .Where(loc => newUser.LocationIds.Contains(loc.Id))
                            .ToListAsync();
                        //_locationService.GetListOfLocations(newUser.LocationIds);
                    }

                    //return await _managerService.UpdateManager(user.Id, newManager);
                    await _context.Managers.AddAsync(newManager);
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                case "Client":
                    var newClient = new Client(newUser.Name, newUser.Email, newUser.Phone, newBirthDay, newUser.Address,
                        password, null, null, user.RegisteredBy, id);

                    if (newUser.SessionIds != null)
                    {
                        newClient.Sessions = await _context.Sessions.Where(ses => newUser.SessionIds.Contains(ses.Id))
                            .ToListAsync();
                        //_sessionService.GetListOfSessions(newUser.SessionIds);
                    }

                    if (newUser.PsychologistIds != null)
                    {
                        newClient.Psychologists = await _context.Psychologists
                            .Where(psy => newUser.PsychologistIds.Contains(psy.Id))
                            .ToListAsync();
                        //_psychologistService.GetListOfPsychologists(newUser.PsychologistIds);
                    }

                    await _context.Clients.AddAsync(newClient);
                    _context.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                //return await _clientService.UpdateClient(user.Id, newClient);

                default:
                    throw new ConstraintException($"UserDTO Type property contains invalid value: {newUser.Type}.");
            }
        }
        else
        {
            
            
            user.Name = newUser.Name;
            user.Email = newUser.Email;
            user.Phone = newUser.Phone;
            user.DateOfBirth = newBirthDay;
            user.Address = newUser.Address;
            user.Password = password;
            
            switch (user.Type)
            {
                case UserType.Admin:
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                case UserType.Client:
                    if (newUser.SessionIds != null)
                    {
                        ((Client)user).Sessions = await _context.Sessions.Where(ses => newUser.SessionIds.Contains(ses.Id))
                            .ToListAsync();
                    }
                    if (newUser.PsychologistIds != null)
                    {
                        ((Client)user).Psychologists = await _context.Psychologists
                            .Where(psy => newUser.PsychologistIds.Contains(psy.Id))
                            .ToListAsync();
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                case UserType.Manager:
                    if (newUser.LocationIds != null)
                    {
                        ((Manager)user).Locations = await _context.Locations
                            .Where(loc => newUser.LocationIds.Contains(loc.Id))
                            .ToListAsync();
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                case UserType.Psychologist:
                    if (newUser.SessionIds != null)
                    {
                       ((Psychologist)user).Sessions = await _context.Sessions
                            .Where(ses => newUser.SessionIds.Contains(ses.Id))
                            .ToListAsync();
                    }

                    if (newUser.SlotIds != null)
                    {
                        ((Psychologist)user).Slots = await _context.Slots.Where(slot => newUser.SlotIds.Contains(slot.Id))
                            .ToListAsync();
                    }

                    if (newUser.ClientIds != null)
                    {
                        ((Psychologist)user).Clients = await _context.Clients
                            .Where(cli => newUser.ClientIds.Contains(cli.Id))
                            .ToListAsync();
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    return true;
                default: return false;
            }
        }
    }

    public async Task<bool> DeleteUser(long id)
    {
        try
        {
            var user = await GetUserById(id);
            if (user != null)
            {
                _context.Remove(user);
                await _context.SaveChangesAsync();
                return true;    
            }

            Console.WriteLine($"User with id {id} was not found.");
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Removing user with id {id} failed.");
            Console.WriteLine(e);
            return false;
        }
    }
    
}