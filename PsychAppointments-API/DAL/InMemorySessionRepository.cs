using PsychAppointments_API.Models;

namespace PsychAppointments_API.DAL;

public class InMemorySessionRepository : IRepository<Session>
{
   private readonly List<Session> _sessions;
    
    public InMemorySessionRepository()
    {
        _sessions = new List<Session>();
        //Prepopulate();
    }

    private void Prepopulate()
    {
        //prepopulate with data
        Console.WriteLine("InMemorySessionRepository has been prepopulated.");
    }
    
    public async Task<Session> GetById(long id)
    {
        var session = _sessions.FirstOrDefault(ses => ses.Id == id);
        return await Task.FromResult(session);
    }

    public async Task<Session> GetByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Session>> GetAll()
    {
        return await Task.FromResult<IEnumerable<Session>>(_sessions);
    }
    
    public async Task<IEnumerable<Session>> GetList(List<long> ids)
    {
        return await Task.FromResult<IEnumerable<Session>>(_sessions.Where(us => ids.Contains(us.Id)));
    }

    public async Task<bool> Add(Session entity)
    {
        try
        {
            _sessions.Add(entity);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> Update(long id, Session entity)
    {
        try
        {
            var session = _sessions.FirstOrDefault(ses => ses.Id == id);
            session.Psychologist = entity.Psychologist;
            session.PartnerPsychologist = entity.PartnerPsychologist;
            session.Blank = entity.Blank;
            session.Location = entity.Location;
            session.Date = entity.Date;
            session.Start = entity.Start;
            session.End = entity.End;
            session.Client = entity.Client;
            session.Price = entity.Price;
            session.Frequency = entity.Frequency;
            session.Slot = entity.Slot;
            session.Description = entity.Description;
        
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> Delete(long id)
    {
        try
        {
            var session = _sessions.FirstOrDefault(ses => ses.Id == id);
            if (session != null)
            {
                _sessions.Remove(session);
                return await Task.FromResult(true);    
            }
            return await Task.FromResult(false);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }
}