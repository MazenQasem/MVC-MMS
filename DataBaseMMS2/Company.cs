        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Company
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }
        public Nullable<int> TariffID { get; set; }
        public string PolicyNo { get; set; }
        public short Coverage { get; set; }
        public int OPPD { get; set; }
        public int IPPD { get; set; }
        public string Address { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string EMailID { get; set; }
        public string PoBox { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public int OperatorID { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public bool Active { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public bool Deleted { get; set; }
        public string ArabicName { get; set; }
        public string ArabicCode { get; set; }
        public Nullable<int> TPAId { get; set; }
        public Nullable<bool> UCF { get; set; }
        public Nullable<int> BillingCollectorId { get; set; }
        public Nullable<int> BillingOfficerId { get; set; }
        public Nullable<System.DateTime> ValidFrom { get; set; }
        public Nullable<System.DateTime> ValidTill { get; set; }
        public Nullable<bool> ArabicInvoice { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> AccountType { get; set; }
        public Nullable<int> ReferralBasis { get; set; }
        public Nullable<int> OPConsultations { get; set; }
        public Nullable<int> WFCLS { get; set; }
        public Nullable<int> WFRS { get; set; }
        public Nullable<int> WFRC { get; set; }
        public Nullable<int> PrintAddress { get; set; }
        public byte FollowRules { get; set; }
        public Nullable<bool> LOAConsultation { get; set; }
        public Nullable<short> EmpInfo { get; set; }
        public string CONTACTPERSONNAME { get; set; }
        public string CONTACTPERSONDESIG { get; set; }
        public string TelNo2 { get; set; }
        public string FaxNo2 { get; set; }
        public string ProviderCode { get; set; }
        public Nullable<bool> RelationDetails { get; set; }
        public decimal FixedConCharges { get; set; }
        public decimal RegCharges { get; set; }
        public decimal InvoiceConFee { get; set; }
        public string BlockReason { get; set; }
        public bool PharmacyCSTHeader { get; set; }
        public Nullable<bool> Aramco { get; set; }
        public Nullable<int> NoofVisits { get; set; }
        public Nullable<short> RegFeePaidBy { get; set; }
        public string PolicyRules { get; set; }
        public bool CONSULTATIONLIMIT { get; set; }
        public Nullable<int> BlockReasonId { get; set; }
        public Nullable<int> SubCategoryId { get; set; }
        public Nullable<int> ApprovalDays { get; set; }
        public Nullable<short> DeductableStatus { get; set; }
        public Nullable<byte> DiscountToPrint { get; set; }
        public Nullable<bool> ByPassExclusionsStatus { get; set; }
        public Nullable<decimal> PerAdvancetoPay { get; set; }
        public Nullable<decimal> DisForAdvancePay { get; set; }
        public string HostName { get; set; }
        public Nullable<bool> CheckMedId { get; set; }
        public Nullable<int> TariffLevel { get; set; }
        public string Insert_Update { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public Nullable<byte> RevisitDays { get; set; }
        public Nullable<int> speccons { get; set; }
        public Nullable<bool> UPLOADED { get; set; }
        public string Attribute4 { get; set; }
        public string Staff_Attribute4 { get; set; }
    }
}
