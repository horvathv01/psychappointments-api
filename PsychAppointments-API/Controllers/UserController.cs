using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
using PsychAppointments_API.Service;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointments_API.Controllers;

[ApiController, Route("user")]
public class UserController : ControllerBase
{
    private IDataProtectionService<User> _userDPS;
    private IUserService _userService;
    
    public UserController(
    IDataProtectionService<User> userDPS,
    IUserService userService
    )
    {
        _userDPS = userDPS;
        _userService = userService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<UserDTO?> GetUser(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _userService.GetUserById(id);
            return await _userDPS.Filter(user, query);    
        }
        return null;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllUsers()
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _userService.GetAllUsers();
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }

    [HttpGet("email/{email}")]
    [Authorize]
    public async Task<UserDTO?> GetUserByEmail(string email)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _userService.GetUserByEmail(email);
            return await _userDPS.Filter(user, query);    
        }
        return null;
    }

    [HttpGet("booking/{email}")]
    [Authorize]
    public async Task<UserDTO?> GetLimitedInfoOfUserForBooking(string email)
    {
        var user = await GetLoggedInUser();
        if (user != null && user.Type != UserType.Client)
        {
            return await _userService.GetFilteredUserDataForBookingByEmail(email);
        }
        return null;
    }

    [HttpGet("allmanagers")]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllManagers()
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _userService.GetAllManagers();
            var allManagers = await _userDPS.Filter(user, query);
            return allManagers.ToList();
        }
        return null;
    }

    [HttpGet("managers/location/{id}")]
    [Authorize]
    public async Task<List<UserDTO>?> GetManagersByLocation(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            Console.WriteLine($"{user.Type} {user.Name} accessed Managers by Location id {id}.");
            var query = async () => await _userService.GetManagersByLocation(id);
            var allPsychologists = await _userDPS.Filter(user, query);
            return allPsychologists.ToList();
        }
        return null;
    }
    
    [HttpGet("psychologists/location/{id}")]
    [Authorize]
    public async Task<List<UserDTO>?> GetPsychologistsByLocation(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            Console.WriteLine($"{user.Type} {user.Name} accessed Psychologists by Location id {id}.");
            var query = async () => await _userService.GetPsychologistsByLocation(id);
            var allPsychologists = await _userDPS.Filter(user, query);
            return allPsychologists.ToList();
        }
        return null;
    }

    [HttpGet("allpsychologists")]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllPsychologists()
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            Console.WriteLine($"{user.Type} {user.Name} accessed GetAllPsychologists.");
            var query = async () => await _userService.GetAllPsychologists();
            var allPsychologists = await _userDPS.Filter(user, query);
            return allPsychologists.ToList();
        }
        return null;
    }

    [HttpGet("allclients")]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllClients()
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            Console.WriteLine($"{user.Type} {user.Name} accessed GetAllClients.");
            var query = async () => await _userService.GetAllClients();
            var allPsychologists = await _userDPS.Filter(user, query);
            return allPsychologists.ToList();
        }
        return null;
    }
    
    [HttpGet("clients/location/{id}")]
    [Authorize]
    public async Task<List<UserDTO>?> GetClientsByLocation(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            Console.WriteLine($"{user.Type} {user.Name} accessed Clients by Location id {id}.");
            var query = async () => await _userService.GetClientsByLocation(id);
            var allPsychologists = await _userDPS.Filter(user, query);
            return allPsychologists.ToList();
        }
        return null;
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UserDTO updatedUser, long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _userService.UpdateUser(id, updatedUser);
            var result = await query();
            if (result)
            {
                return Ok($"User with id {id} has been successfully updated by {user.Type} {user.Name}.");
            }

            return BadRequest("Something went wrong");
        }

        return Unauthorized("User could not be retreived.");
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _userService.DeleteUser(id);
            var result = await query();
            if (result)
            {
                return Ok($"User with id {id} has been successfully deleted by {user.Type} {user.Name}.");
            }

            return BadRequest("Something went wrong");
        }

        return Unauthorized("User could not be retreived.");
    }

    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        return await _userService.GetUserById(userId);
    }
    
}