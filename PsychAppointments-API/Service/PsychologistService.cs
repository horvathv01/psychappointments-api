using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class PsychologistService : IPsychologistService
{
    private readonly DbContext _context;
    
    public PsychologistService(DbContext context)
    {
        _context = context;
    }
    
    public Task<bool> AddPsychologist(Psychologist psychologist)
    {
        throw new NotImplementedException();
    }

    public Task<Psychologist> GetPsychologistById(long id)
    {
        throw new NotImplementedException();
    }

    public Task<Psychologist> GetPsychologistByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<List<Psychologist>> GetAllPsychologists()
    {
        throw new NotImplementedException();
    }

    public Task<List<Psychologist>> GetPsychologistsByLocation(Location location)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdatePsychologist(long id, Psychologist psychologist)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeletePsychologist(long id)
    {
        throw new NotImplementedException();
    }
}