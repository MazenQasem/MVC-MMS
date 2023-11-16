using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IndentDeptIssFun
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
        public static List<IndentDeptView> GetIndents(ParamTable pm)
        {
            List<IndentDeptView> ll = new List<IndentDeptView>();
            try
            {
                string StrSql = "select i.id as id,s.name as station,i.stationslno,d.name as dept,"
                + " i.datetime as [datetime],"
                + " p.name as operator,i.referenceno as refno from departmentorder i,employee p,"
                + " department d,station s where i.stationid = s.id and i.status = 0 "
                + " and i.deptid = d.id and i.operatorid = p.id and i.destinationid = " + pm.gStationid;
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentDeptView iv = new IndentDeptView();
                                         iv.OrderNo = rr["station"].ToString().Substring(0, 2) + rr["stationslno"].ToString();
                    iv.DateTime = String.Format("{0:dd-MMM-yyyy hh:m}", (DateTime)rr["datetime"]);
                    iv.Status = 0;
                    iv.Operator = rr["operator"].ToString();
                    iv.RefNo = rr["refno"].ToString();
                    iv.Department = rr["dept"].ToString();
                    iv.ID = int.Parse(rr["id"].ToString(), 0);
                    ll.Add(iv);
                }
                StrSql = " Select I.Id as ID,s.name as station,I.stationslno,d.name as dept,"
                    + " I.DateTime as [DateTime], P.Name as Operator,I.refno,'' as deptslno, "
                    + " do.stationslno as Orderid "
                    + " from departmentalissues  I,employee P,  department d,departmentorder do, station s "
                    + " where i.orderid = do.id and i.datetime >= getdate()-10 and d.id=i.deptid "
                    + "  and I.OperatorId=P.Id and s.id=do.stationid and i.stationid=" + pm.gStationid;
                DataSet nn = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in nn.Tables[0].Rows)
                {
                    IndentDeptView iv = new IndentDeptView();
                    iv.IssueNo = (int)rr["stationslno"];
                    iv.OrderNo = rr["station"].ToString().Substring(0, 2) + rr["Orderid"].ToString();
                    iv.DateTime = String.Format("{0:dd-MMM-yyyy hh:m}", (DateTime)rr["DateTime"]);
                    iv.Status = 1;
                    iv.Operator = rr["Operator"].ToString();
                    iv.RefNo = rr["refno"].ToString();
                    iv.Department = rr["dept"].ToString();
                    iv.ID = int.Parse(rr["ID"].ToString(), 0);
                    ll.Add(iv);
                }



                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static IndentOrderModel ViewDetails(int mDeptOrderid, int status, int gStationId)
        {
            IndentOrderModel ll = new IndentOrderModel();
            try
            {
                if (status == 0)
                {
                    string StrSql = "Select i.categoryid,i.deptid from departmentorder i where I.Id=" + mDeptOrderid;

                    DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow rr in n.Tables[0].Rows)
                    {
                        ll.OrderID = mDeptOrderid;
                        ll.lbldate = MainFunction.DateFormat(DateTime.Now.ToString(), "yyyy", "MMM", "dd");
                        ll.ToStationID = (int)rr["deptid"];
                        ll.ItemCategory = (int)rr["categoryid"];
                    }


                    
                    StrSql = "Select OD.itemid,od.unitid as issUnitid,cast(od.Quantity  as integer) as Qty,i.unitid,I.Name "
                    + " ,i.drugtype,i.conversionqty,u.name as unit,v.name as issUnit,I.itemcode,isnull(i.QOH,0) as QOH "
                    + " from  Departmentorderdetail OD,departmentorder di,MMS_itemMaster I,packing u,packing v  "
                    + " where OD.orderid=" + mDeptOrderid + " and"
                    + " di.id=od.orderid and I.ID=OD.itemid and i.unitid=u.id and od.unitid=v.id  "
                    + " and i.stationid=" + gStationId
                    + " order by od.slno";
                    DataSet items = MainFunction.SDataSet(StrSql, "tbl2");
                    List<IndentIssueInsertedItemList> IssueList = new List<IndentIssueInsertedItemList>();
                    int sn = 1;
                    foreach (DataRow rr in items.Tables[0].Rows)
                    {
                        IndentIssueInsertedItemList xx = new IndentIssueInsertedItemList();
                        xx.SNO = sn;
                        xx.lLargeQty = (int)rr["QOH"] / MainFunction.GetQuantity((int)rr["unitid"], (int)rr["itemid"]);
                        xx.OrderQty = (int)rr["Qty"];
                        xx.QtyIssue = xx.OrderQty;
                        xx.lSmallOrderQty = xx.OrderQty * MainFunction.GetQuantity((int)rr["issUnitid"], (int)rr["itemid"]);
                        xx.ItemID = (int)rr["itemid"];
                        xx.Item = rr["itemcode"].ToString() + "      " + rr["Name"].ToString();
                        xx.Expiry = rr["unit"].ToString();                          xx.IssueUnits = rr["issUnit"].ToString();
                        xx.IssueUnitID = (int)rr["issUnitid"];
                        xx.conversionqty = (int)rr["conversionqty"];
                        xx.QOH = (int)rr["QOH"];
                        xx.DrugType = (int)rr["drugtype"];
                        xx.IndentUnitID = (int)rr["issUnitid"];
                        xx.PrevUnitID = (int)rr["unitid"];                         IssueList.Add(xx);
                        sn += 1;
                    }
                    ll.IssueList = IssueList;
                }
                else if (status > 0)
                {

                    string StrSql = " Select s.Name as dept,i.categoryid,i.deptid,O.Name as Operator,"
                        + " i.stationslno,I.RefNO,I.DateTime as [DateTime],i.issuedate "
                        + " from departmentalissues i,employee O,department s "
                        + " where O.ID=I.OperatorId and s.Id=I.deptid and i.stationid=" + gStationId
                        + " and I.Id=" + mDeptOrderid;

                    DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                    foreach (DataRow rr in n.Tables[0].Rows)
                    {
                        ll.OrderID = mDeptOrderid;
                        ll.lbldate = MainFunction.DateFormat(rr["DateTime"].ToString(), "yyyy", "MMM", "dd");
                        ll.ToStationID = (int)rr["deptid"];
                        ll.ItemCategory = (int)rr["categoryid"];
                        ll.txtRef = rr["RefNO"].ToString();
                        ll.lbloperator = rr["Operator"].ToString();
                        ll.IssSlNO = (int)rr["stationslno"];
                        ll.dtpBydate = MainFunction.DateFormat(rr["issuedate"].ToString(), "yyyy", "MMM", "dd");                     }


                    
                    StrSql = " Select OD.ServiceID,od.unitid as issUnitid,cast(sum(od.Quantity) as real) as Qty,od.price, "
                    + " i.unitid,I.Name ,i.drugtype,i.conversionqty,u.name as unit,isnull(i.QOH,0) as QOH, "
                    + " v.name as issUnit,I.itemcode "
                    + " from  deptissDetail OD,departmentalissues di,mms_itemMaster I,packing u,packing v "
                    + " where OD.deptissId= " + mDeptOrderid
                    + " and di.id=od.deptissid and I.ID=OD.ServiceID and i.unitid=u.id and od.unitid=v.id "
                    + " and di.stationid= " + gStationId
                    + " and i.stationid= " + gStationId
                    + " and od.deleted=0 "
                    + " group by od.serviceid,od.unitid,od.price,i.unitid,i.name,i.drugtype,i.conversionqty, "
                    + " u.name,v.name,od.slno,i.itemcode,i.QOH "
                    + " order by od.slno ";
                    DataSet items = MainFunction.SDataSet(StrSql, "tbl2");
                    List<IndentIssueInsertedItemList> IssueList = new List<IndentIssueInsertedItemList>();
                    int sn = 1;
                    foreach (DataRow rr in items.Tables[0].Rows)
                    {
                        IndentIssueInsertedItemList xx = new IndentIssueInsertedItemList();
                        xx.SNO = sn;
                        xx.lLargeQty = (int)rr["QOH"] / MainFunction.GetQuantity((int)rr["unitid"], (int)rr["ServiceID"]);
                        xx.OrderQty = (int)(double)(Single)rr["Qty"];
                        xx.QtyIssue = double.Parse(rr["Qty"].ToString());
                        xx.lSmallOrderQty = xx.OrderQty * MainFunction.GetQuantity((int)(byte)rr["issUnitid"], (int)rr["ServiceID"]);

                        xx.ItemID = (int)rr["ServiceID"];

                        xx.Item = rr["itemcode"].ToString() + "      " + rr["Name"].ToString();
                        xx.Expiry = rr["unit"].ToString();  
                        xx.IssueUnits = rr["issUnit"].ToString();
                        xx.IssueUnitID = (int)(byte)rr["issUnitid"];

                        xx.conversionqty = (int)rr["conversionqty"];
                        xx.QOH = (int)rr["QOH"] / xx.conversionqty;
                        xx.DrugType = (int)rr["drugtype"];
                        xx.Price = (Single)rr["price"];
                        xx.IndentUnitID = (int)(byte)rr["issUnitid"];
                        xx.PrevUnitID = (int)rr["unitid"];                         IssueList.Add(xx);
                        sn += 1;
                    }
                    ll.IssueList = IssueList;







                }







                return ll;
            }
            catch (Exception e) { ll.ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }

        public static List<IndentIssueInsertedItemList> InsertItem(int ItemId, int Gstationid, int ToStationID, int mIssueId, List<IndentIssueInsertedItemList> ExistList = null)
        {
            List<IndentIssueInsertedItemList> ll = new List<IndentIssueInsertedItemList>();
            IndentIssueInsertedItemList FakeToHoldErr = new IndentIssueInsertedItemList();
            try
            {

                string Str = "";

                Str = "Select j.UnitId,i.drugtype,I.ID,I.NAME AS name,"
                    + " j.Conversionqty,P.name as Unit,sum(bt.quantity) as QOH "
                    + " from Batchstore Bt,batch b,Item I,itemstore j,packing p "
                    + " where i.id = j.itemid and i.id=b.itemid "
                    + " and bt.batchid=b.batchid and bt.batchno = b.batchno and bt.itemid = b.itemid "
                    + " and j.stationid = " + Gstationid + " and bt.stationid = " + Gstationid + " "
                    + " and i.id=" + ItemId
                    + " and j.unitid=p.id "
                    + " group by j.UnitId,i.drugtype,I.ID,I.NAME,j.Conversionqty,P.name";


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
                        IndentIssueInsertedItemList l = new IndentIssueInsertedItemList();

                        l.SNO = sno;
                        l.ItemID = (int)rr["ID"];
                        l.Item = (string)rr["Name"];
                        l.lLargeQty = (int)rr["QOH"] / (int)rr["Conversionqty"];
                        l.QOH = (int)rr["QOH"];
                        l.conversionqty = (int)rr["Conversionqty"];
                        l.IssueUnitID = (int)rr["UnitId"];
                        l.IssueUnits = rr["Unit"].ToString();
                        l.Expiry = rr["Unit"].ToString();
                        l.PrevUnitID = (int)rr["UnitId"];
                        l.DrugType = (int)rr["drugtype"];

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
        public static IndentIssueInsertedItemList ChangeQty(IndentIssueInsertedItemList lst)
        {
            try
            {
                double qtySum = 0;
                double PrevQty = 0;
                int ItemID = 0;
                int lconvqty = 0;
                double lSmallQty = 0;
                int lGetQty = 0;
                int orderUnitID = 0;
                int orderQty = 0;
                int QtyIssue = 0;
                int MinLVl = 0;
                double QOH = 0;
                int conversionqty = 0;
                int LIndex = 0;
                int IID = 0;
                LIndex += 1;

                IID = LIndex - 1;
                ItemID = lst.ItemID;
                lconvqty = lst.conversionqty;
                orderUnitID = lst.IssueUnitID;
                lSmallQty = lconvqty * lst.QOH;
                orderQty = (int)(double)lst.QtyIssue;                 conversionqty = lst.conversionqty;
                QOH = lst.QOH;
                lGetQty = MainFunction.GetQuantity(lst.IssueUnitID, lst.ItemID);
                MinLVl = lst.MinLevel;
                QtyIssue = (int)lst.QtyIssue;
                if (QtyIssue * lGetQty > lSmallQty)
                {
                    QtyIssue = (lst.batchQty / lGetQty) == 0 ? 1 : (lst.batchQty / lGetQty);
                }



                if (ItemID == lst.ItemID)
                {
                    lGetQty = MainFunction.GetQuantity(lst.IssueUnitID, lst.ItemID);
                    qtySum = qtySum + (lst.QtyIssue * (lGetQty == 0 ? 1 : lGetQty));
                    PrevQty = lst.PrevQty * MainFunction.GetQuantity(lst.PrevUnitID, lst.ItemID);
                }


                if ((qtySum + PrevQty) > (orderQty * MainFunction.GetQuantity(orderUnitID, ItemID)))
                {
                    QtyIssue = 0;
                }
                if (MinLVl > 0 && QtyIssue > 0)
                {

                    if (((MinLVl * (lGetQty == 0 ? 1 : lGetQty)) / 4) >= ((QOH * conversionqty) - (QOH * (lGetQty == 0 ? 1 : lGetQty))))
                    {
                                                 QtyIssue = 0;
                    }

                }
                if (QtyIssue > (orderQty - PrevQty) || QtyIssue > QOH)
                {
                    QtyIssue = 0;

                }

                lst.QtyIssue = QtyIssue;

                return lst;
            }
            catch (Exception e) { lst.ErrMsg = "Cant Process the Request!"; return lst; }



        }

        public static IndentOrderModel Save(IndentOrderModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {
                    string StrSql = "";
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    
                    if (InitBatchArray(ref order, UserInfo.selectedStationID, Trans) == false)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        return order;
                    }



                    StrSql = "Update DepartmentOrder set status = 1 where id = " + order.OrderID;
                    if (MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans) == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Can't Update the main Indent Order.";
                        return order;


                    }

                    StrSql = "Update invmax set maxid=maxid+1 where slno=16 and stationid=0";
                    bool UpdInvmax = MainFunction.SSqlExcuite(StrSql, Trans);
                    int mOrderID = 0;


                    StrSql = "select maxid from invmax where slno=16 and stationid=0";
                    DataSet ds1 = MainFunction.SDataSet(StrSql, "tbl1", Trans);
                    foreach (DataRow rr in ds1.Tables[0].Rows)
                    {
                        mOrderID = (int)rr["maxid"];
                    }

                    StrSql = "Update invmax set maxid=maxid+1 where slno=16 and stationid=" + UserInfo.selectedStationID;
                    bool UpdInvmax2 = MainFunction.SSqlExcuite(StrSql, Trans);
                    int mstOrderID = 0;

                    StrSql = "select maxid from invmax where slno=16 and stationid=" + UserInfo.selectedStationID;
                    DataSet ds2 = MainFunction.SDataSet(StrSql, "tbl1", Trans);
                    foreach (DataRow rr in ds2.Tables[0].Rows)
                    {
                        mstOrderID = (int)rr["maxid"];
                    }
                    int intstSlNo = mstOrderID;
                    int mDeptOrderid = order.OrderID;
                    if (string.IsNullOrEmpty(order.lbldate) == true)
                    {
                        StrSql = "Insert into  departmentalissues (ID,StationSlNo,StationId,deptid, "
                            + " OperatorID,DateTime,RefNo,issuedate,Categoryid,Orderid) Values("
                            + " " + mOrderID + "," + mstOrderID + "," + UserInfo.selectedStationID + "," + order.ToStationID
                            + "," + UserInfo.EmpID + ",sysdatetime(),'" + order.txtRef + "',null," + order.ItemCategory + "," + mDeptOrderid + ")";
                    }
                    else
                    {
                        StrSql = "Insert into departmentalissues (ID,StationSlNo,StationId,deptid, "
                            + " OperatorID,DateTime,RefNo,IssueDate,Categoryid,orderid) Values("
                            + " " + mOrderID + "," + mstOrderID + "," + UserInfo.selectedStationID + "," + order.ToStationID
                            + "," + UserInfo.EmpID + ",sysdatetime(),'" + order.txtRef + "','" + MainFunction.DateFormat(order.lbldate, "dd", "MMM", "yyyy")
                            + "'," + order.ItemCategory + "," + mDeptOrderid + ")";
                    }

                    bool Insert = MainFunction.SSqlExcuite(StrSql, Trans);
                    int ToDeptId = order.ToStationID;

                                         long mTransId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, mOrderID, mstOrderID, 1, 2, ToDeptId);

                    if (mTransId == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Can't Update the main Indent Order.";
                        return order;
                    }


                    int count = 1;
                    foreach (var item in order.arrItem)
                    {

                        StrSql = "Insert into deptissDetail (deptissid,Quantity,ServiceID,BatchNo,BatchId,unitid,Price,"
                             + " deleted,slno,epr,expirydate) Values("
                             + " " + mOrderID + "," + item.Qty + "," + item.ID + ",'" + item.BatchNo + "'," + item.Batchid + ","
                             + item.UnitId + "," + item.Cost + ",0," + count + "," + item.EPR + ",'" + item.ExpDt + "')";
                        bool inseDtl = MainFunction.SSqlExcuite(StrSql, Trans);


                        StrSql = "Update  BatchStore Set Quantity=Quantity-" + item.DedQty + " where ItemId=" + item.ID
                            + " and BatchId=" + item.Batchid + " and batchno='" + item.BatchNo + "' "
                            + " and StationId=" + UserInfo.selectedStationID;

                        bool UpdBatchDtl = MainFunction.SSqlExcuite(StrSql, Trans);

                        if (MainFunction.MazValidateBatchStoreQty(item.ID, int.Parse(item.Batchid), UserInfo.selectedStationID, Trans) == false)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            order.ErrMsg = "Can't Issue Stock for some Item!";
                            return order;

                        }



                        StrSql = "select costprice,sellingprice from batch where batchid = " + item.Batchid;
                        DataSet nx = MainFunction.SDataSet(StrSql, "tbl", Trans);
                        if (nx.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rr in nx.Tables[0].Rows)
                            {
                                if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, item.ID, item.DedQty, item.BatchNo, item.Batchid, (decimal)rr["costprice"],
                                    (decimal)rr["sellingprice"], (decimal)rr["costprice"]) == false)
                                {
                                    if (Con.State == ConnectionState.Open)
                                    {
                                        Con.BeginTransaction().Rollback();
                                        Con.Close();
                                    }
                                    order.ErrMsg = "Can't Update the main Indent Order.";
                                    return order;

                                }

                            }
                        }
                        else
                        {
                            if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, item.ID, item.DedQty, item.BatchNo, item.Batchid,
                                item.Cost, item.Price, item.EPR) == false)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                order.ErrMsg = "Can't Update the main Indent Order.";
                                return order;

                            }

                        }


                        count += 1;

                    } 
                    order.ErrMsg = "Issue Number " + intstSlNo + "  saved sucessfully ";



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

        public static bool InitBatchArray(ref IndentOrderModel pt, int gStationId, SqlTransaction Trans = null)
        {
            try
            {
                List<arrItem> arrItem = new List<arrItem>();


                decimal mQuantity = 0;
                int lGetQty = 0;
                int lItemid = 0;
                double IssueQty = 0;

                for (int i = 0; i < pt.IssueList.Count; i++)
                {
                    lGetQty = MainFunction.GetQuantity((int?)pt.IssueList[i].IssueUnitID, (int)pt.IssueList[i].ItemID);
                    IssueQty = (double)pt.IssueList[i].QtyIssue;
                    mQuantity = (decimal)IssueQty * lGetQty;


                    if (IssueQty > 0)
                    {
                        string SS = "Select B.BatchNo,b.batchid,B.ItemId as ID,c.Quantity,B.costprice,expirydate "
                            + ",b.MRP "
                        + " from batch B,batchstore c "
                        + "  where b.itemid=c.itemid and b.batchid=c.batchid and b.batchno=c.batchno "
                        + " and c.stationid=" + gStationId + " and c.Quantity > 0 and b.itemid=" + pt.IssueList[i].ItemID
                        + " order by b.expirydate,B.startDate";
                        DataSet bt = MainFunction.SDataSet(SS, "tbl", Trans);
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
                                One.ID = (int)rr["ID"];
                                One.BatchNo = (string)rr["BatchNo"];
                                One.Batchid = rr["batchid"].ToString();
                                One.Qty = (decimal)(mQuantity / lGetQty);
                                One.DedQty = (int)(decimal)mQuantity;
                                One.Price = (decimal)rr["MRP"] * lGetQty;
                                One.Cost = (decimal)rr["costprice"] * lGetQty;
                                One.EPR = (decimal)rr["costprice"] * lGetQty;
                                One.UnitId = pt.IssueList[i].IssueUnitID;
                                One.ExpDt = rr["expirydate"].ToString();

                                arrItem.Add(One);
                                
                                goto _ExitBatchLoop;
                            }
                            else
                            {
                                arrItem One = new arrItem();
                                mQuantity = mQuantity - (int)rr["Quantity"];
                                One.ID = (int)rr["ID"];
                                One.BatchNo = (string)rr["BatchNo"];
                                One.Batchid = rr["batchid"].ToString();
                                One.Qty = (decimal)((decimal)(int)rr["Quantity"] / lGetQty);
                                One.Price = (decimal)rr["MRP"] * lGetQty;
                                One.Cost = (decimal)rr["costprice"] * lGetQty;
                                One.EPR = (decimal)rr["costprice"] * lGetQty;
                                One.DedQty = (int)rr["Quantity"];
                                One.UnitId = pt.IssueList[i].IssueUnitID;
                                One.ExpDt = rr["expirydate"].ToString();
                                One.SubId = pt.IssueList[i].substituteid;
                                arrItem.Add(One);
                            }





                        _MoVe_NextBatCH: ;
                        }

                    _ExitBatchLoop: ;

                    }                     lItemid = pt.IssueList[i].ItemID;
                } 
                pt.arrItem = arrItem;

                return true;
            }
            catch (Exception e)
            {
                pt.ErrMsg = "Error when try to read the Item Batch";
                return false;
            }


        }

        public static IndentOrderModel PrintOut(int OrderID, int gStationid)
        {
            IndentOrderModel ll = new IndentOrderModel();
            try
            {
                string StrSql = " Select s.Name as dept,i.categoryid,i.deptid,O.Name as Operator,"
                       + " i.stationslno,I.RefNO,I.DateTime as [DateTime],i.issuedate "
                       + " from departmentalissues i,employee O,department s "
                       + " where O.ID=I.OperatorId and s.Id=I.deptid and i.stationid=" + gStationid
                       + " and I.Id=" + OrderID;

                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.OrderID = OrderID;
                    ll.lbldate = MainFunction.DateFormat(rr["DateTime"].ToString(), "yyyy", "MMM", "dd");
                    ll.ToStationID = (int)rr["deptid"];
                    ll.ItemCategory = (int)rr["categoryid"];
                    ll.txtRef = rr["RefNO"].ToString();
                    ll.lbloperator = rr["Operator"].ToString();
                    ll.IssSlNO = (int)rr["stationslno"];
                    ll.dtpBydate = MainFunction.DateFormat(rr["issuedate"].ToString(), "yyyy", "MMM", "dd"); 
                    ll.ToStationName = MainFunction.GetName("select name from department where id=" + ll.ToStationID, "name");
                    ll.ToStationName = ll.ToStationName.Substring(0, 3) + "---" + ll.ToStationName;
                                         ll.CurrentStationName = MainFunction.GetName("select name from itemgroup where id=" + ll.ItemCategory, "name");

                }


                
                StrSql = "select e.datetime,a.name as itemname,b.name as Category,sum(c.quantity) as quantity"
                    + " ,c.price as costprice,max(c.expirydate) as expirydate,p.name as pack,e.issuedate,"
                    + " b.name as cat,c.unitid,e.stationslno ,a.id as itemid, a.itemcode "
                    + " from item a,itemgroup b,deptissdetail c,departmentalissues e,packing p "
                    + " where c.deptissid=e.id and b.id=e.categoryid and p.id=c.unitid and c.serviceid=a.id "
                    + " and e.stationid= " + gStationid + " and  e.id=" + OrderID
                    + " group by e.stationSlNo,e.IssueDate,p.Name,b.name,a.unitid,a.name, "
                    + " a.drugtype,a.conversionqty,c.slno,a.itemcode,c.price,e.datetime,a.id,c.unitid order by c.slno";
                DataSet items = MainFunction.SDataSet(StrSql, "tbl2");
                List<IndentIssueInsertedItemList> IssueList = new List<IndentIssueInsertedItemList>();
                int sn = 1;
                foreach (DataRow rr in items.Tables[0].Rows)
                {
                    IndentIssueInsertedItemList xx = new IndentIssueInsertedItemList();
                    xx.SNO = sn;
                    xx.Item = rr["itemcode"].ToString() + "      " + rr["itemname"].ToString();
                    xx.QtyIssue = double.Parse(rr["quantity"].ToString());
                    xx.QtyIssue = Math.Round(xx.QtyIssue, 3);
                    xx.IssueUnits = rr["pack"].ToString();
                    xx.Price = (Single)rr["costprice"];
                    xx.Price = (Single)Math.Round(xx.Price, 3);
                    xx.Amount = (Single)(double)xx.QtyIssue * xx.Price;
                    xx.Amount = (Single)Math.Round(xx.Amount, 3);

                    IssueList.Add(xx);
                    sn += 1;
                }
                ll.IssueList = IssueList;

                return ll;
            }
            catch (Exception e) { return ll; };
        }

    }

}





