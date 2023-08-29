namespace PsychAppointments_API.Models;

public class LocationDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<UserDTO> Managers { get; set; }
    public List<UserDTO> Psychologists { get; set; }

    public LocationDTO(string name, Address address, List<Manager>? managers = null, List<Psychologist>? psychologists = null)
    {
        Name = name;
        Address = address;
        Managers = managers != null ? managers.Select(man => new UserDTO(man)).ToList() : new List<UserDTO>();
        Psychologists = psychologists != null ? psychologists.Select(psy => new UserDTO(psy)).ToList() : new List<UserDTO>();
    }

    public LocationDTO(Location location)
    {
        Id = location.Id;
        Name = location.Name;
        Address = location.Address;
        Managers = location.Managers.Select(man => new UserDTO(man)).ToList();
        Psychologists = location.Psychologists.Select(psy => new UserDTO(psy)).ToList();
    }
}