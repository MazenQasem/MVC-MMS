        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bank
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string CITY { get; set; }
        public string PIN { get; set; }
        public Nullable<short> DELETED { get; set; }
        public string State { get; set; }
        public string branch { get; set; }
        public Nullable<int> operatorid { get; set; }
        public string adress { get; set; }
        public string BankCode { get; set; }
        public string HospitalAccNo { get; set; }
        public Nullable<int> DivisionId { get; set; }
        public string site { get; set; }
        public string credit_card_type { get; set; }
    }
}
