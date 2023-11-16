        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemAvgSale
    {
        public int Itemid { get; set; }
        public Nullable<int> Month1 { get; set; }
        public Nullable<int> Month2 { get; set; }
        public Nullable<int> Month3 { get; set; }
        public Nullable<short> Stationid { get; set; }
        public Nullable<float> Average { get; set; }
    }
}
