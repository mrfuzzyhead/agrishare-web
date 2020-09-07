using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class ExportLedgerModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}