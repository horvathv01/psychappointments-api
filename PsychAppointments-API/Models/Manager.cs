namespace PsychAppointments_API.Models;

public class Manager : User
{
    public List<Location> Locations { get; set; }
    
    public Manager(string name, 
        string email, 
        string phone, 
        DateTime dateOfBirth, 
        Address address, 
        string password,
        List<Location>? locations = null,
        User? registeredBy = null) : base(name, email, phone, dateOfBirth, address, password, registeredBy)
    {
        Type = UserType.Manager;
        Locations = locations ?? new List<Location>();
    }
}