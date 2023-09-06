using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface ISessionService
{
    Task<bool> AddSession(Session session);
    
    Task<Session?> GetSessionById(long id);
    Task<List<Session>> GetAllSessions();
    
    Task<List<Session>> GetListOfSessions(List<long> ids);
    Task<List<Session>> GetSessionsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null);
    List<Session> GetSessionsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null);
    List<Session> GetSessionsByClient(Client client, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Session>> GetSessionsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null);
    Task<List<Session>> GetSessionsByDate(DateTime startOfRange, DateTime endOfRange);
    
    Task<bool> UpdateSession(long id, Session session);
    Task<bool> DeleteSession(long id);
}