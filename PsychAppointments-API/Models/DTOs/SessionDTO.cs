namespace PsychAppointments_API.Models;

public class SessionDTO
{
    public long Id { get; set; }
    public UserDTO Psychologist { get; set; }
    public UserDTO? PartnerPsychologist { get; set; }
    public bool Blank { get; set; }
    public LocationDTO Location { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public UserDTO? Client { get; set; }
    public int Price { get; set; }
    public string Frequency { get; set; }
    public SlotDTO Slot { get; set; }
    public string Description { get; set; }

    public SessionDTO(
        Psychologist psychologist,
        Location location, 
        DateTime date, 
        DateTime start, 
        DateTime end, 
        Slot slot,
        int price,
        bool blank = true,
        string description = "",
        SessionFrequency frequency = SessionFrequency.Weekly,
        Client? client = null,
        Psychologist? partnerPsychologist = null)
    {
        Psychologist = new UserDTO(psychologist);
        PartnerPsychologist = PartnerPsychologist != null ? new UserDTO(partnerPsychologist) : null;
        Blank = blank;
        Location = new LocationDTO(location);
        Date = date;
        Start = start;
        End = end;
        Client = new UserDTO(client);
        Price = price;
        Frequency = frequency.ToString();
        Slot = new SlotDTO(slot);
        Description = description;
    }

    public SessionDTO(Session session)
    {
        Psychologist = new UserDTO(session.Psychologist);
        PartnerPsychologist = PartnerPsychologist != null ? new UserDTO(session.PartnerPsychologist) : null;
        Blank = session.Blank;
        Location = new LocationDTO(session.Location);
        Date = session.Date;
        Start = session.Start;
        End = session.End;
        Client = new UserDTO(session.Client);
        Price = session.Price;
        Frequency = session.Frequency.ToString();
        Slot = new SlotDTO(session.Slot);
        Description = session.Description;
    }
}