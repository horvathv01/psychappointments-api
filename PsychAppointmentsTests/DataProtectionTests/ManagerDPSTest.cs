using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointmentsTests.DataProtectionTests;

public class ManagerDPSTest
{
    private InMemoryLocationRepository? _locations;
    private InMemorySessionRepository? _sessions;
    private InMemorySlotRepository? _slots;
    private InMemoryUserRepository? _users;
    
    private ManagerDataProtectionService? _managerDPS;

    private Psychologist _psychologist;
    private Manager _manager;
    
    [SetUp]
    public async Task Setup()
    { 
        _locations = new InMemoryLocationRepository();
        _sessions = new InMemorySessionRepository();
        _slots = new InMemorySlotRepository();
        _users = new InMemoryUserRepository();
        _managerDPS = new ManagerDataProtectionService();
        
        string emailEnd = "@psychappointments.com";
        string phone = "+361/123-4567";
        DateTime birthday = DateTime.MinValue;
        Address adminAddress = new Address("Hungary", "1196", "Budapest", "Petőfi utca", "134/a");
        //add one admin
        Admin admin = new Admin("Admin1", "admin1" + emailEnd, phone, birthday, adminAddress, "1234", null, 1);
        
        //add one psychologist
        List<Session> psychologistSessions = new List<Session>();
        List<Slot> slots = new List<Slot>();
        List<Client> clients = new List<Client>();
        Address psychologistAddress = new Address("Hungary", "1996", "Petőfi utca", "134/p");
        Psychologist psychologist = new Psychologist("Psychologist1", "psychologist1" + emailEnd, phone, 
            birthday, psychologistAddress, "1234", psychologistSessions, slots, clients, admin, 2);
        
        //add one manager
        List<Location> managerLocations = new List<Location>(); 
        Address managerAddress = new Address("Hungary", "1996", "Petőfi utca", "134/m");
        Manager manager = new Manager("Manager1", "manager1" + emailEnd, phone, birthday, managerAddress, "1234", managerLocations, admin, 99);
        
        //add one client
        List<Session> clientSessions = new List<Session>();
        List<Psychologist> clientPsychologists = new List<Psychologist>();
        Address clientAddress = new Address("Hungary", "1996", "Petőfi utca", "134/c");
        Client client = new Client("Client1", "client1" + emailEnd, phone, birthday, clientAddress, 
            "1234", clientSessions, clientPsychologists, manager, 3);
        
        //add one location
        List<Manager> locationManagers = new List<Manager>();
        List<Psychologist> locationPsychologists = new List<Psychologist>();
        Address locationAddress = new Address("Hungary", "1996", "Petőfi utca", "134/l");
        Location location = new Location("Location1", locationAddress, locationManagers, locationPsychologists, 1);
        
        //add one slot
        DateTime day = new DateTime(2023,09,04);
        DateTime slotStart = new DateTime(2023,09, 04, 12, 00, 00);
        DateTime slotEnd = new DateTime(2023,09, 04, 18, 00, 00);
        List<Session> slotSessions = new List<Session>();
        Slot slot = new Slot(psychologist, location, day, slotStart, slotEnd, 55, 10, false, slotSessions, 1);
        
        //add one session
        DateTime sessionStart = new DateTime(2023,09, 04, 13, 00, 00);
        DateTime sessionEnd = new DateTime(2023,09, 04, 14, 00, 00);
        Session session = new Session(psychologist, location, day, sessionStart, sessionEnd, slot, 10000, false,
            "trial", SessionFrequency.None, client, id: 1);
        
        
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
        
        _psychologist = (Psychologist) await _users.GetByEmail("psychologist1" + emailEnd);
        _manager = (Manager)await _users.GetByEmail("manager1" + emailEnd);
        
        //add associated
        await AddAssociated();
        //add not associated
        await AddNotAssociated();
    }

