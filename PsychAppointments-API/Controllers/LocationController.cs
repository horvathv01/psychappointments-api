using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Models;
using PsychAppointments_API.Service;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointments_API.Controllers;

[ApiController, Route("location")]
public class LocationController : ControllerBase
{
    private IDataProtectionService<User> _userDPS;
    private ILocationService _locationService;
    private IUserService _userService;
    
    public LocationController(
        IDataProtectionService<User> userDPS,
        ILocationService locationService,
        IUserService userService
        )
    {
        _userDPS = userDPS;
        _locationService = locationService;
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<List<LocationDTO>?> GetAllLocations()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        var user = await _userService.GetUserById(userId);
        //var user = await GetLoggedInUser();
        
        if (user != null)
        {
            var query = async () => await _locationService.GetAllLocations();
            var allLocations = await _userDPS.Filter(user, query);
            return allLocations.ToList();
        }
        return null;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<LocationDTO?> GetLocationById(long id)
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        var user = await _userService.GetUserById(userId);
        //var user = await GetLoggedInUser();
        
        if (user != null)
        {
            var query = async () => await _locationService.GetLocationById(id);
            var location = await _userDPS.Filter(user, query);
            return location;
        }
        return null;
    }

    [HttpPost]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> AddNewLocation([FromBody] LocationDTO location)
    {
        Console.WriteLine("New location to be added:");
        Console.WriteLine(location);
        var query = async () => await _locationService.AddLocation(location);
        var result = await query();
        if (result)
        {
            string message = $"Location {location.Name} was added successfully";
            Console.WriteLine(message);
            return Ok(message);
        }

        return BadRequest("Something went wrong");
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteLocation(long id)
    {
        Console.WriteLine($"Location with id {id} is to be deleted.");
        var query = async () => await _locationService.DeleteLocation(id);
        var result = await query();
        if (result)
        {
            string message = $"Location with id {id} was deleted successfully.";
            Console.WriteLine(message);
            return Ok(message);
        }
        Console.WriteLine($"Deletion of location with id {id} failed.");
        return BadRequest("Something went wrong");
    }
    

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin, Manager")]
    public async Task<IActionResult> UpdateLocation(long id, LocationDTO location)
    {
        Console.WriteLine("Location to be updated:");
        Console.WriteLine(location);
        var query = async () => await _locationService.UpdateLocation(id, location);
        var result = await query();
        if (result)
        {
            string message = $"Location {location.Name} was updated successfully";
            Console.WriteLine(message);
            return Ok(message);
        }

        return BadRequest("Something went wrong");
    }
    
}