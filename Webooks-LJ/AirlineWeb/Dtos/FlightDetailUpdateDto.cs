﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AirlineWeb.Dtos
{
    public class FlightDetailUpdateDto
    {
        [Required]
        public string FlightCode { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
}
