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
    
    public partial class Booking
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Booking()
        {
            this.BookingUsers = new HashSet<BookingUser>();
            this.Transactions = new HashSet<Transaction>();
            this.Ratings = new HashSet<Rating>();
            this.Journals = new HashSet<Journal>();
            this.Counters = new HashSet<Counter>();
            this.BookingComments = new HashSet<BookingComment>();
            this.BookingProducts = new HashSet<BookingProduct>();
            this.BookingTags = new HashSet<BookingTag>();
        }
    
        public int Id { get; set; }
        public Nullable<int> ListingId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public BookingFor ForId { get; set; }
        public int UserId { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal Quantity { get; set; }
        public decimal Distance { get; set; }
        public bool IncludeFuel { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public decimal HireCost { get; set; }
        public decimal FuelCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal DestinationLatitude { get; set; }
        public decimal DestinationLongitude { get; set; }
        public string Destination { get; set; }
        public string AdditionalInformation { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal TransportDistance { get; set; }
        public BookingStatus StatusId { get; set; }
        public bool PaidOut { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool Deleted { get; set; }
        public decimal Commission { get; set; }
        public decimal AgentCommission { get; set; }
        public int SMSCount { get; set; }
        public decimal SMSCost { get; set; }
        public decimal IMTT { get; set; }
        public decimal TransactionFee { get; set; }
        public string ReceiptPhotoPath { get; set; }
        public short PaymentMethodId { get; set; }
        public Nullable<int> VoucherId { get; set; }
        public decimal VoucherTotal { get; set; }
        public short PaymentStatusId { get; set; }
        public string TagsJson { get; set; }
        public string ProductListJson { get; set; }
        public Nullable<int> SupplierId { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingUser> BookingUsers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual Listing Listing { get; set; }
        public virtual Service Service { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Rating> Ratings { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Journal> Journals { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Counter> Counters { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingComment> BookingComments { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingProduct> BookingProducts { get; set; }
        public virtual Voucher Voucher { get; set; }
        public virtual Supplier Supplier { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BookingTag> BookingTags { get; set; }
    }
}
