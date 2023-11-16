        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Grade
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string GradeName { get; set; }
        public string ArabicName { get; set; }
        public string PolicyNo { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public int CompanyID { get; set; }
        public Nullable<decimal> RoomCharges { get; set; }
        public Nullable<decimal> FixedConCharges { get; set; }
        public Nullable<decimal> InvoiceConFee { get; set; }
        public Nullable<decimal> IPCreditLimit { get; set; }
        public Nullable<int> BedTypeID { get; set; }
        public Nullable<bool> Blocked { get; set; }
        public bool Deleted { get; set; }
        public int OperatorID { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public Nullable<int> TariffID { get; set; }
        public Nullable<int> OPConsultations { get; set; }
    }
}
