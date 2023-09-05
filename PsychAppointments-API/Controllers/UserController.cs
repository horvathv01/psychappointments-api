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

    [HttpGet("user/{id}")]
    [Authorize]
    public async Task<UserDTO?> GetUser(long id)
    {
        var userId = long.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value);
        var user = await _userService.GetUserById(userId);
        if (user != null)
        {
            var query = async () => await _userService.GetUserById(id);
            return await _userDPS.Filter(user, query);    
        }
        return null;
    }

}