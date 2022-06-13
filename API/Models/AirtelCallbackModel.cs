using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class AirtelCallbackModel
    {
        public AirtelCallbackTransaction transaction { get; set; }
    }

    public class AirtelCallbackTransaction
    {
        public string id { get; set; }

        public string message { get; set; }

        public string status_code { get; set; }

        public string airtel_money_id { get; set; }
    }
}