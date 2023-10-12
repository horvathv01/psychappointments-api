using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface ISessionService
{
    Task<bool> AddSession(SessionDTO session);
    
    Task<Session?> GetSessionById(long id);
    Task<IEnumerable<Session>> GetAllSessions();
    Task<IEnumerable<Session>> GetNonBlankSessions();
    
    Task<List<Session>> GetListOfSessions(List<long> ids);
    Task<IEnumerable<Session>> GetSessionsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<IEnumerable<Session>> GetSessionsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<IEnumerable<Session>> GetSessionsByPsychologistLocationAndDates(Psychologist psychologist,
        Location location, DateTime? startOfRange = null, DateTime? endOfRange = null);
    List<Session> GetSessionsByClient(Client client, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Session>> GetSessionsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Session>> GetSessionsByDate(DateTime startOfRange, DateTime endOfRange);
    Task<bool> UpdateSession(long id, SessionDTO session);
    Task<bool> DeleteSession(long id);

    bool Overlap(SessionDTO session1, SessionDTO session2);
}