using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class ProcIssComFun
    {
        public static ProcIssueMain ClearRecord()
        {
            ProcIssueMain MainTbl = new ProcIssueMain();
            string Sql = "select distinct a.DepartmentID AS id,b.Name  from otherprocedures a, Department b where  a.departmentid = b.id  "
                + " And a.Deleted = 0 And b.Deleted = 0  "
                + " Union  select distinct a.DepartmentID,b.Name as DeptName  from bedsideprocedure a, Department b where  a.departmentId = b.id  "
                + " and a.deleted = 0 and b.deleted = 0 "
                + " Union select distinct a.DepartmentID,b.Name as DeptName   from CathProcedure a,Department b where  a.departmentId = b.id and  "
                + " a.deleted = 0 and b.deleted = 0  "
                + " Union select distinct  a.DepartmentID,b.Name as DeptName  From Surgery a,Department b Where a.departmentId = b.id and a.deleted = 0  "
                + " and b.deleted = 0   order by b.name";
            DataSet DeptDS = MainFunction.SDataSet(Sql, "tr");
            List<TempListMdl> tbl = new List<TempListMdl>();
            TempListMdl n = new TempListMdl();
            n.ID = 0; n.Name = "All";
            tbl.Add(n);
            foreach (DataRow rr in DeptDS.Tables[0].Rows)
            {
                TempListMdl Dept = new TempListMdl();
                Dept.ID = (int)rr["id"];
                Dept.Name = rr["Name"].ToString();
                tbl.Add(Dept);
            }
            MainTbl.DeptList = tbl;

            TempListMdl nn = new TempListMdl();
            nn.ID = 0; nn.Name = "<span style='color: crimson;'>Select BillNO</span>";
            List<TempListMdl> tbln = new List<TempListMdl>();
            tbln.Add(nn);
            MainTbl.BillList = tbln;

            TempListMdl xnn = new TempListMdl();
            xnn.ID = 0; xnn.Name = "<span style='color: crimson;'>Select procedure</span>";
            List<TempListMdl> xtbln = new List<TempListMdl>();
            xtbln.Add(xnn);
            MainTbl.ProcList = xtbln;

            return MainTbl;
        }
        public static List<TempListMdl> LoadBillNoList(int DeptID, DateTime dtpFromDate)
        {
            List<TempListMdl> ll = new List<TempListMdl>();
            try
            {
                                 string Condition = "";
                if (DeptID > 0) { Condition = " And DepartmentID=" + DeptID; } else { Condition = " "; }

                string sql = "select distinct id,Billno,BillDateTime from OPCompanyBillDetail  "
                    + " where  convert(varchar(10),billdatetime,105)='" + MainFunction.DateFormat(dtpFromDate.ToString(), "dd", "MM", "yyyy")
                    + "'  And isnull(IsInvDone,0) = 0 And ServiceId = 7 "
                    + Condition + " order by Billno,billdatetime ";
                DataSet Ds = MainFunction.SDataSet(sql, "tbl");
                foreach (DataRow rr in Ds.Tables[0].Rows)
                {
                    TempListMdl NN = new TempListMdl();
                    NN.ID = (int)rr["id"];
                    NN.Name = rr["Billno"].ToString() + "    " + MainFunction.DateFormat(rr["BillDateTime"].ToString(), "dd", "MMM", "yyyy");
                    ll.Add(NN);
                }


                return ll;
            }
            catch (Exception e) { return ll; }


        }
        public static ProcIssueMain GetProcList(int BillNo, string BillTxt)
        {
            ProcIssueMain ll = new ProcIssueMain();
            try
            {
                string Condition = "";
                if (BillNo > 0) { Condition = " and a.id=" + BillNo + " "; } else { Condition = " and a.billno='" + BillTxt + "' "; }
                ll.intProcType = 4;

                string Sql = "select a.id as BillID,a.billno,isnull(a.IsInvDone,0) as IsInvDone,e.Name as ProcName,E.ID as PID, "
                    + " a.ID,A.BillDateTime,a.IssueAuthorityCode+'.'+CAST(a.RegistrationNo as varchar(10)) as Pin,"
                    + " b.Title+' '+b.FirstName+' '+B.MiddleName+' '+b.LastName as Patient,b.Age,c.Name,d.Name as Sex "
                    + " ,Quantity as NoOfSession "
                    + " From OPCompanyBillDetail a,Patient b,AgeType c,Sex d,OtherProcedures e "
                    + " Where a.RegistrationNo = b.RegistrationNo And b.Sex = d.ID And b.agetype = c.ID  "
                    + " AND A.SERVICEID=7 " + Condition + " and a.ItemID=e.ID";
                DataSet ds = MainFunction.SDataSet(Sql, "tbl");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    ll.ProcList = new List<TempListMdl>();
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        ll.lblPin = rr["Pin"].ToString();
                        ll.lblPatient = rr["Patient"].ToString();
                        ll.lblAge = rr["Age"].ToString() + " " + rr["Name"].ToString();
                        ll.lblSex = rr["Sex"].ToString();
                        ll.lblOrdDate = MainFunction.DateFormat(rr["BillDateTime"].ToString(), "dd", "MMM", "yyyy", "hh", "mm", "ss");
                        ll.lngBillId = (int)rr["BillID"];
                        ll.lblProcQty = rr["NoOfSession"].ToString();
                        ll.txtBillNo = rr["billno"].ToString();
                        ll.IsInvDone = (byte)rr["IsInvDone"];
                        ll.lblDateTime = MainFunction.DateFormat(DateTime.Now.ToString(), "dd", "MMM", "yyyy", "hh", "mm", "ss");


                        TempListMdl dtl = new TempListMdl();
                        dtl.ID = (int)rr["PID"];
                        dtl.Name = rr["ProcName"].ToString();
                        ll.ProcList.Add(dtl);
                    }
                }
                else
                {
                    ll.lblPin = "";
                    ll.lblPatient = "";
                    ll.lblAge = "";
                    ll.lblSex = "";
                    ll.lblOrdDate = "";
                    ll.lngBillId = 0;
                    ll.IsInvDone = 0;
                    ll.lblProcQty = "0";
                    ll.txtBillNo = "";

                }
                return ll;
            }
            catch (Exception e) { return ll; }


        }
        public static List<ProcIssueItemList> GetProcedureItems(int ProcID, int ProcQty, int Gstationid)
        {
            List<ProcIssueItemList> ll = new List<ProcIssueItemList>();
            try
            {


                string sql = "Select a.ItemType,b.ProcedureId,a.OrderId,i.itemcode,i.Name as Item,a.ItemId,d.QOH, "
                + " a.Quantity,c.Name as UnitName,a.ItemType,ic.MRP AS COSTPRICE,a.UnitId,b.Deleted,i.Name as OrdItem,i.id as SubstituteId,0 as CostAmount "
                + " from SD_ProcedureProfile a, SM_ProcedureProfile b, Packing C,Item I,AllItemsLastmrp IC, "
                + " (Select sum(quantity) as QOH,ItemID from BatchStore where stationid=" + Gstationid + " and quantity>0 group by ItemID) d "
                + " Where a.orderId = b.ID And b.ProcedureId = " + ProcID + " And a.UnitId = c.ID "
                + " and d.ItemID = i.id  and a.itemid= i.id and IC.ItemID=a.Itemid "
                + " Union "
                + " Select a.ItemType,b.ProcedureId,a.OrderId,i.itemcode,i.Name,a.ItemId,0 as QOH,a.Quantity,c.Name as UnitName,a.ItemType,0 as MRP, "
                + " a.UnitId,b.Deleted,i.Name as OrdItem,i.id as SubstituteId,0 as CostAmount  "
                + " from SD_ProcedureProfile a, SM_ProcedureProfile b, Packing C,Item I "
                + " Where a.orderId = b.ID And b.ProcedureId = " + ProcID + " "
                + " and a.Unitid=c.id and a.itemid=i.id "
                + " and i.ID not in (Select distinct ItemID from BatchStore where stationid=" + Gstationid + " and quantity>0 group by ItemID) "
                + " order by a.ItemType desc";
                int SNO = 1;
                DataSet ds = MainFunction.SDataSet(sql, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ProcIssueItemList nn = new ProcIssueItemList();
                    int QQty = 1;
                    if (ProcQty > 1)
                    {
                        QQty = ProcQty * (int)rr["Quantity"];
                        if ((int)rr["QOH"] < QQty) { QQty = (int)rr["QOH"]; }
                    }
                    else
                    {
                        QQty = (int)rr["Quantity"];
                    }
                    nn.ErrMsg = "";
                    nn.SNO = SNO;
                    nn.ItemCode = rr["itemcode"].ToString();
                    nn.ItemName = rr["Item"].ToString();
                    nn.QOH = (int)rr["QOH"];
                    nn.Units = rr["UnitName"].ToString();
                    nn.Quantity = QQty;
                    nn.Ucost = (decimal)rr["COSTPRICE"];
                    nn.OrderedItem = rr["Item"].ToString();
                    nn.UnitID = (int)rr["UnitId"];
                    nn.ItemID = (int)rr["ItemId"];
                    nn.ItemType = (int)(Int16)rr["ItemType"];
                    nn.IssItemID = (int)rr["ItemId"];
                    ll.Add(nn);
                    SNO += 1;
                }

                if (ll.Count == 0)
                {
                    ProcIssueItemList nn = new ProcIssueItemList();
                    nn.ErrMsg = "No Details Found!";
                    ll.Add(nn);
                }



                return ll;
            }
            catch (Exception e)
            {
                if (ll.Count > 0)
                {
                    ll[0].ErrMsg = "Can't found the Items for this procedure.";
                }
                else
                {
                    ProcIssueItemList nn = new ProcIssueItemList();
                    nn.ErrMsg = "Can't found the Items for this procedure.";
                    ll.Add(nn);
                }
                return ll;
            }


        }
        public static List<ProcIssueItemList> InsertItem(List<ProcIssueItemList> ItemList, int ID, int ReplacedItem, int Gstationid)
        {
            try
            {
                string SQL = "";
                ItemList[0].ErrMsg = "";                  if (ID > 0)                 {
                    SQL = "select b.ItemID,i.name as Item,I.ItemCode,p.ID as UnitID,p.name as Pack,sum(b.quantity) as Qoh,ic.costprice "
                    + " from BatchStore b,Item i,Packing p,Itemlowestunit IL,ItemLastCost ic "
                    + " Where b.Itemid = I.ID And il.Itemid = b.Itemid And il.packid = p.ID and b.ItemID=" + ReplacedItem + " "
                    + " and b.quantity>0 and b.stationid=" + Gstationid + " and il.itemid=ic.Itemid "
                    + " group by b.itemid,i.name,p.name,I.ItemCode,ic.costprice,p.ID ";
                }
                else
                {
                    SQL = "select b.ItemID,i.name as Item,I.ItemCode,p.ID as UnitID,p.name as Pack,sum(b.quantity) as Qoh,ic.costprice "
                    + " from BatchStore b,Item i,Packing p,Itemlowestunit IL,ItemLastCost ic "
                    + " Where b.Itemid = I.ID And il.Itemid = b.Itemid And il.packid = p.ID and b.ItemID=" + ID + " "
                    + " and b.quantity>0 and b.stationid=" + Gstationid + " and il.itemid=ic.Itemid "
                    + " group by b.itemid,i.name,p.name,I.ItemCode,ic.costprice,p.ID ";

                }
                DataSet ds = MainFunction.SDataSet(SQL, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    foreach (ProcIssueItemList nn in ItemList)
                    {
                        if (nn.ItemID == ID)
                        {
                            nn.ItemCode = rr["ItemCode"].ToString();
                            nn.ItemName = rr["Item"].ToString();
                            nn.QOH = (int)rr["Qoh"];
                            nn.Units = rr["Pack"].ToString();
                            nn.Ucost = (decimal)rr["costprice"];
                            nn.UnitID = (int)rr["UnitID"];
                            nn.ItemID = (int)rr["ItemID"];
                            nn.ItemType = 1;
                            break;

                        }


                    }


                }



                return ItemList;

            }
            catch (Exception e) { ItemList[0].ErrMsg = "Error when try to change the item!"; return ItemList; }





        }
        public static MessageModel Save(ProcIssueMain MM, User US)
        {
            MessageModel Result = new MessageModel();
            int gStationID = US.selectedStationID;
            int gSavedById = US.EmpID;

            
            if (InitBatchArray(ref MM, gStationID) == false)
            {
                Result.isSuccess = false;
                Result.Message = "Can't Save this Changes!";
                return Result;
            }



            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    bool Updateinvmax = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=32 and stationid=0", Trans);
                    int mOrderID = MainFunction.GetOneVal("Select MaxID from invmax where slno=32 and stationid=0", "MaxID", Trans);

                    bool UpdateinvmaxStation = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=32 and stationid=" + gStationID, Trans);
                    int mstOrderID = MainFunction.GetOneVal("Select Maxid from invmax where slno=32 and stationid=" + gStationID, "MaxID", Trans);

                    int intstSlNo = mstOrderID;

                    bool Insert = MainFunction.SSqlExcuite("Insert into InvProcIssues  "
                        + "(ID,StationSlno,BillNo,Isstype,IssueDate,PinNo,ProcedureID,CostAmount,OperatorID) "
                        + " Values ( " + mOrderID + "," + mstOrderID + "," + MM.lngBillId + "," + MM.intProcType + ",getdate(),'" + MM.lblPin + "',"
                        + MM.cmbProcedures + " , " + MM.lblTotCost + "," + gSavedById + " )", Trans);

                    long mTranId = MainFunction.SaveInTranOrder(Trans, gStationID, mOrderID, mstOrderID, 1, 30);
                    if (mTranId <= 0)
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

                    int mItemID = 0;
                    int j = 0;
                    foreach (arrItem item in MM.arrItem)
                    {
                        if (item.ID != mItemID)
                        {
                            j += 1;
                        }
                        string Sql = "Insert into InvProcIssuesDet(IssueID,ItemID,SubstituteID,UnitID,ItemType,Quantity,BatchID,BatchNo,EPR,Slno) "
                            + " Values(" + mOrderID + "," + item.ItemID + "," + item.ID + "," + item.UnitId + "," + item.SubId
                            + "," + item.Qty + "," + item.Batchid + ",'" + item.BatchNo + "'," + item.EPR + "," + j + ")";
                        bool insertDetl = MainFunction.SSqlExcuite(Sql, Trans);

                        string StrSql = "Update BatchStore set Quantity=quantity-" + item.Qty
                            + " where ItemId=" + item.ID + " and BatchNo='" + item.BatchNo
                            + "' and batchId=" + item.Batchid + " and stationid=" + gStationID;
                        bool UpdateBatchQty = MainFunction.SSqlExcuite(StrSql, Trans);

                        if (MainFunction.MazValidateBatchStoreQty(item.ID, int.Parse(item.Batchid), gStationID, Trans) == false)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }

                            Result.isSuccess = false;
                            Result.Message = "Can't Issue Stock for some Item!";
                            return Result;
                        }



                        string ss = "select costprice,MRP from batch where batchid = " + item.Batchid;
                        DataSet CosDS = MainFunction.SDataSet(ss, "tbl", Trans);
                        if (CosDS.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow xr in CosDS.Tables[0].Rows)
                            {
                                bool ns = MainFunction.SaveInTranOrderDetail(Trans, mTranId, (long)(int)item.ID, (long)(int)item.Qty, item.BatchNo, item.Batchid.ToString(), (decimal)xr["costprice"], (decimal)xr["MRP"], (decimal)xr["costprice"]);
                                if (ns == false)
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
                        else
                        {
                            bool ns = MainFunction.SaveInTranOrderDetail(Trans, mTranId, (long)(int)item.ID, (long)(int)item.Qty, item.BatchNo, item.Batchid.ToString(), item.Cost, item.Price, item.EPR);
                            if (ns == false)
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

                        mItemID = item.ID;
                    }

                    bool UpdateBill = MainFunction.SSqlExcuite("Update OPCompanyBillDetail set IsInvDone=1 where id=" + MM.lngBillId
                        + " and ItemID=" + MM.cmbProcedures, Trans);


                    Trans.Commit();
                    Result.isSuccess = true;
                    Result.Message = "Record(s) Saved Successfully";
                    return Result;


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


        public static bool InitBatchArray(ref ProcIssueMain pt, int gStationId, SqlTransaction Trans = null)
        {
            try
            {
                List<arrItem> arrItem = new List<arrItem>();
                decimal mQuantity = 0;
                int lItemid = 0;
                for (int i = 0; i < pt.ItemList.Count; i++)
                {
                    mQuantity = pt.ItemList[i].Quantity;
                    if (mQuantity > 0)
                    {
                        string SS = " select ic.mrp,isnull(b.costprice,0) as costprice,Bs.Quantity,B.ItemId as ID ,i.itemcode,i.name, "
                        + " isnull(B.BatchNo,'') as BatchNo,b.batchid,isnull(cast(b.expirydate as varchar(20)),'NULL') as expirydate "
                        + " FROM batch b, BatchStore Bs,item i,allItemslastmrp ic "
                        + " where B.ItemId =" + pt.ItemList[i].ItemID + " and Bs.ItemId =" + pt.ItemList[i].ItemID
                        + " and B.Batchid = Bs.Batchid and B.BatchNo = Bs.BatchNo  and bs.itemid=i.id and "
                        + " b.itemid = i.id and ic.itemid=b.itemid  and Bs.StationId = " + gStationId
                        + "  and Bs.Quantity > 0  Order By b.expirydate,B.Startdate ";

                        DataSet bt = MainFunction.SDataSet(SS, "tbl");
                        foreach (DataRow rr in bt.Tables[0].Rows)
                        {
                            if ((int)rr["ID"] == lItemid)
                            {
                                if ((int)rr["Quantity"] - mQuantity == 0)
                                {
                                    goto _MoVe_NextBatCH;
                                }

                            }
                            if ((int)rr["Quantity"] >= mQuantity)
                            {
                                arrItem One = new arrItem();

                                One.ID = (int)rr["ID"];                                 One.BatchNo = (string)rr["BatchNo"];
                                One.Batchid = rr["batchid"].ToString();
                                One.Qty = mQuantity;
                                One.ItemID = pt.ItemList[i].IssItemID;                                 One.Price = (decimal)rr["mrp"];
                                One.Cost = (decimal)rr["costprice"];
                                One.SubId = pt.ItemList[i].ItemType;                                   One.EPR = (decimal)rr["costprice"];
                                One.UnitId = pt.ItemList[i].UnitID;
                                One.ExpDt = rr["expirydate"].ToString();
                                arrItem.Add(One);
                                
                                goto _ExitBatchLoop;
                            }
                            else
                            {
                                arrItem One = new arrItem();
                                mQuantity = mQuantity - (int)rr["Quantity"];
                                One.ExpDt = rr["expirydate"].ToString();
                                One.ID = (int)rr["ID"];
                                One.BatchNo = (string)rr["BatchNo"];
                                One.Batchid = rr["batchid"].ToString();
                                One.Qty = (int)rr["Quantity"];
                                One.ItemID = (int)rr["Quantity"];
                                One.Price = (decimal)rr["mrp"];
                                One.Cost = (decimal)rr["costprice"];
                                One.SubId = pt.ItemList[i].ItemType;                                   One.EPR = (decimal)rr["costprice"];
                                One.UnitId = pt.ItemList[i].UnitID;
                                arrItem.Add(One);
                            }

                        _MoVe_NextBatCH: ;
                        }

                    _ExitBatchLoop: ;

                    }                     lItemid = pt.ItemList[i].ItemID;
                } 
                pt.arrItem = arrItem;

                return true;
            }
            catch (Exception e)
            {
                return false;
            }


        }
    }

}





