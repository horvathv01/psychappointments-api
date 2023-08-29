using Microsoft.AspNetCore.Identity;

namespace PsychAppointments_API.Service.Factories;

public class HasherFactory : IHasherFactory
{
    public PasswordHasher<string> GetHasher()
    {
        return new PasswordHasher<string>();
    }
}