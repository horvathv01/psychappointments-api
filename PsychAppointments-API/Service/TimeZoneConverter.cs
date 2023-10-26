namespace PsychAppointments_API.Service;

public static class TimeZoneConverter
{
    public static DateTime ConvertToCET(DateTime utcTime)
    {
        /*
        TimeZoneInfo CET = TimeZoneInfo.FindSystemTimeZoneById("Central Europe Standard Time");
        utcTime = DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
        Console.WriteLine(utcTime);
        var result = TimeZoneInfo.ConvertTimeFromUtc(utcTime, CET);
        return result;
        */
        return utcTime;
    }

    public static DateTime Convert(DateTime utcTime, TimeZoneInfo target)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, target);
    }
}