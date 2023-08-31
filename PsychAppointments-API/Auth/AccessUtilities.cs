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

    public PasswordVerificationResult Authenticate(User? user, string password)
    {
        if (user == null)
        {
            return PasswordVerificationResult.Failed;
        }

        string salt = GetSalt(user.Email);
        var result = _hasherFactory.GetHasher().VerifyHashedPassword(salt, user.Password, password);
        return result;
    }

    private string GetSalt(string userEmail)
    {
        string salt = "";
        var arr = String.Concat(userEmail.OrderBy(ch => ch)).ToArray();
        for (int i = 0; i < 5; i++)
        {
            salt += arr[i];
        }

        return salt;
    }
}