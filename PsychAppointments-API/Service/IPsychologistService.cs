using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IPsychologistService
{
    Task<bool> AddPsychologist(Psychologist psychologist);
    
    Task<Psychologist> GetPsychologistById(long id);
    Task<Psychologist> GetPsychologistByEmail(string email);
    Task<List<Psychologist>> GetAllPsychologists();
    Task<List<Psychologist>> GetPsychologistsByLocation(Location location);
    
    Task<bool> UpdatePsychologist(long id, Psychologist psychologist);
    Task<bool> DeletePsychologist(long id);
}