        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class OPLOAService
    {
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public int GradeId { get; set; }
        public int ServiceId { get; set; }
        public Nullable<decimal> LoaAmount { get; set; }
        public Nullable<int> LoaDays { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
    }
}
