using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service.DataProtection;

/// <summary>
/// Typed Data Protection Service (type: user types: Admin, Client, Manager, Psychologist). State of being associated to types
/// is decided based on user identity. State of being associated and type decides filtering need and/or amount.
/// This layer should go between controller and service classes.
/// Filter methods receive user and executable requests, and return DTOs of desired type, which contain filtered data.
/// </summary>
/// <typeparam name="T">Type of User</typeparam>

public interface IDataProtectionService<T>
{
    bool IsAssociated(T user, Session session);
    bool IsAssociated(T user, Slot slot);

    bool IsAssociated(T user, User otherUser);

    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with location, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>List of LocationDTOs, containing filtered information.</returns>
    Task<IEnumerable<LocationDTO>> Filter(T user, Func<Task<IEnumerable<Location>>> query);
    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with location, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>LocationDTO, containing filtered information.</returns>
    Task<LocationDTO> Filter(T user, Func<Task<Location>> query);

    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with session, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>List of SessionDTOs, containing filtered information.</returns>
    Task<IEnumerable<SessionDTO>> Filter(T user, Func<Task<IEnumerable<Session>>> query);
    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with session, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>SessionDTO, containing filtered information.</returns>
    Task<SessionDTO> Filter(T user, Func<Task<Session>> query);

    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with slot, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>List of SlotDTOs, containing filtered information.</returns>
    Task<IEnumerable<SlotDTO>> Filter(T user, Func<Task<IEnumerable<Slot>>> query);
    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with slot, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>SlotDTO, containing filtered information.</returns>
    Task<SlotDTO> Filter(T user, Func<Task<Slot>> query);

    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with user, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>List of UserDTOs, containing filtered information.</returns>
    Task<IEnumerable<UserDTO>> Filter(T user, Func<Task<IEnumerable<User>>> query);
    
    /// <summary>
    /// Filters out data based on user type and identity. If user is not associated with session, pieces of data are filtered out.  
    /// </summary>
    /// <param name="user">The user who is the basis of the association check, filtering is based on user.Type.</param>
    /// <param name="query">Function call of service class that returns query results with data from DB (async).</param>
    /// <returns>UserDTO, containing filtered information.</returns>
    Task<UserDTO> Filter(T user, Func<Task<User>> query);
}