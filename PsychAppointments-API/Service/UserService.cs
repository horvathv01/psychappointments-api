using System.Data;
using PsychAppointments_API.Auth;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class UserService : IUserService
{
    //private readonly DbContext _context;
    private readonly IRepository<User> _userRepository;
    private readonly IPsychologistService _psychologistService;
    private readonly IClientService _clientService;
    private readonly ISessionService _sessionService;
    private readonly ISlotService _slotService;
    private readonly ILocationService _locationService;
    private readonly IManagerService _managerService;
    private readonly IAccessUtilities _hasher;
    
    public UserService(
        //DbContext context
        IRepository<User> userRepository, 
        IAccessUtilities hasher, 
        IPsychologistService psychologistService, 
        IClientService clientService, 
        ISessionService sessionService, 
        ISlotService slotService,
        ILocationService locationService,
        IManagerService managerService
        )
    {
        //_context = context;
        _userRepository = userRepository;
        _hasher = hasher;

        _psychologistService = psychologistService;
        _clientService = clientService;
        _sessionService = sessionService;
        _slotService = slotService;
        _locationService = locationService;
        _managerService = managerService;
    }
    
    
    public async Task<bool> AddUser(UserDTO user)
    {
        var allUsers = await _userRepository.GetAll();
        long id = allUsers.ToList().Count + 1;
        string password = _hasher.HashPassword(user.Password, user.Email);
        long registeredById = user.RegisteredBy ?? 0;
        User? registeredBy = null;
        if (user.RegisteredBy != null)
        {
            registeredBy = await _userRepository.GetById(registeredById);    
        }
        DateTime birthDate = DateTime.MinValue;
        DateTime.TryParse(user.DateOfBirth, out birthDate);
        
        if (user.Type == Enum.GetName(typeof(UserType), UserType.Admin))
        {
            var newAdmin = new Admin(user.Name, user.Email, user.Phone, birthDate, user.Address, password,
                registeredBy, id);
            return await _userRepository.Add(newAdmin);
        } else if (user.Type == Enum.GetName(typeof(UserType), UserType.Psychologist))
        {
            var newPsychologist = new Psychologist(user.Name, user.Email, user.Phone, birthDate, user.Address,
                password, null, null, null, registeredBy, id);
            return await _psychologistService.AddPsychologist(newPsychologist);
        } else if (user.Type == Enum.GetName(typeof(UserType), UserType.Manager))
        {
            var newManager = new Manager(user.Name, user.Email, user.Phone, birthDate, user.Address, password, null, registeredBy, id);
            return await _managerService.AddManager(newManager);
        }
        else
        {
            //meaning: client
            var newClient = new Client(user.Name, user.Email, user.Phone, birthDate, user.Address, password, null, null, registeredBy, id);
            return await _clientService.AddClient(newClient);
        }
    }

    public async Task<User> GetUserById(long id)
    {
        return await _userRepository.GetById(id);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await _userRepository.GetByEmail(email);
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _userRepository.GetAll();
    }

    public async Task<IEnumerable<User>> GetAllPsychologists()
    {
        var allPsychologists = await _psychologistService.GetAllPsychologists();
        return allPsychologists.Cast<User>();
    }

    public async Task<IEnumerable<User>> GetAllClients()
    {
        var allClients = await _clientService.GetAllClients();
        return allClients.Cast<User>().ToList();
    }

    public async Task<IEnumerable<User>> GetAllManagers()
    {
        var allManagers = await _managerService.GetAllManagers();
        return allManagers.Cast<User>().ToList();
    }

    public async Task<IEnumerable<User>> GetListOfUsers(List<long> ids)
    {
        return await _userRepository.GetList(ids);
    }

    public async Task<bool> UpdateUser(long id, UserDTO newUser)
    {
        var user = await _userRepository.GetById(id);
        string password = _hasher.HashPassword(newUser.Password, newUser.Email);
        
        //email is used for password hashing
        string originalEmail = user.Email;
        if (originalEmail != newUser.Email || _hasher.HashPassword(newUser.Password, newUser.Email) != user.Password)
        {
            string newPassword = _hasher.HashPassword(newUser.Password, newUser.Email);
            newUser.Password = newPassword;
        }

        switch (newUser.Type)
        {
            case "Admin":
                var newAdmin = new Admin(user.Name, user.Email, user.Phone, user.DateOfBirth, user.Address, password,
                    user.RegisteredBy, id);
                return await _userRepository.Update(id, newAdmin);
            case "Psychologist":
                var newPsychologist = new Psychologist(user.Name, user.Email, user.Phone, user.DateOfBirth, user.Address,
                    password, null, null, null, user.RegisteredBy, id);
                if (newUser.SessionIds != null)
                {
                    newPsychologist.Sessions = await _sessionService.GetListOfSessions(newUser.SessionIds);
                }
                if (newUser.SlotIds != null)
                {
                    newPsychologist.Slots = await _slotService.GetListOfSlots(newUser.SlotIds);
                }
                if (newUser.ClientIds != null)
                {
                    newPsychologist.Clients = await _clientService.GetListOfClients(newUser.ClientIds);
                }
                return await _psychologistService.UpdatePsychologist(user.Id, newPsychologist);
            case "Manager":
                var newManager = new Manager(user.Name, user.Email, user.Phone, user.DateOfBirth, user.Address, password, null, user.RegisteredBy, id);

                if (newUser.LocationIds != null)
                {
                    newManager.Locations = await _locationService.GetListOfLocations(newUser.LocationIds);
                }
                return await _managerService.UpdateManager(user.Id, newManager);
            case "Client":
                var newClient = new Client(user.Name, user.Email, user.Phone, user.DateOfBirth, user.Address, password, null, null, user.RegisteredBy, id);

                if (newUser.SessionIds != null)
                {
                    newClient.Sessions = await _sessionService.GetListOfSessions(newUser.SessionIds);
                }

                if (newUser.PsychologistIds != null)
                {
                    newClient.Psychologists = await _psychologistService.GetListOfPsychologists(newUser.PsychologistIds);
                }
            
                return await _clientService.UpdateClient(user.Id, newClient);
            default:
                throw new ConstraintException($"UserDTO Type property contains invalid value: {newUser.Type}.");
        } 
    }

    public async Task<bool> DeleteUser(long id)
    {
        return await _userRepository.Delete(id);
    }
}