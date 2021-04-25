using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class ContactModel
    {
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
    }
}