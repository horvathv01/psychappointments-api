using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service.DataProtection;

public class ClientDataProtectionService : IDataProtectionService<Client>
{
    public bool IsAssociated(Client user, Session session)
    {
        return session.Client.Equals(user);
    }

    public bool IsAssociated(Client user, Slot slot)
    {
        foreach (var session in slot.Sessions)
        {
            if (session.Client.Equals(user))
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAssociated(Client user, User otherUser)
    {
        return user.Equals(otherUser);
    }

    public async Task<IEnumerable<LocationDTO>> Filter(Client user, Func<Task<IEnumerable<Location>>> query)
    {
        var result = await query();
        return result.Select(loc => new LocationDTO(loc));
    }

    public async Task<LocationDTO> Filter(Client user, Func<Task<Location>> query)
    {
        var result = await query();
        return new LocationDTO(result);
    }

    public async Task<IEnumerable<SessionDTO>> Filter(Client user, Func<Task<IEnumerable<Session>>> query)
    {
        var queryResult = await query();
        List<SessionDTO> result = new List<SessionDTO>();
        foreach (var session in queryResult)
        {
            result.Add(FilterData(user, session));
        }
        return result;
    }

    public async Task<SessionDTO> Filter(Client user, Func<Task<Session>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }

    public async Task<IEnumerable<SlotDTO>> Filter(Client user, Func<Task<IEnumerable<Slot>>> query)
    {
        var queryResult = await query();
        List<SlotDTO> result = new List<SlotDTO>();
        foreach (var slot in queryResult)
        {
            result.Add(FilterData(user, slot));
        }
        return result;
    }

    public async Task<SlotDTO> Filter(Client user, Func<Task<Slot>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }

    public async Task<IEnumerable<UserDTO>> Filter(Client user, Func<Task<IEnumerable<User>>> query)
    {
        var queryResult = await query();
        List<UserDTO> result = new List<UserDTO>();
        foreach (var u in queryResult)
        {
            result.Add(FilterData(user, u));
        }
        return result;
    }

    public async Task<UserDTO> Filter(Client user, Func<Task<User>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }
    
    //private methods for actual filtering
    public SessionDTO FilterData(Client user, Session session)
    {
        SessionDTO result = new SessionDTO(session);
        //hide in BOTH cases:
        //Psychologist.Slots, .Clients, .Sessions
        //PartnerPsychologist.Slots, .Clients, .Sessions
        //Location.Managers --> manager.Locations (null)
        //Slot
        result.SlotId = null;
        
        if (IsAssociated(user, session))
        {
            
            return result;
        }
        //hide if NOT related:
        //Client
        result.ClientId = null;
        //Description
        result.Description = null;
        
        return result;
    }
    
    public SlotDTO? FilterData(Client user, Slot slot)
    {
        //hide in BOTH cases:
        //everything. --> slots should not be seen by clients
        return null;
    }
    
    public UserDTO FilterData(Client user, User otherUser)
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
                //hide:
                //Locations --> location.Managers --> manager.Locations (null)
                result.LocationIds = ((Manager)otherUser).Locations.Select(loc => loc.Id).ToList();
                break;
            case UserType.Psychologist:
                //hide:
                //Clients
                result.ClientIds = null;
                //Slots
                result.SlotIds = null;
                //Sessions
                result.SessionIds = null;
                break;
            default:
                //this means otherUser is client
                //return whole if otherUser.Equals(user), else return null
                if (IsAssociated(user, otherUser)) return new UserDTO(otherUser);
                return null;
        }

        return result;
    }
}