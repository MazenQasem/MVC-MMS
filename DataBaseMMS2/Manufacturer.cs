        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Manufacturer
    {
        public int id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Grade { get; set; }
        public string ContactPerson { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Nullable<int> POBox { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Page { get; set; }
        public string Cell { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string web_site { get; set; }
        public Nullable<byte> PaymentType { get; set; }
        public Nullable<int> PaymentDays { get; set; }
        public Nullable<short> NetDueDays { get; set; }
        public Nullable<short> DiscountDays { get; set; }
        public Nullable<byte> Percentage { get; set; }
        public Nullable<float> CreditLimit { get; set; }
        public Nullable<bool> VendorStatus { get; set; }
        public Nullable<bool> OverSeas { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<System.DateTime> StartdateTime { get; set; }
        public Nullable<System.DateTime> enddatetime { get; set; }
    }
}
