using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Models;

public class SlotDTO
{
    public long Id { get; set; }
    public long PsychologistId { get; set; }
    public long LocationId { get; set; }
    public DateTime Date { get; set; }
    public DateTime SlotStart { get; set; }
    public DateTime SlotEnd { get; set; }
    public int SessionLength { get; set; }
    public int Rest { get; set; }
    public bool Weekly { get; set; }
    public List<long>? SessionIds { get; set; }

    public SlotDTO(
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
        Id = id;
        PsychologistId = psychologist.Id;
        LocationId = location.Id;
        Date = date;
        SlotStart = slotStart;
        SlotEnd = slotEnd;
        SessionLength = sessionLength;
        Rest = rest;
        Weekly = weekly;
        SessionIds = sessions.Select(ses => ses.Id).ToList();
    }

    public SlotDTO(Slot slot)
    {
        Id = slot.Id;
        PsychologistId = slot.Psychologist.Id;
        LocationId = slot.Location.Id;
        Date = slot.Date;
        SlotStart = slot.SlotStart;
        SlotEnd = slot.SlotEnd;
        SessionLength = slot.SessionLength;
        Rest = slot.Rest;
        Weekly = slot.Weekly;
        SessionIds = slot.Sessions.Select(ses => ses.Id).ToList();
    }
    
    public override string ToString()
    {
        return $"SlotId: {Id}, Psychologist: {PsychologistId}, Sessions: {SessionIds.Count}, Location: {LocationId}, " +
               $"Date: {Date}, Start: {SlotStart}, End: {SlotEnd}, SessionLength: {SessionLength}, Rest: {Rest}, Weekly {Weekly}";
    }
}