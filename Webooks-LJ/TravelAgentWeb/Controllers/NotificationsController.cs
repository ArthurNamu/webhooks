using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelAgentWeb.Data;
using TravelAgentWeb.Dtos;

namespace TravelAgentWeb.Controllers;
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly TravelAgentDbContext _context;

    public NotificationsController(TravelAgentDbContext context)
	{
        _context = context;

    }

    [HttpPost]
    public ActionResult FlightChanges(FlightDetailUpdateDto flightDetailUpdateDto)
    {
        Console.WriteLine($"--> Webhook received from : {flightDetailUpdateDto.Publisher}");

        var secretModel = _context.SubscriptionSecrets.FirstOrDefault(s =>
        s.Publisher == flightDetailUpdateDto.Publisher && s.Secret == flightDetailUpdateDto.Secret);

        if(secretModel == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid Secret!! - Ignore webhook!!");
            Console.ResetColor();
            return Ok();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Valid Webhook - Proceed to process!!");
            Console.WriteLine($"Old price {flightDetailUpdateDto.OldPrice},  New Price {flightDetailUpdateDto.NewPrice} ");
            Console.ResetColor();
            return Ok();
        }
    }
}
