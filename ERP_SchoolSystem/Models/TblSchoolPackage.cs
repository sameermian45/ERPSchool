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
    
    public partial class TblSchoolPackage
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TblSchoolPackage()
        {
            this.TblSchoolBranchesFees = new HashSet<TblSchoolBranchesFee>();
            this.TblSchoolFees = new HashSet<TblSchoolFee>();
            this.TblSchoolMonthlyPayments = new HashSet<TblSchoolMonthlyPayment>();
        }
    
        public int PackageID { get; set; }
        public string PackageName { get; set; }
        public Nullable<double> Price { get; set; }
        public Nullable<bool> HrModule { get; set; }
        public Nullable<int> NoofUserAllow { get; set; }
        public Nullable<bool> StudentAccountAllow { get; set; }
        public Nullable<int> NoofStudentAllow { get; set; }
        public Nullable<bool> PayrollModule { get; set; }
        public Nullable<bool> AccountModule { get; set; }
        public Nullable<bool> ExamModule { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSchoolBranchesFee> TblSchoolBranchesFees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSchoolFee> TblSchoolFees { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TblSchoolMonthlyPayment> TblSchoolMonthlyPayments { get; set; }
    }
}
