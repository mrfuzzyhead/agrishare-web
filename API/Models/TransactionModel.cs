using Agrishare.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agrishare.API.Models
{
    public class TransactionModel
    {
        [Required]
        public int BookingId { get; set; }

        public int VoucherId { get; set; }

        public PaymentGateway Gateway { get; set; } = PaymentGateway.None;

        [Required]
        public List<BookingUserModel> Users { get; set; }
    }

    public class BookingUserModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public decimal Quantity { get; set; }
    }
}