    public async Task AddAssociated()
    {
        string emailEnd = "@psychappointments.com";
        string phone = "+361/123-4567";
        DateTime birthday = DateTime.MinValue;
        Address clientAddress = new Address("Hungary", "1996", "Petőfi utca", "134/c");
        DateTime day = new DateTime(2023,09,04);
        var location = await _locations.GetById(1);
        var admin = await _users.GetByEmail("admin1@psychappointments.com");
        var manager = await _users.GetByEmail("manager1@psychappointments.com");
        
         //add associated psychologist (partner)
        List<Session> psychologist3Sessions = new List<Session>();
        List<Slot> slots3 = new List<Slot>();
        List<Client> clients3 = new List<Client>();
        Address psychologist3Address = new Address("Hungary", "1996", "Petőfi utca", "134/p3");
        Psychologist psychologist3 = new Psychologist("Psychologist3", "psychologist3" + emailEnd, phone, 
            birthday, psychologist3Address, "1234", psychologist3Sessions, slots3, clients3, admin, 6);
        
        //add associated slot (association by partnership - psych3 is psych1's partner)
        DateTime day3 = new DateTime(2023,09,04);
        DateTime slot3Start = new DateTime(2023,09, 04, 12, 00, 00);
        DateTime slot3End = new DateTime(2023,09, 04, 18, 00, 00);
        List<Session> slot3Sessions = new List<Session>();
        Slot slot3 = new Slot(_psychologist, location, day3, slot3Start, slot3End, 50, 10, false, slot3Sessions, 3);
        //add associated slot (association by partnership - psych1 is psych3's partner)
        DateTime day4 = new DateTime(2023,09,04);
        DateTime slot4Start = new DateTime(2023,09, 04, 12, 00, 00);
        DateTime slot4End = new DateTime(2023,09, 04, 18, 00, 00);
        List<Session> slot4Sessions = new List<Session>();
        Slot slot4 = new Slot(psychologist3, location, day4, slot4Start, slot4End, 50, 10, false, slot4Sessions, 4);
        
        //add client3 for session3
        List<Session> client3Sessions = new List<Session>();
        List<Psychologist> client3Psychologists = new List<Psychologist>();
        Client client3 = new Client("Client3", "client3" + emailEnd, phone, birthday, 
            clientAddress, "1234", client3Sessions, client3Psychologists, manager, 7);
        
        //add client4 for session4
        List<Session> client4Sessions = new List<Session>();
        List<Psychologist> client4Psychologists = new List<Psychologist>();
        Client client4 = new Client("Client4", "client4" + emailEnd, phone, birthday, 
            clientAddress, "1234", client4Sessions, client4Psychologists, manager, 8);
        
        //add associated session (association by partnership - psych3 is psych1's partner)
        DateTime session3Start = new DateTime(2023,09, 04, 15, 00, 00);
        DateTime session3End = new DateTime(2023,09, 04, 16, 00, 00);
        Session session3 = new Session(_psychologist, location, day3, session3Start, session3End, slot3, 15000, false,
            "trial", SessionFrequency.None, client3, psychologist3, id: 3);
        //add associated session (association by partnership - psych1 is psych3's partner)
        DateTime session4Start = new DateTime(2023,09, 04, 15, 00, 00);
        DateTime session4End = new DateTime(2023,09, 04, 16, 00, 00);
        Session session4 = new Session(psychologist3, location, day, session4Start, session4End, slot4, 15000, false,
            "trial", SessionFrequency.None, client4, _psychologist, id: 4);
        
        //add associated manager
        List<Location> managerLocations = new List<Location>(); 
        Address managerAddress = new Address("Hungary", "1996", "Petőfi utca", "134/m3");
        Manager manager3 = new Manager("Manager3", "manager3" + emailEnd, phone, birthday, managerAddress, "1234", managerLocations, admin, 88);
        
        _psychologist.Sessions.Add(session3);
        _psychologist.Slots.Add(slot3);
        _psychologist.Clients.Add(client3);
        psychologist3.Clients.Add(client3);
        psychologist3.Sessions.Add(session3);
        
        client3.Sessions.Add(session3);
        client3.Psychologists.Add(_psychologist);
        client3.Psychologists.Add(psychologist3);
        
        location.Psychologists.Add(psychologist3);
        location.Managers.Add(manager3);
        
        slot3.Sessions.Add(session3);
        
        manager3.Locations.Add(location);
        
        //associated 2
        
        psychologist3.Sessions.Add(session4);
        psychologist3.Slots.Add(slot4);
        psychologist3.Clients.Add(client4);
        _psychologist.Clients.Add(client4);
        _psychologist.Sessions.Add(session4);
        
        client4.Sessions.Add(session4);
        client4.Psychologists.Add(psychologist3);
        client4.Psychologists.Add(_psychologist);
        
        slot4.Sessions.Add(session4);
        
        await _sessions.Add(session3);
        await _sessions.Add(session4);

        
        await _users.Add(psychologist3);
        await _users.Add(manager3);
        
        await _users.Add(client3);
        await _users.Add(client4);
        
        await _slots.Add(slot3);
        await _slots.Add(slot4);
    }

