﻿using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class ListingModel
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string Brand { get; set; }
        
        public int? HorsePower { get; set; }

        public int? Year { get; set; }

        public ListingCondition ConditionId { get; set; }

        public bool GroupServices { get; set; }

        public List<ListingPhotoModel> Photos { get; set; }

        public ListingStatus StatusId { get; set; }

        [Required]
        public List<ListingServiceModel> Services { get; set; }

        public bool AvailableWithoutFuel { get; set; }
        public bool AvailableWithFuel { get; set; }
    }

    public class ListingServiceModel
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public bool Mobile { get; set; }
        public decimal TotalVolume { get; set; }
        public QuantityUnit QuantityUnitId { get; set; }
        public TimeUnit TimeUnitId { get; set; }
        public DistanceUnit DistanceUnitId { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal MaximumDistance { get; set; }
        public decimal PricePerQuantityUnit { get; set; }
        public decimal FuelPerQuantityUnit { get; set; }
        public decimal TimePerQuantityUnit { get; set; }
        public decimal? PricePerDistanceUnit { get; set; }
        public decimal FuelPrice { get; set; }
    }


    public class ListingPhotoModel
    {
        public string Filename { get; set; }
        public string Base64 { get; set; }
    }
}