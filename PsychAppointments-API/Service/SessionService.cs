using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class SessionService : ISessionService
{
//private readonly DbContext _context;
    private readonly IRepository<Session> _sessionRepository;
    
    public SessionService(
        //DbContext context, 
        IRepository<Session> sessionRepository
    )
    {
        //_context = context;
        _sessionRepository = sessionRepository;
    }
    
    public async Task<bool> AddSession(Session session)
    {
        return await _sessionRepository.Add(session);
    }

    public async Task<Session?> GetSessionById(long id)
    {
        return await _sessionRepository.GetById(id);
    }

    public async Task<List<Session>> GetAllSessions()
    {
        var allSessions = await _sessionRepository.GetAll();
        return allSessions.ToList();
    }

    public async Task<List<Session>> GetListOfSessions(List<long> ids)
    {
        var sessions = await _sessionRepository.GetList(ids);
        return sessions.ToList();
    }

    public async Task<List<Session>> GetSessionsByLocation(Location location, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        var allSessions = await _sessionRepository.GetAll();
        var locationSessions = allSessions.Where(ses => ses.Location.Equals(location)).ToList();
        if (startOfRange == null || endOfRange == null)
        {
            return locationSessions;
        }

        return locationSessions.Where(ses =>
            ses.Start >= startOfRange
            && ses.End <= endOfRange
            ).ToList();
    }

    public List<Session> GetSessionsByPsychologist(Psychologist psychologist, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        return psychologist.Sessions.Where(ses =>
        ses.Start >= startOfRange
        && ses.End <= endOfRange
        ).ToList();
    }

    public List<Session> GetSessionsByClient(Client client, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        return client.Sessions.Where(ses =>
            ses.Start >= startOfRange
            && ses.End <= endOfRange
        ).ToList();
    }

    public async Task<List<Session>> GetSessionsByManager(Manager manager, DateTime? startOfRange = null, DateTime? endOfRange = null)
    {
        var allSessions = await _sessionRepository.GetAll();
        var managersSessions = allSessions.Where(ses => manager.Locations.Contains(ses.Location)).ToList();
        if (startOfRange == null || endOfRange == null)
        {
            return managersSessions;
        }
        return managersSessions.Where(ses => 
            ses.Start >= startOfRange
            && ses.End <= endOfRange
        ).ToList();
    }

    public async Task<List<Session>> GetSessionsByDate(DateTime startOfRange, DateTime endOfRange)
    {
        var allSessions = await _sessionRepository.GetAll();
        return allSessions.Where(ses =>
            ses.Start >= startOfRange
            && ses.End <= endOfRange
        ).ToList();
    }

    public async Task<bool> UpdateSession(long id, Session session)
    {
        try
        {
            var oldSes = await _sessionRepository.GetById(id);
            oldSes.Psychologist = session.Psychologist;
            oldSes.PartnerPsychologist = session.PartnerPsychologist;
            oldSes.Blank = session.Blank;
            oldSes.Location = session.Location;
            oldSes.Date = session.Date;
            oldSes.Start = session.Start;
            oldSes.End = session.End;
            oldSes.Client = session.Client;
            oldSes.Price = session.Price;
            oldSes.Frequency = session.Frequency;
            oldSes.Slot = session.Slot;
            oldSes.Description = session.Description;

            return await _sessionRepository.Update(id, session);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> DeleteSession(long id)
    {
        return await _sessionRepository.Delete(id);
    }
}