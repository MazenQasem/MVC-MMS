        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class OPLOAOrderModify
    {
        public Nullable<int> AutorityId { get; set; }
        public Nullable<int> Registrationno { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<int> NoDays { get; set; }
        public Nullable<System.DateTime> Datetime { get; set; }
        public Nullable<int> OperatorId { get; set; }
        public string ApprovalNo { get; set; }
        public string Notes { get; set; }
        public Nullable<decimal> PharmacyAmount { get; set; }
        public int ID { get; set; }
    }
}
