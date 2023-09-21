using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Models;
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
    IUserService userService)
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

    [HttpGet("test/{id}")]
    public async Task<UserDTO> GetUserTest(long id)
    {
        var user = await _userService.GetUserById(id);
        return new UserDTO(user);
    }

    [HttpGet("allmanagers")]
    [Authorize]
    public async Task<List<UserDTO>> GetAllManagers()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        var user = await _userService.GetUserById(userId);
        //var user = await GetLoggedInUser();
        
        if (user != null)
        {
            var query = async () => await _userService.GetAllManagers();
            var allManagers = await _userDPS.Filter(user, query);
            return allManagers.ToList();
        }
        return null;
        
        
    }

    [HttpGet("allpsychologists")]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllPsychologists()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value,
            out userId);

        var user = await _userService.GetUserById(userId);
        
        if (user != null)
        {
            Console.WriteLine($"{user.Type} {user.Name} accessed GetAllPsychologists.");
            var query = async () => await _userService.GetAllPsychologists();
            var allPsychologists = await _userDPS.Filter(user, query);
            return allPsychologists.ToList();
        }
        return null;
    }

    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        return await _userService.GetUserById(userId);
    }

}