    public async Task AddNotAssociated()
    {
        string emailEnd = "@psychappointments.com";
        string phone = "+361/123-4567";
        DateTime birthday = DateTime.MinValue;
        Address managerAddress = new Address("Hungary", "1996", "Petőfi utca", "134/m");
        Address clientAddress = new Address("Hungary", "1996", "Petőfi utca", "134/c");
        
          //add not associated location
        List<Manager> location2Managers = new List<Manager>();
        List<Psychologist> location2Psychologists = new List<Psychologist>();
        Address location2Address = new Address("Hungary", "1996", "Petőfi utca", "134/l2");
        Location location2 = new Location("Location2", location2Address, location2Managers, location2Psychologists, 2);
        
        //add not associated manager
        List<Location> manager2Locations = new List<Location>();
        Manager manager2 = new Manager("Manager2", "manager2" + emailEnd, phone, birthday, managerAddress, 
            "1234", manager2Locations, _psychologist, 4);
        
        //add not associated client
        List<Session> client2Sessions = new List<Session>();
        List<Psychologist> client2Psychologists = new List<Psychologist>();
        Client client2 = new Client("Client2", "client2" + emailEnd, phone, birthday, 
            clientAddress, "1234", client2Sessions, client2Psychologists, manager2);
        
        //add not associated psychologist
        List<Session> psychologist2Sessions = new List<Session>();
        List<Slot> slots2 = new List<Slot>();
        List<Client> clients2 = new List<Client>();
        Address psychologist2Address = new Address("Hungary", "1996", "Petőfi utca", "134/p2");
        Psychologist psychologist2 = new Psychologist("Psychologist2", "psychologist2" + emailEnd, phone, 
            birthday, psychologist2Address, "1234", psychologist2Sessions, slots2, clients2, manager2, 5);
        
        //add psychologist2 slot (not associated)
        DateTime day2 = new DateTime(2023,09,04);
        DateTime slot2Start = new DateTime(2023,09, 04, 12, 00, 00);
        DateTime slot2End = new DateTime(2023,09, 04, 18, 00, 00);
        List<Session> slot2Sessions = new List<Session>();
        Slot slot2 = new Slot(psychologist2, location2, day2, slot2Start, slot2End, 50, 10, false, slot2Sessions, 2);
        
        //add not associated session
        DateTime session2Start = new DateTime(2023,09, 04, 15, 00, 00);
        DateTime session2End = new DateTime(2023,09, 04, 16, 00, 00);
        Session session2 = new Session(psychologist2, location2, day2, session2Start, session2End, slot2, 15000, false,
            "trial", SessionFrequency.None, client2, id: 2);
        
        //not associated
        psychologist2.Sessions.Add(session2);
        psychologist2.Slots.Add(slot2);
        psychologist2.Clients.Add(client2);
        
        manager2.Locations.Add(location2);
        client2.Sessions.Add(session2);
        client2.Psychologists.Add(psychologist2);
        
        location2.Psychologists.Add(psychologist2);
        location2.Managers.Add(manager2);
        
        slot2.Sessions.Add(session2);
        
        await _sessions.Add(session2);
        await _users.Add(psychologist2);
        await _users.Add(manager2);

        await _locations.Add(location2);
        await _slots.Add(slot2);
        await _users.Add(client2);
        
        
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
    public async Task TestSessionAccessAssociated()
    {
        //user is associated with session
        var query = async () => await _sessions.GetById(1);
        var queryResult = await _managerDPS.Filter(_manager, query);

        var unFiltered = await query();

        bool result =
            queryResult.Id == unFiltered.Id &&
            queryResult.PsychologistId == unFiltered.Psychologist.Id &&
            queryResult.Blank == unFiltered.Blank &&
            queryResult.LocationId == unFiltered.Location.Id &&
            queryResult.Date == unFiltered.Date &&
            queryResult.Start == unFiltered.Start &&
            queryResult.End == unFiltered.End &&
            queryResult.ClientId == unFiltered.Client.Id &&
            queryResult.Price == unFiltered.Price &&
            queryResult.Frequency == Enum.GetName(typeof(SessionFrequency), unFiltered.Frequency) &&
            queryResult.SlotId == unFiltered.Slot.Id &&
            queryResult.Description == unFiltered.Description;
        
        Assert.That(result);
    }
    
    [Test]
    public async Task TestSessionAccessNotAssociated()
    {
        //user is not associated with session
        var query = async () => await _sessions.GetById(2);
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        Assert.That(queryResult == null);
    }
    
    [Test]
    public async Task TestSlotAccessAssociated()
    {
        //user is associated with slot    
        var query = async () => await _slots.GetById(1);
        var queryResult = await _managerDPS.Filter(_manager, query);

        var unFiltered = await query();

        bool result =
            queryResult.Id == unFiltered.Id &&
            queryResult.LocationId == unFiltered.Location.Id &&
            queryResult.Date == unFiltered.Date &&
            queryResult.SlotStart == unFiltered.SlotStart &&
            queryResult.SlotEnd == unFiltered.SlotEnd &&
            queryResult.SessionLength == unFiltered.SessionLength &&
            queryResult.Rest == unFiltered.Rest &&
            queryResult.Weekly == unFiltered.Weekly &&
            queryResult.SessionIds.SequenceEqual(unFiltered.Sessions.Select(ses => ses.Id).ToList());
        
        
        Assert.That(result);
    }

    [Test]
    public async Task TestSlotAccessNotAssociated()
    {
        //user is not associated with slot
        var query = async () => await _slots.GetById(2);
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        Assert.That(queryResult == null);
    }

    [Test]
    public async Task TestLocationAccessAssociated()
    {
        //user is associated with location
        var query = async () => await _locations.GetById(1);
        var queryResult = await _managerDPS.Filter(_manager, query);

        var unFiltered = await query();
        var unFilteredDTO = new LocationDTO(unFiltered);
        
        Assert.That(unFilteredDTO.Equals(queryResult));
    }
    
    [Test]
    public async Task TestLocationAccessNotAssociated()
    {
        //user is not associated with location
        var query = async () => await _locations.GetById(2);
        var queryResult = await _managerDPS.Filter(_manager, query);

        var unFiltered = await query();
        var unFilteredDTO = new LocationDTO(unFiltered);
        
        Assert.That(unFilteredDTO.Equals(queryResult));

    }
    
    [Test]
    public async Task TestClientAccessAssociated()
    {
        //_manager registered client
        var query = async () => await _users.GetByEmail("client1" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        var unFiltered = await query();

        bool result =
            queryResult.Id == unFiltered.Id &&
            queryResult.Name == unFiltered.Name &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Client) &&
            queryResult.Email == unFiltered.Email &&
            queryResult.Phone == unFiltered.Phone &&
            queryResult.DateOfBirth == unFiltered.DateOfBirth &&
            queryResult.Address.Equals(unFiltered.Address) &&
            queryResult.Password == "" &&
            queryResult.SessionIds != null &&
            queryResult.SessionIds.Count > 0 &&
            queryResult.PsychologistIds.Contains(_psychologist.Id) &&
            queryResult.ClientIds == null &&
            queryResult.SlotIds == null &&
            queryResult.LocationIds == null;
        

        Assert.That(result);
    }
    
    [Test]
    public async Task TestClientAccessNotAssociated()
    {
        //user is not associated with client
        var query = async () => await _users.GetByEmail("client2" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        bool result =
            queryResult.Id == 0 &&
            queryResult.Name == "" &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Client) &&
            queryResult.Email == "" &&
            queryResult.Phone == "" &&
            queryResult.DateOfBirth == DateTime.MinValue &&
            queryResult.Address.Equals(new Address()) &&
            queryResult.Password == "" &&
            queryResult.SessionIds == null &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds == null &&
            queryResult.SlotIds == null &&
            queryResult.LocationIds == null;
        
        Assert.That(result);
    }
    
    [Test]

    public async Task TestManagerAccessAssociated()
    {
        //user is associated with manager
        var query = async () => await _users.GetByEmail("manager3" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        var unFiltered = await query();

        bool result =
            queryResult.Id == unFiltered.Id &&
            queryResult.Name == unFiltered.Name &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Manager) &&
            queryResult.Email == unFiltered.Email &&
            queryResult.Phone == unFiltered.Phone &&
            queryResult.DateOfBirth == DateTime.MinValue &&
            queryResult.Address.Equals(new Address()) &&
            queryResult.Password == "" &&
            queryResult.SessionIds == null &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds == null &&
            queryResult.SlotIds == null &&
            queryResult.LocationIds != null;
        
        Assert.That(result);
        
    }
    
    [Test]

    public async Task TestManagerAccessSelf()
    {
        //user is associated with manager
        var query = async () => await _users.GetByEmail("manager1" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        var unFiltered = await query();
        var unFilteredDTO = new UserDTO(unFiltered);
        unFilteredDTO.Password = "";

        bool result =
            queryResult.Id == unFilteredDTO.Id &&
            queryResult.Name == unFilteredDTO.Name &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Manager) &&
            queryResult.Email == unFilteredDTO.Email &&
            queryResult.Phone == unFilteredDTO.Phone &&
            queryResult.DateOfBirth == unFilteredDTO.DateOfBirth &&
            queryResult.Address.Equals(unFilteredDTO.Address) &&
            queryResult.Password == "" &&
            queryResult.SessionIds == null &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds == null &&
            queryResult.SlotIds == null &&
            queryResult.LocationIds.SequenceEqual(unFilteredDTO.LocationIds);
        
        Assert.That(result);
        
    }
    
    [Test]
    public async Task TestManagerAccessNotAssociated()
    {
        //user is not associated with manager
        var query = async () => await _users.GetByEmail("manager2" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);

        var unFiltered = await query();

        bool result =
            queryResult.Id == 0 &&
            queryResult.Name == unFiltered.Name &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Manager) &&
            queryResult.Email == unFiltered.Email &&
            queryResult.Phone == unFiltered.Phone &&
            queryResult.DateOfBirth == DateTime.MinValue &&
            queryResult.Address.Equals(new Address()) &&
            queryResult.Password == "" &&
            queryResult.SessionIds == null &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds == null &&
            queryResult.SlotIds == null &&
            queryResult.LocationIds != null;
        
        Assert.That(result);
        
    }
    
