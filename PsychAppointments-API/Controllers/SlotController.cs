using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Models;
using PsychAppointments_API.Service;
using PsychAppointments_API.Service.DataProtection;

namespace PsychAppointments_API.Controllers;

[ApiController, Route("slot")]
public class SlotController : ControllerBase
{
    private readonly ISlotService _slotService;
    private readonly IUserService _userService;
    private readonly IPsychologistService _psychologistService;
    private readonly ILocationService _locationService;
    private IDataProtectionService<User> _userDPS;

    public SlotController(
        ISlotService slotService, 
        IUserService userService,
        IPsychologistService psychologistService,
        ILocationService locationService,
        IDataProtectionService<User> userDPS
        )
    {
        _slotService = slotService;
        _userService = userService;
        _psychologistService = psychologistService;
        _locationService = locationService;
        _userDPS = userDPS;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddNewSlot([FromBody] SlotDTO slot)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _slotService.AddSlot(slot, true);
            var result = await query();
            if (result)
            {
                return Ok("A new slot has been successfully added.");
            }

            return BadRequest("Something went wrong.");
        }
        return Unauthorized("User could not be retrieved.");
    }
    
    [HttpGet("psychologist/location")]
    [Authorize]
    public async Task<List<SlotDTO>?> GetSlotsByPsychologistAndLocationWithDates(
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
            var query = async () => await _slotService.GetSlotsByPsychologistLocationAndDates(psychologist, location, startDateParsed, endDateParsed);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    [HttpGet("psychologist")]
    [Authorize]
    public async Task<List<SlotDTO>?> GetSlotsByPsychologistWithDates(
        [FromQuery] long psychologistId,
        [FromQuery] string startDate, 
        [FromQuery] string endDate
    )
    {
        var user = await GetLoggedInUser();
        var psychologist = await _psychologistService.GetPsychologistById(psychologistId);
        
        DateTime startDateParsed = DateTime.MinValue;
        DateTime endDateParsed = DateTime.MinValue;
        DateTime.TryParse(startDate, out startDateParsed);
        DateTime.TryParse(endDate, out endDateParsed);
        
        if (user != null && psychologist != null && startDateParsed != DateTime.MinValue && endDateParsed != DateTime.MinValue)
        {
            startDateParsed = DateTime.SpecifyKind(startDateParsed, DateTimeKind.Utc);
            endDateParsed = DateTime.SpecifyKind(endDateParsed, DateTimeKind.Utc);
            var query = async () => await _slotService.GetSlotsByPsychologistAndDates(psychologist, startDateParsed, endDateParsed);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }

    [HttpGet]
    [Authorize]
    public async Task<List<SlotDTO>?> GetAllSlots()
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _slotService.GetAllSlots();
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    [HttpGet("{id}")]
    [Authorize]
    public async Task<List<SlotDTO>?> GetSlotById(long id)
    {
        var user = await GetLoggedInUser();
        if (user != null)
        {
            var query = async () => await _slotService.GetSlotById(id);
            //return await _userDPS.Filter(user, query);
            return null;
        }
        return null;
    }
    
    [HttpGet("psychologist/{id}")]
    [Authorize]
    public async Task<List<SlotDTO>?> GetSlotsByPsychologist(long id)
    {
        var user = await GetLoggedInUser();
        var psychologist = await _psychologistService.GetPsychologistById(id);
        
        if (user != null && psychologist != null)
        {
            var query = async () => await _slotService.GetSlotsByPsychologistAndDates(psychologist);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    [HttpGet("psychologist/{id}/date")]
    [Authorize]
    public async Task<List<SlotDTO>?> GetSlotsByPsychologistWithDates([FromRoute] long id, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var user = await GetLoggedInUser();
        var psychologist = await _psychologistService.GetPsychologistById(id);
        
        if (user != null && psychologist != null)
        {
            DateTime.SpecifyKind(startDate, DateTimeKind.Utc);
            DateTime.SpecifyKind(endDate, DateTimeKind.Utc);
            var query = async () => await _slotService.GetSlotsByPsychologistAndDates(psychologist, startDate, endDate);
            var result = await _userDPS.Filter(user, query);
            return result.ToList();
        }
        return null;
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateSlot(long id, SlotDTO slot)
    {
        var user = await GetLoggedInUser();
        
        if (user != null)
        {
            var query = async () => await _slotService.UpdateSlot(id, slot);
            var result = await query();
            if (result)
            {
                return Ok("Updating slot was successful.");    
            }

            return BadRequest("Something went wrong");
        }
        return Unauthorized("User could not be retrieved.");
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteSlot(long id)
    {
        var user = await GetLoggedInUser();
        
        if (user != null)
        {
            var query = async () => await _slotService.DeleteSlot(id);
            var result = await query();
            if (result)
            {
                return Ok("Deleting slot was successful.");    
            }

            return BadRequest("Something went wrong");
        }
        return Unauthorized("User could not be retrieved.");
    }
    
    
    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Authentication).Value, out userId);
        return await _userService.GetUserById(userId);
    }
}