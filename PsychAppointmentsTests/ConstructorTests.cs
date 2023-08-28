using PsychAppointments_API.Models;

namespace PsychAppointmentsTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Address address = new Address("Hungary", "1196", "Budapest", "Petőfi utca", "134/b");
        Psychologist psychologist = new Psychologist("Juhos Melanie", "juhos@melanie.hu", "+36123456789", new DateTime(1994, 03, 10), address, "csirke");
        Location location = new Location("Tibavár utca", address);
        DateTime today = DateTime.Today;
        DateTime start = new DateTime(2023, 08, 28, 10, 0, 0);
        DateTime end = new DateTime(2023, 08, 28, 15, 0, 0);
        
        //length of time is 5 hours --> 5 sessions + 5 rests should fit
        int expectation = 5;
        Slot slot = new Slot(psychologist, location, today, start, end);
        
        Assert.That(slot.Sessions.Count, Is.EqualTo(expectation));
    }
}