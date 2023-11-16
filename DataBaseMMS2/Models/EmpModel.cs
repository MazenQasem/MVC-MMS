using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
 
namespace MMS2
{
    public class User
    {
        [Required]
                 [Display(Name = "EmployeeID")]
        public int EmployeeID { get; set; }          public int EmpID { get; set; }           public string LoginDateTime { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
        public int ModuleID { get; set; }   
        public String Name { get; set; }

        [Required]
        public int selectedStationID { get; set; } 
        

        [Display(Name = "StationLists")]
        public List<Station> StationLists { get; set; }

        public List<MenuTable> MenuTables { get; set; }


                              public string gHospitalName { get; set; }
            public string gHospitalAddress1 { get; set; }
            public string gHospitalAddress2 { get; set; }
            public string gHospitalCity { get; set; }
            public string gHospitalPINCode { get; set; }
            public string gHospitalPhoneNo { get; set; }
            public string gHospitalFaxNo { get; set; }
            public string gHospitaleMail { get; set; }
            public string gIACode { get; set; }
            public string gHospitalBranchCode {get;set;}
            public string gHospitalPrefix { get; set; }
            public string CurrentIPAddress { get; set; }
         

                     public string AppName { get; set; }

            public string StationName { get; set; }
            public string ErrMsg { get; set; }

                     public List<ReportParentTable> ReportList { get; set; }
            public List<ReportChildTable> ReportChildList { get; set; }

                     public List<Station> SwapStationList { get; set; }

                     public bool LiveServer { get; set; }

                         public bool ScannerOn { get; set; }
       
    }
    public class Station
    {
        public int StationID { get; set; }
        public String StationName { get; set; }


    }
   
    public class MenuTable
    {
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuTitle { get; set; }
    }

    public class ReportParentTable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int count { get; set; }
        
    }

    public class ReportChildTable
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int ParentID { get; set; }
    }


    public class PosNumberNoZeroAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            int getal;
            if (int.TryParse(value.ToString(), out getal))
            {

                if (getal == 0)
                    return false;

                if (getal > 0)
                    return true;
            }
            return false;

        }
    }

}