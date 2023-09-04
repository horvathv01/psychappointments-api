namespace PsychAppointments_API.Models;

public class LocationDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<long> ManagerIds { get; set; }
    public List<long> PsychologistIds { get; set; }

    public LocationDTO(string name, Address address, List<Manager>? managers = null, List<Psychologist>? psychologists = null)
    {
        Name = name;
        Address = address;
        ManagerIds = managers != null ? managers.Select(man => man.Id).ToList() : new List<long>();
        PsychologistIds = psychologists != null ? psychologists.Select(psy => psy.Id).ToList() : new List<long>();
    }

    public LocationDTO(Location location)
    {
        Id = location.Id;
        Name = location.Name;
        Address = location.Address;
        ManagerIds = location.Managers.Select(man => man.Id).ToList();
        PsychologistIds = location.Psychologists.Select(psy => psy.Id).ToList();
    }

    public override bool Equals(Object obj)
    {
        return obj is LocationDTO &&
               ((LocationDTO)obj).Address.Equals(Address) &&
               ((LocationDTO)obj).Id == Id &&
               ((LocationDTO)obj).Name == Name &&
               ManagerIds.SequenceEqual(((LocationDTO)obj).ManagerIds) &&
               PsychologistIds.SequenceEqual(((LocationDTO)obj).PsychologistIds);
    }
    
    public override string ToString()
    {
        return $"LocationId: {Id}, Name: {Name}, Address: {Address}, Managers: {ManagerIds.Count}, Psychologists: {PsychologistIds.Count}";
    }
}