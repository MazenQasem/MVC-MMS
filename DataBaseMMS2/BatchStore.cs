        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class BatchStore
    {
        public short StationID { get; set; }
        public int ItemID { get; set; }
        public Nullable<int> BatchID { get; set; }
        public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public Nullable<short> Tax { get; set; }
        public Nullable<bool> ItemType { get; set; }
        public Nullable<int> MIDDLEWARE { get; set; }
    }
}
