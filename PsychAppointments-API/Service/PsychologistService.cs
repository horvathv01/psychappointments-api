using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;

namespace PsychAppointments_API.Service;

public class PsychologistService : IPsychologistService
{
    private readonly IRepository<User> _userRepository;
    
    public PsychologistService(
        //DbContext context
        IRepository<User> userRepository
    )
    {
        //_context = context;
        _userRepository = userRepository;
    }
    public async Task<bool> AddPsychologist(Psychologist psychologist)
    {
        return await _userRepository.Add(psychologist);
    }

    public async Task<Psychologist?> GetPsychologistById(long id)
    {
        try
        {
            var user = await _userRepository.GetById(id);
            return (Psychologist)user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Psychologist?> GetPsychologistByEmail(string email)
    {
        try
        {
            var user = await _userRepository.GetByEmail(email);
            return (Psychologist)user;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<Psychologist>> GetAllPsychologists()
    {
        var allUsers = await _userRepository.GetAll();
        return allUsers.Where(us => us.Type == UserType.Psychologist).Select(us => (Psychologist)us).ToList();
    }

    public async Task<List<Psychologist>> GetListOfPsychologists(List<long> ids)
    {
        var allUsers = await _userRepository.GetList(ids);
        return allUsers.Select(us => (Psychologist)us).ToList();
    }

    public async Task<bool> UpdatePsychologist(long id, Psychologist psychologist)
    {
        return await _userRepository.Update(id, psychologist);
    }

    public Task<bool> UpdatePsychologist(long id, UserDTO psychologist)
    {
        throw new NotImplementedException();
    }
}