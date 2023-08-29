using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class SessionService : ISessionService
{
    private readonly DbContext _context;
    
    public SessionService(DbContext context)
    {
        _context = context;
    }
    
    public Task<bool> AddSession(Session session)
    {
        throw new NotImplementedException();
    }

    public Task<Session> GetSessionById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Session>> GetAllSessions()
    {
        throw new NotImplementedException();
    }

    public Task<List<Session>> GetSessionsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Session>> GetSessionsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Session>> GetSessionsByClient(Client client, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Session>> GetSessionsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        throw new NotImplementedException();
    }

    public Task<List<Session>> GetSessionsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSession(long id, Session session)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSession(long id)
    {
        throw new NotImplementedException();
    }
}