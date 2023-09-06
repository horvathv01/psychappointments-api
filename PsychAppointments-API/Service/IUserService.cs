using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IUserService
{
     Task<bool> AddUser(UserDTO user);
     Task<User> GetUserById(long id);
     Task<User> GetUserByEmail(string email);
     Task<List<User>> GetAllUsers();

     Task<List<User>> GetListOfUsers(List<long> ids);
     Task<bool> UpdateUser(long id, UserDTO newUser);
     Task<bool> DeleteUser(long id);
}