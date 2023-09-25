using Microsoft.EntityFrameworkCore;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service;

public class AddressService : IAddressService
{
    private readonly PsychAppointmentContext _context;

    public AddressService(PsychAppointmentContext context)
    {
        _context = context;
    }
    
    
    public async Task<bool> AddAddress(Address address)
    {
        try
        {
            var id = await _context.Addresses.CountAsync() + 1;
            address.Id = id;
            await _context.Addresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<List<Address>> GetAllAddresses()
    {
        return await _context.Addresses.ToListAsync();
    }

    public async Task<bool> DeleteAddress(Address address)
    {
        try
        {
            var toDelete = await GetEquivalent(address);
            if (toDelete == null)
            {
                return false;
            }

            _context.Remove(toDelete);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Address?> GetEquivalent(Address address)
    {
        var allAddresses = await GetAllAddresses();
        foreach (var allAddress in allAddresses)
        {
            if (allAddress.Equals(address))
            {
                return allAddress;
            }
        }
        return null;
    }

    public async Task<bool> ClearOrphanedAddresses()
    {
        try
        {
            var orphanedAddresses = await _context.Addresses
                .Where(address => !_context.Clients.Any(client => client.AddressId == address.Id)
                                  && !_context.Psychologists.Any(psychologist => psychologist.AddressId == address.Id)
                                  && !_context.Managers.Any(manager => manager.AddressId == address.Id)
                                  && !_context.Admins.Any(admin => admin.AddressId == address.Id)
                                  && !_context.Locations.Any(loc => loc.AddressId == address.Id))
                .ToListAsync();
        
            foreach (var address in orphanedAddresses)
            {
                _context.Addresses.Remove(address);
            }
        
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}