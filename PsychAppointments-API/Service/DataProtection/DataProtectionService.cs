using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service.DataProtection;

public class DataProtectionService : IDataProtectionService<User>
{
    private IDataProtectionService<Admin> _adminDPS;
    private IDataProtectionService<Client> _clientDPS;
    private IDataProtectionService<Manager> _managerDPS;
    private IDataProtectionService<Psychologist> _psychologistDPS;

    public DataProtectionService(
        IDataProtectionService<Admin> adminDPS,
        IDataProtectionService<Client> clientDPS,
        IDataProtectionService<Manager> managerDPS,
        IDataProtectionService<Psychologist> psychologistDPS)
    {
        _adminDPS = adminDPS;
        _clientDPS = clientDPS;
        _managerDPS = managerDPS;
        _psychologistDPS = psychologistDPS;
    }
    
    public bool IsAssociated(User user, Session session)
    {
        return user.Type switch
        {
            UserType.Admin => _adminDPS.IsAssociated(((Admin)user), session),
            UserType.Client => _clientDPS.IsAssociated(((Client)user), session), 
            UserType.Manager => _managerDPS.IsAssociated((Manager)user, session), 
            UserType.Psychologist => _psychologistDPS.IsAssociated((Psychologist)user, session),
            _ => _clientDPS.IsAssociated(((Client)user), session),
        };
    }

    public bool IsAssociated(User user, Slot slot)
    {
        return user.Type switch
        {
            UserType.Admin => _adminDPS.IsAssociated(((Admin)user), slot),
            UserType.Client => _clientDPS.IsAssociated(((Client)user), slot), 
            UserType.Manager => _managerDPS.IsAssociated((Manager)user, slot), 
            UserType.Psychologist => _psychologistDPS.IsAssociated((Psychologist)user, slot),
            _ => _clientDPS.IsAssociated(((Client)user), slot),
        };
    }

    public bool IsAssociated(User user, User otherUser)
    {
        return user.Type switch
        {
            UserType.Admin => _adminDPS.IsAssociated(((Admin)user), otherUser),
            UserType.Client => _clientDPS.IsAssociated(((Client)user), otherUser), 
            UserType.Manager => _managerDPS.IsAssociated((Manager)user, otherUser), 
            UserType.Psychologist => _psychologistDPS.IsAssociated((Psychologist)user, otherUser),
            _ => _clientDPS.IsAssociated(((Client)user), otherUser),
        };
    }

    public async Task<IEnumerable<LocationDTO>> Filter(User user, Func<Task<IEnumerable<Location>>?> query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<LocationDTO> Filter(User user, Func<Task<Location?>> query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<IEnumerable<SessionDTO>> Filter(User user, Func<Task<IEnumerable<Session>>>? query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<SessionDTO> Filter(User user, Func<Task<Session>> query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<IEnumerable<SlotDTO>> Filter(User user, Func<Task<IEnumerable<Slot>>>? query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<SlotDTO> Filter(User user, Func<Task<Slot>> query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<IEnumerable<UserDTO>> Filter(User user, Func<Task<IEnumerable<User>>> query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public async Task<UserDTO> Filter(User user, Func<Task<User>> query)
    {
        return user.Type switch
        {
            UserType.Admin => await _adminDPS.Filter(((Admin)user), query),
            UserType.Client => await _clientDPS.Filter(((Client)user), query), 
            UserType.Manager => await _managerDPS.Filter((Manager)user, query), 
            UserType.Psychologist => await _psychologistDPS.Filter((Psychologist)user, query),
            _ => await _clientDPS.Filter(((Client)user), query),
        };
    }

    public SessionDTO FilterData(User user, Session session)
    {
        throw new InvalidOperationException();
    }

    public SlotDTO FilterData(User user, Slot slot)
    {
        throw new InvalidOperationException();
    }

    public UserDTO FilterData(User user, User otherUser)
    {
        throw new InvalidOperationException();
    }
}