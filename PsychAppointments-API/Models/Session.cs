using System.ComponentModel.DataAnnotations.Schema;

namespace PsychAppointments_API.Models;

public class Session
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public Psychologist Psychologist { get; set; }
    public Psychologist? PartnerPsychologist { get; set; }
    public bool Blank { get; set; }
    public Location Location { get; set; }
    public DateTime Date { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public Client? Client { get; set; }
    public int Price { get; set; }
    public SessionFrequency Frequency { get; set; }
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
        Psychologist? partnerPsychologist = null)
    {
        Psychologist = psychologist;
        PartnerPsychologist = partnerPsychologist;
        Blank = blank;
        Location = location;
        Date = date;
        Start = start;
        end = end;
        Client = client;
        Price = price;
        Frequency = frequency;
        Slot = slot;
        Description = description;
    }
}