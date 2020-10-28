using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class PopModel
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public PopPhotoModel Photo { get; set; }
    }

    public class PopPhotoModel
    {
        public string Filename { get; set; }
        public string Base64 { get; set; }
    }
}