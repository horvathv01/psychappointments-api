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
        User? registeredBy = null) : base(name, email, phone, dateOfBirth, address, password, registeredBy)
    {
        Type = UserType.Admin;
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
}