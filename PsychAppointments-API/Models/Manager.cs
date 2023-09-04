using PsychAppointments_API.Models.Enums;

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
    
    public Manager(User user) : base()
    {
        Id = user.Id;
        Name = user.Name;
        Type = UserType.Manager;
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth;
        Address = user.Address;
        Password = user.Password;
        RegisteredBy = user.RegisteredBy;
        Locations = ((Manager)user).Locations;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Manager
               && ((Manager)obj).Id == Id
               && ((Manager)obj).Name == Name
               && ((Manager)obj).Address.Equals(Address)
               && ((Manager)obj).Email == Email
               && ((Manager)obj).Phone == Phone
               && ((Manager)obj).DateOfBirth == DateOfBirth
               && ((Manager)obj).Password == Password
               && ((Manager)obj).Type == Type
               && ((Manager)obj).RegisteredBy.Id == RegisteredBy.Id;
    }
    
    public override string ToString()
    {
        string locationDetails = $"{Locations.Select(loc => $"Loc. Id: {loc.Id}, Loc. Name: {loc.Name}")}";
        string registeredById = RegisteredBy == null ? "none" : RegisteredBy.Id.ToString();
        return $"Admin Id: {Id}, Name: {Name}, Type: {Enum.GetName(typeof(UserType), Type)}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {registeredById}, " +
               $"Locations: {Locations.Count}, LocationDetails: {locationDetails}";
    }
}