        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseReceiptDetail
    {
        public int ReceiptID { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int Packid { get; set; }
        public int FreeQuantity { get; set; }
        public int FreePackId { get; set; }
        public decimal MRP { get; set; }
        public string BatchNo { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<decimal> PurchaseRate { get; set; }
        public Nullable<decimal> epr { get; set; }
        public Nullable<float> discount { get; set; }
        public Nullable<int> slNo { get; set; }
        public bool Full_Part { get; set; }
        public string remarks { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<float> Excise { get; set; }
        public Nullable<float> CST { get; set; }
        public Nullable<float> LST { get; set; }
        public Nullable<float> Tax1 { get; set; }
        public Nullable<float> DisAmt { get; set; }
        public Nullable<float> TaxAmt { get; set; }
        public Nullable<int> BatchId { get; set; }
        public Nullable<int> pono { get; set; }
        public Nullable<decimal> unitepr { get; set; }
        public Nullable<float> adddiscount { get; set; }
        public string PartNo { get; set; }
        public Nullable<int> Manufacturerid { get; set; }
    }
}
