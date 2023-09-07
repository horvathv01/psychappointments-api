using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class Prepopulate : IPrepopulate
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Session> _sessionRepository;
    private readonly IRepository<Slot> _slotRepository;
    private readonly IRepository<Location> _locationRepository;
    //private readonly DbContext _context;
    
    public Prepopulate(
        //DbContext context,
        IRepository<User> userRepository,
        IRepository<Session> sessionRepository,
        IRepository<Slot> slotRepository,
        IRepository<Location> locationRepository 
        )
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _slotRepository = slotRepository;
        _locationRepository = locationRepository;
    }
    
        public async Task PrepopulateInMemory()
    { 
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
        Manager manager = new Manager("Manager1", "manager1" + emailEnd, phone, birthday, managerAddress, "1234", managerLocations, admin, 10);
        
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

        await _userRepository.Add(admin);
        await _userRepository.Add(psychologist);
        await _userRepository.Add(manager);
        await _userRepository.Add(client);
        await _locationRepository.Add(location);
        await _slotRepository.Add(slot);
        await _sessionRepository.Add(session);
        
        
        //add associated
        await AddAssociatedInMemory();
        //add not associated
        await AddNotAssociatedInMemory();
    }

    private async Task AddAssociatedInMemory()
    {
        string emailEnd = "@psychappointments.com";
        string phone = "+361/123-4567";
        DateTime birthday = DateTime.MinValue;
        Address clientAddress = new Address("Hungary", "1996", "Petőfi utca", "134/c");
        DateTime day = new DateTime(2023,09,04);
        var location = await _locationRepository.GetById(1);
        var admin = await _userRepository.GetByEmail("admin1@psychappointments.com");
        var manager = await _userRepository.GetByEmail("manager1@psychappointments.com");
        var psychologist = await _userRepository.GetByEmail("psychologist1@psychappointments.com");
        
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
        Slot slot3 = new Slot((Psychologist)psychologist, location, day3, slot3Start, slot3End, 50, 10, false, slot3Sessions, 3);
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
        Session session3 = new Session((Psychologist)psychologist, location, day3, session3Start, session3End, slot3, 15000, false,
            "trial", SessionFrequency.None, client3, psychologist3, id: 3);
        //add associated session (association by partnership - psych1 is psych3's partner)
        DateTime session4Start = new DateTime(2023,09, 04, 15, 00, 00);
        DateTime session4End = new DateTime(2023,09, 04, 16, 00, 00);
        Session session4 = new Session(psychologist3, location, day, session4Start, session4End, slot4, 15000, false,
            "trial", SessionFrequency.None, client4, (Psychologist)psychologist, id: 4);
        
        //add associated manager
        List<Location> managerLocations = new List<Location>(); 
        Address managerAddress = new Address("Hungary", "1996", "Petőfi utca", "134/m3");
        Manager manager3 = new Manager("Manager3", "manager3" + emailEnd, phone, birthday, managerAddress, "1234", managerLocations, admin, 11);
        
        ((Psychologist)psychologist).Sessions.Add(session3);
        ((Psychologist)psychologist).Slots.Add(slot3);
        ((Psychologist)psychologist).Clients.Add(client3);
        psychologist3.Clients.Add(client3);
        psychologist3.Sessions.Add(session3);
        
        client3.Sessions.Add(session3);
        client3.Psychologists.Add((Psychologist)psychologist);
        client3.Psychologists.Add(psychologist3);
        
        location.Psychologists.Add(psychologist3);
        location.Managers.Add(manager3);
        
        slot3.Sessions.Add(session3);
        
        manager3.Locations.Add(location);
        
        //associated 2
        
        psychologist3.Sessions.Add(session4);
        psychologist3.Slots.Add(slot4);
        psychologist3.Clients.Add(client4);
        ((Psychologist)psychologist).Clients.Add(client4);
        ((Psychologist)psychologist).Sessions.Add(session4);
        
        client4.Sessions.Add(session4);
        client4.Psychologists.Add(psychologist3);
        client4.Psychologists.Add((Psychologist)psychologist);
        
        slot4.Sessions.Add(session4);
        
        await _sessionRepository.Add(session3);
        await _sessionRepository.Add(session4);

        
        await _userRepository.Add(psychologist3);
        await _userRepository.Add(manager3);
        
        await _userRepository.Add(client3);
        await _userRepository.Add(client4);
        
        await _slotRepository.Add(slot3);
        await _slotRepository.Add(slot4);
    }

    private async Task AddNotAssociatedInMemory()
    {
        string emailEnd = "@psychappointments.com";
        string phone = "+361/123-4567";
        DateTime birthday = DateTime.MinValue;
        Address managerAddress = new Address("Hungary", "1996", "Petőfi utca", "134/m");
        Address clientAddress = new Address("Hungary", "1996", "Petőfi utca", "134/c");
        var psychologist = await _userRepository.GetByEmail("psychologist1@psychappointments.com");
        
          //add not associated location
        List<Manager> location2Managers = new List<Manager>();
        List<Psychologist> location2Psychologists = new List<Psychologist>();
        Address location2Address = new Address("Hungary", "1996", "Petőfi utca", "134/l2");
        Location location2 = new Location("Location2", location2Address, location2Managers, location2Psychologists, 2);
        
        //add not associated manager
        List<Location> manager2Locations = new List<Location>();
        Manager manager2 = new Manager("Manager2", "manager2" + emailEnd, phone, birthday, managerAddress, 
            "1234", manager2Locations, (Psychologist)psychologist, 4);
        
        //add not associated client
        List<Session> client2Sessions = new List<Session>();
        List<Psychologist> client2Psychologists = new List<Psychologist>();
        Client client2 = new Client("Client2", "client2" + emailEnd, phone, birthday, 
            clientAddress, "1234", client2Sessions, client2Psychologists, manager2, 9);
        
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
        
        await _sessionRepository.Add(session2);
        await _userRepository.Add(psychologist2);
        await _userRepository.Add(manager2);

        await _locationRepository.Add(location2);
        await _slotRepository.Add(slot2);
        await _userRepository.Add(client2);
        
        
    }

    public async Task PrepopulateDB()
    {
        
        
        
        await AddAssociatedDB();
        await AddNotAssociatedDB();
    }

    private async Task AddNotAssociatedDB()
    {
        
    }

    private async Task AddAssociatedDB()
    {
        
    }
}