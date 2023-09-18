using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IClientService
{
    Task<bool> AddClient(Client client);
    
    Task<Client?> GetClientById(long id);
    Task<Client?> GetClientByEmail(string email);
    Task<List<Client>> GetAllClients();
    Task<List<Client>> GetListOfClients(List<long> ids);
    List<Client?> GetClientsByLocation(Location location);
    List<Client?> GetClientsBySlot(Slot slot);
    
    Task<bool> UpdateClient(long id, Client client);
    Task<bool> UpdateClient(long id, UserDTO client);
}