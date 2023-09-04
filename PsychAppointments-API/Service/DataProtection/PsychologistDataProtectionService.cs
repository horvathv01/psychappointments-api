using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service.DataProtection;

public class PsychologistDataProtectionService : IDataProtectionService<Psychologist>
{
    public bool IsAssociated(Psychologist user, Session session)
    {
        return session.Psychologist.Equals(user) || session.PartnerPsychologist.Equals(user);
    }

    public bool IsAssociated(Psychologist user, Slot slot)
    {
        return slot.Psychologist.Equals(user);
    }

    //this assumes query depth of Manager.Locations and Psychologist.Sessions -> Session.Location
    public bool IsAssociated(Psychologist user, User otherUser)
    {
        //is admin? -> return false
        if (otherUser.Type == UserType.Admin)
        {
            return false;
        }
            //is self? --> return true
        if (user.Id == otherUser.Id)
        {
            return true;
        }

        //is client of mine? --> return true
        if (otherUser.Type == UserType.Client)
        {
            return user.Clients.Contains(otherUser);
        }
        //is manager of location where my session is at? --> return true
        if (otherUser.Type == UserType.Manager)
        {
            foreach (var session in user.Sessions)
            {
                if (((Manager)otherUser).Locations.Contains(session.Location))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private bool IsAssociated(Psychologist user, Location location)
    {
        foreach (var session in user.Sessions)
        {
            if (session.Location.Equals(location))
            {
                return true;
            }
        }

        return false;
    }

    public async Task<IEnumerable<LocationDTO>> Filter(Psychologist user, Func<Task<IEnumerable<Location>>> query)
    {
        var result = await query();
        return result.Select(loc => new LocationDTO(loc));
    }

    public async Task<LocationDTO> Filter(Psychologist user, Func<Task<Location>> query)
    {
        var result = await query();
        return new LocationDTO(result);
    }

    public async Task<IEnumerable<SessionDTO>> Filter(Psychologist user, Func<Task<IEnumerable<Session>>> query)
    {
        var queryResult = await query();
        List<SessionDTO> result = new List<SessionDTO>();
        foreach (var session in queryResult)
        {
            result.Add(FilterData(user, session));
        }
        return result;
    }

    public async Task<SessionDTO> Filter(Psychologist user, Func<Task<Session>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }

    public async Task<IEnumerable<SlotDTO>> Filter(Psychologist user, Func<Task<IEnumerable<Slot>>> query)
    {
        var queryResult = await query();
        List<SlotDTO> result = new List<SlotDTO>();
        foreach (var slot in queryResult)
        {
            result.Add(FilterData(user, slot));
        }
        return result;
    }

    public async Task<SlotDTO> Filter(Psychologist user, Func<Task<Slot>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }

    public async Task<IEnumerable<UserDTO>> Filter(Psychologist user, Func<Task<IEnumerable<User>>> query)
    {
        var queryResult = await query();
        List<UserDTO> result = new List<UserDTO>();
        foreach (var u in queryResult)
        {
            result.Add(FilterData(user, u));
        }
        return result;
    }

    public async Task<UserDTO> Filter(Psychologist user, Func<Task<User>> query)
    {
        var queryResult = await query();
        return FilterData(user, queryResult);
    }
    
    public SessionDTO FilterData(Psychologist user, Session session)
    {
        SessionDTO result = new SessionDTO(session);
        if (IsAssociated(user, session))
        {
            //hide in BOTH cases:
            //.Location.Manager.Locations
            //.Client.Sessions
            //.Client.Psychologists
            return result;
        }
        
        //hide if not related:
        //return bare minimum
        result.PsychologistId = null;
        result.PartnerPsychologistId = null;
        result.ClientId = null;
        result.Price = null;
        result.Frequency = "";
        result.SlotId = null;
        result.Description = "";
        
        return result;
    }
    
    public SlotDTO? FilterData(Psychologist user, Slot slot)
    {
        //hide in BOTH cases:
        if (IsAssociated(user, slot))
        {
            return new SlotDTO(slot);
        }
        //hide if not related:
        return null;
    }

    public UserDTO FilterData(Psychologist user, User otherUser)
    {
        var result = new UserDTO(otherUser);
        result.Address = new Address();
        result.DateOfBirth = DateTime.MinValue;
        result.RegisteredBy = 0;
        //address
        //birthday
        //registeredby
        
        switch (otherUser.Type)
        {
            case UserType.Admin:
                //hide everything but Type
                return new UserDTO(UserType.Admin.ToString());
            case UserType.Manager:
                //if not associated, hide:
                //Locations --> location.Managers --> manager.Locations (null)
                break;
            case UserType.Psychologist:
                //is self:
                if (otherUser.Id == user.Id)
                {
                    return new UserDTO(otherUser);
                }
                //is other psychologist, hide:
                //Clients
                result.ClientIds = null;
                //Slots
                result.SlotIds = null;
                //Sessions
                result.SessionIds = ((Psychologist)otherUser).Sessions.Where(ses => ses.Blank)
                    .Select(ses => ses.Id).ToList();
                break;
            default:
                //this means otherUser is client
                //return all if this is user's client, return null if not
                if (IsAssociated(user, otherUser)) return new UserDTO(otherUser);
                return null;
        }
        return result;
    }
}