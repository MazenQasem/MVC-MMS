        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class AlterMRP
    {
        public int itemid { get; set; }
        public int batchid { get; set; }
        public Nullable<decimal> oldcp { get; set; }
        public Nullable<decimal> oldmrp { get; set; }
        public Nullable<decimal> newcp { get; set; }
        public Nullable<decimal> newmrp { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public int operatorid { get; set; }
        public Nullable<System.DateTime> OldExpDate { get; set; }
        public Nullable<System.DateTime> NewExpDate { get; set; }
    }
}
