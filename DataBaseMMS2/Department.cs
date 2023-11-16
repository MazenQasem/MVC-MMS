        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Department
    {
        public int ID { get; set; }
        public string DeptCode { get; set; }
        public string Name { get; set; }
        public string AccountCode { get; set; }
        public string DeptClassID { get; set; }
        public string RecordID { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public bool Deleted { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public Nullable<int> OperatorID { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDateTime { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public string ArabicName { get; set; }
        public string ArabicCode { get; set; }
        public Nullable<int> OLDID { get; set; }
        public Nullable<byte> NonSGHDept { get; set; }
        public string Ora_Code { get; set; }
        public Nullable<bool> UPLOADED { get; set; }
        public Nullable<System.DateTime> UDATETIME { get; set; }
    }
}
