namespace PsychAppointments_API.Service;

public interface IDataProtectionService
{
    //comes between controller and service classes
    //receives user with every request
    //creates dto's out of query results
    //filters out pieces of data which should not be seen by user
}