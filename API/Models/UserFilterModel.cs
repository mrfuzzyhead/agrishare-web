using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class UserFilterModel
    {
        public string Query { get; set; }
        public Gender Gender { get; set; }
        public UserFilterView View { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}