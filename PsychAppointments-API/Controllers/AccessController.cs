using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Auth;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
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
    public async Task<IActionResult> RegisterUser([FromBody] UserDTO user)
    {
        //check if user has already registered with this email
        var registeredUser = await _userService.GetUserByEmail(user.Email);
        Console.WriteLine(user);
        if (registeredUser != null)
        {
            Console.WriteLine(registeredUser);
            return Conflict("This email has already been registered.");
        }

        //user.Type = Enum.GetName(typeof(UserType), UserType.Client);
        await _userService.AddUser(user);
        
        return Ok($"{user.Type} {user.Name} has been successfully registered.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser()
    {
        string authorizationHeader = HttpContext.Request.Headers["Authorization"];

        var base64String = Convert.FromBase64String(authorizationHeader);
        var credentials = Encoding.UTF8.GetString(base64String);
        var parts = credentials.Split(":");
        var email = parts[0];
        var pass = parts[1];
        var user = await _userService.GetUserByEmail(email);
        
        if (user == null)
        {
            Console.WriteLine("Authorization failed: user == null");
            return Unauthorized();
        }

        var authenticated = _hasher.Authenticate(email, user.Password, pass);

        if (authenticated == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Authentication, user.Id.ToString())
            };
            var roleName = Enum.GetName(typeof(UserType), user.Type);
            claims.Add(new Claim(ClaimTypes.Role, roleName ?? throw new InvalidOperationException("Invalid role name")));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                authProperties);
            
            Console.WriteLine($"Authorization successful,{user.Type} {user.Name} logged in successfully.");
            UserDTO loggedInUser = new UserDTO(user);
            loggedInUser.Password = "";
            
            return Ok(loggedInUser);    
        }
        Console.WriteLine("Authorization failed: password mismatch");
        return Unauthorized();
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutUser()
    {
        try
        {
            string userName = HttpContext.User.Identity.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Console.WriteLine($"{userName} logged out successfully.");
            return Ok($"{userName} logged out successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Logout failed: {e.Message}");
        }
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UserDTO newUser)
    {
        //email is used for password hashing!!!! we need to rehash password with new email and save it!
        long userId = long.Parse(HttpContext.User.Claims.First(claim => claim.Type == ClaimTypes.Authentication).Value);
        var user = await _userService.GetUserById(userId);
        
        var result = await _userService.UpdateUser(userId, newUser);
        if (result)
        {
            return Ok($"{Enum.GetName(typeof(UserType), user.Type)} {user.Name} has been updated successfully");    
        }

        return BadRequest("Something went wrong");
    }


}