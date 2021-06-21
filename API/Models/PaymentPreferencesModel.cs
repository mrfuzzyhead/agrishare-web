using Agrishare.Core.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class PaymentPreferencesModel
    {
        public bool Cash { get; set; }
        public bool BankTransfer { get; set; }
        public bool MobileMoney { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }

    }
}