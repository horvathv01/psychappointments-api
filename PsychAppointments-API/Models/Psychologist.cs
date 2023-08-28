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
}