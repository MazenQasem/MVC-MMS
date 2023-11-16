using System;
using System.Collections.Generic;
using System.Data;

namespace MMS2
{
    public static class EmployeeEntry
    {
        public static User GetEmpID(User u)
        {
            string pass = "";

            String sqlStr = " SELECT id,name,password from employee where employeeid='" + u.EmployeeID + "'";
            DataSet ds = MainFunction.SDataSet(sqlStr, "EmpID");
            if (ds.Tables[0].Rows.Count == 0)
            {
                u.ErrMsg = "Employeed is not correct !";
                return u;
            }

            foreach (DataRow row in ds.Tables["EmpID"].Rows)
            {
                u.EmpID = int.Parse(row["id"].ToString());
                u.Name = row["name"].ToString();
                pass = row["password"].ToString();
                u.ModuleID = 103;
            }

                         string ServerIPAddress = MainFunction.GetName("SELECT dec.local_net_address as IP FROM sys.dm_exec_connections AS dec " +
                                    " WHERE dec.session_id = @@SPID; ", "IP");

                         u.LiveServer = false;
            if (ServerIPAddress != "130.1.2.90")             {
                u.LiveServer = true;
                
                                                                                                 }

            
                                                                                                                                                                                                                                          
                          
            u.gHospitalName = "Hospital Name";
            u.gHospitalAddress1 = "Address1";
            u.gHospitalAddress2 = "Address2";
            u.gHospitalCity = "City";
            u.gHospitalPhoneNo = "00-0000-00";
            u.gHospitalFaxNo = "00-000-000";
            u.gHospitaleMail = "mail@hospital.com";
            u.gIACode = "SA01";
            u.gHospitalPINCode = "SA01";
            u.gHospitalBranchCode = "BranchCode";
            u.gHospitalPrefix = "HOS";
            u.CurrentIPAddress = MainFunction.GetIPAddress();

            MainFunction.CheckMyTables();



            return u;
        }




        public static User GetMenuList(User u)
        {

            

                                                                                           
                                                                                                        
             
             
            String sqlStr = " SELECT DISTINCT LTRIM(UPPER(FEATURE_MENU_ITEM_NAME)) AS MenuName,MF.ID as MenuID,MF.name as MenuTitle " +
                             " FROM  M_FEATURES MF  order by  MenuTitle";

            u.MenuTables = new List<MenuTable>();
            DataSet ds = MainFunction.SDataSet(sqlStr, "MenuList");
            foreach (DataRow row in ds.Tables["MenuList"].Rows)
            {
                MenuTable Menus = new MenuTable();
                Menus.MenuName = row["MenuName"].ToString();
                Menus.MenuID = int.Parse(row["MenuID"].ToString());
                Menus.MenuTitle = row["MenuTitle"].ToString();

                u.MenuTables.Add(Menus);

            }




            return u;
        }

        public static User GetUserStations()
        {
            User u = new User();
                                                    
            String sqlStr = "select a.id as id,a.name as name " +
                            " from Station a " +
                            " where a.deleted =0  " +
                            " group by a.id,a.name order by a.name";

            DataSet ds = MainFunction.SDataSet(sqlStr, "StationList");
            u.StationLists = new List<Station>();
            foreach (DataRow row in ds.Tables["StationList"].Rows)
            {
                Station s = new Station();
                s.StationID = Int32.Parse(row["id"].ToString());
                s.StationName = row["name"].ToString();
                u.StationLists.Add(s);
            }

            return u;
        }

        public static User GetReportList(User u)
        {
            string ListOfReportID = " (1223,1164,1097,1168,1236,1176,1103,1104,1114,1189,1087,1234,1241,1101,1102,1233,1240 ) ";

            String sqlStr = "select ID,Name,ParentID ,"
                                + "(select count(*) "
                                + " from MMSRPTMAP A,MMSREPORTS B "
                                + " where b.ParentID =Mst.ID  and A.REPORTID = B.ID AND B.PARENTID<>0 and a.Stationid=" + u.selectedStationID
                                + " and b.id in " + ListOfReportID + " ) as count "
                          + " from MMSReports Mst where ParentID=0 ";
            DataSet HR = MainFunction.SDataSet(sqlStr, "tbl");
            List<ReportParentTable> Parent = new List<ReportParentTable>();
            foreach (DataRow rr in HR.Tables[0].Rows)
            {
                ReportParentTable l = new ReportParentTable();
                l.ID = (int)rr["ID"];
                l.Name = rr["Name"].ToString();
                l.count = (int)rr["count"];
                Parent.Add(l);
            }
            u.ReportList = Parent;


            sqlStr = "select b.ID,b.Name as name,b.ParentID  "
                    + " from MMSRPTMAP A,MMSREPORTS B "
                    + " where b.ParentID>0 and A.REPORTID = B.ID AND B.PARENTID<>0 and a.Stationid=" + u.selectedStationID
                    + " and b.id in " + ListOfReportID;
            DataSet dt = MainFunction.SDataSet(sqlStr, "tbl");
            List<ReportChildTable> chd = new List<ReportChildTable>();
            foreach (DataRow rr in dt.Tables[0].Rows)
            {
                ReportChildTable nn = new ReportChildTable();
                nn.ID = (int)rr["ID"];
                nn.Name = rr["name"].ToString();
                nn.ParentID = (int)rr["ParentID"];
                chd.Add(nn);
            }
            u.ReportChildList = chd;





            return u;
        }

