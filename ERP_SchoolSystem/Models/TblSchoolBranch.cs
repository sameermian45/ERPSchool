//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP_SchoolSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TblSchoolBranch
    {
        public int BranchID { get; set; }
        public string BranchTitle { get; set; }
        public int SchoolID { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    
        public virtual TblSchool TblSchool { get; set; }
    }
}
