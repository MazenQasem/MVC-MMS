        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class ItemGroup
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<int> Parent { get; set; }
        public Nullable<int> MaxID { get; set; }
        public Nullable<int> Fixed { get; set; }
        public Nullable<int> Medical { get; set; }
        public Nullable<bool> MaintDept { get; set; }
        public Nullable<byte> Equipment { get; set; }
        public Nullable<byte> OverSea { get; set; }
        public Nullable<bool> Deleted { get; set; }
    }
}
