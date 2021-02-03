using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class ProductAvailabilityModel
    {
        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public List<int> ProductIds { get; set; }

        public int Days { get; set; } = 1;
    }
}