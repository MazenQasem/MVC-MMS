using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace MMS2
{
    public class MainFunction
    {


        public static String SqlConn = "data source=localhost;initial catalog=HIS;persist security info=True;user id=sa;password=*****;MultipleActiveResultSets=True;App=EntityFramework";


        public static SqlConnection MainConn()
        {
            SqlConnection scon = new SqlConnection(SqlConn);
            return scon;
        }


        public static DataSet SDataSet(String SQL, String TableName, SqlTransaction Trans = null)
        {
            if (Trans != null)
            {
                SqlConnection Sconn = Trans.Connection;
                SqlCommand SCom = new SqlCommand(SQL, Sconn, Trans);
                SCom.CommandType = CommandType.Text;
                SCom.CommandTimeout = 400;
                SqlDataAdapter SAdpter = new SqlDataAdapter();
                DataSet Rset = new DataSet();
                SAdpter.SelectCommand = SCom;
                SAdpter.Fill(Rset, TableName);
                return Rset;
            }
            else
            {
                using (SqlConnection Sconn = new SqlConnection(SqlConn))
                {
                    Sconn.Open();
                    using (SqlCommand SCom = new SqlCommand("", Sconn))
                    {
                        SCom.CommandText = SQL;
                        SCom.CommandTimeout = 400;
                        SqlDataAdapter SAdpter = new SqlDataAdapter();
                        DataSet Rset = new DataSet();
                        SAdpter.SelectCommand = SCom;
                        SAdpter.Fill(Rset, TableName);
                        return Rset;
                    }

                }


            }


        }
        public static Boolean SSqlExcuite(String STRSQL, SqlTransaction Scon = null)
        {
            try
            {
                if (Scon != null)
                {
                    SqlConnection xcon = Scon.Connection;
                    SqlCommand xcom = new SqlCommand(STRSQL, xcon, Scon);
                    xcom.CommandType = CommandType.Text;
                    xcom.CommandTimeout = 400;
                    if (xcon.State == ConnectionState.Closed) { xcon.Open(); }
                    xcom.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    using (SqlConnection con = new SqlConnection(SqlConn))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand(STRSQL, con))
                        {
                            cmd.ExecuteNonQuery();
                            return true;
                        }
                    }


                }

            }
            catch (Exception _oErr) { throw _oErr; return false; }
        }

        public static int SSqlExcuiteRecordNumber(String STRSQL, SqlTransaction Scon)
        {
            try
            {
                SqlConnection xcon = Scon.Connection;
                SqlCommand xcom = new SqlCommand(STRSQL, xcon, Scon);
                xcom.CommandType = CommandType.Text;
                xcom.CommandTimeout = 400;
                if (xcon.State == ConnectionState.Closed) { xcon.Open(); }
                int RecrodsAffected = xcom.ExecuteNonQuery();
                return RecrodsAffected;
            }
            catch (Exception _oErr) { return 0; }
        }
        public static int GetInTransValue(String FieldName, String TableName, String Condition, SqlTransaction Trans)
        {
            int xVal = 0;
            String sqlStr = "select " + FieldName + " as id from " + TableName + " " + Condition;
            DataSet ds = MainFunction.SDataSet(sqlStr, "CheckTbl", Trans);
            if (ds.Tables["CheckTbl"].Rows.Count > 0)
            {
                foreach (DataRow Rr in ds.Tables[0].Rows)
                {
                    xVal = int.Parse(Rr["id"].ToString());
                    xVal += 1;
                }

            }
            else
            {
                xVal = 0;
            }
            return xVal;

        }
        public static bool UserAllowedFunction(User U, int gFeatureID, int gFuncationId)
        {
            try
            {
                string Sqlstr = "SELECT ROLE_ID FROM L_ROLEFUNCTIONS WHERE (STATION_ID = " + U.selectedStationID +
                              " AND MODULE_ID = " + U.ModuleID + "  AND FEATURE_ID = " + gFeatureID +
                              " AND FUNCTION_ID = " + gFuncationId + " AND ROLE_ID IN  " +
                              " (SELECT ROLE_ID FROM L_USERROLES WHERE USER_ID = " + U.EmpID + ")) ";

                DataSet UserRights = SDataSet(Sqlstr, "Users");
                if (UserRights.Tables[0].Rows.Count > 0) { return true; } else { return false; }


            }
            catch (Exception e)
            {
                String msg = e.Message.ToString();
                return false;
            }
        }
        public static bool UserAllowedMenu(User u, int gFeatureID)
        {
            String sqlStr = " SELECT DISTINCT LTRIM(UPPER(FEATURE_MENU_ITEM_NAME)) AS MenuName,MF.ID as MenuID,MF.name as MenuTitle " +
                             " FROM L_USERROLES UR, L_ROLEFunctions RF, M_FEATURES MF " +
                            " WHERE   UR.USER_ID      = " + u.EmpID +
                            " AND     UR.ROLE_ID      = RF.ROLE_ID " +
                            " AND     RF.STATION_ID   = " + u.selectedStationID +
                            " AND     RF.MODULE_ID    = " + u.ModuleID +
                            " AND     MF.ID           = " + gFeatureID +
                            " AND     MF.ID           = RF.FEATURE_ID order by MenuID ";

            DataSet ds = MainFunction.SDataSet(sqlStr, "MenuList");
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {

                return false;
            }





        }

        public static List<MMS_ItemMaster> CashedAllItems(int stationid, String SearchBy = "")
        {
            try
            {
                List<MMS_ItemMaster> ItemList = new List<MMS_ItemMaster>();
                String sqlStr = "select * from MMS_ItemMaster   where stationid=" + stationid + " and deleted=0   " +
SearchBy
+ "  order by name";

                DataSet ds1 = MainFunction.SDataSet(sqlStr, "ItemList");
                foreach (DataRow VR in ds1.Tables["ItemList"].Rows)
                {
                    MMS_ItemMaster uu = new MMS_ItemMaster();
                    uu.Id = int.Parse(VR["id"].ToString());
                    uu.ItemCode = VR["itemcode"].ToString();
                    uu.Name = VR["Name"].ToString();
                    uu.CategoryID = int.Parse(VR["CategoryID"].ToString());
                    uu.EUB = MainFunction.NullToBool(VR["EUB"].ToString());
                    uu.Schedule = MainFunction.NullToBool(VR["Schedule"].ToString());
                    uu.ROL = MainFunction.NullToInteger(VR["ROL"].ToString());
                    uu.QOH = 0; uu.ROQ = MainFunction.NullToInteger(VR["ROQ"].ToString());
                    uu.Tax = MainFunction.NullToInteger(VR["TAX"].ToString());
                    uu.UnitID = MainFunction.NullToInteger(VR["unitID"].ToString());
                    uu.ConversionQty = MainFunction.NullToInteger(VR["conversionqty"].ToString());
                    uu.Strength = VR["strength"].ToString();
                    uu.Strength_no = MainFunction.NullToDecmial(VR["Strength_no"].ToString());
                    uu.Strength_Unit = VR["Strength_Unit"].ToString();
                    uu.StartDateTime = DateTime.Parse(VR["startDateTime"].ToString());
                    uu.ManufacturerId = MainFunction.NullToInteger(VR["ManufacturerId"].ToString());
                    uu.MaxLevel = MainFunction.NullToInteger2(VR["MaxLevel"].ToString());
                    uu.MinLevel = MainFunction.NullToInteger2(VR["MinLevel"].ToString());
                    uu.ABC = MainFunction.NullToByte(VR["abc"].ToString());
                    uu.FSN = MainFunction.NullToByte(VR["FSN"].ToString());
                    uu.VED = MainFunction.NullToByte(VR["VED"].ToString());
                    uu.SellingPrice = MainFunction.NullToDecmial(VR["SellingPrice"].ToString());
                    uu.MRPItem = MainFunction.NullToBool(VR["MRPItem"].ToString());
                    uu.DrugType = MainFunction.NullToInteger(VR["DrugType"].ToString());

                    uu.CssdItem = MainFunction.NullToBool(VR["CssdItem"].ToString());
                    uu.BatchStatus = MainFunction.NullToBool(VR["batchstatus"].ToString());
                    uu.Narcotic = MainFunction.NullToBool(VR["Narcotic"].ToString());
                    uu.NonStocked = MainFunction.NullToBool(VR["nonstocked"].ToString());
                    uu.ItemPrefix = VR["ItemPrefix"].ToString();

                    uu.Consignment = MainFunction.NullToBool(VR["Consignment"].ToString());
                    uu.Approval = MainFunction.NullToBool(VR["Approval"].ToString());
                    uu.ProfitCenter = VR["ProfitCenter"].ToString();

                    uu.FixedAsset = MainFunction.NullToBool(VR["FixedAsset"].ToString());
                    uu.IssueType = MainFunction.NullToByte(VR["issuetype"].ToString());
                    uu.DrugState = MainFunction.NullToByte(VR["drugstate"].ToString());
                    uu.DuplicateLabel = MainFunction.NullToBool(VR["DuplicateLabel"].ToString());
                    uu.PartNumber = VR["partNumber"].ToString();
                    uu.CSSDApp = MainFunction.NullToInteger(VR["CSSDApp"].ToString());
                    uu.DRUGCONTROL = MainFunction.NullToBool(VR["DRUGCONTROL"].ToString());

                    uu.CriticalItem = MainFunction.NullToBool(VR["CriticalItem"].ToString());
                    uu.Feasibility = MainFunction.NullToBool(VR["Feasibility"].ToString());

                    uu.ModelNo = VR["ModelNo"].ToString();
                    if (string.IsNullOrEmpty(VR["iscocktail"].ToString()) == true && VR["iscocktail"].ToString() != "1")
                    {
                        uu.iscocktailbool = false;
                    }
                    else
                    {
                        uu.iscocktailbool = true;
                    }

                    if (uu.IssueType == 0)
                    {
                        uu.DepartmentIssueBool = false;
                        uu.IndentIssueBool = false;
                    }
                    else if (uu.IssueType == 1)
                    {
                        uu.DepartmentIssueBool = true;
                        uu.IndentIssueBool = false;
                    }
                    else if (uu.IssueType == 2)
                    {
                        uu.DepartmentIssueBool = false;
                        uu.IndentIssueBool = true;
                    }
                    else if (uu.IssueType == 3)
                    {
                        uu.DepartmentIssueBool = true;
                        uu.IndentIssueBool = true;
                    }

                    if (uu.EUB == true)
                    {
                        uu.EUBbool = true;
                    }
                    else
                    {
                        uu.EUBbool = false;
                    }

                    if (uu.Schedule == true)
                    {
                        uu.Schedulebool = true;
                    }
                    else
                    {
                        uu.Schedulebool = false;
                    }



                    uu.ProfitCentreID = MainFunction.NullToByte(VR["ProfitCentreID"].ToString());
                    uu.catalogueno = VR["catalogueno"].ToString();

                    ItemList.Add(uu);
                }

                return ItemList;

            }
            catch (Exception e)
            {
                String msg = e.Message.ToString();
                return null;
            }


        }
        public static bool NullToBool(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return false; }
                if (Val.ToUpper() == "FALSE") { return false; }
                if (Val == "0") { return false; }

                if (Val == "1") { return true; }
                return true;


            }
            catch (Exception e) { return false; }


        }
        public static byte NullToByte(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return byte.Parse(Val);


            }
            catch (Exception e) { return 0; }


        }
        public static Single NullToSingleOrReal(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return Single.Parse(Val);


            }
            catch (Exception e) { return 0; }


        }
        public static int NullToInteger(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return int.Parse(Val);


            }
            catch (Exception e) { return 0; }


        }
        public static String NullToString(string Val, bool Upper = false, bool Lower = false)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return ""; }
                if (Upper == true)
                {
                    return Val.Trim().ToUpper();
                }
                if (Lower == true)
                {
                    return Val.Trim().ToLower();
                }
                return Val.Trim();


            }
            catch (Exception e) { return "Null"; }


        }
        public static int BoolToInteger(bool Val)
        {
            try
            {
                if (Val == true)
                {
                    return 1;

                }
                else
                {
                    return 0;

                }


            }
            catch (Exception e) { return 0; }


        }
        public static int? NullToInteger2(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return int.Parse(Val);


            }
            catch (Exception e) { return 0; }


        }
        public static int? NullToOne(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return int.Parse(Val);


            }
            catch (Exception e) { return 1; }


        }
        public static float NullToFloat(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return float.Parse(Val);


            }
            catch (Exception e) { return 0; }

        }
        public static decimal NullToDecmial(string Val)
        {
            try
            {
                if (string.IsNullOrEmpty(Val) == true) { return 0; }

                return decimal.Parse(Val);


            }
            catch (Exception e) { return 0; }

        }
        public static Boolean CheckTranTable(string TranTbl, string TransTblDtl)
        {
            try
            {
                DataSet ds = SDataSet("Select * from sysobjects where type='u' and (name ='" + TranTbl.Trim() + "' OR " + " name ='" + TransTblDtl.Trim() + "')", "t");
                if (ds.Tables["t"].Rows.Count == 2) { return true; } else { return false; }
            }
            catch (Exception e)
            { return false; }
        }
        public static int GetTotalRecordCount(string Query, string fieldName)
        {
            int total = 0;
            DataSet sd = SDataSet(Query, "tb");
            foreach (DataRow r in sd.Tables[0].Rows)
            {

                total = int.Parse(r[fieldName].ToString());
            }

            return total;

        }
        public static String GetName(string Query, string fieldName, SqlTransaction TRANS = null)
        {
            try
            {
                String Name = "";
                DataSet sd = new DataSet();
                if (TRANS != null)
                {
                    sd = SDataSet(Query, "tb", TRANS);
                }
                else
                {
                    sd = SDataSet(Query, "tb");
                }
                foreach (DataRow r in sd.Tables[0].Rows)
                {

                    Name = r[fieldName].ToString();
                }

                return Name;
            }
            catch (Exception e) { return ""; }
        }
        public static int GetID(string Query, string fieldName)
        {
            int ID = 0;
            DataSet sd = SDataSet(Query, "tb");
            foreach (DataRow r in sd.Tables[0].Rows)
            {

                ID = (int)r[fieldName];
            }

            return ID;

        }
        public static int GetOneVal(string Query, string fieldName, SqlTransaction trans = null)
        {
            int ID = 0;
            if (trans != null)
            {
                DataSet sd = SDataSet(Query, "tb", trans);
                foreach (DataRow r in sd.Tables[0].Rows)
                {

                    ID = (int)r[fieldName];
                }

            }
            else
            {

                DataSet sd = SDataSet(Query, "tb");
                foreach (DataRow r in sd.Tables[0].Rows)
                {

                    ID = (int)r[fieldName];
                }
            }
            return ID;

        }
        public static byte GetOneValB(string Query, string fieldName)
        {
            byte ID = 0;
            DataSet sd = SDataSet(Query, "tb");
            foreach (DataRow r in sd.Tables[0].Rows)
            {

                ID = (byte)r[fieldName];
            }

            return ID;

        }
        public static List<TempListMdl> LoadComboList(String Str, int Stationid = 0)
        {
            List<TempListMdl> tnList = new List<TempListMdl>();
            TempListMdl tn = new TempListMdl();
            if (Stationid != 0) { Str += Stationid; }
            DataSet ds = MainFunction.SDataSet(Str, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new TempListMdl();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.Name = r["Name"].ToString();
                tnList.Add(tn);
            }
            return tnList;
        }
        public static List<TempListMdl> LoadComboList(String Str, int NoneElementVal, string NoneElementText)
        {
            List<TempListMdl> tnList = new List<TempListMdl>();

            TempListMdl noneVlaue = new TempListMdl();
            noneVlaue.ID = NoneElementVal;
            noneVlaue.Name = NoneElementText;
            tnList.Add(noneVlaue);


            TempListMdl tn = new TempListMdl();

            DataSet ds = MainFunction.SDataSet(Str, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new TempListMdl();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.Name = r["Name"].ToString();
                tnList.Add(tn);
            }



            return tnList;
        }
        public static List<TempListMdl> LoadOptionList(String Str, int Stationid = 0)
        {
            List<TempListMdl> tnList = new List<TempListMdl>();
            TempListMdl tn = new TempListMdl();
            if (Stationid != 0) { Str += Stationid; }
            DataSet ds = MainFunction.SDataSet(Str, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new TempListMdl();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.Name = r["Name"].ToString();
                tn.Selected = MainFunction.NullToBool(r["selected"].ToString());
                tnList.Add(tn);
            }
            return tnList;
        }

        public static List<Selec2List> LoadSelec2List(String Str)
        {
            List<Selec2List> tnList = new List<Selec2List>();
            Selec2List tn = new Selec2List();

            DataSet ds = MainFunction.SDataSet(Str, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new Selec2List();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.Name = r["Name"].ToString();
                tnList.Add(tn);
            }
            return tnList;
        }

        public static int GetQuantity(int? UnitID, int ItemId)
        {
            int conQTY = 1;
            int temp = 1;
            while (temp == 1)
            {


                DataSet ds = MainFunction.SDataSet("select Quantity,Packid from itempacking where itemid="
                    + ItemId + " and parent =" + UnitID, "t1");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        conQTY *= int.Parse(rr["Quantity"].ToString());
                        UnitID = int.Parse(rr["Packid"].ToString());

                    }
                }
                else
                {
                    break;
                }

            }
            return conQTY;

        }
        public static String getRegNumber(string strUHID)
        {
            try
            {
                String xstrUHID = strUHID.Trim();
                int NNN = 0;

                if (xstrUHID.IndexOf(".") > 0)
                {
                    int startIn = xstrUHID.IndexOf(".");
                    int endin = xstrUHID.Length - startIn - 1;

                    xstrUHID = xstrUHID.Substring(startIn + 1, endin);
                    NNN = int.Parse(xstrUHID);
                    xstrUHID = NNN.ToString();
                }
                else
                {
                    xstrUHID = "";

                };

                return xstrUHID;
            }
            catch (Exception e)
            {
                return "";
            }
        }
        public static String getAuthorityCode(string strUHID)
        {
            String xstrUHID = strUHID.Trim();
            if (xstrUHID.IndexOf(".") > 0)
            {
                xstrUHID = xstrUHID.Substring(0, xstrUHID.LastIndexOf("."));
            }
            else
            {
                xstrUHID = "";

            };

            return xstrUHID;
        }
        public static String getUHID(String gIAcode, String lngRegno, bool WithZeros)
        {
            String SgetUHID = "";
            if (WithZeros == true)
            {
                SgetUHID = gIAcode + "." + lngRegno.PadLeft(10, '0');
            }
            else
            {

                SgetUHID = gIAcode + "." + lngRegno;
            }
            return SgetUHID;
        }
        public static bool CheckEmployee(int ID, string IssueCode)
        {
            string StrSql = "select id from employee where IAcode='" + IssueCode
                + "' and regno= " + ID;
            DataSet empdst = MainFunction.SDataSet(StrSql, "tbl");
            if (empdst.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public static long SaveInTranOrder(SqlTransaction TRANS, int Gstationid, long ID, long stationslno, byte isstype, int DetailType,
            int TostationId = 0, string prefix = "", long slno = 0, long pinno = 0)
        {
            try
            {
                long mIdkey = 0;
                DataSet ds = SDataSet("select max(Idkey) Idkey from TransOrder_" + DateTime.Now.Year + "_" + DateTime.Now.Month, "tbl", TRANS);

                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    if (string.IsNullOrEmpty(rr["Idkey"].ToString()) == true)
                    {
                        mIdkey = 1;
                    }
                    else
                    {
                        mIdkey = (int)rr["Idkey"] + 1;

                    }
                }

                string SQL = "Insert into TransOrder_" + DateTime.Now.Year + "_" + DateTime.Now.Month +
                "(IdKey,Id,StationSlNo,DateTime,IssType,DetailType,StationId,ToStationId,Prefix,Slno,pinno) "
                + " Values(" + mIdkey + "," + ID + "," + stationslno
                + ",GETDATE()," + isstype + "," + DetailType + "," + Gstationid + ","
                + TostationId + ",'" + prefix + "'," + slno + "," + pinno + ")";

                bool InsertIntoTrans = SSqlExcuite(SQL, TRANS);
                return mIdkey;

            }
            catch (Exception en)
            {
                CheckTransTable();
                return 0;
            }
        }
        public static bool SaveInTranOrderDetail(SqlTransaction Trans, long IdKey, long Itemid, long Quantity, string BatchNo,
            string Batchid, decimal CP = 0, decimal MRP = 0, decimal Epr = 0)
        {
            try
            {
                string StrSql = "Insert into TransOrderDetail_" + DateTime.Now.Year + "_" + DateTime.Now.Month +
                    "(IdKey,ItemId,Quantity,Batchno,BatchId,CP,MRP,EPR) Values(" + IdKey + "," + Itemid + "," +
                    Quantity + ",'" + BatchNo + "'," + Batchid + "," + CP + "," + MRP + "," + Epr + ")";
                bool nn = SSqlExcuite(StrSql, Trans);
                return true;
            }
            catch (Exception e) { return false; }


        }

        public static bool CancelInTranOrderAndDetail(SqlTransaction TRANS, int Gstationid, long ID, DateTime OrderDate, long DetailType)
        {
            try
            {
                string StrSql = "Delete from transorderdetail_" + OrderDate.Year + "_" + OrderDate.Month
                    + " where idkey=(select idkey from transorder_" + OrderDate.Year + "_" + OrderDate.Month
                    + " where id=" + ID + " And DetailType = " + DetailType + " and stationid=" + Gstationid + ")";
                int del = MainFunction.SSqlExcuiteRecordNumber(StrSql, TRANS);
                if (del == 0) { return false; }
                StrSql = "Delete from transorder_" + OrderDate.Year + "_" + OrderDate.Month
                + " where id=" + ID + " And DetailType = " + DetailType + " and stationid=" + Gstationid;
                int del2 = MainFunction.SSqlExcuiteRecordNumber(StrSql, TRANS);
                if (del2 == 0) { return false; }
                return true;

            }
            catch (Exception e) { return false; }
        }
        public static string CheckTransTable()
        {
            try
            {
                string strTbName, strTbName1;
                strTbName = "TransOrder_" + DateTime.Now.Year + "_" + DateTime.Now.Month;
                strTbName1 = "TransOrderDetail_" + DateTime.Now.Year + "_" + DateTime.Now.Month;
                DataSet ds = SDataSet("Select * from sysobjects where type='u' and name ='" + strTbName + "'", "tbl");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    string StrSql = "Create table dbo." + strTbName + "(IdKey int unique not null,Id int not null" +
                    ",StationSlNo int not null,DateTime DateTime not null,IssType tinyint" +
                    ",DetailType smallint not null,StationId int not null,ToStationid int null" +
                    ",Prefix varchar(10) null,slno int null,PinNo int Null) ON MMSFile";
                    bool CreateTbl1 = SSqlExcuite(StrSql);
                    StrSql = "Create Clustered Index C_Dt on " + strTbName + "(DateTime) ON IndexFile";
                    bool CreateTbl1Clusterd = SSqlExcuite(StrSql);

                    string StrSql2 = "Create Table dbo." + strTbName1 + "(IDKey int not null,ItemId int not null" +
                   ",Quantity int not null,BatchNo varchar(20) null,BatchId int null,CP real null" +
                   ",MRP real null,EPR real null) ON MMSFile";
                    bool CreateTbl1Details = SSqlExcuite(StrSql2);

                    return "Try Again";
                }
                return "Error Creating TransTable!";

            }
            catch (Exception e) { return "Unable to create table in server;" + e.Message; }
        }
        public static String DateFormat(String Sdate,
         String F1 = "", String F2 = "", String F3 = "",
         String T1 = "", String T2 = "", String T3 = "",
         String DateSprator = "-", String TimeSprator = ":"
         )
        {
            String MM;
            String MMM;
            String MMMM;
            String dd;
            String yy;
            String yyyy;
            String hh;
            String mm;
            String ss;
            String BuildDate = "";
            DateTime Cdate = DateTime.Parse(Sdate);

            MM = Cdate.ToString("MM");
            MMM = Cdate.ToString("MMM");
            MMMM = Cdate.ToString("MMMM");
            dd = Cdate.ToString("dd");
            yy = Cdate.ToString("yy");
            yyyy = Cdate.ToString("yyyy");
            hh = Cdate.ToString("HH");
            mm = Cdate.ToString("mm");
            ss = Cdate.ToString("ss");
            if (string.IsNullOrEmpty(F1) == false)
            {
                switch (F1)
                {
                    case "MM": BuildDate += MM; break;
                    case "MMM": BuildDate += MMM; break;
                    case "MMMM": BuildDate += MMMM; break;
                    case "dd": BuildDate += dd; break;
                    case "yy": BuildDate += yy; break;
                    case "yyyy": BuildDate += yyyy; break;
                    default: break;
                }
                BuildDate += DateSprator;
                switch (F2)
                {
                    case "MM": BuildDate += MM; break;
                    case "MMM": BuildDate += MMM; break;
                    case "MMMM": BuildDate += MMMM; break;
                    case "dd": BuildDate += dd; break;
                    case "yy": BuildDate += yy; break;
                    case "yyyy": BuildDate += yyyy; break;
                    default: break;
                }
                BuildDate += DateSprator;
                switch (F3)
                {
                    case "MM": BuildDate += MM; break;
                    case "MMM": BuildDate += MMM; break;
                    case "MMMM": BuildDate += MMMM; break;
                    case "dd": BuildDate += dd; break;
                    case "yy": BuildDate += yy; break;
                    case "yyyy": BuildDate += yyyy; break;
                    default: break;
                }
            }


            if (String.IsNullOrEmpty(T1) == false)
            {
                BuildDate += " ";
                switch (T1)
                {
                    case "hh": BuildDate += hh; break;
                    case "mm": BuildDate += mm; break;
                    case "ss": BuildDate += ss; break;
                    default: break;
                }
                BuildDate += TimeSprator;
                switch (T2)
                {
                    case "hh": BuildDate += hh; break;
                    case "mm": BuildDate += mm; break;
                    case "ss": BuildDate += ss; break;
                    default: break;
                }
                if (string.IsNullOrEmpty(T3) == false) { BuildDate += TimeSprator; }
                switch (T3)
                {
                    case "hh": BuildDate += hh; break;
                    case "mm": BuildDate += mm; break;
                    case "ss": BuildDate += ss; break;
                    default: break;
                }
            }


            return BuildDate;



        }



        public static bool CheckMyTables()
        {
            string strsql = "";
            try
            {
                DataSet dV = MainFunction.SDataSet("select top 1 * from MMS_ItemMaster", "tbl");
                foreach (DataRow rr in dV.Tables[0].Rows)
                {

                }

            }
            catch (Exception e)
            {
                strsql = " CREATE view MMS_ItemMaster "
                + " as select distinct  "
                + " Id,Name,Name1,Strength,CategoryID,ManufacturerId,j.MaxLevel,j.MinLevel,j.ROL, "
                + " j.ROQ,bt.QOH,bt.totalcost,bt.expirydate,j.ABC,j.FSN,j.VED,SellingPrice,i.StartDateTime,i.Enddatetime,ItemCode,i.Deleted,EUB, "
                + " ProfitCenter,j.UnitID,j.Tax,Schedule,MRPItem,j.ConversionQty,i.DrugType,i.DeletedBY, "
                + " i.CssdItem,i.BatchStatus,i.Strength_no,Narcotic,NonStocked,ItemPrefix,Consignment, "
                + " Approval,ProfitCentreID,FixedAsset,IssueType,i.DrugState,i.Strength_Unit,i.DuplicateLabel, "
                + " i.PartNumber,i.CSSDApp,DRUGCONTROL,i.ModelNo,CriticalItem,Feasibility,iscocktail,catalogueno,j.StationID as stationid "
                + " ,I.ORA_CODE,CASE isnull(bt.stid,0) WHEN 0 THEN 0 ELSE 1 END AS BatchFlag "
                + " from item i  "
                + " LEFT join itemstore j on i.id=j.itemid  "
                + " left  join "
                + " (Select a.itemid,sum(b.quantity) as QOH,sum(b.quantity*a.costprice) as TotalCost,max(a.expirydate) as ExpiryDate, b.stationid as stid   "
                + " from batch a inner join batchstore b on a.batchid=b.batchid and a.batchno=b.batchno  and a.itemid=b.itemid  "
                + " group by a.itemid,b.stationid   "
                + " ) as bt on j.stationid=bt.stid and i.id=bt.itemid ";

                bool CreateViewMasterITem = MainFunction.SSqlExcuite(strsql);

            }



            try
            {
                DataSet ds = MainFunction.SDataSet("select top 1  * from MMS_CashIssueRptMaster", "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {

                }
            }
            catch (Exception e)
            {
                strsql = "CREATE TABLE [dbo].[MMS_CashIssueRptMaster]( " +
                        "	[BillID] [int] NULL, " +
                        "	[RegNo] [varchar](100) NULL, " +
                        "	[PTName] [varchar](100) NULL, " +
                        "	[Duplicate] [varchar](100) NULL, " +
                        "	[bno] [varchar](100) NULL, " +
                        "	[cashbillno] [varchar](100) NULL, " +
                        "	[creditbillno] [varchar](100) NULL, " +
                        "	[BillType] [varchar](100) NULL, " +
                        "	[Cancelled] [varchar](100) NULL, " +
                        "	[CompanyName] [varchar](100) NULL, " +
                        "	[Employeeid] [varchar](100) NULL, " +
                        "	[dtpIssueDT] [varchar](100) NULL, " +
                        "	[DoctorName] [varchar](100) NULL, " +
                        "	[DisDoctorName] [varchar](100) NULL, " +
                        "	[PaidTitle] [varchar](100) NULL, " +
                        "	[CashierName] [varchar](100) NULL, " +
                        "	[BillAmount] [numeric](18, 2) NULL, " +
                        "	[DisPer] [numeric](18, 0) NULL, " +
                        "	[Dis] [numeric](18, 0) NULL, " +
                        "	[NetAmt] [numeric](18, 2) NULL, " +
                        "	[deductper] [numeric](18, 0) NULL, " +
                        "	[deductableamt] [numeric](18, 2) NULL, " +
                        "	[balance] [numeric](18, 2) NULL, " +
                        "	[DonationAmt] [numeric](18, 2) NULL " +
                        ")";


                bool CreateMasterReport = MainFunction.SSqlExcuite(strsql);


            }

            try
            {
                DataSet dt = MainFunction.SDataSet("select top 1  * from MMS_CashIssueRptDetail", "tbl");
                foreach (DataRow rr in dt.Tables[0].Rows)
                {

                }
            }
            catch (Exception e)
            {
                strsql = "CREATE TABLE [dbo].[MMS_CashIssueRptDetail]( " +
                        "	[BillID] [int] NULL, " +
                        "	[ItemName] [varchar](100) NULL, " +
                        "	[ItemCode] [varchar](60) NULL, " +
                        "	[quantity] [int] NULL, " +
                        "	[ItemAmt] [numeric](18, 2) NULL " +
                        ") ";


                bool CreateDetailReport = MainFunction.SSqlExcuite(strsql);
            }


            try
            {
                DataSet dt = MainFunction.SDataSet("select COUNT(*) AS CN FROM SYS.OBJECTS WHERE NAME='MMS_SP_GetGeneric' ", "tbl");
                int CN = 0;
                foreach (DataRow rr in dt.Tables[0].Rows)
                {
                    CN = (int)rr["CN"];
                }

                if (CN == 0)
                {
                    strsql = "CREATE PROCEDURE MMS_SP_GetGeneric (@ItemID int,@StationID int,@WithQTY int)" +
                        "AS " +
                        " BEGIN " +
                        " declare @cc int =0; " +
                        " select @cc=count(*) from itemgeneric a,itemgeneric b where a.genericid = b.genericid and a.itemid = @ItemID; " +
                        " IF @cc=0  " +
                        " BEGIN " +
                        "   IF @WITHqty>0  " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID and a.QOH>0; " +
                        "       END " +
                        "   ELSE " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID --and a.QOH>0; " +
                        "       END  " +
                        "   END " +
                        "  ELSE " +
                        "   BEGIN " +
                        "   IF @WITHqty>0  " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a  " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID and a.QOH>0 " +
                        "       and a.id in (select distinct b.itemid from itemgeneric a,itemgeneric b  " +
                        "           Where a.genericid = b.genericid and a.itemid = @ItemID " +
                        "           group by b.itemid); " +
                        "       END " +
                        "   ELSE " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID --and a.QOH>0 " +
                        "       and a.id in (select distinct b.itemid from itemgeneric a,itemgeneric b  " +
                        "       Where a.genericid = b.genericid and a.itemid = @ItemID " +
                        "       group by b.itemid);" +
                        "   END " +
                        "   END " +
                        " END ";
                    bool CreateGenericProcedure = MainFunction.SSqlExcuite(strsql);

                }


            }
            catch (Exception e)
            {
                strsql = "CREATE PROCEDURE MMS_SP_GetGeneric (@ItemID int,@StationID int,@WithQTY int)" +
                        "AS " +
                        " BEGIN " +
                        " declare @cc int =0; " +
                        " select @cc=count(*) from itemgeneric a,itemgeneric b where a.genericid = b.genericid and a.itemid = @ItemID; " +
                        " IF @cc=0  " +
                        " BEGIN " +
                        "   IF @WITHqty>0  " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID and a.QOH>0; " +
                        "       END " +
                        "   ELSE " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID; " +
                        "       END  " +
                        "   END " +
                        "  ELSE " +
                        "   BEGIN " +
                        "   IF @WITHqty>0  " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a  " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID and a.QOH>0 " +
                        "       and a.id in (select distinct b.itemid from itemgeneric a,itemgeneric b  " +
                        "           Where a.genericid = b.genericid and a.itemid = @ItemID " +
                        "           group by b.itemid); " +
                        "       END " +
                        "   ELSE " +
                        "       BEGIN " +
                        "       select distinct a.id as ID,a.Name as Name from mms_itemmaster a " +
                        "       where a.drugtype=0 and a.deleted=0 and a.stationid=@StationID " +
                        "       and a.id in (select distinct b.itemid from itemgeneric a,itemgeneric b  " +
                        "       Where a.genericid = b.genericid and a.itemid = @ItemID " +
                        "       group by b.itemid);" +
                        "   END " +
                        "   END " +
                        " END ";


                bool CreateGenericProcedure = MainFunction.SSqlExcuite(strsql);
            }


            return true;
        }


        public static bool MazenAutoPrint(string Arg)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo startInfo;


                Process p = new Process();

                
                startInfo = new System.Diagnostics.ProcessStartInfo(@"link to my exe file to run the script", Arg);
                p.StartInfo = startInfo;

                p.Start();
                return true;
            }
            catch (Exception e) { return false; }
        }

        public static string Cast(string val, int objType)
        {
            int i = 0;
            double db = 0;
            decimal dc = 0;
            long lo = 0;
            byte bt = 0;
            switch (objType)
            {
                case 0:
                    if (int.TryParse(val, out i) == true)
                    { return int.Parse(val).ToString(); }; break;
                case 1:
                    if (double.TryParse(val, out db) == true)
                    { return double.Parse(val).ToString(); }; break;
                case 2:
                    if (decimal.TryParse(val, out dc) == true)
                    { return decimal.Parse(val).ToString(); }; break;
                case 3:
                    if (long.TryParse(val, out lo) == true)
                    { return long.Parse(val).ToString(); }; break;
                case 4:
                    if (byte.TryParse(val, out bt) == true)
                    { return byte.Parse(val).ToString(); }; break;
                default:
                    break;
            }

            return "0";

        }



        public static bool CheckQOH_Min(int ItemID, string ItemName, int ConvQty, int MinLvl, long QOH, int DispatchQty)
        {
            try
            {
                if (MinLvl > 0)
                {
                    if ((MinLvl / ConvQty / 4) >= (QOH - DispatchQty))
                    {
                        return true;
                    }
                }
                return false;

            }
            catch (Exception e) { return true; }


        }



        public static DataTable ExecuteSQLAndReturnDataTable(string sql)
        {
            using (SqlConnection CN = new SqlConnection(SqlConn))
            {
                try
                {
                    CN.Open();
                    DataTable dt = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(sql, CN))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = 400;
                        SqlDataReader rs = cmd.ExecuteReader();
                        dt.Load(rs);
                        rs.Close();
                        rs.Dispose();
                        return dt;
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static int GetQuantityDirectIpIssue(int? UnitID, int ItemId)
        {
            int conQTY = 1;


            DataSet ds = MainFunction.SDataSet("select Quantity,Packid from itempacking where itemid=" + ItemId + " and parent =" + UnitID, "t1");
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    conQTY *= int.Parse(rr["Quantity"].ToString());
                    UnitID = int.Parse(rr["Packid"].ToString());
                }
                conQTY = (conQTY == 0 ? 1 : conQTY);
            }


            return conQTY;

        }


        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        public static bool MazValidateBatchStoreQty(int ItemID, int BatchID, int StationID, SqlTransaction Trans)
        {
            try
            {
                string StrSql = "select Quantity from batchstore where itemid=" + ItemID + " and batchid=" + BatchID
                             + " and stationid=" + StationID;
                DataSet ds = MainFunction.SDataSet(StrSql, "tb", Trans);
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    if ((int)rr["Quantity"] < 0)
                    { return false; }

                }

                return true;
            }
            catch (Exception e) { return false; }


        }

        public static string ListToSelect2Col(string Str)
        {
            try
            {
                string strlist = "";
                DataSet ds = MainFunction.SDataSet(Str, "tb");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    strlist += "<option value='" + rr["ID"].ToString() + "'>" + rr["Name"].ToString() + "</option>";

                }
                return strlist;

            }
            catch (Exception e) { return ""; }


        }


    }

}
