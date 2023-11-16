        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemStore
    {
        public int StationID { get; set; }
        public int ItemID { get; set; }
        public Nullable<int> UnitID { get; set; }
        public Nullable<int> MaxLevel { get; set; }
        public Nullable<int> MinLevel { get; set; }
        public Nullable<int> QOH { get; set; }
        public Nullable<int> ROL { get; set; }
        public Nullable<int> ROQ { get; set; }
        public Nullable<byte> ABC { get; set; }
        public Nullable<byte> FSN { get; set; }
        public Nullable<byte> ved { get; set; }
        public Nullable<int> Tax { get; set; }
        public Nullable<int> ConversionQty { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public byte[] TimeStamp { get; set; }
    }
}
