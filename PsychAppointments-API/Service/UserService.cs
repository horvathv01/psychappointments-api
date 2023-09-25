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
    private readonly IAddressService _addressService;
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
        IAccessUtilities hasher,
        IAddressService addressService
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
        _addressService = addressService;
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
        int userQuantity = await _context.Psychologists.CountAsync() + await _context.Clients.CountAsync() + await _context.Managers.CountAsync() +
                           await _context.Admins.CountAsync();
        //var allUsers = await _userRepository.GetAll();
        long id = userQuantity + 1; //userId should never be 0
        string password = _hasher.HashPassword(user.Password, user.Email);
        User? registeredBy = null;
        if (user.RegisteredBy != null && user.RegisteredBy != 0)
        {
            registeredBy = await GetUserById((long)user.RegisteredBy);    
        }
        DateTime birthDate = DateTime.MinValue;
        DateTime.TryParse(user.DateOfBirth, out birthDate);
        birthDate = DateTime.SpecifyKind(birthDate, DateTimeKind.Utc);
        var address = await _addressService.GetEquivalent(user.Address);

        if (address == null)
        {
            address = user.Address;
            //new Address(user.Address.Country, user.Address.Zip, user.Address.City, user.Address.Street, user.Address.Rest);
        }
        
        try
        {
            if (user.Type == Enum.GetName(typeof(UserType), UserType.Admin))
            {
                var newAdmin = new Admin(user.Name, user.Email, user.Phone, birthDate, address, password,
                    registeredBy, id);
                if (registeredBy == null)
                {
                    newAdmin.RegisteredBy = newAdmin;    
                }
                else
                {
                    newAdmin.RegisteredBy = registeredBy;
                }
                await _context.Admins.AddAsync(newAdmin);
                await _context.SaveChangesAsync();
                return true;
            } else if (user.Type == Enum.GetName(typeof(UserType), UserType.Psychologist))
            {
                var newPsychologist = new Psychologist(user.Name, user.Email, user.Phone, birthDate, address,
                    password, new List<Session>(), new List<Slot>(), new List<Client>(), null, id);
                if (registeredBy == null)
                {
                    newPsychologist.RegisteredBy = newPsychologist;    
                }
                else
                {
                    newPsychologist.RegisteredBy = registeredBy;
                }
                
                await _context.Psychologists.AddAsync(newPsychologist);
                await _context.SaveChangesAsync();
                return true;
            } else if (user.Type == Enum.GetName(typeof(UserType), UserType.Manager))
            {
                var newManager = new Manager(user.Name, user.Email, user.Phone, birthDate, address, password, new List<Location>(), null, id);
                if (registeredBy == null)
                {
                    newManager.RegisteredBy = newManager;    
                }
                else
                {
                    newManager.RegisteredBy = registeredBy;
                }
                await _context.Managers.AddAsync(newManager);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                //meaning: client
                var newClient = new Client(user.Name, user.Email, user.Phone, birthDate, address, password, new List<Session>(), new List<Psychologist>(), null, id);
                if (registeredBy == null)
                {
                    newClient.RegisteredBy = newClient;    
                }
                else
                {
                    newClient.RegisteredBy = registeredBy;
                }
                await _context.Clients.AddAsync(newClient);
                await _context.SaveChangesAsync();
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
            return await _context.Clients
                .Include(cli => cli.Address)
                .Include(cli => cli.Psychologists)
                .Include(cli => cli.Sessions)
                .FirstOrDefaultAsync(cli => cli.Id == id);
        }
        var psychologist = await _context.Psychologists.FirstOrDefaultAsync(psy => psy.Id == id);
        if (psychologist != null)
        {
            return await _context.Psychologists
                .Include(psy => psy.Address)
                .Include(psy => psy.Sessions)
                .Include(psy => psy.Clients)
                .Include(psy => psy.Slots)
                .FirstOrDefaultAsync(psy => psy.Id == id);
        }
        var manager = await _context.Managers.FirstOrDefaultAsync(man => man.Id == id);
        if (manager != null)
        {
            return await _context.Managers
                .Include(man => man.Address)
                .Include(man => man.Locations)
                .FirstOrDefaultAsync(man => man.Id == id);
        }
        var admin = await _context.Admins
            .Include(adm => adm.Address)
            .FirstOrDefaultAsync(adm => adm.Id == id);
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
            return await _context.Clients
                .Include(cli => cli.Address)
                .Include(cli => cli.Psychologists)
                .Include(cli => cli.Sessions)
                .FirstOrDefaultAsync(cli => cli.Email == email);
        }
        var psychologist = await _context.Psychologists.FirstOrDefaultAsync(psy => psy.Email == email);
        if (psychologist != null)
        {
            return await _context.Psychologists
                .Include(psy => psy.Address)
                .Include(psy => psy.Sessions)
                .Include(psy => psy.Clients)
                .Include(psy => psy.Slots)
                .FirstOrDefaultAsync(psy => psy.Email == email);
        }
        var manager = await _context.Managers.FirstOrDefaultAsync(man => man.Email == email);
        if (manager != null)
        {
            return await _context.Managers
                .Include(man => man.Address)
                .Include(man => man.Locations)
                .FirstOrDefaultAsync(man => man.Email == email);
        }
        var admin = await _context.Admins
            .Include(adm => adm.Address)
            .FirstOrDefaultAsync(adm => adm.Email == email);
        if (admin != null)
        {
            return admin;
        }

        return null;
        
        //return await _userRepository.GetByEmail(email);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        var clients = await _context.Clients
            .Include(cli => cli.Address)
            .ToListAsync();
        var psychologists = await _context.Psychologists
            .Include(psy => psy.Address)
            .ToListAsync();
        var managers = await _context.Managers
            .Include(man => man.Address)
            .ToListAsync();
        var admins = await _context.Admins
            .Include(man => man.Address)
            .ToListAsync();

        return clients.Cast<User>()
            .Union(psychologists)
            .Union(managers)
            .Union(admins)
            .ToList();
        //return await _userRepository.GetAll();
    }

    public async Task<IEnumerable<User>> GetAllPsychologists()
    {
        return await _context.Psychologists
            .Include(psy => psy.Clients)
            .Include(psy => psy.Sessions)
                .ThenInclude(ses => ses.Location)
            .Include(psy => psy.Sessions)
                .ThenInclude(ses => ses.Client)
            .Include(psy => psy.Slots)
            .ToListAsync();
        //var allPsychologists = await _psychologistService.GetAllPsychologists();
        //return allPsychologists.Cast<User>();
    }

    public async Task<IEnumerable<User>> GetAllClients()
    {
        return await _context.Clients
            .Include(cli => cli.Psychologists)
            .Include(cli => cli.Sessions)
            .ThenInclude(ses => ses.Location)
            .ToListAsync();
        //var allClients = await _clientService.GetAllClients();
        //return allClients.Cast<User>().ToList();
    }

    public async Task<IEnumerable<User>> GetAllManagers()
    {
        return await _context.Managers
            .Include(man => man.Locations)
            .ToListAsync();
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

    public async Task<IEnumerable<User>> GetManagersByLocation(long id)
    {
        var location = await _context.Locations
            .Include(loc => loc.Managers)
            .ThenInclude(man => man.Locations)
            .FirstOrDefaultAsync(loc => loc.Id == id);
        
        if (location != null)
        {
            return location.Managers.ToList();    
        }
        return new List<User>();
    }
    
    public async Task<IEnumerable<User>> GetPsychologistsByLocation(long id)
    {
        var location = await _context.Locations
            .Include(loc => loc.Psychologists)
            .FirstOrDefaultAsync(loc => loc.Id == id);

        if (location != null)
        {
            var ids = location.Psychologists.Select(psy => psy.Id).ToList();
            return await GetListOfUsers(ids);    
        }

        return new List<User>();
    }

    public async Task<IEnumerable<User>> GetClientsByLocation(long id)
    {
        var locationSessions = await _context.Sessions.Where(ses => ses.Location.Id == id)
            .Include(ses => ses.Client)
            .ToListAsync();
        
        if (locationSessions.Count > 0)
        {
            var ids = locationSessions.Select(ses => ses.Client.Id).ToList();
            return await GetListOfUsers(ids);    
        }
        return new List<User>();
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
        newBirthDay = DateTime.SpecifyKind(newBirthDay, DateTimeKind.Utc);

        if (Enum.GetName(typeof(UserType), user.Type) != newUser.Type)
        {
            //USER TYPE CONVERSION, new ID needed
            newUser.Id = await _context.Psychologists.CountAsync() + await _context.Clients.CountAsync() + await _context.Managers.CountAsync() +
                 await _context.Admins.CountAsync() + 1;
            await AddUser(newUser);
            _context.Remove(user);
            await _context.SaveChangesAsync();
            await _addressService.ClearOrphanedAddresses();
            return true;
        }
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
                    await _addressService.ClearOrphanedAddresses();
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
                    await _addressService.ClearOrphanedAddresses();
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
                    await _addressService.ClearOrphanedAddresses();
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
                    await _addressService.ClearOrphanedAddresses();
                    return true;
                default: return false;
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
                await _addressService.ClearOrphanedAddresses();
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