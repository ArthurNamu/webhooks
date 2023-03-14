using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookSubscriptionController :ControllerBase
{
	private readonly AirlineDbContext _context;
	private readonly IMapper _mapper;

	public WebhookSubscriptionController(AirlineDbContext context, IMapper mapper)
	{
		_context = context;
		_mapper =  mapper;
	}

	[HttpGet("{secret}", Name = "GetSubscriptionsBySecret")]
	public ActionResult<WebhookSubscriptionReadDto> GetSubscriptionsBySecret(string secret)
	{
		var subscriprion = _context.WebhookSubscriptions.FirstOrDefault(s => s.Secret == secret);

		if (subscriprion == null)
			return NotFound();
		return Ok(_mapper.Map<WebhookSubscriptionReadDto>(subscriprion));
	}
            [HttpPost]
	public ActionResult<WebhookSubscriptionReadDto> CreateSubscription(WebhookSubscriptionCreateDto webhookSubscriptionCreateDto)
	{
		var subscription = _context.WebhookSubscriptions.FirstOrDefault(s => s.WebhookURI == webhookSubscriptionCreateDto.WebhookURI);

		if(subscription == null)
		{
			subscription = _mapper.Map<WebhookSubscription>(webhookSubscriptionCreateDto);

			subscription.Secret = Guid.NewGuid().ToString();
			subscription.WebhookPublisher = "KenyaAirways";
			try
			{
				_context.Add(subscription);
				_context.SaveChanges();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}

			var webhookBubscriptionReadDto = _mapper.Map<WebhookSubscriptionReadDto>(subscription);

			return CreatedAtRoute(nameof(GetSubscriptionsBySecret), new { secret = webhookBubscriptionReadDto.Secret }, webhookBubscriptionReadDto);
		}
		else
		{
			return NoContent();
		}
	}
}
