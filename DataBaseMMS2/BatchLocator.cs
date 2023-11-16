        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class BatchLocator
    {
        public Nullable<int> ItemID { get; set; }
        public int BatchID { get; set; }
        public string BatchNo { get; set; }
        public int StationID { get; set; }
        public int Quantity { get; set; }
        public int RackID { get; set; }
        public int ShelfID { get; set; }
        public int OperatorID { get; set; }
        public System.DateTime StartDateTime { get; set; }
    }
}
