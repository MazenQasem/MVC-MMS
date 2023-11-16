using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class AdjReceiptFun
    {
        public static AdjReceiptModel ClearIndent()
        {
            AdjReceiptModel nn = new AdjReceiptModel();
            List<TempListMdl> station = new List<TempListMdl>();
            DataSet ds = MainFunction.SDataSet("select id,name from station where deleted=0  order by name", "tbl");
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl l = new TempListMdl();
                l.ID = (int)rr["id"];
                l.Name = rr["name"].ToString();

                station.Add(l);
            }
            nn.StationList = station;
            return nn;
        }

        public static List<AdjView> GetAdjRecList(ParamTable pm)
        {
            List<AdjView> ll = new List<AdjView>();
            try
            {
                DateTime FromDate = new DateTime();
                DateTime ToDate = new DateTime();
                if (string.IsNullOrEmpty(pm.Sdate) == true || string.IsNullOrEmpty(pm.Edate))
                {
                    FromDate = DateTime.Now.AddDays(-10);
                    ToDate = DateTime.Now.AddDays(1);
                }
                else
                {
                    FromDate = DateTime.Parse(pm.Sdate);
                    ToDate = DateTime.Parse(pm.Edate).AddDays(1);
                }
                pm.Sdate = String.Format("{0:dd-MMM-yyyy}", FromDate);
                pm.Edate = String.Format("{0:dd-MMM-yyyy}", ToDate);

                string StrSql = " Select a.Id as ID,a.DateTime as ADateTime,b.Name as Operator,a.RefNo  "
                + " from adjustreceipt a,Employee b  "
                + " where  	a.datetime>='" + pm.Sdate + "' and a.datetime<='" + pm.Edate + "' "
                + " and a.OperatorId=b.Id and stationid = " + pm.gStationid + " order by a.Id ";
                ;
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    AdjView iv = new AdjView();
                    iv.AdjRecID = (int)rr["ID"];
                    iv.Number = iv.AdjRecID;
                    iv.Date = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["ADateTime"]);
                    iv.Operator = rr["Operator"].ToString();
                    iv.RefNo = rr["RefNo"].ToString();

                    ll.Add(iv);
                }

                return ll;
            }
            catch (Exception e) { return ll; };

        }

        public static List<AdjItemsInserted> InsertItem(int ItemId, int Gstationid, List<AdjItemsInserted> ExistList = null, bool NewBatch = false)
        {
            List<AdjItemsInserted> ll = new List<AdjItemsInserted>();
            AdjItemsInserted FakeToHoldErr = new AdjItemsInserted();
            try
            {

                int lGetQty;
                double lLargeQty;
                decimal cprice;

                int sno = 0;
                if (ExistList == null)
                {
                    sno = 1;
                }
                else
                {
                    sno = ExistList.Count + 1;
                }

                if (NewBatch == false)
                {
                    string Str = "Select s.ConversionQty,s.unitid,i.drugtype,BS.Quantity as qoh,B.BatchNo,b.batchid,B.costprice,b.expirydate, "
                    + " p.name as unit,i.itemcode,i.name,i.ID as ItemID "
                    + " FROM  Batch B ,Item I,itemstore s,batchstore bs,packing p where bs.batchno=b.batchno and b.batchid=bs.batchid and "
                    + " bs.itemid=b.itemid and bs.quantity>0 and s.itemid = i.id and s.stationid = " + Gstationid
                    + " and s.unitid=p.id and bs.stationid=" + Gstationid + " and b.ItemId=" + ItemId + " and I.id=b.itemid ";
                    DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        foreach (DataRow rr in ds.Tables[0].Rows)
                        {
                            AdjItemsInserted dd = new AdjItemsInserted();
                            lGetQty = (int)rr["ConversionQty"] > 0 ? (int)rr["ConversionQty"] : 1;
                            lLargeQty = (int)rr["qoh"] / lGetQty;
                            cprice = (decimal)rr["costprice"] * lGetQty;
                            dd.SNO = sno;
                            dd.Item = (string)rr["itemcode"] + " - " + rr["name"].ToString();
                            dd.BatchNo = (string)rr["BatchNo"];
                            dd.QOH = lLargeQty;
                            dd.PRate = Math.Round(cprice, 2);
                            dd.Expiry = MainFunction.DateFormat(rr["expirydate"].ToString(), "dd", "MMM", "yyyy");
                            dd.Quantity = 0;
                            dd.Reason = "";
                            dd.ConversionQty = (int)rr["ConversionQty"];
                            dd.DrugType = (int)rr["drugtype"];
                            dd.BatchID = (int)rr["batchid"];
                            dd.UnitID = (int)rr["unitid"];
                            dd.CategoryID = 0;
                            dd.SellingPrice = 0;
                            dd.UOM = (string)rr["unit"];
                            dd.ItemCode = (string)rr["itemcode"];
                            dd.ItemID = (int)rr["ItemID"];

                            if (ExistList != null)
                            {
                                ExistList.Add(dd);
                                ll = ExistList;
                            }
                            else
                            {
                                ll.Add(dd);
                            }
                            sno += 1;
                        }

                    }
                    else
                    {
                        FakeToHoldErr.BatchNotFound = true;
                        ll.Add(FakeToHoldErr);
                    }
                }
                else if (NewBatch == true)
                {
                    decimal UnitCostPrice = 0;
                    string GetLastMrp = "Select b.mrp,b.costprice from Batch B ,Item I  "
                      + " where  b.ItemId=" + ItemId + " and I.id=b.itemid  "
                      + " and b.startdate =(select max(startdate) from Batch where itemid =" + ItemId + "  ) ";
                    DataSet Mrp = MainFunction.SDataSet(GetLastMrp, "t1");
                    if (Mrp.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rr in Mrp.Tables[0].Rows)
                        {
                            UnitCostPrice = (decimal)rr["mrp"];
                        }

                    }
                    else { UnitCostPrice = 0; }

                    string GetOtherInfo = "Select s.ConversionQty,s.unitid,i.CategoryId,i.drugtype,0 as qoh,'1234' as BatchNo,"
                    + " 0 as batchid,0 as costprice,'' as expirydate,p.name as unit,i.itemcode,i.name,i.id as ItemID "
                    + " from Item I,itemstore s, "
                    + " packing p where s.itemid = i.id and s.stationid = " + Gstationid
                    + " and s.unitid=p.id and I.id=" + ItemId;
                    DataSet OtherInfo = MainFunction.SDataSet(GetOtherInfo, "t2");
                    foreach (DataRow rr in OtherInfo.Tables[0].Rows)
                    {
                        AdjItemsInserted dd = new AdjItemsInserted();
                        lGetQty = (int)rr["ConversionQty"] > 0 ? (int)rr["ConversionQty"] : 1;
                        lLargeQty = 0;
                        cprice = UnitCostPrice * lGetQty;
                        dd.SNO = sno;
                        dd.Item = (string)rr["itemcode"] + " - " + rr["name"].ToString();
                        dd.BatchNo = (string)rr["BatchNo"];
                        dd.QOH = lLargeQty;
                        dd.PRate = Math.Round(cprice, 2);
                        dd.Expiry = DateTime.Now.AddYears(1).ToString("dd MMM yyyy");
                        dd.Quantity = 0;
                        dd.Reason = "";
                        dd.ConversionQty = (int)rr["ConversionQty"];
                        dd.DrugType = (int)rr["drugtype"];
                        dd.BatchID = (int)rr["batchid"];
                        dd.UnitID = (int)rr["unitid"];
                        dd.CategoryID = (int)rr["CategoryId"];
                        dd.SellingPrice = Math.Round(cprice, 2);
                        dd.UOM = (string)rr["unit"];
                        dd.ItemCode = (string)rr["itemcode"];
                        dd.ItemID = (int)rr["ItemID"];
                        dd.NewBatchFlag = NewBatch;

                        if (ExistList != null)
                        {
                            ExistList.Add(dd);
                            ll = ExistList;
                        }
                        else
                        {
                            ll.Add(dd);
                        }
                        sno += 1;
                    }

                }

                return ll;
            }
            catch (Exception e)
            {
                if (ll.Count == 0)
                {
                    FakeToHoldErr.ErrMsg = "Item Error: " + e.Message;
                    ll.Add(FakeToHoldErr);
                }
                else
                {
                    ll[0].ErrMsg = "Item Error: " + e.Message;
                }
                return ll;
            }


        }

        public static List<AdjItemsInserted> ViewDetails(int orderID)
        {
            List<AdjItemsInserted> ll = new List<AdjItemsInserted>();
            try
            {
                string StrSql = "Select L.ItemId,L.Quantity,I.Name as Item,i.drugtype, "
                    + " L.BatchNo,B.costprice,B.ExpiryDate,L.Remarks,p.name as unitname,I.ITEMCODE "
                    + " from adjustReceiptdetail L,Batch B,Item I,PACKING P "
                    + " where P.id = L.unitid AND L.adjreceiptId= " + orderID
                    + " and l.batchid=b.batchid and L.ItemId=B.ItemId and B.BatchNo=L.BatchNo "
                    + " and L.ItemId=I.Id ";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    AdjItemsInserted dd = new AdjItemsInserted();
                    dd.SNO = sno;
                    dd.Item = (string)rr["ITEMCODE"] + " - " + rr["Item"].ToString();
                    dd.BatchNo = (string)rr["BatchNo"];
                    dd.QOH = 0;
                    dd.PRate = (decimal)rr["costprice"];
                    dd.Expiry = MainFunction.DateFormat(rr["ExpiryDate"].ToString(), "dd", "MMM", "yyyy");
                    dd.Quantity = (decimal)rr["Quantity"];
                    dd.Reason = rr["Remarks"].ToString();
                    dd.ConversionQty = 0;
                    dd.DrugType = (int)rr["drugtype"];
                    dd.BatchID = 0;
                    dd.UnitID = 0;
                    dd.CategoryID = 0;
                    dd.SellingPrice = 0;
                    dd.UOM = (string)rr["unitname"];
                    dd.ItemCode = (string)rr["ITEMCODE"];
                    dd.NewBatchFlag = false;
                    dd.ItemID = (int)rr["ItemId"];

                    sno += 1;
                    ll.Add(dd);
                }
                return ll;
            }
            catch (Exception e) { ll[0].ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }

        public static AdjReceiptModel Save(AdjReceiptModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();


                    int mReceiptId = 0;
                    int lstSlno = 0;
                    int Gstationid = UserInfo.selectedStationID;
                    int gSavedById = UserInfo.EmpID;

                    bool n1 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=23 and stationid=0", Trans);

                    DataSet d1 = MainFunction.SDataSet("select MaxId from invmax where slno=23 and stationid=0", "tb1", Trans);
                    foreach (DataRow rr in d1.Tables[0].Rows)
                    {
                        mReceiptId = (int)rr["MaxId"];
                    }

                    bool n2 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=23 and stationid=" + Gstationid, Trans);
                    DataSet d2 = MainFunction.SDataSet("select MaxId from invmax where slno=23 and stationid=" + Gstationid, "tb1", Trans);
                    foreach (DataRow rr in d2.Tables[0].Rows)
                    {
                        lstSlno = (int)rr["MaxId"];
                    }

                                         string StrSql = "Insert into adjustreceipt (Id,DateTime,StationId,OperatorId,RefNo,stationslno) Values(" +
                                       +mReceiptId + ",'" + DateTime.Now + "'," + Gstationid + "," + gSavedById + ",'"
                                       + order.txtRefNo.Trim() + "'," + lstSlno + ")";
                    bool InsertAdjHeader = MainFunction.SSqlExcuite(StrSql, Trans);

                                         long mTranId = MainFunction.SaveInTranOrder(Trans, Gstationid, mReceiptId, mReceiptId, 0, 1);

                    if (mTranId == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Error while saving!";
                        return order;
                    }

                    for (int i = 0; i < order.SelectedItem.Count; i++)
                    {
                        if (order.SelectedItem[i].Quantity > 0)
                        {
                            int lconvqty = order.SelectedItem[i].ConversionQty;
                            decimal iqty = order.SelectedItem[i].Quantity * lconvqty;
                            int Batchid = order.SelectedItem[i].BatchID;
                            if (Batchid == 0)                             {
                                if (order.SelectedItem[i].Expiry != null)
                                {

                                    DateTime ExpDate = DateTime.Now.AddDays(350);
                                                                                                              StrSql = "Insert into Batch(ItemId,BatchNo,ExpiryDate,Quantity,CostPrice,Tax,SellingPrice,StartDate,MRP,PurRate,unitepr) Values(" +
                                        +order.SelectedItem[i].ItemID + ",'" + order.SelectedItem[i].BatchNo.Trim() + "','"
                                        + MainFunction.DateFormat(ExpDate.ToString(), "dd", "MMM", "yyyy", "hh", "mm", "ss") + "',"
                                        + order.SelectedItem[i].Quantity * lconvqty + "," + order.SelectedItem[i].PRate / lconvqty + ","
                                        + "0," + order.SelectedItem[i].PRate / lconvqty + ",'" + DateTime.Now
                                        + "'," + order.SelectedItem[i].PRate / lconvqty + "," + order.SelectedItem[i].PRate
                                        + "," + order.SelectedItem[i].PRate / lconvqty + ")";
                                }
                                else
                                {
                                                                         StrSql = "Insert into Batch(ItemId,BatchNo,ExpiryDate,Quantity,CostPrice,Tax,SellingPrice,StartDate,MRP,PurRate,unitepr) Values("
                                         + order.SelectedItem[i].ItemID + ",'" + order.SelectedItem[i].BatchNo.Trim() + "','"
                                         + MainFunction.DateFormat(order.SelectedItem[i].Expiry, "MM", "dd", "yyyy", "", "", "", "/") + "',"
                                         + order.SelectedItem[i].Quantity * lconvqty + "," + order.SelectedItem[i].PRate / lconvqty + ","
                                         + "0," + order.SelectedItem[i].PRate / lconvqty + ",'" + DateTime.Now
                                         + "'," + order.SelectedItem[i].PRate / lconvqty + "," + order.SelectedItem[i].PRate
                                         + "," + order.SelectedItem[i].PRate / lconvqty + ")";
                                }
                                bool InsertDetail = MainFunction.SSqlExcuite(StrSql, Trans);


                                Batchid = MainFunction.GetOneVal("select Max(batchID) as BatchID from batch", "BatchID", Trans);
                                if (Batchid == 0) { Batchid = 1; }

                                
                                StrSql = "Insert into batchstore (BatchId,ItemId,BatchNo,Quantity,Stationid) Values("
                                         + Batchid + "," + order.SelectedItem[i].ItemID + ",'" + order.SelectedItem[i].BatchNo.Trim()
                                         + "'," + order.SelectedItem[i].Quantity * lconvqty + "," + Gstationid + ")";

                                bool InsertBatchStore = MainFunction.SSqlExcuite(StrSql, Trans);
                            } 
                            DataSet BatDset = MainFunction.SDataSet("select CostPrice,sellingprice from batch where batchid = " + Batchid, "tbl", Trans);
                            if (BatDset.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in BatDset.Tables[0].Rows)
                                {
                                    StrSql = "Insert into adjustReceiptDetail(adjreceiptID,ItemId,Quantity,BatchNo,Remarks,batchid,unitid,epr,qoh) Values("
                                        + mReceiptId + "," + order.SelectedItem[i].ItemID
                                        + "," + order.SelectedItem[i].Quantity + ",'" + order.SelectedItem[i].BatchNo.Trim()
                                        + "','" + order.SelectedItem[i].Reason + "'," + Batchid + ","
                                        + order.SelectedItem[i].UnitID + "," + (decimal)rr["CostPrice"] * order.SelectedItem[i].ConversionQty
                                        + "," + order.SelectedItem[i].QOH + ")";
                                    bool InsertIntoAdjTable = MainFunction.SSqlExcuite(StrSql, Trans);

                                    if (order.SelectedItem[i].BatchID > 0)
                                    {
                                                                                 StrSql = "Update batchstore Set Quantity=Quantity+" + iqty + " where ItemId="
                                            + order.SelectedItem[i].ItemID + " and Batchid=" + order.SelectedItem[i].BatchID
                                            + " and stationid = " + Gstationid;
                                        bool updateBatchstore = MainFunction.SSqlExcuite(StrSql, Trans);

                                    }

                                     
                                                                         bool TransDtl = MainFunction.SaveInTranOrderDetail(Trans, mTranId, order.SelectedItem[i].ItemID, (long)(decimal)iqty, order.SelectedItem[i].BatchNo.Trim(),
                                                    Batchid.ToString(), (decimal)rr["CostPrice"], (decimal)rr["sellingprice"], (decimal)rr["CostPrice"]);
                                    if (TransDtl == false)
                                    {
                                        if (Con.State == ConnectionState.Open)
                                        {
                                            Con.BeginTransaction().Rollback();
                                            Con.Close();
                                        }
                                        order.ErrMsg = "Error while saving!";
                                        return order;

                                    }
                                                                                                                                                                                    }
                            }
                            else
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                order.ErrMsg = "Main Batch Not Found!";
                                return order;
                            }
                        }
                    }
                    order.ErrMsg = "Adjustment No " + lstSlno + " saved sucessfully ";
                    Trans.Commit();
                    return order;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                order.ErrMsg = "Error while saving!";
                return order;
            }

        }

    }

}





