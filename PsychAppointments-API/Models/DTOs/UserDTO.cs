using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class UserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
    public string Password { get; set; }
    public long? RegisteredBy { get; set; }
    
    //admin: currently same as user but specific
    //client:
    public List<long>? SessionIds { get; set; } = null; //also: psychologist
    public List<long>? PsychologistIds { get; set; } = null;
    
    //psychologist:
    public List<long>? ClientIds { get; set; } = null;
    public List<long>? SlotIds { get; set; } = null;
    
    //manager:
    public List<long>? LocationIds { get; set; } = null;

    public UserDTO(
        long id, 
        string name, 
        string type, 
        string email, 
        string phone, 
        string dateOfBirth, 
        Address address, 
        string password, 
        long registeredBy = 0,
        List<long>? sessions = null,
        List<long>? psychologists = null,
        List<long>? clients = null,
        List<long>? slots = null,
        List<long>? locations = null
            )
    {
        DateTime birthDay = new DateTime(1994, 07, 24);
        DateTime.TryParse(dateOfBirth, out birthDay);
        
        Id = id;
        Name = name;
        Type = type;
        Email = email;
        Phone = phone;
        DateOfBirth = birthDay; 
        Address = address;
        Password = password;
        RegisteredBy = registeredBy;
        SessionIds = sessions; 
        PsychologistIds = psychologists;
        ClientIds = clients;
        SlotIds = slots;
        LocationIds = locations;
    }

    public UserDTO(User user)
    {
        switch (user.Type)
        {
            case UserType.Admin:
                //nothing to set
                break;
            case UserType.Client:
                Client client = (Client)user;
                SessionIds = client.Sessions.Select(ses => ses.Id).ToList();
                PsychologistIds = client.Psychologists.Select(psy => psy.Id).ToList();
                break;
            case UserType.Manager:
                Manager manager = (Manager)user;
                LocationIds = manager.Locations.Select(loc => loc.Id).ToList();
                break;
            case UserType.Psychologist:
                Psychologist psychologist = (Psychologist)user;
                SessionIds = psychologist.Sessions.Select(ses => ses.Id).ToList();
                ClientIds = psychologist.Clients.Select(cli => cli.Id).ToList();
                SlotIds = psychologist.Slots.Select(slot => slot.Id).ToList();
                break;
        }

        Id = user.Id;
        Name = user.Name;
        Type =  user.Type.ToString();
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth;
        Address = user.Address;
        Password = user.Password;
        RegisteredBy = user.RegisteredBy != null ? user.RegisteredBy.Id : null;
    }

    public UserDTO(string type, string name = "")
    { 
    Id = 0;
    Name = name == "" ? "" : name;
    Type = type;
    Email = "";
    Phone = "";
    DateOfBirth = DateTime.MinValue;
    Address = new Address();
    Password = "";
    RegisteredBy = 0;
    }

    public override string ToString()
    {
        return $"UserDTO Id: {Id}, Name: {Name}, Type: {Type}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {RegisteredBy}, " +
               $"Sessions: {SessionIds.Count}, Clients: {ClientIds.Count}, Slots: {SlotIds.Count}, " +
               $"Locations: {LocationIds.Count}, Psychologists: {PsychologistIds.Count}.";
    }

}