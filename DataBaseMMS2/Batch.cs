        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Batch
    {
        public int ItemID { get; set; }
        public int BatchID { get; set; }
        public string BatchNo { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
        public Nullable<int> Supplierid { get; set; }
        public Nullable<decimal> CostPrice { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public int Quantity { get; set; }
        public Nullable<bool> ItemType { get; set; }
        public Nullable<decimal> PurRate { get; set; }
        public Nullable<decimal> UnitEpr { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<int> MIDDLEWARE { get; set; }
    }
}
