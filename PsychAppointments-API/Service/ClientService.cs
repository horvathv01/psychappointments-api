using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class ClientService : IClientService
{
    private readonly DbContext _context;
    
    public ClientService(DbContext context)
    {
        _context = context;
    }
    
    
    public Task<bool> AddClient(Client client)
    {
        throw new NotImplementedException();
    }

    public Task<Client> GetClientById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Client> GetClientByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<List<Client>> GetAllClients()
    {
        throw new NotImplementedException();
    }

    public Task<List<Client>> GetClientsByLocation(Location location)
    {
        throw new NotImplementedException();
    }

    public Task<List<Client>> GetClientsByPsychologist(Psychologist psychologist)
    {
        throw new NotImplementedException();
    }

    public Task<List<Client>> GetClientsBySlot(Slot slot)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateClient(long id, Client client)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteClient(long id)
    {
        throw new NotImplementedException();
    }
}