namespace PsychAppointments_API.Models;

public class Admin : User
{
    public Admin(string name, 
        string email, 
        string phone, 
        DateTime dateOfBirth, 
        Address address, 
        string password,
        User? registeredBy = null) : base(name, email, phone, dateOfBirth, address, password, registeredBy)
    {
        Type = UserType.Admin;
    }
}