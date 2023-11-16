using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IndentReturnFun
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
        public static List<IndentView> GetIndents(ParamTable pm)
        {
            List<IndentView> ll = new List<IndentView>();
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

                string StrSql = " Select R.Id as ID,R.DateTime,S.Name as ReturnTo,O.Name as ReturnedBy,R.Status,r.refno "
                                + " ,s.id destinationid ,isnull(r.issueid,0) issueid,r.ReturnType "
                                + " from employee O,Station S,IndentReturn R "
                                + " where S.ID=R.DestinationId and O.Id=R.OperatorId "
                                + " and R.datetime >=  '" + pm.Sdate + "' and r.datetime<='" + pm.Edate + "' "
                                + " and R.Status < 3 "
                                + " and R.SourceId=" + pm.gStationid;


                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentView iv = new IndentView();
                    iv.IndentID = (int)rr["ID"];
                                                              iv.IndentDateTime = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["DateTime"]);
                    iv.referenceno = rr["refno"].ToString();
                    iv.Status = (byte)rr["Status"];
                    iv.IndentTo = rr["ReturnTo"].ToString();
                    iv.IndentByName = rr["ReturnedBy"].ToString();
                    iv.RetrunIssueID = (int)rr["issueid"];
                    iv.ReturnType = (bool)rr["ReturnType"];
                    iv.ToStationID = (int)rr["destinationid"];
                    ll.Add(iv);
                }

                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static List<IndentReturnedItemList> ViewDetails(int orderID)
        {
            List<IndentReturnedItemList> ll = new List<IndentReturnedItemList>();
            try
            {
                string StrSql = " Select R.ItemId,R.Quantity,r.unitid,I.Name as Item,i.drugtype,R.BatchNo,"
                    + " r.batchid,r.ExpiryDate,R.Remarks "
                    + " from IndentReturnDetail R,Item I where R.ReturnID=" + orderID + " and R.ItemId=I.Id ";

                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentReturnedItemList dd = new IndentReturnedItemList();
                    dd.SNO = sno;
                    dd.ID = (int)rr["ItemId"];
                    dd.Quantity = (int)rr["Quantity"];
                    dd.UnitID = (int)rr["unitid"];
                    dd.Name = (string)rr["Item"];
                    dd.DrugType = (int)rr["drugtype"];
                    dd.BatchNo = (string)rr["BatchNo"];
                    dd.BatchID = (int)rr["batchid"];
                    dd.ExpiryDate = rr["ExpiryDate"].ToString();
                    dd.Remarks = (string)rr["Remarks"];


                    sno += 1;
                    ll.Add(dd);
                }
                return ll;
            }
            catch (Exception e) { ll[0].ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }
        public static IndentOrderModel FindIssue(int slno, int GstationID)
        {
            IndentOrderModel ll = new IndentOrderModel();
            try
            {
                string Str = "select top 1 isnull(i.drugtype,0) as Drugtype,a.id as issueid,a.SourceId,a.DestinationId,a.Categoryid,b.Substituteid as ItemId, "
            + " i.name as Item,i.itemcode, a.status from IndentIssue a,IndentIssueDetail b,Item i where "
            + " a.id = b.issueid and a.destinationid = " + GstationID + " and a.status = 1 and "
            + " a.stationslno = " + slno + " and b.substituteid = i.id ";
                DataSet ds = MainFunction.SDataSet(Str, "tbl1");

                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        ll.ToStationID = (int)rr["SourceId"];
                        ll.ItemCategory = (int)rr["Categoryid"];
                        ll.IssSlNO = slno;
                        ll.mIssueID = (int)rr["issueid"];
                        ll.CurrentStation = GstationID;
                    }


                }
                else
                {
                    ll.ErrMsg = "Indent Return with this number does not exist";
                }
                return ll;
            }
            catch (Exception e) { ll.ErrMsg = "Indent Return with this number does not exist"; return ll; }



        }
        public static List<IndentReturnedItemList> InsertItem(int ItemId, int Gstationid, int ToStationID, int mIssueId, List<IndentReturnedItemList> ExistList = null)
        {
            List<IndentReturnedItemList> ll = new List<IndentReturnedItemList>();
            IndentReturnedItemList FakeToHoldErr = new IndentReturnedItemList();
            try
            {

                string Str = "";
                if (mIssueId > 0)
                {
                    Str = "Select d.UnitId,i.drugtype,b.batchid,Bt.Quantity,B.ExpiryDate,Bt.BatchNo,ip.Conversionqty,I.ID,I.NAME AS name, "
            + " CAST(D.QUANTITY AS INTEGER) as IssQty,p.name as Unit,isnull((select sum(Quantity) from IndentReturn ir,IndentReturnDetail ird "
            + " where ir.status <> 3 and ir.id = ird.returnid and ir.issueid = " + mIssueId + " and ird.itemid = " + ItemId + "),0) as returnedquantity "
            + " from Batchstore Bt,batch b,Item I,indentissuedetail d,itemPacking ip,packing p "
            + " Where i.id = b.Itemid And bt.batchid = b.batchid And bt.BatchNo = b.BatchNo And bt.Itemid = b.Itemid "
            + " and bt.stationid = " + Gstationid + " and Bt.Quantity>0 and i.id=" + ItemId + " and  bt.batchid = d.batchid and d.substituteid = i.id "
            + " and d.issueid = " + mIssueId + " and i.id = ip.itemid and d.unitid = ip.packid and d.unitid = p.id ";

                }
                else
                {
                    Str = "Select j.UnitId,i.drugtype,b.batchid,Bt.Quantity,B.ExpiryDate,I.ID,I.NAME AS name,"
               + " Bt.BatchNo,j.Conversionqty,'' as Unit,0 as IssQty,0 as returnedquantity from "
               + " Batchstore Bt,batch b,Item I,itemstore j where i.id = j.itemid and i.id=b.itemid "
               + " and bt.batchid=b.batchid and bt.batchno = b.batchno and bt.itemid = b.itemid "
               + " and j.stationid = " + Gstationid + " and bt.stationid = " + Gstationid + " and "
               + " Bt.Quantity>0 and i.id=" + ItemId;
                }

                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int sno = 0;
                    if (ExistList == null)
                    {
                        sno = 1;
                    }
                    else
                    {
                        sno = ExistList.Count + 1;
                    }
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        IndentReturnedItemList l = new IndentReturnedItemList();
                                                 l.SNO = sno;
                        l.ID = (int)rr["ID"];
                        l.Name = (string)rr["Name"];
                        l.UnitID = (int)rr["UnitId"];
                        l.DrugType = (int)rr["drugtype"];
                        l.Remarks = " ";
                        l.BatchID = (int)rr["batchid"];
                        l.BatchNo = (string)rr["BatchNo"];
                        l.ExpiryDate = rr["ExpiryDate"].ToString();
                        l.converQty = (int)rr["Conversionqty"];
                        l.Quantity = 0;
                        l.QOH = (int)rr["Quantity"] / (int)rr["Conversionqty"];
                                                                          l.issQty = int.Parse(MainFunction.Cast(rr["IssQty"].ToString(), 0));
                        l.retQty = int.Parse(MainFunction.Cast(rr["returnedquantity"].ToString(), 0));
                        l.unit = (string)rr["Unit"];
                        if (sno > 1)
                        {
                            ExistList.Add(l);
                            ll = ExistList;
                        }
                        else
                        {
                            ll.Add(l);
                        }
                    }
                }
                else
                {
                    if (ll == null)
                    {
                        FakeToHoldErr.ErrMsg = "Item not mapped to this Station";
                        ll.Add(FakeToHoldErr);
                    }
                    else
                    {
                        ll[0].ErrMsg = "Item not mapped to this Station";
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


        public static IndentOrderModel Save(IndentOrderModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    if (order.OrderID == 0)                       {
                        int mReturnId = 0;
                        long mTranId = 0;
                        int returnType = 0;

                        bool n1 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=7 and stationid=0", Trans);
                        DataSet d1 = MainFunction.SDataSet("select MaxId from invmax where slno=7 and stationid=0", "tb1", Trans);
                        foreach (DataRow rr in d1.Tables[0].Rows)
                        {
                            mReturnId = (int)rr["MaxId"];
                        }

                        if (order.mIssueID > 0) { returnType = 1; }


                        string StrSql = "Insert into IndentReturn (Id,DateTime,OperatorId,SourceId,DestinationId,Status,RefNo,ReturnType,Issueid)"
                                      + " values (" + mReturnId + ",sysdatetime()," + UserInfo.EmpID + "," + UserInfo.selectedStationID + ","
                                      + " " + order.ToStationID + ",0,'" + order.txtRef + "'," + returnType + "," + order.mIssueID + ")";
                        bool ins = MainFunction.SSqlExcuite(StrSql, Trans);

                        mTranId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, mReturnId, mReturnId, 1, 16, order.ToStationID);

                        if (mTranId == 0)                          {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            order.ErrMsg = "Error while saving!";
                            return order;

                        }




                        int count = 1;
                        foreach (var it in order.ReturnList)
                        {
                            decimal cost = 0;
                            decimal price = 0;

                            StrSql = "select costprice,sellingprice from batch where batchid = " + it.BatchID;
                            DataSet dsw = MainFunction.SDataSet(StrSql, "tbl1", Trans);
                            foreach (DataRow rr in dsw.Tables[0].Rows)
                            {
                                cost = (decimal)rr["costprice"];
                                price = (decimal)rr["sellingprice"];
                            }

                            StrSql = "Insert into IndentReturnDetail(ReturnID,ItemId,Quantity,BatchNo,Remarks,Unitid,expirydate,batchid,epr) Values("
                                    + mReturnId + "," + it.ID + "," + it.Quantity + ",'" + it.BatchNo + "','" + it.Remarks
                                    + "'," + it.UnitID + ",'" + it.ExpiryDate + "'," + it.BatchID + "," + cost * it.converQty + ")";
                            bool insdtl = MainFunction.SSqlExcuite(StrSql, Trans);


                            StrSql = "Update batchstore Set Quantity=Quantity-" + it.Quantity * it.converQty + " where ItemId=" + it.ID
                                + " and BatchNo='" + it.BatchNo + "' and batchid=" + it.BatchID + " and stationid = " + UserInfo.selectedStationID;
                            bool UpdateBatch = MainFunction.SSqlExcuite(StrSql, Trans);

                            if (MainFunction.MazValidateBatchStoreQty(it.ID, it.BatchID, UserInfo.selectedStationID, Trans) == false)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                order.ErrMsg = "Can't Issue Stock for some Item!";
                                return order;

                            }

                            
                            StrSql = "select * from batchstore where batchid=" + it.BatchID + " and batchno='" + it.BatchNo + "' "
                                + " and stationid=" + order.ToStationID;
                            DataSet batchCheck = MainFunction.SDataSet(StrSql, "tb", Trans);
                            if (batchCheck.Tables[0].Rows.Count == 0)
                            {
                                order.ErrMsg = "this item didn't received from Selected Station , No Batch Found!";
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }

                                return order;


                            }


                            bool nn = MainFunction.SaveInTranOrderDetail(Trans, mTranId, it.ID, it.Quantity * it.converQty,
                                it.BatchNo, it.BatchID.ToString(), cost, price, cost);

                            if (nn == false)                              {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                order.ErrMsg = "Error while saving!";
                                return order;

                            }



                            count += 1;
                        }
                        order.ErrMsg = "Indent Return No " + mReturnId + " saved sucessfully ";
                    }
                    else                      {
                        int mReturnId = order.OrderID;
                        string StrSql = "Update IndentReturn set status= 3 where Id=" + mReturnId + " and status =0";
                        bool update = MainFunction.SSqlExcuite(StrSql, Trans);

                        int count = 1;
                        DateTime dt = DateTime.Now;
                        foreach (var it in order.ReturnList)
                        {
                            int lGetQty = it.Quantity * MainFunction.GetQuantity(it.UnitID, it.ID);

                            StrSql = "Update batchstore Set Quantity=Quantity+" + lGetQty + " where ItemId=" + it.ID
                            + " and BatchNo='" + it.BatchNo + "' and stationid = " + UserInfo.selectedStationID
                            + " and batchid = " + it.BatchID;
                            bool updateBatchstore = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "Update batch Set Quantity=Quantity+" + lGetQty + " where ItemId=" + it.ID
                            + " and BatchNo='" + it.BatchNo + "' and batchid = " + it.BatchID;
                            bool updateBatch = MainFunction.SSqlExcuite(StrSql, Trans);



                            dt = Convert.ToDateTime(order.lbldate);



                            count += 1;
                        }

                        bool nn = MainFunction.CancelInTranOrderAndDetail(Trans, UserInfo.selectedStationID, mReturnId, dt, 16);

                        if (nn == false)                          {
                            order.ErrMsg = "Error while saving!";
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }

                            return order;

                        }

                        order.ErrMsg = "Indent Return No " + mReturnId + " Cancelled sucessfully ";
                    }

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
                if (string.IsNullOrEmpty(order.ErrMsg) == true)
                {
                    order.ErrMsg = "Error while saving!";
                } 
                return order;
            }

        }


        public static IndentOrderModel PrintOut(int OrderID, int gStationid)
        {
            IndentOrderModel ll = new IndentOrderModel();
            try
            {
                string StrSql = " Select R.Id as ID,R.DateTime,S.Name as ReturnTo,O.Name as ReturnedBy,R.Status,r.refno "
                              + " ,s.id destinationid ,isnull(r.issueid,0) issueid,r.ReturnType "
                              + " from employee O,Station S,IndentReturn R "
                              + " where S.ID=R.DestinationId and O.Id=R.OperatorId "
                              + " and R.Status < 3 and r.id=" + OrderID
                              + " and R.SourceId=" + gStationid;

                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.OrderID = (int)rr["id"];
                    ll.ToStationName = rr["ReturnTo"].ToString();
                    ll.dtpBydate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["DateTime"]);
                    ll.Status = (Byte)rr["status"];

                }
                List<IndentReturnedItemList> lst = ViewDetails(OrderID);

                ll.ReturnList = lst;
                return ll;
            }
            catch (Exception e) { return ll; };
        }

    }

}





