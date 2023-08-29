using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IClientService
{
    Task<bool> AddClient(Client client);
    
    Task<Client> GetClientById(long id);
    Task<Client> GetClientByEmail(string email);
    Task<List<Client>> GetAllClients();
    Task<List<Client>> GetClientsByLocation(Location location);
    Task<List<Client>> GetClientsByPsychologist(Psychologist psychologist);
    Task<List<Client>> GetClientsBySlot(Slot slot);
    
    Task<bool> UpdateClient(long id, Client client);
    Task<bool> DeleteClient(long id);
}