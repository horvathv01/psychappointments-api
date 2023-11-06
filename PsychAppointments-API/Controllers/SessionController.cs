using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
using PsychAppointments_API.Service;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointments_API.Controllers;

[ApiController, Route("session")]
public class SessionController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly IUserService _userService;
    private readonly IPsychologistService _psychologistService;
    private readonly ILocationService _locationService;
    private IDataProtectionService<User> _userDPS;

    public SessionController(
        ISessionService sessionService, 
        IUserService userService,
        IPsychologistService psychologistService,
        ILocationService locationService,
        IDataProtectionService<User> userDPS
    )
    {
        _sessionService = sessionService;
        _userService = userService;
        _psychologistService = psychologistService;
        _locationService = locationService;
        _userDPS = userDPS;
    }
    
    [HttpGet] 
    [Authorize]
    public async Task<List<SessionDTO>?> GetAllSessions()
    {
        var user = await GetLoggedInUser();

        if (user != null)
        {
            var query = async () => await _sessionService.GetAllSessions();
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    //get non-blank sessions only
    [HttpGet("nonblank")]
    [Authorize]
    public async Task<List<SessionDTO>?> GetNonBlankSessions()
    {
        var user = await GetLoggedInUser();
        
        if (user != null)
        {
            var query = async () => await _sessionService.GetNonBlankSessions();
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<SessionDTO?> GetSessionById(long id)
    {
        var user = await GetLoggedInUser();

        if (user != null)
        {
            var query = async () => await _sessionService.GetSessionById(id);
            return await _userDPS.Filter(user, query);
        }
        return null;
    } 
    
    
    //get sessions by psychologist
    [HttpGet("psychologist/{id}")]
    [Authorize]
    public async Task<List<SessionDTO>?> GetSessionsByPsychologist(long psychologistId)
    {
        var user = await GetLoggedInUser();
        var psychologist = await _psychologistService.GetPsychologistById(psychologistId);
        
        if (user != null && psychologist != null)
        {
            var query = async () => await _sessionService.GetSessionsByPsychologist(psychologist);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    //get sessions by psychologist and dates
    [HttpGet("psychologist")]
    [Authorize]
    public async Task<List<SessionDTO>?> GetSessionsByPsychologistWithDates(
        [FromQuery] long id,
        [FromQuery] string startDate, 
        [FromQuery] string endDate
        )
    {
        var user = await GetLoggedInUser();
        var psychologist = await _psychologistService.GetPsychologistById(id);
        
        DateTime startDateParsed = DateTime.MinValue;
        DateTime endDateParsed = DateTime.MinValue;
        DateTime.TryParse(startDate, out startDateParsed);
        DateTime.TryParse(endDate, out endDateParsed);
        
        if (user != null && psychologist != null && startDateParsed != DateTime.MinValue && endDateParsed != DateTime.MinValue)
        {
            startDateParsed = DateTime.SpecifyKind(startDateParsed, DateTimeKind.Utc);
            endDateParsed = DateTime.SpecifyKind(endDateParsed, DateTimeKind.Utc);
            var query = async () => await _sessionService.GetSessionsByPsychologist(psychologist, startDateParsed, endDateParsed);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    //get sessions by psychologist and location
    [HttpGet("psychologist/location")]
    [Authorize]
    public async Task<List<SessionDTO>?> GetSessionsByPsychologistAndLocationWithDates(
        [FromQuery] long psychologistId,
        [FromQuery] long locationId, 
        [FromQuery] string startDate, 
        [FromQuery] string endDate
    )
    {
        var user = await GetLoggedInUser();
        var psychologist = await _psychologistService.GetPsychologistById(psychologistId);
        
        var location = await _locationService.GetLocationById(locationId);
        
        DateTime startDateParsed = DateTime.MinValue;
        DateTime endDateParsed = DateTime.MinValue;
        DateTime.TryParse(startDate, out startDateParsed);
        DateTime.TryParse(endDate, out endDateParsed);
        
        if (user != null && psychologist != null && location != null && startDateParsed != DateTime.MinValue && endDateParsed != DateTime.MinValue)
        {
            startDateParsed = DateTime.SpecifyKind(startDateParsed, DateTimeKind.Utc);
            endDateParsed = DateTime.SpecifyKind(endDateParsed, DateTimeKind.Utc);
            var query = async () => await _sessionService.GetSessionsByPsychologistLocationAndDates(psychologist, location, startDateParsed, endDateParsed);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    [HttpGet("location")]
    [Authorize]
    public async Task<List<SessionDTO>?> GetSessionsByLocationWithDates(
        [FromQuery] long id, 
        [FromQuery] string startDate, 
        [FromQuery] string endDate
    )
    {
        var user = await GetLoggedInUser();
        
        var location = await _locationService.GetLocationById(id);
        
        DateTime startDateParsed = DateTime.MinValue;
        DateTime endDateParsed = DateTime.MinValue;
        DateTime.TryParse(startDate, out startDateParsed);
        DateTime.TryParse(endDate, out endDateParsed);
        
        
        
        if (user != null && location != null && startDateParsed != DateTime.MinValue && endDateParsed != DateTime.MinValue)
        {
            startDateParsed = DateTime.SpecifyKind(startDateParsed, DateTimeKind.Utc);
            endDateParsed = DateTime.SpecifyKind(endDateParsed, DateTimeKind.Utc);
            var query = async () => await _sessionService.GetSessionsByLocation(location, startDateParsed, endDateParsed);
            var result = await _userDPS.Filter(user, query);
            
            return result.ToList();
        }
        return new List<SessionDTO>();
    }
    
    [HttpGet("client")]
    [Authorize]
    public async Task<List<SessionDTO>?> GetSessionsByClientAndDates(
        [FromQuery] long id, 
        [FromQuery] string startDate, 
        [FromQuery] string endDate
    )
    {
        var user = await GetLoggedInUser();
        
        Client? client = (user != null && user.Id == id) ? (Client?)user : (Client?)await _userService.GetUserById(id);
        
        DateTime startDateParsed = DateTime.MinValue;
        DateTime endDateParsed = DateTime.MinValue;
        DateTime.TryParse(startDate, out startDateParsed);
        DateTime.TryParse(endDate, out endDateParsed);
        
        if (user != null && client != null && client.Type == UserType.Client && startDateParsed != DateTime.MinValue && endDateParsed != DateTime.MinValue)
        {
            startDateParsed = DateTime.SpecifyKind(startDateParsed, DateTimeKind.Utc);
            endDateParsed = DateTime.SpecifyKind(endDateParsed, DateTimeKind.Utc);
            var query = async () => await _sessionService.GetSessionsByClient(client, startDateParsed, endDateParsed);
            var result = await _userDPS.Filter(user, query);
            
            return result.ToList();
        }
        return new List<SessionDTO>();
    }
    

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddSession([FromBody] SessionDTO session)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            try
            {
                var query = async () => await _sessionService.AddSession(session);
                await query();
                return Ok("Session was added successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest("Something went wrong.");
            }
        }
        return Unauthorized();
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateSession([FromBody] SessionDTO newSession, long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {

            var session = await _sessionService.GetSessionById(id);
            bool userIsAssociatedWithSession;
            if (session != null)
            {
                userIsAssociatedWithSession = session.Blank || _userDPS.IsAssociated(user, session);
            }
            else
            {
                return BadRequest("Something went wrong: session not found.");
            }
            //if user is not associated with session, don't let him change anything!
            if(!userIsAssociatedWithSession)
            {
                return Unauthorized("Procedure is unauthorized");
            }

            if (user.Type != UserType.Psychologist && user.Type != UserType.Admin)
            {
                //only admins and the session's psychologists can change price
                newSession.Price = session.Price;
            }
            
            var query = async () => await _sessionService.UpdateSession(id, newSession);
            var result = await query();
            if (result)
            {
                return Ok("Session was updated successfully.");
            }
            return BadRequest("Something went wrong.");
        }
        return Unauthorized();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteSession(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var session = await _sessionService.GetSessionById(id);
            bool userIsAssociatedWithSession;
            if (session != null)
            {
                userIsAssociatedWithSession = _userDPS.IsAssociated(user, session);
            }
            else
            {
                return BadRequest("Something went wrong: session not found.");
            }
            //don't let anyone delete a session --> admin or session's psychologist and partner only!
            if(!(userIsAssociatedWithSession && (user.Type == UserType.Admin || user.Type == UserType.Psychologist)))
            {
                return Unauthorized("Procedure is unauthorized");
            }
            

            var query = async () => await _sessionService.DeleteSession(id);
            var result = await query();
            if (result)
            {
                return Ok("Session was deleted successfully.");
            }
            return BadRequest("Something went wrong.");
        }
        return Unauthorized();
    }
    
    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        return await _userService.GetUserById(userId);
    }
}