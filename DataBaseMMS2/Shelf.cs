        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Shelf
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public Nullable<short> Stationid { get; set; }
    }
}
