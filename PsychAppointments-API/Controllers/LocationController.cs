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
    [Authorize]
    public async Task<IActionResult> AddNewLocation([FromBody] LocationDTO location)
    {
        var query = async () => await _locationService.AddLocation(location);
        var result = await query();
        if (result)
        {
            return Ok($"Location {location.Name} was added successfully");
        }

        return BadRequest("Something went wrong");
    }
    
}