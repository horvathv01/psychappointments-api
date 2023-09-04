using System.Reflection;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointmentsTests.DataProtectionTests;

public class AdminDPSTest
{
    private InMemoryLocationRepository? _locations;
    private InMemorySessionRepository? _sessions;
    private InMemorySlotRepository? _slots;
    private InMemoryUserRepository? _users;

    private AdminDataProtectionService? _adminDps;

    private Admin _admin;
    
    [SetUp]
    public async Task Setup()
    { 
        _locations = new InMemoryLocationRepository();
        _sessions = new InMemorySessionRepository();
        _slots = new InMemorySlotRepository();
        _users = new InMemoryUserRepository();
        _adminDps = new AdminDataProtectionService();
        
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
        
        //add value to admin:
        _admin = (Admin) await _users.GetByEmail("admin1@psychappointments.com");
    }

    [TearDown]
    public void TearDown()
    {
        _locations = null;
        _sessions = null;
        _slots = null;
        _users = null;
    }

    private bool NoPropertyIsNull(Object obj)
    {
        Type type = obj.GetType();
        foreach (var propertyInfo in type.GetProperties())
        {
            object propValue = propertyInfo.GetValue(obj);
            if(propValue == null)
            {
                Console.WriteLine($"This is null: {propertyInfo}");
                return false;
            }
        }
        return true;
    }

    [Test]
    public async Task TestSessionAccess()
    {
        var query = async () => await _sessions.GetById(0);
        var queryResult = await _adminDps.Filter(_admin, query);

        bool result = NoPropertyIsNull(queryResult);
        Assert.That(result);
    }
    
    [Test]
    public async Task TestSlotAccess1()
    {
        var query = async () => await _slots.GetById(0);
        var queryResult = await _adminDps.Filter(_admin, query);

        bool result = NoPropertyIsNull(queryResult);
        Assert.That(result);    
    }


    [Test]
    public async Task TestLocationAccess1()
    {
        var query = async () => await _locations.GetById(0);
        var queryResult = await _adminDps.Filter(_admin, query);

        bool result = NoPropertyIsNull(queryResult);
        Assert.That(result);
    }

    [Test]

    public async Task TestClientAccess1()
    {
        var query = async () => await _users.GetByEmail("client1" + "@psychappointments.com");
        var queryResult = await _adminDps.Filter(_admin, query);
        
        bool result =
            queryResult.Id != null &&
            queryResult.Name != null &&
            queryResult.Type != null &&
            queryResult.Email != null &&
            queryResult.Phone != null &&
            queryResult.DateOfBirth != null &&
            queryResult.Address != null &&
            queryResult.Password != null &&
            queryResult.RegisteredBy != null &&
            queryResult.SessionIds != null &&
            queryResult.PsychologistIds != null;
        Assert.That(result);
    }
    
    [Test]

    public async Task TestManagerAccess1()
    {
        var query = async () => await _users.GetByEmail("manager1" + "@psychappointments.com");
        var queryResult = await _adminDps.Filter(_admin, query);


        bool result =
            queryResult.Id != null &&
            queryResult.Name != null &&
            queryResult.Type != null &&
            queryResult.Email != null &&
            queryResult.Phone != null &&
            queryResult.DateOfBirth != null &&
            queryResult.Address != null &&
            queryResult.Password != null &&
            queryResult.RegisteredBy != null &&
            queryResult.LocationIds != null;
        
        Assert.That(result);
    }
    
    [Test]

    public async Task TestPsychologistAccess1()
    {
        var query = async () => await _users.GetByEmail("psychologist1" + "@psychappointments.com");
        var queryResult = await _adminDps.Filter(_admin, query);

        bool result =
            queryResult.Id != null &&
            queryResult.Name != null &&
            queryResult.Type != null &&
            queryResult.Email != null &&
            queryResult.Phone != null &&
            queryResult.DateOfBirth != null &&
            queryResult.Address != null &&
            queryResult.Password != null &&
            queryResult.RegisteredBy != null &&
            queryResult.SessionIds != null &&
            queryResult.SlotIds != null &&
            queryResult.ClientIds != null;
        
        Assert.That(result);
    }
    
    [Test]

    public async Task TestAdminAccess1()
    {
        var query = async () => await _users.GetByEmail("admin1"+ "@psychappointments.com");
        var queryResult = await _adminDps.Filter(_admin, query);

        bool result =
            queryResult.Id != null &&
            queryResult.Name != null &&
            queryResult.Type != null &&
            queryResult.Email != null &&
            queryResult.Phone != null &&
            queryResult.DateOfBirth != null &&
            queryResult.Address != null &&
            queryResult.Password != null;
            
        Assert.That(result);
    }
}