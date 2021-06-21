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
    
    public partial class Listing
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Listing()
        {
            this.Services = new HashSet<Service>();
        }
    
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal AverageRating { get; set; }
        public int RatingCount { get; set; }
        public string Location { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string Brand { get; set; }
        public Nullable<int> HorsePower { get; set; }
        public Nullable<int> Year { get; set; }
        public bool AvailableWithoutFuel { get; set; }
        public bool AvailableWithFuel { get; set; }
        public ListingCondition ConditionId { get; set; }
        public bool GroupServices { get; set; }
        public string PhotoPaths { get; set; }
        public ListingStatus StatusId { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool Deleted { get; set; }
        public int RegionId { get; set; }
        public bool Trending { get; set; }
    
        public virtual User User { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Service> Services { get; set; }
        public virtual Region Region { get; set; }
    }
}
