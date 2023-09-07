using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.DAL;
using PsychAppointments_API.Models;
using PsychAppointments_API.Models.Enums;
using PsychAppointments_API.Service;

namespace PsychAppointments_API.Controllers;

[ApiController, Route("prepopulate")]
public class PrepopulateController : ControllerBase
{
    private readonly IPrepopulate _prepopulate;

    public PrepopulateController(IPrepopulate prepopulate)
    {
        _prepopulate = prepopulate;
    }

    [HttpGet]
    public async Task<IActionResult> Prepopulate()
    {
        try
        {
            await _prepopulate.PrepopulateInMemory();
            //await _prepopulate.PrepopulateDB();
            string message = "InMemory repositories have been prepopulated";
            Console.WriteLine(message);
            return Ok(message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return BadRequest("something went wrong");
        }
        
    }
}