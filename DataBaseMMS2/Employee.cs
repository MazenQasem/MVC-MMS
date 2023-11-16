        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Employee
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string EmpCode { get; set; }
        public Nullable<int> Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> Sex { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<short> Age { get; set; }
        public string HAdd1 { get; set; }
        public string HCity { get; set; }
        public string HState { get; set; }
        public string HCountry { get; set; }
        public string HPINCode { get; set; }
        public string HPhoneNo { get; set; }
        public string WAdd1 { get; set; }
        public string WCity { get; set; }
        public string WState { get; set; }
        public string WCountry { get; set; }
        public string WPINCode { get; set; }
        public string WPhoneNo { get; set; }
        public string FaxNo { get; set; }
        public string PagerNo { get; set; }
        public string CellNo { get; set; }
        public string EMail { get; set; }
        public string ECity { get; set; }
        public string EAdd1 { get; set; }
        public string EState { get; set; }
        public string ECountry { get; set; }
        public string EPINcode { get; set; }
        public string EPhoneNo { get; set; }
        public string Qualification { get; set; }
        public string PlaceOfContact { get; set; }
        public string EContactPerson { get; set; }
        public string ContactTime { get; set; }
        public string Timings { get; set; }
        public string Remarks { get; set; }
        public Nullable<byte> EmployeeType { get; set; }
        public Nullable<byte> VisitingProf { get; set; }
        public Nullable<byte> IsPractisingDoctor { get; set; }
        public Nullable<int> DivisionID { get; set; }
        public Nullable<int> DepartmentID { get; set; }
        public Nullable<int> DesignationID { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public Nullable<int> SubCategoryID { get; set; }
        public Nullable<int> Medical { get; set; }
        public Nullable<byte> Supervisor { get; set; }
        public string Name { get; set; }
        public string ArabicName { get; set; }
        public Nullable<int> NationID { get; set; }
        public string IACode { get; set; }
        public Nullable<int> RegNo { get; set; }
        public Nullable<int> OPMarkUpPercent { get; set; }
        public string Password { get; set; }
        public string GLCode { get; set; }
        public Nullable<int> OperatorID { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public int Deleted { get; set; }
        public Nullable<bool> Indent { get; set; }
        public string SystemName { get; set; }
        public Nullable<bool> LoggedYN { get; set; }
        public string LoggedIPAddress { get; set; }
        public string Locked_YN { get; set; }
        public Nullable<System.DateTime> PW_SET_DATE { get; set; }
        public Nullable<System.DateTime> PWD_SET_DATE { get; set; }
        public string PWD_EXPIRED_YN { get; set; }
        public Nullable<System.DateTime> USER_START_TIME { get; set; }
        public Nullable<System.DateTime> USER_END_TIME { get; set; }
        public string Insert_Update { get; set; }
        public Nullable<bool> IsUploaded { get; set; }
        public string TempRegNo { get; set; }
        public string Arabiccode { get; set; }
        public string OldEmpCode { get; set; }
        public string ADUserName { get; set; }
        public string InsuranceNumber { get; set; }
        public Nullable<int> WorkHours { get; set; }
        public string BranchCode { get; set; }
        public Nullable<bool> IsHalfDayDuty { get; set; }
    }
}
