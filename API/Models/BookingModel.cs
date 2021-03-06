﻿using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class BookingModel
    {
        public int Id { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Required]
        public BookingFor ForId { get; set; }

        public string Location { get; set; }

        [Required]
        public decimal Latitude { get; set; }

        [Required]
        public decimal Longitude { get; set; }

        public string Destination { get; set; }

        public decimal DestinationLatitude { get; set; }

        public decimal DestinationLongitude { get; set; }

        [Required]
        public decimal Quantity { get; set; }

        public bool IncludeFuel { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string AdditionalInformation { get; set; }

        public decimal TotalVolume { get; set; }

        public decimal HireCost { get; set; }
        public decimal FuelCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal TransportDistance { get; set; }
        public decimal Distance { get; set; }
    }
}