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
    private SessionDTO FilterData(Client user, Session session)
    {
        SessionDTO result = new SessionDTO(session);
        if (IsAssociated(user, session))
        {
            result.Psychologist.Slots = null;
            result.Psychologist.Sessions = null;
            return result;
        }

        result.Id = 0;
        result.PartnerPsychologist = null;
        result.Psychologist = FilterData(user, session.Psychologist);
        result.Client = FilterData(user, session.Client);
        result.Price = 0;
        result.Frequency = SessionFrequency.None.ToString();
        result.Slot = FilterData(user, session.Slot);
        return result;
    }
    
    private SlotDTO? FilterData(Client user, Slot slot)
    {
        //slots should not be seen by clients at all
        return null;
        /*
        if (IsAssociated(user, slot))
        {
            SlotDTO result = new SlotDTO(slot);
            //filter sessions of slot to only include ones associated with current client
            result.Sessions = slot.Sessions.Select(ses => FilterData(user, ses)).ToList();
            //kill psychologist's references to other sessions and slots
            result.Psychologist.Sessions = result.Sessions;
            result.Psychologist.Slots = null;
            return result;
        }
        return new SlotDTO(new LocationDTO(slot.Location), slot.Date, slot.SlotStart, slot.SlotEnd);
        */
    }
    
    private UserDTO FilterData(Client user, User otherUser)
    {
        var result = new UserDTO(otherUser);
        if (IsAssociated(user, otherUser))
        {
            return result;
        }
        switch (otherUser.Type)
        {
            case UserType.Admin:
                //leave nothing, only type
                return new UserDTO(UserType.Admin.ToString());
            case UserType.Manager:
                //leave nothing, only managed locations
                    //even there, 
                result.Id = 0;
                result.DateOfBirth = DateTime.MinValue; 
                result.Address = new Address();
                result.Password = "";
                result.RegisteredBy = 0;
                result.Sessions = null; 
                result.Psychologists = null;
                result.Clients = null;
                result.Slots = null;
                result.Locations = result.Locations.Select(loc =>
                {
                    loc.Psychologists = loc.Psychologists.Select(psy => new UserDTO(UserType.Psychologist.ToString(), psy.Name)).ToList();
                    loc.Managers = loc.Managers.Select(man => new UserDTO(UserType.Manager.ToString(), man.Name)).ToList();
                    return loc;
                }).ToList();
                return result;
            case UserType.Psychologist:
                result.Id = 0;
                result.DateOfBirth = DateTime.MinValue; 
                result.Address = new Address();
                result.Password = "";
                result.RegisteredBy = 0;
                result.Sessions = result.Sessions.Where(ses => ses.Blank).ToList();
                result.Psychologists = null;
                result.Clients = null;
                result.Slots = null;
                result.Locations = result.Locations.Select(loc =>
                {
                    loc.Psychologists = loc.Psychologists.Select(psy => new UserDTO(UserType.Psychologist.ToString(), psy.Name)).ToList();
                    return loc;
                }).ToList();
                return result;
            default: return new UserDTO(UserType.Client.ToString()); 
        }
    }
}