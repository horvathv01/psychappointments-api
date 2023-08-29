using PsychAppointments_API.Models;
using Microsoft.AspNetCore.Identity;
using PsychAppointments_API.Service.Factories;

namespace PsychAppointments_API.Auth;

public class AccessUtilities : IAccessUtilities
{
    private readonly IHasherFactory _hasherFactory;

    public AccessUtilities(IHasherFactory hasherFactory)
    {
        _hasherFactory = hasherFactory;
    }
    
    public string HashPassword(string password, string userEmail)
    {
        string salt = GetSalt(userEmail);   
        return _hasherFactory.GetHasher().HashPassword(salt, password);
    }

    public async Task<bool> Authenticate(User? user, string password)
    {
        if (user == null)
        {
            return false;
        }

        string salt = GetSalt(user.Email);
        var result = _hasherFactory.GetHasher().VerifyHashedPassword(salt, user.Password, password);
        return result == PasswordVerificationResult.Success;
    }

    private string GetSalt(string userEmail)
    {
        string salt = "";
        for (int i = 0; i < 5; i++)
        {
            salt += String.Concat(userEmail.OrderBy(ch => ch)).ToArray()[i];    
        }

        return salt;
    }
}