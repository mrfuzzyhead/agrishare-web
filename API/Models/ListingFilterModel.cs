using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class ListingFilterModel
    {
        public string Query { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }

    }
}