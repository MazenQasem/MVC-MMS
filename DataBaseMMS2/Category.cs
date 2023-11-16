        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Category
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public short CategoryType { get; set; }
        public string PayAfter { get; set; }
        public bool InsuranceCard { get; set; }
        public bool IquamaID { get; set; }
        public bool RefLetter { get; set; }
        public string Address { get; set; }
        public string TelephoneNo { get; set; }
        public string FaxNo { get; set; }
        public string EmailID { get; set; }
        public string PoBox { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public bool Active { get; set; }
        public int OperatorID { get; set; }
        public Nullable<System.DateTime> DateTime { get; set; }
        public bool Deleted { get; set; }
        public string arabicname { get; set; }
        public string arabiccode { get; set; }
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
        public Nullable<int> OPPD { get; set; }
        public Nullable<int> IPPD { get; set; }
        public Nullable<byte> Coverage { get; set; }
        public string CONTACTPERSONNAME { get; set; }
        public string CONTACTPERSONDESIG { get; set; }
        public string TelNo2 { get; set; }
        public string FaxNo2 { get; set; }
        public string ProviderCode { get; set; }
        public decimal FixedConCharges { get; set; }
        public decimal RegCharges { get; set; }
        public decimal InvoiceConFee { get; set; }
        public string BlockReason { get; set; }
        public bool PharmacyCSTHeader { get; set; }
        public Nullable<short> RegfeePaidby { get; set; }
        public string PolicyRules { get; set; }
        public Nullable<int> ApprovalDays { get; set; }
        public bool ByPassExclusionsStatus { get; set; }
        public Nullable<int> TariffID { get; set; }
        public Nullable<int> BillingCollectorId { get; set; }
        public Nullable<int> BillingOfficerId { get; set; }
        public Nullable<bool> LOAConsultation { get; set; }
        public Nullable<bool> UCF { get; set; }
        public string CatGroup { get; set; }
        public string Insert_Update { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public string Ora_Code { get; set; }
        public Nullable<byte> RevisitDays { get; set; }
        public Nullable<int> speccons { get; set; }
        public string Attribute4 { get; set; }
    }
}
