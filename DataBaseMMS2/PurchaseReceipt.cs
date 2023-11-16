        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseReceipt
    {
        public int ID { get; set; }
        public Nullable<int> POrderId { get; set; }
        public string ReceiptNo { get; set; }
        public System.DateTime DateTime { get; set; }
        public System.DateTime ReceiptDate { get; set; }
        public int OperatorId { get; set; }
        public Nullable<int> Supplierid { get; set; }
        public Nullable<decimal> OrderAmount { get; set; }
        public int stationid { get; set; }
        public int stationslno { get; set; }
        public Nullable<bool> posted { get; set; }
        public Nullable<float> Discount { get; set; }
        public Nullable<float> other_deductions { get; set; }
        public Nullable<float> Freight { get; set; }
        public Nullable<float> Excise { get; set; }
        public Nullable<float> CST { get; set; }
        public Nullable<float> LST { get; set; }
        public Nullable<float> Octroi { get; set; }
        public Nullable<int> RecBayId { get; set; }
        public string invNo { get; set; }
        public Nullable<byte> type { get; set; }
        public Nullable<int> gatepassno { get; set; }
        public Nullable<byte> PRType { get; set; }
        public Nullable<decimal> disamount { get; set; }
        public string reference { get; set; }
        public Nullable<int> categoryid { get; set; }
        public Nullable<bool> InvConfirmed { get; set; }
        public Nullable<System.DateTime> InvConfirmedDatetime { get; set; }
        public Nullable<int> InvConfirmedOperatorId { get; set; }
        public Nullable<System.DateTime> InvDate { get; set; }
        public Nullable<byte> LocatorStatus { get; set; }
    }
}
