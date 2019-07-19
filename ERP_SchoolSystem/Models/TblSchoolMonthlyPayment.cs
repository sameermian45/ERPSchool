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
    
    public partial class TblSchoolMonthlyPayment
    {
        public int ID { get; set; }
        public Nullable<int> SchoolID { get; set; }
        public Nullable<int> PackageID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<int> BranchPercentage { get; set; }
        public Nullable<double> BranchAmount { get; set; }
        public Nullable<double> PaymentReceived { get; set; }
        public Nullable<int> PaymentMonth { get; set; }
        public Nullable<int> PaymentYear { get; set; }
        public string PaymentMode { get; set; }
        public Nullable<System.DateTime> AddedOn { get; set; }
        public Nullable<int> AddedBy { get; set; }
        public string OrderNo { get; set; }
        public string Token { get; set; }
        public Nullable<System.DateTime> TokenExpiry { get; set; }
        public string Status { get; set; }
        public string BatchNumber { get; set; }
        public string AuthorizeId { get; set; }
        public string TransactionNumber { get; set; }
        public string TransactionResponse { get; set; }
        public string Message { get; set; }
        public string TransactionRefNumber { get; set; }
        public Nullable<bool> IsShopPay { get; set; }
        public Nullable<bool> IsMobilePay { get; set; }
        public Nullable<bool> isPay { get; set; }
        public Nullable<System.DateTime> PaymentDate { get; set; }
    
        public virtual TblSchool TblSchool { get; set; }
        public virtual TblSchoolBranch TblSchoolBranch { get; set; }
        public virtual TblSchoolPackage TblSchoolPackage { get; set; }
    }
}
