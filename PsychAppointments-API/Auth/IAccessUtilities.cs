using Microsoft.AspNetCore.Identity;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Auth;

public interface IAccessUtilities
{
    string HashPassword(string password, string userEmail);
    PasswordVerificationResult Authenticate(User? user, string password);
}