using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service.DataProtection;

public class PsychologistDataProtectionService : IDataProtectionService<Psychologist>
{
    public bool IsAssociated(Psychologist user, Session session)
    {
        throw new NotImplementedException();
    }

    public bool IsAssociated(Psychologist user, Slot slot)
    {
        throw new NotImplementedException();
    }

    public bool IsAssociated(Psychologist user, User otherUser)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LocationDTO>> Filter(Psychologist user, Func<Task<IEnumerable<Location>>> query)
    {
        throw new NotImplementedException();
    }

    public Task<LocationDTO> Filter(Psychologist user, Func<Task<Location>> query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SessionDTO>> Filter(Psychologist user, Func<Task<IEnumerable<Session>>> query)
    {
        throw new NotImplementedException();
    }

    public Task<SessionDTO> Filter(Psychologist user, Func<Task<Session>> query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<SlotDTO>> Filter(Psychologist user, Func<Task<IEnumerable<Slot>>> query)
    {
        throw new NotImplementedException();
    }

    public Task<SlotDTO> Filter(Psychologist user, Func<Task<Slot>> query)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserDTO>> Filter(Psychologist user, Func<Task<IEnumerable<User>>> query)
    {
        throw new NotImplementedException();
    }

    public Task<UserDTO> Filter(Psychologist user, Func<Task<User>> query)
    {
        throw new NotImplementedException();
    }
    
    private SessionDTO FilterData(Psychologist user, Session session)
    {
        SessionDTO result = new SessionDTO(session);
        result.Client = FilterData(user, session.Client);
        result.Location.Psychologists = session.Location.Psychologists.Select(psy =>
        {
            var newPsy = new UserDTO(psy);
            if (newPsy.Id != user.Id)
            {
                newPsy.Clients = null;
                newPsy.Locations = null;    
            }
            return newPsy;
        }).ToList();
        if (IsAssociated(user, session))
        {
            result.Psychologist.Slots = session.Psychologist.Slots.Select(slot => new SlotDTO(slot)).ToList();
            result.Psychologist.Sessions = session.Psychologist.Sessions.Select(ses => new SessionDTO(ses)).ToList();
            return result;
        }
        result.Psychologist.Slots = null;
        result.Psychologist.Sessions = session.Psychologist.Sessions.Select(ses => new SessionDTO(ses.Blank, ses.Location, ses.Date, ses.Start, ses.End)).ToList();
        result.Id = 0;
        result.PartnerPsychologist = FilterData(user, session.PartnerPsychologist);
        result.Psychologist = FilterData(user, session.Psychologist);
        result.Price = 0;
        result.Frequency = SessionFrequency.None.ToString();
        result.Slot = FilterData(user, session.Slot);
        return result;
    }
    
    private SlotDTO? FilterData(Psychologist user, Slot slot)
    {
        if (IsAssociated(user, slot))
        {
            SlotDTO result = new SlotDTO(slot);
            result.Sessions = slot.Sessions.Select(ses => new SessionDTO(ses)).ToList();
            result.Psychologist.Sessions = result.Sessions;
            result.Psychologist.Slots = result.Psychologist.Slots;
            return result;
        }

        return null;
    }

    private UserDTO FilterData(Psychologist user, User otherUser)
    {
        var result = new UserDTO(otherUser);
        if (IsAssociated(user, otherUser))
        {
            if (otherUser.Equals(user))
            {
                return result;
            }
            //will be the case if:
            //otherUser has been registered by user
            if (otherUser.RegisteredBy.Equals(user))
            {
                //manager?
                if (otherUser.Type == UserType.Manager)
                {
                    result.Locations = ((Manager)otherUser).Locations.Select(loc =>
                    {
                        loc.Psychologists = null;
                        return loc;
                    }).Select(loc => new LocationDTO(loc)).ToList();
                }
                
            }
            //otherUser is manager of associated location
            //otherUser is PartnerPsychologist in one or more sessions
            //otherUser is the same as user
            

            //otherUser is psychologist, working in a location that is managed by user
            
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
                    loc.Psychologists = loc.Psychologists
                        .Select(psy => new UserDTO(UserType.Psychologist.ToString(), psy.Name)).ToList();
                    loc.Managers = loc.Managers.Select(man => new UserDTO(UserType.Manager.ToString(), man.Name))
                        .ToList();
                    return loc;
                }).ToList();
                return result;
            case UserType.Psychologist:
                result.Id = 0;
                result.DateOfBirth = DateTime.MinValue;
                result.Address = new Address();
                result.Password = "";
                result.RegisteredBy = 0;
                result.Sessions = result.Sessions.Select(ses =>
                {
                    //bool blank, Location location, DateTime date, DateTime start, DateTime end, int price = 0
                    ses.Psychologist = new UserDTO(UserType.Psychologist.ToString(), ses.Psychologist.Name);
                    ses.PartnerPsychologist = ses.PartnerPsychologist == null
                        ? null
                        : new UserDTO(UserType.Psychologist.ToString(), ses.PartnerPsychologist.Name);
                    ses.Location.Psychologists = null;
                    ses.Client = null;
                    ses.Price = 0;
                    ses.Frequency = "";
                    ses.Slot = null;
                    ses.Description = "";
                    return ses;
                }).ToList();
                result.Psychologists = null;
                result.Clients = null;
                result.Slots = null;
                result.Locations = result.Locations.Select(loc =>
                {
                    loc.Psychologists = loc.Psychologists
                        .Select(psy => new UserDTO(UserType.Psychologist.ToString(), psy.Name)).ToList();
                    return loc;
                }).ToList();
                return result;
            default: return new UserDTO(UserType.Client.ToString());
        }
    }
}