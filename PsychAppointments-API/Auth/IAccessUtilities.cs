using PsychAppointments_API.Models;

namespace PsychAppointments_API.Auth;

public interface IAccessUtilities
{
    string HashPassword(string password, string userEmail);
    Task<bool> Authenticate(User? user, string password);
}