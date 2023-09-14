using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class UserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string DateOfBirth { get; set; }
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

    [Newtonsoft.Json.JsonConstructor]
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
        Id = id;
        Name = name;
        Type = type;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth; 
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
        DateOfBirth = user.DateOfBirth.ToString();
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
    DateOfBirth = DateTime.MinValue.ToString();
    Address = new Address();
    Password = "";
    RegisteredBy = null;
    }

    public override string ToString()
    {
        string sessions = SessionIds != null ? SessionIds.Count.ToString() : "null";
        string clients = ClientIds != null ? ClientIds.Count.ToString() : "null";
        string slots = SlotIds != null ? SlotIds.Count.ToString() : "null";
        string locations = LocationIds != null ? LocationIds.Count.ToString() : "null";
        string psychologists = PsychologistIds != null ? PsychologistIds.Count.ToString() : "null";
        
        return $"UserDTO Id: {Id}, Name: {Name}, Type: {Type}, Email: {Email}, " +
               $"Phone: {Phone}, DateOfBirth: {DateOfBirth}, Address: {Address}, Password: {Password}, RegisteredBy: {RegisteredBy}, " +
               $"Sessions: {sessions}, Clients: {clients}, Slots: {slots}, " +
               $"Locations: {locations}, Psychologists: {psychologists}.";
    }

}