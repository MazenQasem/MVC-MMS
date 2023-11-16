        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Doctor
    {
        public string Arabiccode { get; set; }
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmpCode { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> Sex { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<short> Age { get; set; }
        public string CellNo { get; set; }
        public string EMail { get; set; }
        public string Qualification { get; set; }
        public string PlaceOfContact { get; set; }
        public string ContactTime { get; set; }
        public int Deleted { get; set; }
        public Nullable<int> Title { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Password { get; set; }
        public Nullable<byte> EmployeeType { get; set; }
        public string Name { get; set; }
        public Nullable<int> RegNo { get; set; }
        public string IACode { get; set; }
        public string ArabicName { get; set; }
        public Nullable<int> OPMarkUpPercent { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public string SystemName { get; set; }
        public Nullable<bool> LoggedYN { get; set; }
        public string LoggedIPAddress { get; set; }
        public string Locked_YN { get; set; }
        public Nullable<System.DateTime> PW_SET_DATE { get; set; }
        public Nullable<System.DateTime> PWD_SET_DATE { get; set; }
        public string PWD_EXPIRED_YN { get; set; }
        public Nullable<System.DateTime> USER_START_TIME { get; set; }
        public Nullable<System.DateTime> USER_END_TIME { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public byte VISITINGPROF { get; set; }
        public Nullable<bool> IsUploaded { get; set; }
    }
}
