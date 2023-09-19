using System.ComponentModel.DataAnnotations.Schema;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public abstract class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
    public string Password { get; set; }
    [ForeignKey("RegisteredById")]
    public long RegisteredById { get; set; }
    public User? RegisteredBy { get; set; }

    public User(string name, string email, string phone, DateTime dateOfBirth, Address address, string password, User? registeredBy = null, long id = 0)
    {
        Name = name;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth;
        Address = address;
        Password = password;
        RegisteredBy = registeredBy;
        Id = id;
    }

    public User()
    {
        
    }

    public override bool Equals(Object obj)
    {
        return obj is User &&
               ((User)obj).Name == Name &&
               ((User)obj).Email == Email &&
               ((User)obj).Phone == Phone &&
               ((User)obj).DateOfBirth == DateOfBirth &&
               ((User)obj).Address.Equals(Address) &&
               ((User)obj).Password == Password &&
               ((User)obj).RegisteredBy.Equals(RegisteredBy) &&
               ((User)obj).Id == Id &&
               ((User)obj).Type == Type;
    }
    
}