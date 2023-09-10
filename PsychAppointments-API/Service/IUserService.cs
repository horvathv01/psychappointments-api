using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IUserService
{
     Task<bool> AddUser(UserDTO user);
     Task<User> GetUserById(long id);
     Task<User> GetUserByEmail(string email);
     Task<IEnumerable<User>> GetAllUsers();

     Task<IEnumerable<User>> GetAllPsychologists();
     
     Task<IEnumerable<User>> GetAllClients();
     
     Task<IEnumerable<User>> GetAllManagers();

     Task<IEnumerable<User>> GetListOfUsers(List<long> ids);
     Task<bool> UpdateUser(long id, UserDTO newUser);
     Task<bool> DeleteUser(long id);
}