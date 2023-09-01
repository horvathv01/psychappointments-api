using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service.DataProtection;

public class AdminDataProtectionService : IDataProtectionService<Admin>
{
    public bool IsAssociated(Admin user, Session session)
    {
        return true;
    }

    public bool IsAssociated(Admin user, Slot slot)
    {
        return true;
    }

    public bool IsAssociated(Admin user, User otherUser)
    {
        return true;
    }

    public async Task<IEnumerable<LocationDTO>> Filter(Admin user, Func<Task<IEnumerable<Location>>> query)
    {
        var result = await query();
        return result.Select(loc => new LocationDTO(loc));
    }

    public async Task<LocationDTO> Filter(Admin user, Func<Task<Location>> query)
    {
        var result = await query();
        return new LocationDTO(result);
    }

    public async Task<IEnumerable<SessionDTO>> Filter(Admin user, Func<Task<IEnumerable<Session>>> query)
    {
        var result = await query();
        return result.Select(ses => new SessionDTO(ses));
    }

    public async Task<SessionDTO> Filter(Admin user, Func<Task<Session>> query)
    {
        var result = await query();
        return new SessionDTO(result);
    }

    public async Task<IEnumerable<SlotDTO>> Filter(Admin user, Func<Task<IEnumerable<Slot>>> query)
    {
        var result = await query();
        return result.Select(slot => new SlotDTO(slot));
    }

    public async Task<SlotDTO> Filter(Admin user, Func<Task<Slot>> query)
    {
        var result = await query();
        return new SlotDTO(result);
    }

    public async Task<IEnumerable<UserDTO>> Filter(Admin user, Func<Task<IEnumerable<User>>> query)
    { 
        var result = await query();
        return result.Select(u => new UserDTO(u));
    }

    public async Task<UserDTO> Filter(Admin user, Func<Task<User>> query)
    {
        var result = await query();
        return new UserDTO(result);
    }

    public SessionDTO FilterData(Admin user, Session session)
    {
        return new SessionDTO(session);
    }

    public SlotDTO FilterData(Admin user, Slot slot)
    {
        return new SlotDTO(slot);
    }

    public UserDTO FilterData(Admin user, User otherUser)
    {
        return new UserDTO(otherUser);
    }
}