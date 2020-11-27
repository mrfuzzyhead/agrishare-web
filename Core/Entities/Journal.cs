//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Agrishare.Core.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Journal
    {
        public int Id { get; set; }
        public int RegionId { get; set; }
        public JournalType TypeId { get; set; }
        public string Title { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> BookingId { get; set; }
        public decimal Amount { get; set; }
        public bool Reconciled { get; set; }
        public string EcoCashReference { get; set; }
        public System.DateTime Date { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool Deleted { get; set; }
    
        public virtual Booking Booking { get; set; }
        public virtual User User { get; set; }
        public virtual Region Region { get; set; }
    }
}
