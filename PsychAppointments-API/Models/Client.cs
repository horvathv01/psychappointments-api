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
}