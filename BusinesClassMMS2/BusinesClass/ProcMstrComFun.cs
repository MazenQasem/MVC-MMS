using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class ProcMstrComFun
    {
        public static ProcMstrComMdl ClearRecord()
        {
            ProcMstrComMdl MainTbl = new ProcMstrComMdl();

            return MainTbl;
        }

        public static ProcMstrComMdl LoadLists(int DeptID = 0)
        {
            ProcMstrComMdl MainTbl = new ProcMstrComMdl();
            string Condition = "";
            string StrSql = "";
            if (DeptID > 0)
            {
                Condition = " and b.id= " + DeptID + " ";
            }
            else
            {
                Condition = " ";
            }
            StrSql = "select distinct a.Id,a.Code,a.Name as ProcedureName, a.DepartmentID,b.Name as DeptName,0 as ProcType "
              + " from otherprocedures a, Department b where "
              + " a.departmentid = b.id And a.Deleted = 0 And b.Deleted = 0 " + Condition
              + " Union "
              + " select distinct a.Id,a.Code,a.Name as ProcedureName, a.DepartmentID,b.Name as DeptName,1 as ProcType "
              + " from bedsideprocedure a, Department b where "
              + " a.departmentId = b.id and a.deleted = 0 and b.deleted = 0 " + Condition
              + " Union "
              + " select distinct a.Id,a.Code,a.Name as ProcedureName, a.DepartmentID,b.Name as DeptName,2 as ProcType  "
              + " from CathProcedure a, Department b where "
              + " a.departmentId = b.id and a.deleted = 0 and b.deleted = 0 " + Condition
              + " Union "
              + " select distinct a.Id,a.Code,a.Name as ProcedureName, a.DepartmentID,b.Name as DeptName,3 as ProcType "
              + " From"
              + " Surgery a, Department b"
              + " Where"
              + " a.departmentId = b.id and a.deleted = 0 and b.deleted = 0  " + Condition
              + " order by b.Name,a.Code";

            DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
            switch (DeptID)
            {
                case 0:
                    {
                        string DeptName = "";
                        List<TempListMdl> deptlist = new List<TempListMdl>();
                        foreach (DataRow rr in ds.Tables[0].Rows)
                        {
                            if (DeptName != rr["DeptName"].ToString().ToUpper())
                            {
                                TempListMdl dtl = new TempListMdl();
                                dtl.ID = (int)rr["DepartmentID"];
                                dtl.Name = rr["DeptName"].ToString().ToUpper();
                                DeptName = rr["DeptName"].ToString().ToUpper();
                                deptlist.Add(dtl);
                            }
                        }
                        MainTbl.DeptList = deptlist;
                        break;
                    }
                default:
                    {
                        List<TempListMdl> ProcList = new List<TempListMdl>();
                        List<TempListMdl> ProcTYpe = new List<TempListMdl>();
                        foreach (DataRow rr in ds.Tables[0].Rows)
                        {
                            TempListMdl dtl = new TempListMdl();
                            dtl.ID = (int)rr["Id"];
                            dtl.Name = rr["Code"].ToString().ToUpper() + ' ' + rr["ProcedureName"].ToString().ToUpper();
                            ProcList.Add(dtl);

                            TempListMdl Tdtl = new TempListMdl();
                            Tdtl.ID = (int)rr["Id"];
                            Tdtl.Name = rr["ProcType"].ToString().ToUpper();
                            ProcTYpe.Add(Tdtl);
                        }
                        MainTbl.ProcList = ProcList;
                        MainTbl.ProcTypeList = ProcTYpe;
                        break;
                    }
            }
            return MainTbl;

        }

        public static ProcMstrComMdl GetProcDetailItems(int lProcId)
        {
            ProcMstrComMdl mm = new ProcMstrComMdl();
            mm.lOrderId = 0;             mm.NewRecord = false;             try
            {
                string StrSql = "Select a.OrderId,a.ItemName,a.ItemId,a.Quantity,c.Name as UnitName, "
                + " a.ItemType,a.UnitId,isnull(b.Deleted,0) as Deleted "
                + " from SD_ProcedureProfile a, SM_ProcedureProfile b, Packing C "
                + " Where a.OrderId = b.Id and b.ProcedureId = " + lProcId
                + " and a.UnitId = c.ID ";
                List<ItemInsertedList> dtl = new List<ItemInsertedList>();
                int SNO = 1;
                DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ItemInsertedList item = new ItemInsertedList();
                    item.SNO = SNO;
                    item.ItemName = rr["ItemName"].ToString();
                    item.Units = rr["UnitName"].ToString();
                    item.Quantity = (int)rr["Quantity"];
                    item.ItemType = (int)(Int16)rr["ItemType"];
                    item.UnitID = (int)rr["UnitId"];
                    item.ItemID = (int)rr["ItemId"];
                    mm.lOrderId = (int)rr["OrderId"];
                    mm.Discountinue = (Boolean)rr["Deleted"];
                    dtl.Add(item);
                    SNO += 1;
                }
                mm.ItemList = dtl;
                mm.ErrMsg = "";
                if (mm.ItemList.Count > 0)
                {
                    mm.NewRecord = false;
                }
                else
                {
                    mm.lOrderId = 0;
                    mm.NewRecord = true;
                }

                return mm;
            }
            catch (Exception e) { mm.ErrMsg = "Can't Load Procedure Details!"; return mm; }
        }

        public static List<ItemInsertedList> InsertItem(int ItemID, List<ItemInsertedList> ItemList)
        {
            List<ItemInsertedList> mm = new List<ItemInsertedList>();
            try
            {
                string SQL = " SELECT i.id as ItemID,convert(char(150),i.Name) as Item,i.itemcode,i.DrugType,"
                + " isnull(P.ID,0) AS UnitID,isnull(p.Name,'') as UOM "
                + " FROM  item i inner join "
                + " (SELECT B.ITEMID,max(B.SLNO) AS SLNO FROM ItemPacking B GROUP BY ITEMID ) AS LowUOM  on lowuom.ItemID=i.ID "
                + " inner JOIN ItemPacking IP ON (LOWUOM.ItemID=IP.ItemID AND LowUOM.SLNO=IP.Slno) "
                + " inner JOIN Packing P ON IP.PackID=P.ID  "
                + " where i.Deleted=0 and i.id=" + ItemID
                + " group by i.id,i.Name,i.itemcode,i.DrugType,P.ID,p.Name order by 2 ";
                if (ItemList != null)
                {
                    if (ItemList.Count > 0)
                    {
                        ItemList[0].ErrMsg = "";
                        mm = ItemList;
                    }
                }
                DataSet ds = MainFunction.SDataSet(SQL, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ItemInsertedList item = new ItemInsertedList();
                    item.SNO = 0;
                    item.ErrMsg = "";
                    item.ItemName = rr["itemcode"].ToString() + "  " + rr["Item"].ToString();
                    item.Units = rr["UOM"].ToString();
                    item.Quantity = 0;
                    item.ItemType = 0;
                    item.UnitID = (int)rr["UnitID"];
                    item.ItemID = (int)rr["ItemID"];
                    mm.Add(item);
                }

                                 int SNO = 1;
                foreach (ItemInsertedList iitem in mm)
                {
                    iitem.SNO = SNO;
                    SNO += 1;
                }

                return mm;
            }
            catch (Exception e)
            {
                if (mm.Count > 0)
                {
                    mm[0].ErrMsg = "Can't Insert this Item!";
                }
                else
                {
                    ItemInsertedList xs = new ItemInsertedList();
                    xs.ErrMsg = "Can't Insert this Item!";
                    mm.Add(xs);
                }

                return mm;
            }


        }

        public static MessageModel Save(ProcMstrComMdl MM, User US)
        {
            MessageModel Result = new MessageModel();
            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    if (MM.NewRecord == true)
                    {
                        string StrSql = "Insert into SM_ProcedureProfile (ProcedureId,DepartmentId,Deleted,StartDateTime,OperatorId,ProcType) "
                        + " Values ( " + MM.ProcID + "," + MM.DeptID + " ,  0, Getdate(), " + US.EmpID + "," + MM.ProcType + ")";
                        int iRecAffec = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                        if (iRecAffec <= 0)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            Result.isSuccess = false;
                            Result.Message = "Can't Save this Changes!";
                            return Result;
                        }
                        int maxID = MainFunction.GetOneVal("Select Max(id) as MaxId from SM_ProcedureProfile", "MaxId", Trans);
                        if (maxID <= 0)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            Result.isSuccess = false;
                            Result.Message = "Can't Save this Changes!";
                            return Result;
                        }

                        foreach (ItemInsertedList item in MM.ItemList)
                        {
                            StrSql = "Insert into SD_ProcedureProfile (OrderId,ItemName,ItemId,Quantity,UnitId,ItemType) "
                            + " Values ( " + maxID + " , '" + item.ItemName + "', " + item.ItemID + " , " + item.Quantity
                            + ", " + item.UnitID + " , " + item.ItemType + ")";
                            int inseritem = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                            if (inseritem <= 0)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                Result.isSuccess = false;
                                Result.Message = "Can't Save this Changes!";
                                return Result;

                            }
                        }

                        Trans.Commit();
                        Result.isSuccess = true;
                        Result.Message = "Record(s) Saved Successfully";
                        return Result;
                    }
                    else
                    {
                        string StrSql = "";
                        if (MM.Discountinue == true)
                        {
                            StrSql = "Update SM_ProcedureProfile Set Deleted = 1 where Id = " + MM.lOrderId;
                        }
                        else
                        { StrSql = "Update SM_ProcedureProfile Set Deleted = 0 where Id = " + MM.lOrderId; }
                        bool Update = MainFunction.SSqlExcuite(StrSql, Trans);


                        StrSql = "Delete from SD_ProcedureProfile where OrderId = " + MM.lOrderId;
                        int iRecAffec = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                        if (iRecAffec <= 0)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            Result.isSuccess = false;
                            Result.Message = "Can't Save this Changes!";
                            return Result;
                        }

                        foreach (ItemInsertedList item in MM.ItemList)
                        {
                            StrSql = "Insert into SD_ProcedureProfile (OrderId,ItemName,ItemId,Quantity,UnitId,ItemType) "
                            + " Values ( " + MM.lOrderId + " , '" + item.ItemName + "', " + item.ItemID + " , " + item.Quantity
                            + ", " + item.UnitID + " , " + item.ItemType + ")";
                            int inseritem = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                            if (inseritem <= 0)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                Result.isSuccess = false;
                                Result.Message = "Can't Save this Changes!";
                                return Result;

                            }
                        }

                        Trans.Commit();
                        Result.isSuccess = true;
                        Result.Message = "Record(s) Modified Successfully";
                        return Result;
                    }

                }
            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                
                Result.isSuccess = false;
                Result.Message = "Can't Save this Changes!";
                return Result;
            }

        }
    }

}





