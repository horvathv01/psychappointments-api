using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class Psychologist : User
{
    public List<Client> Clients { get; set; }
    public List<Slot> Slots { get; set; }
    public List<Session> Sessions { get; set; }
    
    public Psychologist(
        string name, 
        string email, 
        string phone, 
        DateTime dateOfBirth, 
        Address address, 
        string password, 
        List<Session>? sessions = null,
        List<Slot>? slots = null,
        List<Client>? clients = null,
        User? registeredBy = null
        ) : base(name, email, phone, dateOfBirth, address, password, registeredBy)
    {
        Type = UserType.Psychologist;
        Clients = clients ?? new List<Client>();
        Slots = slots ?? new List<Slot>();
        Sessions = sessions ?? new List<Session>();
    }
    
    public Psychologist(User user) : base()
    {
        Id = user.Id;
        Name = user.Name;
        Type = UserType.Psychologist;
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth;
        Address = user.Address;
        Password = user.Password;
        RegisteredBy = user.RegisteredBy;
        Sessions = ((Psychologist)user).Sessions;
        Slots = ((Psychologist)user).Slots;
        Clients = ((Psychologist)user).Clients;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Psychologist
               && ((Psychologist)obj).Id == Id
               && ((Psychologist)obj).Name == Name
               && ((Psychologist)obj).Address.Equals(Address)
               && ((Psychologist)obj).Email == Email
               && ((Psychologist)obj).Phone == Phone
               && ((Psychologist)obj).DateOfBirth == DateOfBirth
               && ((Psychologist)obj).Password == Password
               && ((Psychologist)obj).Type == Type
               && ((Psychologist)obj).RegisteredBy.Id == RegisteredBy.Id;
    }
    
    public override string ToString()
    {
        string sessionDetails = $"{Sessions.Count} + {Sessions.Select(ses => $"Sess. Id: {ses.Id}, Client Id: {ses.Client.Id}, Slot. Id: {ses.Slot.Id}")}";
        string registeredById = RegisteredBy == null ? "none" : RegisteredBy.Id.ToString();
        return $"Admin Id: {Id}, Name: {Name}, Type: {Enum.GetName(typeof(UserType), Type)}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {registeredById}, " +
               $"Sessions: {Sessions.Count}, Clients: {Clients.Count}, Slots: {Slots.Count}, SessionDetails: {sessionDetails}";
    }
}