﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class AgrishareEntities : DbContext
    {
        public AgrishareEntities()
            : base("name=AgrishareEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Booking> Bookings { get; set; }
        public virtual DbSet<BookingUser> BookingUsers { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<Faq> Faqs { get; set; }
        public virtual DbSet<Listing> Listings { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Counter> Counters { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }
        public virtual DbSet<Journal> Journals { get; set; }
        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<TransactionFee> TransactionFees { get; set; }
        public virtual DbSet<Voucher> Vouchers { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierService> SupplierServices { get; set; }
        public virtual DbSet<UserVoucher> UserVouchers { get; set; }
        public virtual DbSet<BookingComment> BookingComments { get; set; }
        public virtual DbSet<BookingTag> BookingTags { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
    }
}
