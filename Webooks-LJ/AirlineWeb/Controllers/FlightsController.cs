﻿using AirlineWeb.Data;
using AirlineWeb.Dtos;
using AirlineWeb.MessageBus;
using AirlineWeb.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AirlineWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightsController : ControllerBase
    {
        private readonly AirlineDbContext _context;
        private readonly IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;

        public FlightsController(AirlineDbContext context, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _context = context;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
        }
        [HttpGet("{flightCode}", Name = "GetFlightDetailsByCode")]
        public ActionResult<FlightDetailReadDto> GetFlightDetailsByCode(string flightCode)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightCode);

            if (flight == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<FlightDetailReadDto>(flight));
        }

        [HttpPost]
        public ActionResult<FlightDetailReadDto> CreateFlight(FlightDetailCreateDto flightDetailCreateDto)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.FlightCode == flightDetailCreateDto.FlightCode);

            if (flight == null)
            {
                var flightDetailsModel = _mapper.Map<FlightDetail>(flightDetailCreateDto);
                try
                {
                    _context.Add(flightDetailsModel);
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                var flightDetailReadDto = _mapper.Map<FlightDetailReadDto>(flightDetailsModel);

                return CreatedAtRoute(nameof(GetFlightDetailsByCode), new { flightCode = flightDetailReadDto.FlightCode }, flightDetailReadDto);
            }

            return BadRequest();
        }

        [HttpPut("{id}")]
        public ActionResult UpdateFlightDetail(int id, FlightDetailUpdateDto flightDetailUpdateDto)
        {
            var flight = _context.FlightDetails.FirstOrDefault(f => f.Id == id);

            if(flight == null)
            {
                return NotFound();
            }

            decimal oldPrice = flight.Price;

            _mapper.Map(flightDetailUpdateDto, flight);

            try
            {
                _context.SaveChanges();
                if(oldPrice != flight.Price)
                {
                    Console.WriteLine("Price changed - place message on bus");
                    var message = new NotificationMessageDto
                    {
                        WebhookType = "Price Changed",
                        NewPrice = flight.Price,
                        OldPrice = oldPrice,
                        FlightCode = flight.FlightCode
                    };
                    _messageBusClient.SendMessage(message);
                }
                else
                {
                    Console.WriteLine("No Price Change");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
