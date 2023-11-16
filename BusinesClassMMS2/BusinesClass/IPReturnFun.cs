using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IPReturnFun
    {
        public static IPReturnHeader FindPatient(int PIN, string StationName, string ISsAuth)
        {
            IPReturnHeader dd = new IPReturnHeader();
            List<IPReturnViewList> ll = new List<IPReturnViewList>();
            try
            {
                string StrSql = "select a.title + '. ' + a.firstname + ' ' + a.middlename + ' ' + a.lastname as ptname,"
                    + " b.id,b.stationslno,b.datetime,c.name,d.name as bedname,isnull(a.vip,0) as VIP "
                    + ", a.ipid ,a.doctorid,d.id as BedID"
                    + " from inpatient a,drugreturn b,employee c,bed d  "
                    + " where b.ipid=a.ipid and b.operatorid=c.id  and b.bedid=d.id "
                    + " and a.registrationno=" + PIN + " and a.issueauthoritycode='" + ISsAuth + "' order by b.id ";

                DataSet ds = MainFunction.SDataSet(StrSql.ToLower(), "tbl");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        IPReturnViewList xx = new IPReturnViewList();
                        xx.ReturnNo = StationName.Substring(0, 2) + " " + rr["stationslno"].ToString();
                        xx.DateTime = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "");
                        xx.Bed = rr["bedname"].ToString();
                        xx.Operator = rr["name"].ToString().Trim();
                        xx.VIP = (bool)rr["vip"];
                        xx.BedID = (int)rr["bedid"];
                        xx.ReturnID = (int)rr["id"];
                        xx.gIACode = ISsAuth;
                        xx.Pin = PIN;
                        xx.IPID = (int)rr["ipid"];
                        xx.PatientName = rr["ptname"].ToString();
                        xx.DoctorID = (int)rr["doctorid"];
                        dd.PatientName = xx.PatientName;

                        ll.Add(xx);
                    }
                    dd.ListView = ll;
                }
                else
                {
                    dd.ErrMsg = "Not Found";
                }
                return dd;
            }
            catch (Exception e) { return dd; }


        }
        public static IPReturnHeader ViewDetailsFun(int OrderID, int IPID)
        {
            IPReturnHeader ip = new IPReturnHeader();
            try
            {
                
                List<IPReturnDetail> rsRecordsetTable = new List<IPReturnDetail>();
                DataSet rsRecordset = MainFunction.SDataSet("Select B.Quantity as qty,B.serviceId,isnull(b.price,0) as price,b.orderid "
                    + "  from DrugReturnDetail B where B.OrderId = " + OrderID, "tbl");
                int Sno = 1;
                foreach (DataRow rr in rsRecordset.Tables[0].Rows)
                {
                    int lGetQty = 0;
                    decimal TotQty = 0;
                    decimal cprice = 0;
                    string ItemName = "";
                    int RsQty = 0;
                    
                    DataSet IssDataset = MainFunction.SDataSet("Select b.dispatchquantity as qty,b.unitid,b.substituteid "
                                        + " from drugorderdetailsubstitute B left join drugorder c on B.OrderId=c.id "
                                        + " where  c.ipid =" + IPID
                                        + " and b.substituteid=" + (int)rr["serviceId"], "tbl1");

                    foreach (DataRow yy in IssDataset.Tables[0].Rows)
                    {
                        lGetQty = MainFunction.GetQuantity((int)yy["unitid"], (int)rr["serviceId"]);
                        TotQty = TotQty + (decimal)yy["qty"] * lGetQty;
                        cprice = (decimal)rr["price"];
                    }
                    
                    DataSet rsRet = MainFunction.SDataSet(" Select b.serviceid,a.name,sum(b.quantity) as Qty "
                                                            + " from item a left join drugreturndetail b on a.id=b.serviceid  "
                                                            + " left join DrugReturn c on c.id=b.OrderId "
                                                            + " where c.IPID=  " + IPID
                                                            + " and  a.id= " + (int)rr["serviceId"]
                                                            + " group by b.serviceId,a.Name", "tbl");
                    
                    

               
                    
                    
                    foreach (DataRow xx in rsRet.Tables[0].Rows)
                    {
                        ItemName = xx["name"].ToString();
                        RsQty = (int)xx["Qty"];
                    }

                    IPReturnDetail rec = new IPReturnDetail();
                    rec.SNO = Sno;
                    rec.Item = ItemName;
                    rec.QtyIss = TotQty;
                    rec.UOM = "No";
                    rec.TotRet = (int)RsQty;
                    rec.Rtns = (int)(Int16)rr["qty"];
                    rec.Rate = Math.Round(cprice, 2);
                    rec.Amount = Math.Round((rec.Rtns * rec.Rate), 2);
                    rec.OrderID = (int)rr["orderid"];
                    ip.Total += rec.Amount;
                    rsRecordsetTable.Add(rec);
                    Sno++;
                }
                ip.Details = rsRecordsetTable;
                return ip;
            }
            catch (Exception e) { ip.ErrMsg = e.Message; return ip; }

        }

        public static IPReturnHeader GetNewPT(int IPID)
        {
            IPReturnHeader ll = new IPReturnHeader();
            try
            {
                string StrSql = "select a.title + '. ' + a.firstname + ' ' + a.middlename + ' ' + a.lastname as ptname,"
                    + "b.id,b.name,a.doctorid ,isnull(a.VIP,0) as VIP "
                    + " from inpatient a, bed b where   a.ipid=b.ipid and a.ipid=" + IPID;
                DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ll.PatientName = rr["ptname"].ToString();
                    ll.Bed = rr["name"].ToString();
                    ll.BedID = (int)rr["id"];
                    ll.DoctorID = (int)rr["doctorid"];
                    ll.VIP = (bool)rr["vip"];
                }

                return ll;
            }
            catch (Exception e) { ll.ErrMsg = e.Message; return ll; }
        }

        public static IPReturnHeader InsertItem(int ItemID, int IPID, int GstationID)
        {
            IPReturnHeader ll = new IPReturnHeader();
            try
            {
                int lGetQty = 0;
                decimal TotQty = 0;
                decimal cprice = 0;
                int totret = 0;
                string strBatch = "";
                int batchid = 0;
                int OrderDIx = 0;
                int SNO = 1;
                string UOM = "";
                int PackID = 0;

                string StrSql = "Select b.orderid,b.dispatchquantity as qty,isnull(b.batchno,'') as batchno,"
                      + " b.batchid,b.unitid,b.price,i.name as ItemName from  drugorderdetailSubstitute B,drugorder c ,Item I"
                      + " where B.OrderId=c.id and b.substituteid=i.id and b.substituteid=" + ItemID
                      + " and c.IPID =" + IPID
                      + " and c.dispatched=3 and c.Tostationid = " + GstationID
                      + " order by b.batchid";
                List<IPReturnDetail> allt = new List<IPReturnDetail>();
                DataSet ds = MainFunction.SDataSet(StrSql, "tbl");
                if (ds.Tables[0].Rows.Count == 0)
                {
                    ll.ErrMsg = "Quantity & Batch Details Not Available For This Drug";
                    return ll;
                }
                else
                {

                    foreach (DataRow rr in ds.Tables[0].Rows)
                    {
                        IPReturnDetail dtl = new IPReturnDetail();
                        lGetQty = MainFunction.GetQuantity((int)rr["unitid"], ItemID);
                        totret = 0;
                        TotQty = TotQty + (decimal)rr["qty"] * lGetQty;
                        cprice = (decimal)rr["price"] / lGetQty;
                        strBatch = rr["batchno"].ToString();
                        batchid = (int)rr["batchid"];
                        OrderDIx = (int)rr["orderid"];
                        
                        string StrSql1 = "select isnull(sum( a.quantity),0) as qty from  drugreturndetail a,Drugreturn b "
                             + " where  a.orderid=b.id and a.serviceid=" + ItemID
                             + " and b.tostationid = " + GstationID
                             + "  and a.batchid = " + batchid
                             + "  and a.drugorderid = " + OrderDIx
                             + " and b.ipid=" + IPID;

                        
               
                        DataSet rsTotRet = MainFunction.SDataSet(StrSql1, "tbl");
                        foreach (DataRow yy in rsTotRet.Tables[0].Rows)
                        {
                            totret = (int)yy["qty"];
                        }
                        
                        string StrSql2 = "select a.packid,b.name from itempacking a,packing b where a.packid=b.id and a.itemid=" + ItemID
                        + " and slno=(select max(slno) from itempacking where itemid=" + ItemID + ")";
                        DataSet rspack = MainFunction.SDataSet(StrSql2, "tbl");
                        foreach (DataRow xx in rspack.Tables[0].Rows)
                        {
                            UOM = xx["name"].ToString();
                            PackID = (int)xx["packid"];
                        }

                        dtl.SNO = SNO++;
                        dtl.Item = rr["ItemName"].ToString();
                        dtl.QtyIss = TotQty;
                        dtl.UOM = UOM;
                        dtl.TotRet = totret;
                        dtl.Rate = Math.Round(cprice, 2);
                        dtl.BatchNo = strBatch;
                        dtl.UnitID = PackID;
                        dtl.BatchID = batchid;
                        dtl.OrderID = OrderDIx;
                        dtl.ItemID = ItemID;

                        allt.Add(dtl);
                        TotQty = 0;

                    }


                }

                ll.Details = allt;


                return ll;
            }
            catch (Exception e) { ll.ErrMsg = e.Message; return ll; }


        }

        public static IPReturnHeader Save(IPReturnHeader order, User UserInfo)
        {
            try
            {
                SqlConnection Con = MainFunction.MainConn();
                using (Con)
                {
                    
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    int OrdType = 0;
                    long mTransId = 0;
                    int Gstationid = UserInfo.selectedStationID;
                    int gSavedById = UserInfo.EmpID;
                    int Pin = int.Parse(MainFunction.getRegNumber(order.RegNo));
                    if (order.CategoryID == 17) { OrdType = 1; } else { OrdType = 0; }

                    bool UpdateOrderMax = MainFunction.SSqlExcuite("update OrderMaxId set maxid=maxid+1 where tableid = 26 and stationid=0", Trans);
                    int intMaxId = MainFunction.GetOneVal("select MaxId from OrderMaxid where TABLEID = 26  and stationid=0", "MaxId", Trans);

                    bool UpdateOrderMax1 = MainFunction.SSqlExcuite("update OrderMaxID set maxid=maxid+1 where Tableid = 26 and stationid="
                        + Gstationid, Trans);

                    int mstOrderID = MainFunction.GetOneVal("Select maxid from OrderMaxId where TableId = 26 and stationid=" + Gstationid, "MaxId", Trans);

                    string StrSql = "insert into drugreturn (id,BedID,IPID,StationID,OperatorID,DateTime,stationslno,status,receivedby, "
                        + "   receiveddatetime,tostationid,OrderType,doctorid) values "
                        + "(" + intMaxId + "," + order.BedID + "," + order.IPID + "," + Gstationid + "," + gSavedById + ","
                        + "sysdatetime()," + mstOrderID + ",1," + gSavedById + ",getdate()," + Gstationid + "," + OrdType + "," + order.DoctorID + ")";

                    bool InsertDrugReturn = MainFunction.SSqlExcuite(StrSql, Trans);

                    mTransId = MainFunction.SaveInTranOrder(Trans, Gstationid, intMaxId, mstOrderID, 0, 7, 0, "", 0, Pin);
                    if (mTransId == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Can't Update the main TransTable Order.";
                        return order;
                    }

                    foreach (var itm in order.Details)
                    {
                        if (itm.Rtns > 0)
                        {
                            decimal CostPrice = 0;
                            decimal MRP = 0;
                            IPReturnDetail dtl = new IPReturnDetail();
                            DataSet costds = MainFunction.SDataSet("select isnull(costprice,0)  as costprice,isnull(mrp,0) as mrp from batch where batchid = " + itm.BatchID, "tbl", Trans);
                            foreach (DataRow xx in costds.Tables[0].Rows)
                            {
                                CostPrice = (decimal)xx["costprice"];
                                MRP = (decimal)xx["mrp"];
                            }

                            string StrSql2 = "insert into drugreturndetail (OrderID,ServiceID,Quantity,batchno,price,unitid,batchid,costprice,DrugOrderID) values"
                             + "(" + intMaxId + "," + itm.ItemID + "," + itm.Rtns + ",'" + itm.BatchNo + "',"
                             + itm.Rate + "," + itm.UnitID + "," + itm.BatchID + "," + CostPrice + "," + itm.OrderID + ")";
                            bool insertdtl = MainFunction.SSqlExcuite(StrSql2, Trans);

                            int intRet = itm.Rtns * MainFunction.GetQuantity(itm.UnitID, itm.ItemID);

                            string StrSql3 = "update batchstore set quantity= quantity+" + intRet + " where itemid=" + itm.ItemID
                                + " and batchno='" + itm.BatchNo + "' and stationid=" + Gstationid + " and batchid = " + itm.BatchID;

                            bool updateBatch = MainFunction.SSqlExcuite(StrSql3, Trans);



                            if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, itm.ItemID, intRet, itm.BatchNo, itm.BatchID.ToString(),
                                CostPrice, MRP, CostPrice) == false)
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


                    Trans.Commit();
                    order.ErrMsg = "IP Returns Have Been Recorded";
                    return order;

                }






                return order;
            }
            catch (Exception e) { order.ErrMsg = e.Message; return order; }


        }

    }

}





