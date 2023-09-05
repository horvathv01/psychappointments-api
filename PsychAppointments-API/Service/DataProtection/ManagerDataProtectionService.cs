using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service.DataProtection;

/// <summary>
/// assumes query depth of Psychologist, including Psychologist.Sessions, including Session.Location
/// assumes query depth of session.psychologist, psychologist.sessions
/// assumes query depth of slot.psychologist, psychologist.sessions
/// </summary>
public class ManagerDataProtectionService : IDataProtectionService<Manager>
{
    public bool IsAssociated(Manager user, Session session)
    {
        return user.Locations.Contains(session.Location);
    }

    public bool IsAssociated(Manager user, Slot slot)
    {
        return user.Locations.Contains(slot.Location);
    }

    public bool IsAssociated(Manager user, User otherUser)
    {
        //this assumes query depth of Psychologist, including Psychologist.Sessions, including Session.Location
        //does psychologist work in one of my managed locations?
        if (otherUser.Type == UserType.Psychologist)
        {
            foreach (var userLocation in user.Locations)
            {
                foreach (var sessionLocation in ((Psychologist)otherUser).Sessions)
                {
                    if (userLocation.Equals(sessionLocation))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        //are we managers of the same place(s)?
        if (otherUser.Type == UserType.Manager)
        {
            foreach (var userLocation in user.Locations)
            {
                if (((Manager)otherUser).Locations.Contains(userLocation))
                {
                    return true;
                }
            }

            return false;
        }
        //is self?
        if (otherUser.Equals(user))
        {
            return true;
        }
        //did I register this account (client)?
        return ((Client)otherUser).RegisteredBy.Equals(user);
    }

    private bool IsAssociated(Manager user, Location location)
    {
        return location.Managers.Any(man => man.Id == user.Id);
    }

    public async Task<IEnumerable<LocationDTO>> Filter(Manager user, Func<Task<IEnumerable<Location>>> query)
    {
        var result = await query();
        return result.Select(loc => new LocationDTO(loc));
    }

    public async Task<LocationDTO> Filter(Manager user, Func<Task<Location>> query)
    {
        var result = await query();
        return new LocationDTO(result);
    }

    public async Task<IEnumerable<SessionDTO>> Filter(Manager user, Func<Task<IEnumerable<Session>>> query)
    {
        var queryResult = await query();
        List<SessionDTO> result = new List<SessionDTO>();
        foreach (var session in queryResult)
        {
            result.Add(FilterData(user, session));
        }
        return result;
    }

    public async Task<SessionDTO> Filter(Manager user, Func<Task<Session>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }

    public async Task<IEnumerable<SlotDTO>> Filter(Manager user, Func<Task<IEnumerable<Slot>>> query)
    {
        var queryResult = await query();
        List<SlotDTO> result = new List<SlotDTO>();
        foreach (var slot in queryResult)
        {
            result.Add(FilterData(user, slot));
        }
        return result;
    }

    public async Task<SlotDTO> Filter(Manager user, Func<Task<Slot>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }

    public async Task<IEnumerable<UserDTO>> Filter(Manager user, Func<Task<IEnumerable<User>>> query)
    {
        var queryResult = await query();
        List<UserDTO> result = new List<UserDTO>();
        foreach (var u in queryResult)
        {
            result.Add(FilterData(user, u));
        }
        return result;
    }

    public async Task<UserDTO> Filter(Manager user, Func<Task<User>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }
    
        //private methods for actual filtering
        
    //this assumes query depth of session.psychologist --> psychologist.sessions
    public SessionDTO? FilterData(Manager user, Session session)
    {
        
        if (IsAssociated(user, session))
        {
            var result = new SessionDTO(session);
            //hide in BOTH cases:
            //client IF not registered by user
            result.ClientId = session.Client.RegisteredBy.Equals(user) ? session.Client.Id : 0;
            //psychologist's Sessions and Slots that are at other location
            
            return result;
        }
        
        //hide if NOT related:
        //this means that session is not at manager's locations --> needs no info about it
        return null;
    }
    //this assumes query depth of slot.psychologist --> psychologist.sessions
    public SlotDTO? FilterData(Manager user, Slot slot)
    {
        if (IsAssociated(user, slot))
        {
            var result = new SlotDTO(slot);
            //hide in BOTH cases:
            //psychologist's Sessions and Slots that are at other location
            
            return result;
        }
        //hide if NOT related:
        //this means that slot is not at manager's locations --> needs no info about it
        return null;
    }
    
    public UserDTO FilterData(Manager user, User otherUser)
    {
        var result = new UserDTO(otherUser);
        result.Address = new Address();
        result.DateOfBirth = DateTime.MinValue;
        result.RegisteredBy = 0;
        result.Id = 0;
        result.Password = "";
        //address
        //birthday
        //registeredby
        
        switch (otherUser.Type)
        {
            case UserType.Admin:
                //hide everything but Type
                return new UserDTO(UserType.Admin.ToString());
            case UserType.Manager:
                //is self:
                if (otherUser.Id == user.Id)
                {
                    var self = new UserDTO(otherUser);
                    self.Password = "";
                    return self;
                }

                if (IsAssociated(user, otherUser))
                {
                    //this means they manage the same locations
                    result.Id = otherUser.Id;
                }
                //direct personal details have been hidden already; nothing else to hide from colleague
                return result;
            case UserType.Psychologist:
                //is other psychologist, hide:
                //Clients
                result.Id = otherUser.Id;
                result.ClientIds = ((Psychologist)otherUser).Clients.Where(cli => IsAssociated(user, cli)).Select(cli => cli.Id).ToList();
                //Slots
                result.SlotIds = ((Psychologist)otherUser).Slots.Where(slot => IsAssociated(user, slot)).Select(slot => slot.Id).ToList();
                //Sessions
                result.SessionIds = ((Psychologist)otherUser).Sessions.Where(ses => IsAssociated(user, ses)).Select(ses => ses.Id).ToList();
                break;
            default:
                //this means otherUser is client
                //return all if otherUser has been registered by user, bare minimum if not
                if (IsAssociated(user, otherUser))
                {
                    var associatedClient = new UserDTO(otherUser);
                    associatedClient.Password = "";
                    return associatedClient;
                }
                
                result.Name = "";
                result.Type = Enum.GetName(typeof(UserType), UserType.Client);
                result.Email = "";
                result.Phone = "";
                result.SessionIds = null;
                result.PsychologistIds = null;
                result.ClientIds = null;
                result.SlotIds = null;
                result.LocationIds = null;
                
                return result;
        }
        return result;
    }
}