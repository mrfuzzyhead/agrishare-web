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
    
    public partial class Category
    {
        public int Id { get; set; }
        public Nullable<int> ParentId { get; set; }
        public string Title { get; set; }
        public System.DateTime DateCreated { get; set; }
        public System.DateTime LastModified { get; set; }
        public bool Deleted { get; set; }
        public string TitleShona { get; set; }
        public string TitleNdebele { get; set; }
        public string TitleLuganda { get; set; }
    }
}
