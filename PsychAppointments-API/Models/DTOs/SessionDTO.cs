using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class SessionDTO
{
    public long Id { get; set; }
    public long? PsychologistId { get; set; }
    public long? PartnerPsychologistId { get; set; }
    public bool Blank { get; set; }
    public long? LocationId { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public long? ClientId { get; set; }
    public int? Price { get; set; }
    public string Frequency { get; set; }
    public long? SlotId { get; set; }
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
        Psychologist? partnerPsychologist = null, 
        long id = 0)
    {
        Id = id;
        PsychologistId = psychologist.Id;
        PartnerPsychologistId = partnerPsychologist != null ? partnerPsychologist.Id : null;
        Blank = blank;
        LocationId = location.Id;
        Date = date;
        Start = start;
        End = end;
        ClientId = client.Id;
        Price = price;
        Frequency = frequency.ToString();
        SlotId = slot.Id;
        Description = description;
    }

    public SessionDTO(Session session)
    {
        Id = session.Id;
        PsychologistId = session.Psychologist.Id;
        PartnerPsychologistId = session.PartnerPsychologist != null ? session.PartnerPsychologist.Id : null;
        Blank = session.Blank;
        LocationId = session.Location.Id;
        Date = session.Date;
        Start = session.Start;
        End = session.End;
        ClientId = session.Client != null ? session.Client.Id : null;
        Price = session.Price;
        Frequency = session.Frequency.ToString();
        SlotId = session.Slot.Id;
        Description = session.Description;
    }

    public SessionDTO()
    {
        
    }
    
    public override string ToString()
    {
        return $"SessionId: {Id}, Slot: {SlotId} Psychologist: {PsychologistId}, Blank: {Blank}, Location: {LocationId}, " +
               $"Date: {Date}, Start: {Start}, End: {End}, Client: {ClientId}, Price: {Price}, Frequency: {Frequency}";
        
    }
}