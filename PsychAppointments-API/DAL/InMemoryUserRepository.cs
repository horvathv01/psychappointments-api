using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.DAL;

public class InMemoryUserRepository : IRepository<User>
{
    private readonly List<User> _users;

    public InMemoryUserRepository()
    {
        _users = new List<User>();
        //Prepopulate();
    }

    private void Prepopulate()
    {
        //prepopulate with data
        Console.WriteLine("InMemoryUserRepository has been prepopulated.");
    }
    
    
    public async Task<User> GetById(long id)
    {
        var user = _users.FirstOrDefault(user => user.Id == id);
        return await Task.FromResult(user);
    }

    public async Task<User> GetByEmail(string email)
    {
        var user = _users.FirstOrDefault(user => user.Email == email);
        return await Task.FromResult(user);
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await Task.FromResult<IEnumerable<User>>(_users);
    }

    public async Task<IEnumerable<User>> GetList(List<long> ids)
    {
        return await Task.FromResult<IEnumerable<User>>(_users.Where(us => ids.Contains(us.Id)));
    }

    public async Task<bool> Add(User entity)
    {
        try
        {
            _users.Add(entity);
            return await Task.FromResult(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return await Task.FromResult(false);
        }
    }

    public async Task<bool> Update(long id, User entity)
    {
        try
        {
            var user = _users.FirstOrDefault(user => user.Id == id);
            if (user.Type != entity.Type)
            {
                //user type conversion?
                User newUser;
                switch (entity.Type)
                {
                    case UserType.Admin:
                        newUser = new Admin(entity);
                        break;
                    case UserType.Client:
                        newUser = new Client(entity);
                        break;
                    case UserType.Manager:
                        newUser = new Manager(entity);
                        break;
                    case UserType.Psychologist:
                        newUser = new Psychologist(entity);
                        break;
                    default:
                        //default is required for Add to work.
                        //user type is Client
                        newUser = new Client(entity);
                        break;
                }
                _users.Remove(user);
                _users.Add(newUser);
                return await Task.FromResult(true);    
            }
            //changing any other properties:
            user.Name = entity.Name;
            user.Email = entity.Email;
            user.Phone = entity.Phone;
            user.DateOfBirth = entity.DateOfBirth;
            user.Address = entity.Address;
            user.Password = entity.Password;
            //changing type-specific properties:
            switch (user.Type)
            {
                case UserType.Client:
                    ((Client)user).Psychologists = ((Client)entity).Psychologists;
                    ((Client)user).Sessions = ((Client)entity).Sessions;
                    break;
                case UserType.Manager:
                    ((Manager)user).Locations = ((Manager)entity).Locations;
                    break;
                case UserType.Psychologist:
                    ((Psychologist)user).Sessions = ((Psychologist)entity).Sessions;
                    ((Psychologist)user).Clients = ((Psychologist)entity).Clients;
                    ((Psychologist)user).Slots = ((Psychologist)entity).Slots;
                    break;
            }
            
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
            var user = _users.FirstOrDefault(user => user.Id == id);
            if (user != null)
            {
                _users.Remove(user);
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