using System.ComponentModel.DataAnnotations.Schema;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class Slot
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [ForeignKey("PsychologistId")]
    public long PsychologistId { get; set; }
    public Psychologist Psychologist { get; set; }
    
    [ForeignKey("LocationId")]
    public long LocationId { get; set; }
    public Location Location { get; set; }
    public DateTime Date { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public int SessionLength { get; set; }
    public int Rest { get; set; }
    public bool Weekly { get; set; }
    public List<Session> Sessions { get; set; }

    public Slot(
        Psychologist psychologist,
        Location location,
        DateTime date,
        DateTime slotStart,
        DateTime slotEnd,
        int sessionLength = 50,
        int rest = 10,
        bool weekly = false,
        List<Session>? sessions = null,
        long id = 0
        )
    {
        Psychologist = psychologist;
        Location = location;
        Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        SlotStart = DateTime.SpecifyKind(slotStart, DateTimeKind.Utc);
        SlotEnd = DateTime.SpecifyKind(slotEnd, DateTimeKind.Utc);
        SessionLength = sessionLength;
        Rest = rest;
        Weekly = weekly;
        Sessions = sessions ?? new List<Session>();
        Id = id;
    }

    public Slot()
    {
        
    }

    public override bool Equals(object? obj)
    {
        return obj is Slot
               && ((Slot)obj).Id == Id
               && ((Slot)obj).Psychologist.Equals(Psychologist)
               && ((Slot)obj).Location.Equals(Location)
               && ((Slot)obj).Date == Date
               && ((Slot)obj).SlotStart == SlotStart
               && ((Slot)obj).SlotEnd == SlotEnd
               && ((Slot)obj).SessionLength == SessionLength
               && ((Slot)obj).Rest == Rest
               && ((Slot)obj).Weekly == Weekly;
    }
    
    public override string ToString()
    {
        return $"SlotId: {Id}, Psychologist: {Psychologist.Id}, Sessions: {Sessions.Count}, Location: {Location.Id + " " + Location.Name}, " +
               $"Date: {Date}, Start: {SlotStart}, End: {SlotEnd}, SessionLength: {SessionLength}, Rest: {Rest}, Weekly {Weekly}";
    }
}