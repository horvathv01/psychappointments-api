namespace PsychAppointments_API.Models;

public class Address
{
    public string Country { get; set; }
    public string Zip { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string Rest { get; set; }

    public Address(string country = "", string zip = "", string city = "", string street = "", string rest = "")
    {
        Country = country;
        Zip = zip;
        City = city;
        Street = street;
        Rest = rest;
    }
}