    [Test]
    public async Task TestPsychologistAccessAssociated()
    {
        //user is not associated with psychologist
        var query = async () => await _users.GetByEmail("psychologist1" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);
        
        var unFiltered = await query();
        var sessionIds = ((Psychologist)unFiltered).Sessions.Where(ses => _manager.Locations.Contains(ses.Location))
            .Select(ses => ses.Id)
            .ToList();

        var clientIds = ((Psychologist)unFiltered).Clients.Where(cli => cli.RegisteredBy.Equals(_manager))
            .Select(cli => cli.Id)
            .ToList();

        var slotIds = ((Psychologist)unFiltered).Slots.Where(slot => _manager.Locations.Contains(slot.Location))
            .Select(slot => slot.Id)
            .ToList();
        
        bool result =
            queryResult.Id == unFiltered.Id &&
            queryResult.Name == unFiltered.Name &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Psychologist) &&
            queryResult.Email == unFiltered.Email &&
            queryResult.Phone == unFiltered.Phone &&
            queryResult.DateOfBirth == DateTime.MinValue &&
            queryResult.Address.Equals(new Address()) &&
            queryResult.Password == "" &&
            queryResult.SessionIds.SequenceEqual(sessionIds) && queryResult.SessionIds.Count > 0 &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds.SequenceEqual(clientIds)  && queryResult.ClientIds.Count > 0 &&
            queryResult.SlotIds.SequenceEqual(slotIds) && queryResult.SlotIds.Count > 0 &&
            queryResult.LocationIds == null;
        
