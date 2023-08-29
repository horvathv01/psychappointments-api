using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class UserService : IUserService
{
    private readonly DbContext _context;
    
    public UserService(DbContext context)
    {
        _context = context;
    }
    
    
    public Task<bool> AddUser(User user)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<List<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateUser(long id, User user)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUser(long id)
    {
        throw new NotImplementedException();
    }
}