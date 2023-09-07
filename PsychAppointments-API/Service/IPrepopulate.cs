namespace PsychAppointments_API.Service;

public interface IPrepopulate
{
    Task PrepopulateDB();
    Task PrepopulateInMemory();
}