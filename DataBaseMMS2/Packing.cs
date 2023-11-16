        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Packing
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public string UOMCode { get; set; }
    }
}
