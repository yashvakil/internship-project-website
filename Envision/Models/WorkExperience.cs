//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Envision.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class WorkExperience
    {
        public string S_Id { get; set; }
        public string Name { get; set; }
        public string GoogleId { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Designation { get; set; }
    
        public virtual Student Student { get; set; }
    }
}