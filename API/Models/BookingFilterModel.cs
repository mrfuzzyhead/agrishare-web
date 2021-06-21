using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class BookingFilterModel
    {
        public string Query { get; set; }
        public int UserId { get; set; }
        public int AgentId { get; set; }
        public int Status { get; set; } = -1;
        public int Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}