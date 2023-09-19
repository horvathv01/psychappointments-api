using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class Session
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [ForeignKey("PsychologistId")]
    public long PsychologistId { get; set; }
    public Psychologist Psychologist { get; set; }
    
    [ForeignKey("PartnerPsychologistId")]
    public long? PartnerPsychologistId { get; set; }
    public Psychologist? PartnerPsychologist { get; set; }
    public bool Blank { get; set; }
    
    [ForeignKey("LocationId")]
    public long LocationId { get; set; }
    public Location Location { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    
    [ForeignKey("ClientId")]
    public long ClientId { get; set; }
    public Client? Client { get; set; }
    public int Price { get; set; }
    public SessionFrequency Frequency { get; set; }
    
    [ForeignKey("SlotId")]
    public long SlotId { get; set; }
    public Slot Slot { get; set; }
    public string Description { get; set; }

    public Session(
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
        long id = 0
        )
    {
        Psychologist = psychologist;
        PartnerPsychologist = partnerPsychologist;
        Blank = blank;
        Location = location;
        Date = date;
        Start = start;
        End = end;
        Client = client;
        Price = price;
        Frequency = frequency;
        Slot = slot;
        Description = description;
        Id = id;
    }

    public Session()
    {
        
    }

    public override bool Equals(object? obj)
    {
        return obj is Session
               && ((Session)obj).Id == Id
               && ((Session)obj).Psychologist.Equals(Psychologist)
               && ((Session)obj).Blank == Blank
               && ((Session)obj).Location.Equals(Location)
               && ((Session)obj).Date == Date
               && ((Session)obj).Start == Start
               && ((Session)obj).End == End
               && ((Session)obj).Client.Equals(Client)
               && ((Session)obj).Price == Price
               && ((Session)obj).Frequency == Frequency
               && ((Session)obj).Slot.Equals(Slot);
    }
    
    public override string ToString()
    {
        string clientId = Client == null ? "none" : Client.Id.ToString();
        return $"SessionId: {Id}, Slot: {Slot.Id} Psychologist: {Psychologist.Id}, Blank: {Blank}, Location: {Location.Id + " " + Location.Name}, " +
               $"Date: {Date}, Start: {Start}, End: {End}, Client: {clientId}, Price: {Price}, Frequency: {Enum.GetName(typeof(SessionFrequency), Frequency)}";
    }
}