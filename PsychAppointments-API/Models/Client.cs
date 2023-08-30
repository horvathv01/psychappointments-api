using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class Client : User
{
    public List<Session> Sessions { get; set; }
    public List<Psychologist> Psychologists { get; set; }
    
    public Client(
        string name, 
        string email, 
        string phone, 
        DateTime dateOfBirth, 
        Address address, 
        string password, 
        List<Session>? sessions = null,
        List<Psychologist>? psychologists = null,
        User? registeredBy = null) : base(name, email, phone, dateOfBirth, address, password, registeredBy)
    {
        Type = UserType.Client;
        Sessions = sessions ?? new List<Session>();
        Psychologists = psychologists ?? new List<Psychologist>();
    }

    public override bool Equals(object? obj)
    {
        return obj is Client
               && ((Client)obj).Id == Id
               && ((Client)obj).Name == Name
               && ((Client)obj).Address.Equals(Address)
               && ((Client)obj).DateOfBirth == DateOfBirth
               && ((Client)obj).Email == Email
               && ((Client)obj).Phone == Phone
               && ((Client)obj).Password == Password
               && ((Client)obj).Type == Type
               && ((Client)obj).RegisteredBy.Id == RegisteredBy.Id;
    }
}