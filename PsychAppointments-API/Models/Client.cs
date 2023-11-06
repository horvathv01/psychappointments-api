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
        Id = id;
        Type = UserType.Client;
        Sessions = sessions ?? new List<Session>();
        Psychologists = psychologists ?? new List<Psychologist>();
        RegisteredBy = registeredBy == null ? null : registeredBy;
    }

    public Client()
    {
        
    }
    
    public Client(User user)
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
        return obj is Client otherClient
               && otherClient.Id == Id
               && otherClient.Name == Name
               && otherClient.Address.Equals(Address)
               && otherClient.DateOfBirth == DateOfBirth
               && otherClient.Email == Email
               && otherClient.Phone == Phone
               && otherClient.Password == Password
               && otherClient.Type == Type
               && (RegisteredBy == null || (otherClient.RegisteredBy != null && otherClient.RegisteredBy.Id == RegisteredBy.Id));
    }
    
    public override string ToString()
    {
        string registeredById = RegisteredBy == null ? "none" : RegisteredBy.Id.ToString();
        return $"Client Id: {Id}, Name: {Name}, Type: {Enum.GetName(typeof(UserType), Type)}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {registeredById}, " +
               $"Sessions: {Sessions.Count}, Psychologists: {Psychologists.Count}";
    }
}