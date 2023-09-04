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

    public Address()
    {
        Country = "";
        Zip = "";
        City = "";
        Street = "";
        Rest = "";
    }

    public override bool Equals(object? obj)
    {
        return obj is Address
               && ((Address)obj).Country == Country
               && ((Address)obj).Zip == Zip
               && ((Address)obj).City == City
               && ((Address)obj).Street == Street
               && ((Address)obj).Rest == Rest;
    }

    public override string ToString()
    {
        return $"Country: {Country}, Zip: {Zip}, City: {City}, Street: {Street}, Rest: {Rest}";
    }
}