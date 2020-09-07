using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class ReconcileLedgerModel
    {
        public File ExcelFile { get; set; }
    }
}