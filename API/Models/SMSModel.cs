using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class SMSModel
    {
        [Required]
        public string Message { get; set; }

    }
}