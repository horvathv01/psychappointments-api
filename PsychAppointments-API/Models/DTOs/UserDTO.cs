namespace PsychAppointments_API.Models;

public class UserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
    public string Password { get; set; }
    public User RegisteredBy { get; }
    
    //admin: currently same as user but specific
    //client:
    public List<Session>? Sessions { get; set; } = null; //also: psychologist
    public List<Psychologist>? Psychologists { get; set; } = null;
    
    //psychologist:
    public List<Client>? Clients { get; set; } = null;
    public List<Slot>? Slots { get; set; } = null;
    
    //manager:
    public List<Location>? Locations { get; set; } = null;

    public UserDTO(User user)
    {
        switch (user.Type)
        {
            case UserType.Admin:
                //nothing extra to set
                break;
            case UserType.Client:
                Client client = (Client)user;
                Sessions = client.Sessions;
                break;
            case UserType.Manager:
                break;
            case UserType.Psychologist:
                break;
        }
    }



}