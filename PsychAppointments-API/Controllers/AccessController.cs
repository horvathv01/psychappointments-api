using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Auth;
using PsychAppointments_API.Models;
using PsychAppointments_API.Service;

namespace PsychAppointments_API.Controllers;

[ApiController, Route("access")]
public class AccessController : ControllerBase
{
    private readonly IAccessUtilities _hasher;
    private readonly IUserService _userService;

    public AccessController(IAccessUtilities hasher, IUserService userService)
    {
        _hasher = hasher;
        _userService = userService;
    }

    [HttpPost("registration")]
    public async Task<IActionResult> RegisterUser([FromBody] User user)
    {
        
        return Ok();
    }


}