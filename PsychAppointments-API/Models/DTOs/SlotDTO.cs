namespace PsychAppointments_API.Models;

public class SlotDTO
{
    public long Id { get; set; }
    public UserDTO Psychologist { get; set; }
    public LocationDTO Location { get; set; }
    public DateTime Date { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public int SessionLength { get; set; }
    public int Rest { get; set; }
    public bool Weekly { get; set; }
    public List<SessionDTO> Sessions { get; set; }

    public SlotDTO(
        Psychologist psychologist,
        Location location,
        DateTime date,
        DateTime slotStart,
        DateTime slotEnd,
        int sessionLength = 50,
        int rest = 10,
        bool weekly = false,
        List<Session> sessions = null
    )
    {
        Psychologist = new UserDTO(psychologist);
        Location = new LocationDTO(location);
        Date = date;
        SlotStart = slotStart;
        SlotEnd = slotEnd;
        SessionLength = sessionLength;
        Rest = rest;
        Weekly = weekly;
        Sessions = sessions.Select(ses => new SessionDTO(ses)).ToList();
    }

    public SlotDTO(Slot slot)
    {
        Psychologist = new UserDTO(slot.Psychologist);
        Location = new LocationDTO(slot.Location);
        Date = slot.Date;
        SlotStart = slot.SlotStart;
        SlotEnd = slot.SlotEnd;
        SessionLength = slot.SessionLength;
        Rest = slot.Rest;
        Weekly = slot.Weekly;
        Sessions = slot.Sessions.Select(ses => new SessionDTO(ses)).ToList();
    }
}