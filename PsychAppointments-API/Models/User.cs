using System.ComponentModel.DataAnnotations.Schema;

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
    public User RegisteredBy { get; }

    public User(string name, string email, string phone, DateTime dateOfBirth, Address address, string password, User? registeredBy = null)
    {
        Name = name;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth;
        Address = address;
        Password = password;
        RegisteredBy = registeredBy;
    }
}