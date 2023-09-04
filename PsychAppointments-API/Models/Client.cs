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
        User? registeredBy = null,
        long id = 0) : base(name, email, phone, dateOfBirth, address, password, registeredBy, id)
    {
        Type = UserType.Client;
        Sessions = sessions ?? new List<Session>();
        Psychologists = psychologists ?? new List<Psychologist>();
    }
    
    public Client(User user) : base()
    {
        Id = user.Id;
        Name = user.Name;
        Type = UserType.Client;
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth;
        Address = user.Address;
        Password = user.Password;
        RegisteredBy = user.RegisteredBy;
        Sessions = ((Client)user).Sessions;
        Psychologists = ((Client)user).Psychologists;
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
    
    public override string ToString()
    {
        string registeredById = RegisteredBy == null ? "none" : RegisteredBy.Id.ToString();
        return $"Client Id: {Id}, Name: {Name}, Type: {Enum.GetName(typeof(UserType), Type)}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {registeredById}, " +
               $"Sessions: {Sessions.Count}, Psychologists: {Psychologists.Count}";
    }
}