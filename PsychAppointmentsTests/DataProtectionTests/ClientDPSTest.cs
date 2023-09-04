using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointmentsTests.DataProtectionTests;

public class ClientDPSTest
{
   private InMemoryLocationRepository? _locations;
    private InMemorySessionRepository? _sessions;
    private InMemorySlotRepository? _slots;
    private InMemoryUserRepository? _users;

    private ClientDataProtectionService? _adminDps;
    
    [SetUp]
    public async void Setup()
    { 
        _locations = new InMemoryLocationRepository();
        _sessions = new InMemorySessionRepository();
        _slots = new InMemorySlotRepository();
        _users = new InMemoryUserRepository();
        _adminDps = new ClientDataProtectionService();
        
        string emailEnd = "@psychappointments.com";
        string phone = "+361/123-4567";
        DateTime birthday = DateTime.MinValue;
        Address adminAddress = new Address("Hungary", "1196", "Budapest", "Petőfi utca", "134/a");
        //add one admin
        Admin admin = new Admin("Admin1", "admin1" + emailEnd, phone, birthday, adminAddress, "1234");
        
        //add one psychologist
        List<Session> psychologistSessions = new List<Session>();
        List<Slot> slots = new List<Slot>();
        List<Client> clients = new List<Client>();
        Address psychologistAddress = new Address("Hungary", "1996", "Petőfi utca", "134/p");
        Psychologist psychologist = new Psychologist("Psychologist1", "psychologist1" + emailEnd, phone, 
            birthday, psychologistAddress, "1234", psychologistSessions, slots, clients, admin);
        
        //add one manager
        List<Location> managerLocations = new List<Location>(); 
        Address managerAddress = new Address("Hungary", "1996", "Petőfi utca", "134/m");
        Manager manager = new Manager("Manager1", "manager1" + emailEnd, phone, birthday, managerAddress, "1234", managerLocations, admin);
        
        //add one client
        List<Session> clientSessions = new List<Session>();
        List<Psychologist> clientPsychologists = new List<Psychologist>();
        Address clientAddress = new Address("Hungary", "1996", "Petőfi utca", "134/c");
        Client client = new Client("Client1", "client1" + emailEnd, phone, birthday, clientAddress, "1234", clientSessions, clientPsychologists, manager);
        
        //add one location
        List<Manager> locationManagers = new List<Manager>();
        List<Psychologist> locationPsychologists = new List<Psychologist>();
        Address locationAddress = new Address("Hungary", "1996", "Petőfi utca", "134/l");
        Location location = new Location("Location1", locationAddress, locationManagers, locationPsychologists);
        
        //add one slot
        DateTime day = new DateTime(2023,09,04);
        DateTime slotStart = new DateTime(2023,09, 04, 12, 00, 00);
        DateTime slotEnd = new DateTime(2023,09, 04, 18, 00, 00);
        List<Session> slotSessions = new List<Session>();
        Slot slot = new Slot(psychologist, location, day, slotStart, slotEnd, 50, 10, false, slotSessions);
        
        //add one session
        DateTime sessionStart = new DateTime(2023,09, 04, 13, 00, 00);
        DateTime sessionEnd = new DateTime(2023,09, 04, 14, 00, 00);
        Session session = new Session(psychologist, location, day, sessionStart, sessionEnd, slot, 10000, false,
            "trial", SessionFrequency.None, client, psychologist);
        
        //establish connections
        psychologist.Sessions.Add(session);
        psychologist.Slots.Add(slot);
        psychologist.Clients.Add(client);
        
        manager.Locations.Add(location);
        client.Sessions.Add(session);
        client.Psychologists.Add(psychologist);
        
        location.Psychologists.Add(psychologist);
        location.Managers.Add(manager);
        
        slot.Sessions.Add(session);

        await _users.Add(admin);
        await _users.Add(psychologist);
        await _users.Add(manager);
        await _users.Add(client);
        await _locations.Add(location);
        await _slots.Add(slot);
        await _sessions.Add(session);
    }

    [TearDown]
    public void TearDown()
    {
        _locations = null;
        _sessions = null;
        _slots = null;
        _users = null;
    }

    [Test]
    public void TestSessionAccess1()
    {
        //user is associated with session
    }

    [Test]
    public void TestSessionAccess2()
    {
        //user is not associated with session
    }

    [Test]
    public void TestSlotAccess1()
    {
        //user is associated with slot    
    }

    [Test]
    public void TestSlotAccess2()
    {
        //user is not associated with slot
    }

    [Test]
    public void TestLocationAccess1()
    {
        //user is associated with location
    }
    
    [Test]
    public void TestLocationAccess2()
    {
        //user is not associated with location
    }

    [Test]

    public void TestClientAccess1()
    {
        //user is associated with client
    }
    
    [Test]
    public void TestClientAccess2()
    {
        //user is not associated with client
    }
    
    [Test]

    public void TestManagerAccess1()
    {
        //user is associated with manager
    }
    
    [Test]
    public void TestManagerAccess2()
    {
        //user is not associated with manager
    }
    
    [Test]

    public void TestPsychologistAccess1()
    {
        //user is associated with psychologist
    }
    
    [Test]
    public void TestPsychologistAccess2()
    {
        //user is not associated with psychologist
    }
    
    [Test]

    public void TestAdminAccess1()
    {
        //admins should be able to see other admins
    }
}