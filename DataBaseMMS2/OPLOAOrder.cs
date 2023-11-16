        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class OPLOAOrder
    {
        public string IssueAuthoritycode { get; set; }
        public Nullable<int> RegistrationNo { get; set; }
        public string Name { get; set; }
        public Nullable<decimal> LOAamount { get; set; }
        public Nullable<decimal> LOAbalance { get; set; }
        public Nullable<int> NoDays { get; set; }
        public string Letterno { get; set; }
        public bool Approval { get; set; }
        public int AuthorityId { get; set; }
        public string AuthorityBillNo { get; set; }
        public Nullable<System.DateTime> LoaDateTime { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> CompanyId { get; set; }
        public Nullable<int> GradeId { get; set; }
        public Nullable<int> DoctorId { get; set; }
        public string MedIDNumber { get; set; }
        public Nullable<System.DateTime> LOAExpiryDate { get; set; }
        public Nullable<bool> Checked { get; set; }
        public Nullable<int> TPAId { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<short> IsLOA { get; set; }
        public Nullable<decimal> PharmacyAmount { get; set; }
        public Nullable<short> LOAType { get; set; }
        public Nullable<int> ICDId { get; set; }
        public string ICDCode { get; set; }
        public string ICDDescription { get; set; }
        public string SGHAuthorityID { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> EndDatetime { get; set; }
        public Nullable<int> OperatorId { get; set; }
        public string PolicyNo { get; set; }
    }
}