        public static List<Station> SwapStationList(User u)
        {
            
                                                                                                                                 string sqlStr = " select id,name from station order by name;";

            DataSet ds = MainFunction.SDataSet(sqlStr, "StationList");
            List<Station> st = new List<Station>();
            foreach (DataRow row in ds.Tables["StationList"].Rows)
            {
                Station s = new Station();
                s.StationID = Int32.Parse(row["id"].ToString());
                s.StationName = row["name"].ToString();
                st.Add(s);
            }
            return st;
        }



        public static User SwapAccess(User u)
        {
            string pass = "";

            String sqlStr = " SELECT id,name,password from employee where employeeid='" + u.EmployeeID + "'";
            DataSet ds = MainFunction.SDataSet(sqlStr, "EmpID");
            if (ds.Tables[0].Rows.Count == 0)
            {
                u.ErrMsg = "Employeed is not correct !";
                return u;
            }

            foreach (DataRow row in ds.Tables["EmpID"].Rows)
            {
                u.EmpID = int.Parse(row["id"].ToString());
                u.Name = row["name"].ToString();
                pass = row["password"].ToString();
                u.ModuleID = 103;
            }

                                                                              

                         string ServerIPAddress = MainFunction.GetName("SELECT dec.local_net_address as IP FROM sys.dm_exec_connections AS dec " +
                                    " WHERE dec.session_id = @@SPID; ", "IP");

                         u.LiveServer = false;
            if (ServerIPAddress != "130.1.2.90")
            {
                u.LiveServer = true;
            }




            sqlStr = " SELECT NAME, ADDRESS1, ADDRESS2, CITY, PHONENO, FAXNO, EMAIL, ISSUEAUTHORITYCODE, PINCODE" +
                    " , ORA_BRANCHID, addinformation " +
                    " FROM ORGANISATIONDETAILS";
            DataSet dH = MainFunction.SDataSet(sqlStr, "HospitalInfo");
            foreach (DataRow row in dH.Tables["HospitalInfo"].Rows)
            {
                u.gHospitalName = row["Name"].ToString();
                u.gHospitalAddress1 = row["ADDRESS1"].ToString();
                u.gHospitalAddress2 = row["ADDRESS2"].ToString();
                u.gHospitalCity = row["CITY"].ToString();
                u.gHospitalPhoneNo = row["PHONENO"].ToString();
                u.gHospitalFaxNo = row["FAXNO"].ToString();
                u.gHospitaleMail = row["EMAIL"].ToString();
                u.gIACode = row["ISSUEAUTHORITYCODE"].ToString();
                u.gHospitalPINCode = row["PINCODE"].ToString();

                u.gHospitalBranchCode = row["ORA_BRANCHID"].ToString();
                u.gHospitalPrefix = row["addinformation"].ToString();
                u.CurrentIPAddress = MainFunction.GetIPAddress();



                MainFunction.CheckMyTables();
            }




            return u;
        }

        public static String DecryptNew(String StringToDecrypt)
        {

        

        Dimensions:
            Double dblCountLength;
            Int16 intLengthChar;
            String strCurrentChar;
            Double dblCurrentChar;
            Int16 intCountChar;
            Int16 intRandomSeed;
            Int16 intBeforeMulti;
            Int16 intAfterMulti;
            Int16 intSubNinetyNine;
            Int16 intInverseAsc;
            String HoldStr = "";
        MainCode:
            for (dblCountLength = 0; dblCountLength < StringToDecrypt.Length; dblCountLength++)
            {
                String Temp = StringToDecrypt.Substring((int)(dblCountLength), 1);
                intLengthChar = Convert.ToInt16(Temp);
                strCurrentChar = StringToDecrypt.Substring((int)(dblCountLength + 1), intLengthChar);

                dblCurrentChar = 0;
                for (intCountChar = 0; intCountChar < strCurrentChar.Length; intCountChar++)
                {
                    char cc = Convert.ToChar(strCurrentChar.Substring(intCountChar, 1));
                    Int16 charval = (Int16)cc;
                    Int16 Formla1 = (Int16)(charval - 33);
                    Double Formla2 = (long)Math.Pow(93, (Int16)(strCurrentChar.Length - intCountChar - 1));
                    dblCurrentChar = dblCurrentChar + (long)(Formla1 * Formla2);
                }
                intRandomSeed = Convert.ToInt16(dblCurrentChar.ToString().Substring(2, 2));
                intBeforeMulti = Convert.ToInt16(dblCurrentChar.ToString().Substring(0, 2) + dblCurrentChar.ToString().Substring(4, 2));
                intAfterMulti = (Int16)(intBeforeMulti / intRandomSeed);
                intSubNinetyNine = (Int16)(intAfterMulti - 99);
                intInverseAsc = (Int16)(256 - intSubNinetyNine);
                HoldStr += Convert.ToString((char)(intInverseAsc));
                dblCountLength = (Int16)(dblCountLength + intLengthChar);
            }
            

            return HoldStr;
        }



        
    }
}