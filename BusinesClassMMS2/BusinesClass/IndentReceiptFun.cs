using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IndentReceiptFun
    {
        public static IndentOrderModel ClearIndent(int gStation)
        {
            IndentOrderModel nn = new IndentOrderModel();
            List<TempListMdl> station = new List<TempListMdl>();
            DataSet ds = MainFunction.SDataSet("select id,name from station where stores=1 and deleted=0 and  id <>" + gStation + " order by name", "tbl");
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
        public static List<ReceiptIndentView> GetIndents(ParamTable pm)
        {
            List<ReceiptIndentView> ll = new List<ReceiptIndentView>();
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
                    ToDate = DateTime.Parse(pm.Edate);
                    ToDate = ToDate.AddDays(1);
                }
                pm.Sdate = String.Format("{0:dd-MMM-yyyy}", FromDate);
                pm.Edate = String.Format("{0:dd-MMM-yyyy}", ToDate);

                string StrSql = "  Select a.StationSlno as slno,a.Id as IssueNo,b.stationslno,a.IndentId,a.Status,b.[DateTime] as IndentDate "
                + " ,b.ReferenceNo,E.Name as IndentedBy,a.DateTime as IssuedAt,f.name as IssuedBy,S.name as StationName, "
                + " isnull( b.iscocktail,0) as iscocktail,src.prefix "
                + " from IndentIssue a,Indent b,Employee E,employee f ,Station S ,station src "
                + " where s.id=a.sourceid and b.Id=a.IndentId and B.OperatorID=E.Id and a.operatorid = f.id "
                + " and src.id=b.sourceid "
                + " and a.status=0 and b.sourceid= " + pm.gStationid
                + " and a.datetime>='" + pm.Sdate + "' "
                + " and a.datetime<'" + pm.Edate + "' "
                + " order by a.StationSlno, a.datetime desc ";
                 

                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ReceiptIndentView iv = new ReceiptIndentView();
                    iv.IndentNo = rr["prefix"].ToString() + " " + rr["StationSlno"].ToString();
                                                              iv.Slno = rr["slno"].ToString();
                    iv.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["IndentDate"]);
                    iv.ReferenceNo = rr["ReferenceNo"].ToString();
                    iv.IndentBy = rr["IndentedBy"].ToString();
                    iv.IssueOn = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["IssuedAt"]);
                    iv.IssuedBy = rr["IssuedBy"].ToString();
                    iv.Status = (byte)rr["Status"];
                    iv.IndentID = (int)rr["IndentId"];
                    iv.isCocktail = (byte)rr["iscocktail"];
                    iv.IssueNo = (int)rr["IssueNo"];
                    if ((byte)rr["Status"] > 0)
                    {
                        iv.ReceivedAt = MainFunction.GetName(" Select a.DateTime as Rdate from IndentReceipt a,Employee b "
                                        + " where a.OperatorId=b.Id and a.issueid = " + iv.IssueNo, "Rdate");
                        iv.ReceivedAt = String.Format("{0:dd-MMM-yyyy}", iv.ReceivedAt);
                        iv.ReceivedBy = MainFunction.GetName(" Select b.Name from IndentReceipt a,Employee b "
                                        + " where a.OperatorId=b.Id and a.issueid = " + iv.IssueNo, "Name");
                    }

                    ll.Add(iv);
                }


                
                StrSql = "  Select a.StationSlno as slno,a.Id as IssueNo,b.stationslno,a.IndentId,a.Status,b.[DateTime] as IndentDate "
                + " ,b.ReferenceNo,E.Name as IndentedBy,a.DateTime as IssuedAt,f.name as IssuedBy,S.name as StationName, "
                + " isnull( b.iscocktail,0) as iscocktail,src.prefix "
                + " from IndentIssue a,Indent b,Employee E,employee f ,Station S ,station src "
                + " where s.id=a.sourceid and b.Id=a.IndentId and B.OperatorID=E.Id and a.operatorid = f.id "
                + " and src.id=b.sourceid "
                + " and a.status>0 and b.sourceid= " + pm.gStationid
                + " and a.datetime>='" + pm.Sdate + "' "
                + " and a.datetime<'" + pm.Edate + "' "
                + " order by a.datetime desc ";


                DataSet nn = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in nn.Tables[0].Rows)
                {
                    ReceiptIndentView iv = new ReceiptIndentView();
                    iv.IndentNo = rr["prefix"].ToString() + " " + rr["StationSlno"].ToString();
                                                              iv.Slno = rr["slno"].ToString();
                    iv.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["IndentDate"]);
                    iv.ReferenceNo = rr["ReferenceNo"].ToString();
                    iv.IndentBy = rr["IndentedBy"].ToString();
                    iv.IssueOn = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["IssuedAt"]);
                    iv.IssuedBy = rr["IssuedBy"].ToString();
                    iv.Status = (byte)rr["Status"];
                    iv.IndentID = (int)rr["IndentId"];
                    iv.isCocktail = (byte)rr["iscocktail"];
                    iv.IssueNo = (int)rr["IssueNo"];
                    if ((byte)rr["Status"] > 0)
                    {
                        iv.ReceivedAt = MainFunction.GetName(" Select a.DateTime as Rdate from IndentReceipt a,Employee b "
                                        + " where a.OperatorId=b.Id and a.issueid = " + iv.IssueNo, "Rdate");
                        iv.ReceivedAt = String.Format("{0:dd-MMM-yyyy}", iv.ReceivedAt);
                        iv.ReceivedBy = MainFunction.GetName(" Select b.Name from IndentReceipt a,Employee b "
                                        + " where a.OperatorId=b.Id and a.issueid = " + iv.IssueNo, "Name");
                    }

                    ll.Add(iv);
                }

                List<ReceiptIndentView> yy = ll.OrderByDescending(o => o.IndentNo).ToList();

                return yy;
            }
            catch (Exception e) { return ll; };

        }
        public static List<IndentReceiptDtl> ViewDetails(int mIndentId, int mIssueId, int MStatus)
        {
            List<IndentReceiptDtl> ll = new List<IndentReceiptDtl>();
            try
            {
                string StrSql = "select a.name,b.itemid,e.destinationid,b.quantity as ordqty,d.name as Unit,a.itemcode "
                + "  from item a,indentdetail b,packing d,indent e where "
                + " b.itemid = a.Id And  b.indentid = e.Id and b.UnitId=d.Id"
                + " and  e.id=" + mIndentId + " order by slno";
                int SourceStation = 0;
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                int TempItemID = 0;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    
                    StrSql = "select a.itemid,a.unitid,a.quantity as issuedqty,i.itemcode,i.name as itemname "
                        + " ,a.substituteid,a.batchno,a.batchid,b.sellingprice,c.name as unit "
                        + " from indentissuedetail a,batch b,item i,packing c "
                        + " where a.batchID=b.batchID and a.itemid = i.id and a.substituteid=b.itemid "
                        + " and c.id=a.unitid and a.issueid=" + mIssueId + " and a.itemid=" + (int)rr["itemid"];
                    DataSet xx = MainFunction.SDataSet(StrSql, "tbl");
                    double OrderQty = 0;
                    foreach (DataRow yy in xx.Tables[0].Rows)
                    {
                        IndentReceiptDtl dd = new IndentReceiptDtl();
                        if (TempItemID != (int)rr["itemid"])
                        {
                            dd.Item = rr["itemcode"].ToString() + "   " + rr["name"].ToString();
                            dd.OrderQty = (double)rr["ordqty"];
                            dd.OrderUnitName = rr["Unit"].ToString();
                            OrderQty = dd.OrderQty;
                        }
                        else
                        {
                            sno -= 1;

                        }
                        dd.SNO = sno;
                        SourceStation = (int)rr["destinationid"];
                        dd.ItemID = (int)rr["itemid"];
                        TempItemID = (int)rr["itemid"];



                        if (dd.ItemID == (int)yy["itemid"] && OrderQty > 0)
                        {
                            if (dd.IssuedQty == 0)
                            {
                                if ((int)yy["itemid"] == (int)yy["substituteid"])
                                {
                                    decimal cur = (decimal)yy["sellingprice"] * MainFunction.GetQuantity((int)yy["unitid"], (int)yy["itemid"]);
                                    dd.BatchNo = yy["batchno"].ToString();
                                    dd.IssuedQty = (double)yy["issuedqty"];
                                    dd.IssueUnitName = yy["unit"].ToString();
                                    dd.ReceivedQty = (double)yy["issuedqty"];
                                    dd.ReceivedUnitName = yy["unit"].ToString();
                                    dd.SellingPrice = cur;
                                    dd.UnitID = (int)yy["unitid"];
                                    dd.BatchID = (int)yy["batchid"];

                                }
                                else
                                {
                                    decimal cur = (decimal)yy["sellingprice"] * MainFunction.GetQuantity((int)yy["unitid"], (int)yy["substituteid"]);
                                    dd.Item = MainFunction.GetName("select ItemCode + '  ' + name as Name from Item where item where id=" + (int)yy["substituteid"], "Name");
                                    dd.BatchNo = yy["batchno"].ToString();
                                    dd.IssuedQty = (double)yy["issuedqty"];
                                    dd.IssueUnitName = yy["unit"].ToString();
                                    dd.ReceivedQty = (double)yy["issuedqty"];
                                    dd.ReceivedUnitName = yy["unit"].ToString();
                                    dd.SellingPrice = cur;
                                    dd.UnitID = (int)yy["unitid"];
                                    dd.BatchID = (int)yy["batchid"];
                                    dd.ItemID = (int)yy["substituteid"];
                                }

                            }
                            else
                            {
                                decimal cur = (decimal)yy["sellingprice"] * MainFunction.GetQuantity((int)yy["unitid"], (int)yy["substituteid"]);
                                if ((int)yy["itemid"] == (int)yy["substituteid"])
                                {
                                    dd.BatchNo = yy["batchno"].ToString();
                                    dd.IssuedQty = (double)yy["issuedqty"];
                                    dd.IssueUnitName = yy["unit"].ToString();
                                    dd.ReceivedQty = (double)yy["issuedqty"];
                                    dd.ReceivedUnitName = yy["unit"].ToString();
                                    dd.SellingPrice = cur;
                                    dd.UnitID = (int)yy["unitid"];
                                    dd.BatchID = (int)yy["batchid"];

                                }
                                else
                                {
                                    dd.Item = MainFunction.GetName("select ItemCode + '  ' + name as Name from Item where item where id=" + (int)yy["substituteid"], "Name");
                                    dd.BatchNo = yy["batchno"].ToString();
                                    dd.IssuedQty = (double)yy["issuedqty"];
                                    dd.IssueUnitName = yy["unit"].ToString();
                                    dd.ReceivedQty = (double)yy["issuedqty"];
                                    dd.ReceivedUnitName = yy["unit"].ToString();
                                    dd.SellingPrice = cur;
                                    dd.UnitID = (int)yy["unitid"];
                                    dd.BatchID = (int)yy["batchid"];

                                }
                                dd.ItemID = (int)yy["substituteid"];
                            }
                            goto Next_rec;
                        }
                        else if (dd.ItemID == (int)yy["substituteid"] && OrderQty > 0)
                        {
                            decimal cur = (decimal)yy["sellingprice"] * MainFunction.GetQuantity((int)yy["unitid"], (int)yy["substituteid"]);
                            dd.Item = MainFunction.GetName("select ItemCode + '  ' + name as Name from Item where item where id=" + (int)yy["substituteid"], "Name");
                            dd.BatchNo = yy["batchno"].ToString();
                            dd.IssuedQty = (double)yy["issuedqty"];
                            dd.IssueUnitName = yy["unit"].ToString();
                            dd.ReceivedQty = (double)yy["issuedqty"];
                            dd.ReceivedUnitName = yy["unit"].ToString();
                            dd.SellingPrice = cur;
                            dd.UnitID = (int)yy["unitid"];
                            dd.BatchID = (int)yy["batchid"];
                            dd.ItemID = (int)yy["substituteid"];
                            goto Next_rec;
                        }
                    Next_rec:
                        sno += 1;
                        ll.Add(dd);
                    }

                }


                
                if (MStatus == 1 || MStatus == 2)
                {
                    StrSql = "Select quantity,itemid,batchno,batchid,isnull(b.stationslno,0) stationslno "
                        + " from indentReceiptdetail,indentreceipt b "
                    + " where b.issueid=indentreceiptdetail.issueid and "
                    + " indentreceiptdetail.IssueId=" + mIssueId + " order by ItemId";
                    DataSet rec = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow rr in rec.Tables[0].Rows)
                    {


                        foreach (var item in ll)
                        {
                            if ((int)rr["itemid"] == item.ItemID && rr["batchno"].ToString() == item.BatchNo &&
                                (int)rr["batchid"] == item.BatchID)
                            {
                                item.ReceivedQty = (double)rr["quantity"];
                                goto Next_rec2;
                            }
                        }
                    Next_rec2:
                        ll[0].StationSlno = (int)rr["stationslno"];
                        ll[0].MaxSlno = (int)rr["stationslno"];
                    }

                }




                return ll;
            }
            catch (Exception e) { ll[0].ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }
        public static IndentOrderModel Save(IndentOrderModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    int maxstationslno = 0;
                    int MSTATUS = 1;
                    long mTranId = 0;
                    int mReceiptId = 0;
                    int mIndentId = order.IndentID;
                    int mIssueId = order.mIssueID;
                    int Gstationid = UserInfo.selectedStationID;
                    int SourceStation = 0;
                    long lblIssueNo = (long)(int)order.IssueNo;
                    DataSet d1 = MainFunction.SDataSet("select isnull(max(stationslno),0) as stationslno from indentreceipt where stationid=" + Gstationid + " ", "tb1", Trans);
                    foreach (DataRow rr in d1.Tables[0].Rows)
                    {
                        if ((int)rr["stationslno"] > 0)
                        {
                            maxstationslno = (int)rr["stationslno"] + 1;
                        }
                        else
                        {
                            maxstationslno = 1;
                        }
                    }

                    string StrSql = "insert into indentreceipt(IssueID,DateTime,OperatorId,stationId,stationslno) values"
                            + " (" + mIssueId + ",sysdatetime()," + UserInfo.EmpID + "," + Gstationid + "," + maxstationslno + ")";
                    bool ins = MainFunction.SSqlExcuite(StrSql, Trans);

                    StrSql = "UPDATE INDENTISSUE SET STATUS =" + MSTATUS + "  WHERE  ID = " + mIssueId
                        + " and destinationid = " + Gstationid + " and Status = 0";
                    int i = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                    if (i == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Error while saving!";
                        return order;
                    }

                    StrSql = "select max(id) RecNo from indentreceipt where stationid = " + Gstationid;
                    DataSet max = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    foreach (DataRow rr in max.Tables[0].Rows)
                    {
                        mReceiptId = (int)rr["RecNo"];
                    }

                    StrSql = "UPDATE INDENT SET RECSTATUS = 1 WHERE  ID = " + mIndentId;
                    bool update = MainFunction.SSqlExcuite(StrSql, Trans);

                    StrSql = "select sourceid from indentissue where id  = " + mIssueId + " and destinationid = " + Gstationid;
                    DataSet sqn = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    foreach (DataRow rr in sqn.Tables[0].Rows)
                    {
                        mTranId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, mIssueId,
                             lblIssueNo, 0, 5, (int)rr["sourceid"], null, lblIssueNo);

                        SourceStation = (int)rr["sourceid"];
                    }
                    if (mTranId == 0)                      {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Error while saving!";
                        return order;
                    }

                    foreach (var itm in order.ItemReceiptList)
                    {
                        if (itm.ReceivedQty > 0)
                        {
                            double Qty = itm.ReceivedQty * MainFunction.GetQuantity(itm.UnitID, itm.ItemID);
                            StrSql = "insert into indentreceiptdetail (IssueId,ItemId,Quantity,Batchno,unitid,receiptid,BatchId) values"
                                   + " (" + mIssueId + "," + itm.ItemID + "," + itm.ReceivedQty + ","
                                   + "'" + itm.BatchNo + "'," + itm.UnitID + "," + mReceiptId + ","
                                   + itm.BatchID + ")";
                            bool InserItm = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "Select batchId from Batchstore where ItemId=" + itm.ItemID
                                + " and stationid=" + Gstationid + " and batchId=" + itm.BatchID;
                            DataSet BtQ = MainFunction.SDataSet(StrSql, "tbl", Trans);
                            if (BtQ.Tables[0].Rows.Count == 0)
                            {

                                StrSql = "Insert into batchstore (ItemID,BatchNo,stationid,Quantity,BatchId) Values("
                                + itm.ItemID + ",'" + itm.BatchNo + "'," + Gstationid + ","
                                + Qty + "," + itm.BatchID + ")";
                                bool Inserbt = MainFunction.SSqlExcuite(StrSql, Trans);
                            }
                            else
                            {
                                foreach (DataRow rr in BtQ.Tables[0].Rows)
                                {

                                    StrSql = "update batchstore set quantity = QUANTITY + " + Qty + " where"
                                        + " itemid = " + itm.ItemID + " and"
                                        + " stationid = " + Gstationid + " and"
                                        + " batchid =" + itm.BatchID;
                                    bool Updatebt = MainFunction.SSqlExcuite(StrSql, Trans);

                                }
                            }

                            StrSql = "select itemid from itemstore where itemid = " + itm.ItemID
                                + " and stationid = " + Gstationid;
                            DataSet ChkecItemstore = MainFunction.SDataSet(StrSql, "tbl", Trans);
                            if (ChkecItemstore.Tables[0].Rows.Count == 0)
                            {
                                StrSql = "insert into itemstore (itemid,maxlevel,minlevel,qoh,rol,roq,abc,fsn,stationid,deleted,startdatetime,enddatetime,ved,tax,unitid,conversionqty) "
                                + " select itemid,maxlevel,minlevel,0,rol,roq,abc,fsn,"
                                + Gstationid + ",0,getdate(),null,ved,tax,unitid,conversionqty from itemstore "
                                + " where itemid = " + itm.ItemID
                                + " and stationid = " + SourceStation;
                                bool insertrStoreItem = MainFunction.SSqlExcuite(StrSql, Trans);
                            }

                            StrSql = "select costprice,sellingprice from batch where batchid = " + itm.BatchID;
                            DataSet nx = MainFunction.SDataSet(StrSql, "tbl", Trans);
                            if (nx.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in nx.Tables[0].Rows)
                                {
                                    if (MainFunction.SaveInTranOrderDetail(Trans, mTranId, itm.ItemID, (long)Qty, itm.BatchNo, itm.BatchID.ToString(),
                                        (decimal)rr["costprice"],
                                        (decimal)rr["sellingprice"], (decimal)rr["costprice"]) == false)
                                    {
                                        if (Con.State == ConnectionState.Open)
                                        {
                                            Con.BeginTransaction().Rollback();
                                            Con.Close();
                                        }
                                        order.ErrMsg = "Can't Update the Trans Order.";
                                        return order;

                                    }

                                }
                            }
                            else
                            {
                                if (MainFunction.SaveInTranOrderDetail(Trans, mTranId, itm.ItemID, (long)Qty, itm.BatchNo, itm.BatchID.ToString()) == false)
                                {
                                    if (Con.State == ConnectionState.Open)
                                    {
                                        Con.BeginTransaction().Rollback();
                                        Con.Close();
                                    }
                                    order.ErrMsg = "Can't Update the Trans Order.";
                                    return order;

                                }

                            }

                        }
                    }




                    order.ErrMsg = "Indent Receipt No " + mReceiptId + " saved sucessfully ";


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





