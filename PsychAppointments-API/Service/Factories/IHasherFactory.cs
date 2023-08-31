using Microsoft.AspNetCore.Identity;

namespace PsychAppointments_API.Service.Factories
{
    public interface IHasherFactory
    {
        PasswordHasher<string> GetHasher();
    }
}