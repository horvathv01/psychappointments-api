using System.ComponentModel.DataAnnotations.Schema;

namespace PsychAppointments_API.Models;

public class Location
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public Address Address { get; set; }
    public List<Manager> Managers { get; set; }
    public List<Psychologist> Psychologists { get; set; }

    public Location(string name, Address address, List<Manager>? managers = null, List<Psychologist>? psychologists = null)
    {
        Name = name;
        Address = address;
        Managers = managers ?? new List<Manager>();
        Psychologists = psychologists ?? new List<Psychologist>();
    }

    public override bool Equals(object? obj)
    {
        return obj is Location
               && ((Location)obj).Id == Id
               && ((Location)obj).Name == Name
               && ((Location)obj).Address.Equals(Address);
    }
}