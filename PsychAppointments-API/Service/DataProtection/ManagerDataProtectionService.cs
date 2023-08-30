using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service.DataProtection;

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
        if (otherUser.Equals(user))
        {
            return true;
        }
        return otherUser.RegisteredBy.Equals(user);
    }

    private bool IsAssociated(Manager user, LocationDTO location)
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
    private SessionDTO FilterData(Manager user, Session session)
    {
        SessionDTO result = new SessionDTO(session);
        result.Client = FilterData(user, session.Client);
        result.Location.Psychologists = session.Location.Psychologists.Select(psy =>
        {
            var newPsy = new UserDTO(psy);
            newPsy.Clients = null;
            newPsy.Locations = null;
            return newPsy;
        }).ToList();
        if (IsAssociated(user, session))
        {
            result.Psychologist.Slots = null;
            result.Psychologist.Sessions = null;
            return result;
        }

        result.Id = 0;
        result.PartnerPsychologist = FilterData(user, session.PartnerPsychologist);
        result.Psychologist = FilterData(user, session.Psychologist);
        result.Price = 0;
        result.Frequency = SessionFrequency.None.ToString();
        result.Slot = FilterData(user, session.Slot);
        return result;
    }
    
    private SlotDTO? FilterData(Manager user, Slot slot)
    {
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

        return null;
    }
    
    private UserDTO FilterData(Manager user, User otherUser)
    {
        var result = new UserDTO(otherUser);
        if (IsAssociated(user, otherUser))
        {
            //will be the case if:
            //otherUser is the same as user
            //otherUser has been registered by user
            if (otherUser.Equals(user) || otherUser.RegisteredBy.Equals(user))
            {
                if (otherUser.Type == UserType.Psychologist)
                {
                    //if psychologist: filter sessions to those where client is associated to us
                    result.Sessions = ((Psychologist)otherUser).Sessions.Where(ses => 
                        IsAssociated(user, ses.Client)).Select(ses => new SessionDTO(ses)).ToList();
                }
                return result;
            }
            //otherUser is psychologist, working in a location that is managed by user
            result.Sessions = ((Psychologist)otherUser).Sessions.Select(ses => FilterData(user, ses)).ToList();
            result.Slots = ((Psychologist)otherUser).Slots.Select(slot => FilterData(user, slot)).ToList();
            result.Clients = ((Psychologist)otherUser).Clients.Where(cli => cli.Type == UserType.Client)
                .Select(cli => FilterData(user, cli).Id).ToList();
            return result;
        }
        switch (otherUser.Type)
        {
            case UserType.Admin:
                //leave nothing, only type
                return new UserDTO(UserType.Admin.ToString());
            case UserType.Manager:
                //leave nothing, only managed locations
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
                result.Sessions = result.Sessions.Where(ses => IsAssociated(user, ses.Location)).ToList();
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