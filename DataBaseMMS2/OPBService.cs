        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class OPBService
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MasterTable { get; set; }
        public string OrderTable { get; set; }
        public string DetaiTtable { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public bool Deleted { get; set; }
        public string ServiceCode { get; set; }
        public string PriceTable { get; set; }
        public Nullable<int> DisplayServiceId { get; set; }
        public string SupportTable { get; set; }
        public Nullable<int> MarkUp { get; set; }
        public string Ora_Code { get; set; }
    }
}