        Assert.That(result);
            
        
    }
    
    [Test]
    public async Task TestPsychologistAccessNotAssociated()
    {
        //user is not associated with psychologist
        string email = "psychologist2@psychappointments.com";
        var query = async () => await _users.GetByEmail(email);
        var queryResult = await _managerDPS.Filter(_manager, query);
            
        var unFiltered = await query();
        var sessionIds = ((Psychologist)unFiltered).Sessions.Where(ses => _manager.Locations.Contains(ses.Location))
            .Select(ses => ses.Id)
            .ToList();

        var clientIds = ((Psychologist)unFiltered).Clients.Where(cli => cli.RegisteredBy.Equals(_manager))
            .Select(cli => cli.Id)
            .ToList();

        var slotIds = ((Psychologist)unFiltered).Slots.Where(slot => _manager.Locations.Contains(slot.Location))
            .Select(slot => slot.Id)
            .ToList();
        
        bool result =
            queryResult.Id == unFiltered.Id &&
            queryResult.Name == unFiltered.Name &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Psychologist) &&
            queryResult.Email == unFiltered.Email &&
            queryResult.Phone == unFiltered.Phone &&
            queryResult.DateOfBirth == DateTime.MinValue &&
            queryResult.Address.Equals(new Address()) &&
            queryResult.Password == "" &&
            queryResult.SessionIds.SequenceEqual(sessionIds) && queryResult.SessionIds.Count == 0 &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds.SequenceEqual(clientIds)  && queryResult.ClientIds.Count == 0 &&
            queryResult.SlotIds.SequenceEqual(slotIds) && queryResult.SlotIds.Count == 0 &&
            queryResult.LocationIds == null;
        
        Assert.That(result);
    }


    
    
    [Test]

    public async Task TestAdminAccess()
    {
        //only admins should be able to see other admins
        var query = async () => await _users.GetByEmail("admin1" + "@psychappointments.com");
        var queryResult = await _managerDPS.Filter(_manager, query);

        bool result =
            queryResult.Id == 0 &&
            queryResult.Name == "" &&
            queryResult.Type == Enum.GetName(typeof(UserType), UserType.Admin) &&
            queryResult.Email == "" &&
            queryResult.Phone == "" &&
            queryResult.DateOfBirth == DateTime.MinValue &&
            queryResult.Address.Equals(new Address()) &&
            queryResult.Password == "" &&
            queryResult.RegisteredBy == null &&
            queryResult.SessionIds == null &&
            queryResult.PsychologistIds == null &&
            queryResult.ClientIds == null &&
            queryResult.SlotIds == null &&
            queryResult.LocationIds == null;
        
        Assert.That(result);
    }
    
 
}