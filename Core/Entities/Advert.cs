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
    
    public partial class Advert
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PhotoPath { get; set; }
        public string LinkUrl { get; set; }
        public int ImpressionCount { get; set; }
        public int ClickCount { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool Deleted { get; set; }
        public int RegionId { get; set; }
    
        public virtual Region Region { get; set; }
    }
}
