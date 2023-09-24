using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public interface IAddressService
{
    Task<bool> AddAddress(Address address);

    //Task<Address> GetAddressById(long id);

    Task<List<Address>> GetAllAddresses();

    Task<bool> DeleteAddress(Address address);

    Task<Address?> GetEquivalent(Address address);
}