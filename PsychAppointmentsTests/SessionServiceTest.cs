using PsychAppointments_API.Models;

namespace PsychAppointmentsTests;

public class SessionServiceTest
{
    [Test]
    public void DateRangeLogicTest()
    {
        var date = new DateTime(2023, 10, 12);
        var startDate = new DateTime(2023, 10, 3);
        var endDate = new DateTime(2023, 10, 15);

        var list = new List<Session>();

        var session = new Session();
        session.Date = date;
        
        list.Add(session);

        var result = list.Where(ses => ses.Date >= startDate &&
                                       ses.Date <= endDate);
        
        Assert.That(result.Any());
    }
}