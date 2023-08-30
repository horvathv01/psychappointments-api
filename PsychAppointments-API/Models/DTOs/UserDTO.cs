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
    public long RegisteredBy { get; set; }
    
    //admin: currently same as user but specific
    //client:
    public List<SessionDTO>? Sessions { get; set; } = null; //also: psychologist
    public List<long>? Psychologists { get; set; } = null;
    
    //psychologist:
    public List<long>? Clients { get; set; } = null;
    public List<SlotDTO>? Slots { get; set; } = null;
    
    //manager:
    public List<LocationDTO>? Locations { get; set; } = null;

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
        List<SessionDTO>? sessions = null,
        List<long>? psychologists = null,
        List<long>? clients = null,
        List<SlotDTO>? slots = null,
        List<LocationDTO>? locations = null
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
        RegisteredBy = RegisteredBy;
        Sessions = sessions; 
        Psychologists = psychologists;
        Clients = clients;
        Slots = slots;
        Locations = locations;
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
                Sessions = client.Sessions.Select(ses => new SessionDTO(ses)).ToList();
                break;
            case UserType.Manager:
                Manager manager = (Manager)user;
                Locations = manager.Locations.Select(loc => new LocationDTO(loc)).ToList();
                break;
            case UserType.Psychologist:
                Psychologist psychologist = (Psychologist)user;
                Sessions = psychologist.Sessions.Select(ses => new SessionDTO(ses)).ToList();
                Clients = psychologist.Clients.Select(cli => cli.Id).ToList();
                Slots = psychologist.Slots.Select(slot => new SlotDTO(slot)).ToList();
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
        RegisteredBy = user.RegisteredBy.Id;
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
    
    



}