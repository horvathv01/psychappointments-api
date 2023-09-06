using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class ManagerService : IManagerService
{
    //private readonly DbContext _context;
    private readonly IRepository<User> _userRepository;
    
    public ManagerService(
        //DbContext context
        IRepository<User> userRepository
    )
    {
        //_context = context;
        _userRepository = userRepository;
    }
    
    public async Task<bool> AddManager(Manager manager)
    {
        return await _userRepository.Add(manager);
    }

    public async Task<Manager?> GetManagerById(long id)
    {
        try
        {
            var user = await _userRepository.GetById(id);
            return (Manager)user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Manager?> GetManagerByEmail(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmail(email);
            return (Manager)user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<Manager>> GetAllManagers()
    {
        var allUsers = await _userRepository.GetAll();
        return allUsers.Where(us => us.Type == UserType.Manager).Select(us => (Manager)us).ToList();
    }

    public async Task<List<Manager>> GetListOfManagers(List<long> ids)
    {
        var allUsers = await _userRepository.GetList(ids);
        return allUsers.Select(us => (Manager)us).ToList();
    }

    public async Task<bool> UpdateManager(long id, Manager manager)
    {
        return await _userRepository.Update(id, manager);
    }
}