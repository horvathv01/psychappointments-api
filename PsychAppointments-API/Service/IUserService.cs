using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IUserService
{
     Task<bool> AddUser(UserDTO user);
     Task<User> GetUserById(long id);
     Task<User> GetUserByEmail(string email);
     Task<List<User>> GetAllUsers();
     Task<bool> UpdateUser(long id, User user);
     Task<bool> DeleteUser(long id);
}