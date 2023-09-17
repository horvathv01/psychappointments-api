using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class Admin : User
{
    public Admin(string name, 
        string email, 
        string phone, 
        DateTime dateOfBirth, 
        Address address, 
        string password,
        User? registeredBy = null, 
        long id = 0) : base(name, email, phone, dateOfBirth, address, password, registeredBy, id)
    {
        Type = UserType.Admin;
    }

    public Admin()
    {
        
    }

    public Admin(User user) : base()
    {
        Id = user.Id;
        Name = user.Name;
        Type = UserType.Admin;
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth;
        Address = user.Address;
        Password = user.Password;
        RegisteredBy = user.RegisteredBy;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Admin
               && ((Admin)obj).Id == Id
               && ((Admin)obj).Name == Name
               && ((Admin)obj).Address.Equals(Address)
               && ((Admin)obj).Email == Email
               && ((Admin)obj).Phone == Phone
               && ((Admin)obj).DateOfBirth == DateOfBirth
               && ((Admin)obj).Password == Password
               && ((Admin)obj).Type == Type
               && ((Admin)obj).RegisteredBy.Id == RegisteredBy.Id;
    }

    public override string ToString()
    {
        string registeredById = RegisteredBy == null ? "none" : RegisteredBy.Id.ToString();
        return $"Admin Id: {Id}, Name: {Name}, Type: {Enum.GetName(typeof(UserType), Type)}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {registeredById}";
    }
}