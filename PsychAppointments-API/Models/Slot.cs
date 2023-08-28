using System.ComponentModel.DataAnnotations.Schema;

namespace PsychAppointments_API.Models;

public class Slot
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public Psychologist Psychologist { get; set; }
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
        List<Session> sessions = null
        )
    {
        Psychologist = psychologist;
        Location = location;
        Date = date;
        SlotStart = slotStart;
        SlotEnd = slotEnd;
        SessionLength = sessionLength;
        Rest = rest;
        Weekly = weekly;
        Sessions = sessions == null ? PrepopulateSessions() : sessions;
    }

    private List<Session> PrepopulateSessions()
    {
        List<Session> sessions = new List<Session>();
        double ts = (SlotEnd - SlotStart).TotalMinutes;
        

        if (ts < SessionLength)
        {
            throw new ArgumentException("Slot is too short to fit a session with the provided length.");
        }
        //how many (session + rest) fits in the slot?
        int sessionCount = Convert.ToInt32(Math.Floor(ts / (SessionLength + Rest)));
        //would giving up the last rest result in an extra session? 
        int plusOneSessionTotalLength = (sessionCount + 1) * (SessionLength + Rest) - Rest;
        if (plusOneSessionTotalLength <= ts)
        {
            //plus one session fits
            sessionCount += 1;
        }

        DateTime start = SlotStart;
        for (int i = 0; i < sessionCount; i++)
        {
            DateTime end = start.AddMinutes(SessionLength);
            SessionFrequency frequency;
            if (Weekly == true)
            {
                frequency = SessionFrequency.Weekly;
            }
            else
            {
                frequency = SessionFrequency.None;
            }
            Session ses = new Session(Psychologist, Location, Date, start, end, this, 0, true, "", frequency, null, null);
            start = end.AddMinutes(SessionLength + Rest);
            sessions.Add(ses);
        }
        return sessions;
    }



}