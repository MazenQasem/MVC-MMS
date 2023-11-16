using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IndentIssueFun
    {
        public static IndentOrderModel ClearIndent()
        {
            IndentOrderModel nn = new IndentOrderModel();
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
        public static List<IndentIssueView> GetIndents(ParamTable pm)
        {
            List<IndentIssueView> ll = new List<IndentIssueView>();
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

                string StrSql = "select a.name as sourcename,b.name,c.id,substring(a.name,1,3) + CAST(c.stationslno AS VARCHAR(100)) AS stationslno,"
                              + " c.[datetime],c.deliverydate,c.referenceno,c.status,C.SOURCEID,c.CategoryId,"
                              + " isnull(c.iscocktail,0) as iscocktail  "
                              + " from station a,employee b,indent c where c.sourceid=a.id and  "
                              + " c.operatorid =b.id and c.status =0  and c.destinationid=" + pm.gStationid + " "
                              + " and c.sourceid= " + pm.stationid + "  "
                              + " and c.datetime>='" + pm.Sdate + "' and c.datetime<'" + pm.Edate + "' "
                              + " order by c.stationslno";






                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentIssueView iv = new IndentIssueView();
                    iv.IndentNo = (string)rr["stationslno"];
                    iv.IndentFrom = (string)rr["sourcename"];
                    iv.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    iv.DeliveryBy = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    iv.SentBy = (string)rr["name"];

                    iv.RefNo = (string)rr["referenceno"];
                                         iv.IndentTxtRef = (string)rr["referenceno"];

                    iv.categoryID = (int)rr["CategoryId"];
                    iv.status = (byte)rr["status"];
                    iv.IndentId = (int)rr["id"];
                    iv.isCocktail = (byte)rr["iscocktail"];
                    iv.sourceid = (int)rr["SOURCEID"];
                    iv.IndentIssueID = 0;
                    iv.QuerySeq = 1;
                    ll.Add(iv);
                }


                StrSql = "select a.name as sourcename,b.name,c.id,c.recstatus,substring(a.name,1,3) + CAST(c.stationslno AS VARCHAR(100)) AS stationslno"
                        + ",c.[datetime],c.deliverydate,c.referenceno,c.status,c.sourceid,d.datetime as issdate,e.name as issname, c.categoryid,isnull(c.iscocktail,0)  iscocktail "
                        + " ,cast(d.stationslno as varchar(100)) as stationno, d.id as IssueID "
                        + " from station a,employee b,indent c,INDENTISSUE D ,employee e where c.sourceid=a.id and"
                        + " c.operatorid =b.id and d.operatorid=e.id and C.ID=D.indentID AND c.status =1 and "
                        + " c.datetime>='" + pm.Sdate + "' and c.datetime<'" + pm.Edate + "' AND "                           + "  c.destinationid=" + pm.gStationid + " and c.sourceid= " + pm.stationid + " order by c.stationslno ";
                n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentIssueView iv = new IndentIssueView();
                    iv.IndentNo = (string)rr["stationslno"];
                    iv.IndentFrom = (string)rr["sourcename"];
                    iv.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    iv.DeliveryBy = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    iv.SentBy = (string)rr["name"];
                    iv.IssuedBy = (string)rr["issname"];
                    iv.IssueDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["issdate"]);
                    iv.RefNo = (string)rr["stationno"];
                    iv.IndentTxtRef = (string)rr["stationno"];
                    iv.categoryID = (int)rr["categoryid"];
                    iv.status = (byte)rr["status"];
                    iv.IndentId = (int)rr["id"];
                    iv.isCocktail = (byte)rr["iscocktail"];
                    iv.sourceid = (int)rr["sourceid"];
                    iv.recstatus = (byte)rr["recstatus"];
                    iv.IndentIssueID = (int)rr["IssueID"];
                    iv.QuerySeq = 2;
                    ll.Add(iv);
                }

                StrSql = "select distinct a.name as sourcename,b.name,c.id,c.recstatus,substring(a.name,1,3) + cast(c.stationslno as varchar(100)) as stationslno,"
                    + " c.[datetime],c.deliverydate,c.referenceno,c.status, "
                    + " (select max(datetime) from indentissue z where z.indentid=d.indentid) as issdate, e.name as issname,"
                    + " cast( (select max(stationslno) from indentissue z1 where z1.indentid=d.indentid) as varchar(100)) as stationno,"
                    + " a.id as sourceid,isnull(c.iscocktail,0) iscocktail,d.Id as IssueID "
                    + " from station a,employee b,indent c,INDENTISSUE D ,employee e where c.sourceid=a.id and"
                    + " c.operatorid =b.id and C.ID=D.indentID AND (c.status =2 or c.recstatus>0) and e.id=d.operatorid "
                    + " and c.destinationid=" + pm.gStationid + " and c.sourceid= " + pm.stationid
                    + " AND c.datetime>='" + pm.Sdate + "' and c.datetime<'" + pm.Edate + "'  "                       + "  and datediff(d,c.DateTime,getdate())<=60 order by c.stationslno";
                n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentIssueView iv = new IndentIssueView();
                    iv.IndentNo = (string)rr["stationslno"];
                    iv.IndentFrom = (string)rr["sourcename"];
                    iv.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    iv.DeliveryBy = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    iv.SentBy = (string)rr["name"];
                    iv.IssuedBy = (string)rr["issname"];
                    iv.IssueDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["issdate"]);

                    iv.RefNo = (string)rr["stationno"];
                    iv.IndentTxtRef = (string)rr["stationno"];
                                         iv.status = (byte)rr["status"];
                    iv.IndentId = (int)rr["id"];
                    iv.isCocktail = (byte)rr["iscocktail"];
                    iv.sourceid = (int)rr["sourceid"];
                    iv.recstatus = (byte)rr["recstatus"];
                    iv.IndentIssueID = (int)rr["IssueID"];
                    iv.QuerySeq = 3;
                    ll.Add(iv);
                }


                List<IndentIssueView> yy = ll.OrderByDescending(o => o.status).OrderByDescending(o => o.IndentNo).ToList();

                return yy;
            }
            catch (Exception e) { return ll; };

        }
        public static IndentOrderModel ViewDetails(int orderID, int gStationid, int status)
        {
            IndentOrderModel IndDetail = new IndentOrderModel();
            List<IndentInsertedItemList> ll = new List<IndentInsertedItemList>();

            try
            {
                if (status == 0)
                {

                    string StrSql = " Select b.unitid as ordunitid,a.id as itemID,a.name as item,isnull(a.qoh,0) qoh,a.unitid,"
                    + " a.conversionqty,b.quantity as qtyord,c.name as unit,d.name as ordUnit,a.conversionqty,a.itemcode  "
                    + " from MMS_ItemMaster a,indentdetail b,packing c,packing d "
                    + " where d.id=b.unitid and b.itemid=a.id and a.unitid=c.id and b.indentid= " + orderID
                    + " and a.stationid= " + gStationid
                    + " order by slno ";
                    DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                    int sno = 1;
                    foreach (DataRow rr in n.Tables[0].Rows)
                    {
                        IndentInsertedItemList dd = new IndentInsertedItemList();
                        dd.SNO = sno;
                        dd.ID = (int)rr["itemID"];
                        dd.Name = (string)rr["itemcode"] + "   " + (string)rr["item"];
                        dd.conversionqty = MainFunction.GetQuantity((int)rr["ordunitid"], (int)rr["itemID"]);
                        int nnn = (int)rr["qoh"] / ((dd.conversionqty == 0) ? 1 : dd.conversionqty);
                        dd.UnitName = nnn.ToString() + " -- " + (string)rr["ordUnit"];
                        dd.Remarks = rr["qtyord"].ToString() + " -- " + (string)rr["ordUnit"];
                        sno += 1;
                        ll.Add(dd);
                    }

                }




                List<IndentIssueInsertedItemList> ss = new List<IndentIssueInsertedItemList>();
                if (status == 0)                 {
                                         string StrSql = "Select b.unitid as ordunitid,a.id as itemID, sum(e.quantity) as batchqty,a.name as item,a.qoh, "
                            + " a.unitid,a.conversionqty,b.quantity as qtyord,c.name as unit,isnull(a.Minlevel,0) MinLevel, "
                            + " d.name as ordUnit,b.slno,a.itemcode "
                            + " from mms_itemmaster a,indentdetail b,batch e,packing c, "
                            + " packing d where b.itemid = e.itemid and e.quantity > 0 and d.id=b.unitid "
                            + " and b.itemid=a.id and a.unitid=c.id and "
                            + " b.indentid=" + orderID + " and stationid=" + gStationid
                            + " group by b.unitid,a.id,a.name,a.minlevel,a.qoh,a.unitid,a.conversionqty,b.quantity,c.name,d.name,b.slno,a.itemcode order by b.slno";
                    DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                    int sno = 1;
                    foreach (DataRow rr in n.Tables[0].Rows)
                    {
                        IndentIssueInsertedItemList dd = new IndentIssueInsertedItemList();
                        int Conversionqty = MainFunction.GetQuantity((int)rr["ordunitid"], (int)rr["itemID"]);
                        double lordQty = (double)((int)rr["qoh"] / Conversionqty > (double)rr["qtyord"] ? (double)rr["qtyord"] : (int)rr["qoh"] / Conversionqty);
                        double TotQty = (double)((int)rr["qoh"] / Conversionqty > (int)rr["qoh"] ? (int)rr["qoh"] : (int)rr["qoh"] / Conversionqty);
                        double lminlevel = 0;
                        if ((int)rr["MinLevel"] > 0)
                        {
                            lminlevel = (int)rr["MinLevel"] / Conversionqty;
                        }
                        else
                        {
                            lminlevel = 0;
                        }
                        if ((int)rr["qoh"] > 0)
                        {
                            dd.SNO = sno;
                            dd.Item = (string)rr["itemcode"] + "    " + (string)rr["item"];
                            dd.QOH = TotQty;
                            dd.QtyIssue = lordQty;
                            dd.IssueUnits = (string)rr["ordUnit"];
                            dd.ItemID = (int)rr["itemID"];
                            dd.substituteid = (int)rr["itemID"];
                            dd.DrugType = 0;
                            dd.lstsub = 0;
                            dd.IssueUnitID = (int)rr["ordunitid"];
                            dd.conversionqty = Conversionqty;
                            dd.OrderQty = (int)(double)rr["qtyord"];
                            dd.IndentUnitID = (int)rr["ordunitid"];
                            dd.batchQty = (int)rr["batchqty"];
                            sno += 1;
                            ss.Add(dd);
                        }
                    }




                }
                else if (status == 1 || status == 2)
                {

                    double lordQty = 0;
                    string StrSql = "Select b.unitid as ordunitid,(a.id) as itemID,a.name as item,isnull(a.qoh,0) qoh,a.unitid"
                                  + ",a.conversionqty,b.quantity as qtyord,c.name as unit,d.name as ordUnit,"
                                  + " a.conversionqty,a.itemcode "
                                  + " from mms_itemMaster a, indentdetail b,packing c,packing d "
                                  + " where d.id=b.unitid and b.itemid=a.id and a.unitid=c.id "
                                  + " and b.indentid=" + orderID + " and stationid= " + gStationid
                                  + " order by slno";

                    DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                    int sno = 1;
                    foreach (DataRow rr in n.Tables[0].Rows)
                    {
                        int Conversionqty = MainFunction.GetQuantity((int)rr["ordunitid"], (int)rr["itemID"]);
                        IndentInsertedItemList dd = new IndentInsertedItemList();
                        dd.SNO = sno;
                        dd.ID = (int)rr["itemID"];
                        dd.Name = (string)rr["itemcode"] + "   " + (string)rr["item"];

                        dd.conversionqty = Conversionqty;
                        double nnn = (int)rr["qoh"] / ((dd.conversionqty == 0) ? 1 : dd.conversionqty);
                        dd.UnitName = nnn.ToString() + " -- " + (string)rr["ordUnit"];

                        lordQty = (double)((int)rr["qoh"] > (double)rr["qtyord"] ? (double)rr["qtyord"] : (int)rr["qoh"]);
                        dd.Remarks = lordQty.ToString() + " -- " + (string)rr["ordUnit"];
                        sno += 1;
                        ll.Add(dd);

                    }

                    if (dispDetail(orderID, gStationid, ref ss, status, lordQty) == true) { }
                }
                else
                {
                    if (dispDetail(orderID, gStationid, ref ss) == true) { }
                }
                IndDetail.SelectedItem = ll;
                IndDetail.IssueList = ss;
                return IndDetail;
            }
            catch (Exception e) { IndDetail.ErrMsg = "Error when try to load details: " + e.Message; return IndDetail; }


        }
        public static bool dispDetail(int Orderid, int gstation, ref List<IndentIssueInsertedItemList> ss, int MSTATUS = 2, double LOrderQty = 0)
        {
            try
            {
                int intId = 0;
                int sno = 1;
                int Countxx = 0;
                string StrSql = "select e.id as id,x.conversionqty as subconv,a.conversionqty,d.itemid,d.expirydate,"
                    + " d.substituteid,a.name as ord,b.quantity as ordQty,u.name as unitord,"
                    + " x.name as iss,sum(isnull(d.quantity,0)) as issQty,y.name as unitiss,b.unitid as indentunitid,"
                    + " d.unitid as indentissueunitid,d.batchno,a.itemcode,d.sqno from"
                    + " item a,mms_itemmaster x,indentdetail b,indent c,indentissuedetail d,indentissue e,packing u,"
                    + " packing y where"
                    + " a.id=b.itemid and c.id=b.indentid and x.id=d.SUBSTITUTEid and d.issueid=e.id and "
                    + " c.id = e.indentid And b.unitid=u.id and d.unitid=y.id and b.itemid=d.itemid "
                    + " and c.id= " + Orderid + " and x.stationid=" + gstation
                    + " group by a.name,x.name,"
                    + " b.quantity,u.name,y.name,d.itemid,d.substituteid ,a.conversionqty,"
                    + " x.conversionqty,d.expirydate,b.unitid,d.unitid,d.batchno,a.itemcode,e.id,d.sqno order by d.sqno ";
                DataSet nn = MainFunction.SDataSet(StrSql, "tbl");

                if (nn.Tables[0].Rows.Count > 0)
                {
                    StrSql = "select unitid from indentissuedetail where issueid = " + nn.Tables[0].Rows[0]["id"]
                    + " and itemid = " + nn.Tables[0].Rows[0]["itemid"];
                    DataSet xx = MainFunction.SDataSet(StrSql, "tbl");
                    if (xx.Tables[0].Rows.Count > 0)
                    {
                        Countxx = xx.Tables[0].Rows.Count;
                    }
                }





                foreach (DataRow rr in nn.Tables[0].Rows)
                {

                    if (Countxx > 0)
                    {
                        if (MSTATUS == 1)
                        {
                            IndentIssueInsertedItemList dd = new IndentIssueInsertedItemList();
                            dd.SNO = sno;
                            dd.Item = (string)rr["itemcode"] + "    " + (string)rr["iss"];
                            dd.Batch = (string)rr["batchno"];
                            dd.Expiry = rr["expirydate"].ToString();
                            dd.QOH = LOrderQty;
                            dd.QtyIssue = (double)rr["issQty"];
                            dd.IssueUnits = (string)rr["unitiss"];
                            dd.ItemID = (int)rr["itemid"];
                            dd.substituteid = (int)rr["substituteid"];
                                                                                      dd.IssueUnitID = (int)rr["indentissueunitid"];
                                                         dd.PrevQty = (int)(double)rr["issQty"];
                            dd.OrderQty = (int)(double)rr["ordQty"];
                                                         dd.IndentUnitID = (int)rr["indentunitid"];
                            sno += 1;
                            ss.Add(dd);
                        }
                        else
                        {
                            IndentIssueInsertedItemList dd = new IndentIssueInsertedItemList();
                            dd.SNO = sno;
                            dd.Item = (string)rr["itemcode"] + "    " + (string)rr["iss"];
                            dd.Batch = (string)rr["batchno"];
                            dd.Expiry = rr["expirydate"].ToString();
                                                         dd.QtyIssue = (double)rr["issQty"];
                            dd.IssueUnits = (string)rr["unitiss"];
                            dd.ItemID = (int)rr["itemid"];
                            dd.substituteid = (int)rr["substituteid"];
                                                                                      dd.IssueUnitID = (int)rr["indentissueunitid"];
                                                                                      dd.OrderQty = (int)(double)rr["ordQty"];
                                                         dd.IndentUnitID = (int)rr["indentunitid"];
                            sno += 1;
                            ss.Add(dd);
                        }




                    }
                    else
                    {
                        if (MSTATUS == 1)
                        {
                            IndentIssueInsertedItemList dd = new IndentIssueInsertedItemList();
                            dd.SNO = sno;
                            dd.Item = (string)rr["itemcode"] + "    " + (string)rr["iss"];
                            dd.Expiry = rr["expirydate"].ToString();
                            dd.QOH = LOrderQty;
                            dd.IssueUnits = (string)rr["unitiss"];

                            dd.ItemID = (int)rr["itemid"];
                            dd.substituteid = (int)rr["substituteid"];

                            dd.IssueUnitID = (int)rr["indentunitid"];
                            dd.PrevQty = (int)(double)rr["issQty"];
                            dd.OrderQty = (int)(double)rr["ordQty"];

                            dd.IndentUnitID = (int)rr["indentunitid"];
                            sno += 1;
                            ss.Add(dd);
                        }
                        else
                        {
                            IndentIssueInsertedItemList dd = new IndentIssueInsertedItemList();
                            dd.SNO = sno;
                            dd.Item = (string)rr["itemcode"] + "    " + (string)rr["iss"];
                            dd.Expiry = rr["expirydate"].ToString();
                            dd.QtyIssue = (double)rr["issQty"];
                            dd.IssueUnits = (string)rr["unitiss"];

                            dd.ItemID = (int)rr["itemid"];
                            dd.substituteid = (int)rr["substituteid"];

                            dd.IssueUnitID = (int)rr["indentunitid"];

                            dd.OrderQty = (int)(double)rr["ordQty"];

                            dd.IndentUnitID = (int)rr["indentunitid"];
                            sno += 1;
                            ss.Add(dd);
                        }
                    }




                }

                return true;

            }
            catch (Exception e)
            {
                return false;
            }

        }
        public static List<IndentIssueInsertedItemList> InsertItem(int ItemId, int Gstationid, List<IndentIssueInsertedItemList> ExistList = null, int IndentID = 0, int Generic = 0)
        {
            List<IndentIssueInsertedItemList> ll = new List<IndentIssueInsertedItemList>();
            IndentIssueInsertedItemList FakeToHoldErr = new IndentIssueInsertedItemList();
            try
            {
                if (Generic == 0)
                {
                    string Str = " Select i.ID,i.itemcode ,i.Name,i.unitid,isnull(I.QOH,0) as qoh,i.conversionqty,i.drugtype, "
                              + " u.name as unit ,cast(ind.quantity as int) as quantity "
                              + " from mms_itemmaster i,packing u, IndentDetail ind  "
                              + " where ind.ItemId = i.Id and i.unitid=u.id "
                              + " and i.id=" + ItemId + "  and stationid=" + Gstationid
                              + " and ind.indentid=" + IndentID;
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
                            l.Item = (string)rr["itemcode"] + "    " + (string)rr["Name"];
                                                                                      l.QOH = (int)rr["qoh"];
                                                         l.IssueUnits = (string)rr["unit"];
                            l.ItemID = (int)rr["ID"];
                            l.DrugType = 0;
                                                         l.IssueUnitID = (int)rr["unitid"];
                            l.conversionqty = (int)rr["conversionqty"];
                                                                                                                                                                                                           
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
                }
                else if (Generic > 0)
                {

                    string Str = " Select i.ID,i.itemcode,i.Name,i.unitid,i.qoh,i.minlevel,i.conversionqty,"
                               + " i.drugtype,u.name as unit "
                               + " from mms_itemmaster i,packing u "
                               + " where i.unitid=u.id and i.id=" + Generic + " and i.stationid=" + Gstationid;

                    DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int sno = 0;
                        int lminlevel = 0;
                        int dblQohConQty = 0;
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
                            if ((int)rr["qoh"] > 0)
                            {
                                if ((int)rr["minlevel"] > 0)
                                {
                                    lminlevel = (int)rr["minlevel"] / (int)rr["conversionqty"];
                                }
                                else
                                {
                                    lminlevel = 0;
                                }
                                dblQohConQty = (int)rr["qoh"] / (int)rr["conversionqty"];
                                IndentIssueInsertedItemList l = new IndentIssueInsertedItemList();
                                l.SNO = sno;
                                l.Item = (string)rr["itemcode"] + "    " + (string)rr["Name"];
                                                                                                  l.QOH = dblQohConQty;
                                                                 l.IssueUnits = (string)rr["unit"];
                                l.ItemID = (int)rr["ID"];                                 l.DrugType = 0;
                                l.lstsub = ItemId;                                  l.IssueUnitID = (int)rr["unitid"];
                                l.conversionqty = (int)rr["conversionqty"];
                                                                                                                                                                                                                                      l.MinLevel = lminlevel;

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
                            else
                            {
                                if (ll == null)
                                {
                                    FakeToHoldErr.ErrMsg = "Item not mapped to this Station";
                                    ll.Add(FakeToHoldErr);
                                }
                                else
                                {
                                    ll[0].ErrMsg = "Cannot issue when quantity is zero.";
                                }

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
                orderUnitID = lst.IndentUnitID;
                lSmallQty = lconvqty * lst.QOH;
                orderQty = lst.OrderQty;
                conversionqty = lst.conversionqty;
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


                    
                    int MStatus = 0;
                    int lGetQty = 0;
                    double qtySum = 0;
                    double lprevqty = 0;
                    string StrSql = "Select b.unitid as ordunitid,a.id as itemID,a.name as item,a.qoh,a.unitid,a.conversionqty,"
                             + " b.quantity as qtyord,c.name as unit,d.name as ordUnit"
                             + " from mms_itemmaster a,indentdetail b,packing c,packing d "
                             + " where d.id=b.unitid and b.itemid=a.id and a.unitid=c.id and b.indentid=" + order.OrderID
                             + " and a.stationid=" + UserInfo.selectedStationID + " order by slno";
                    DataSet sn = MainFunction.SDataSet(StrSql, "tbl");
                    foreach (DataRow rr in sn.Tables[0].Rows)
                    {
                        foreach (var nn in order.IssueList)
                        {
                            if (nn.substituteid == (int)rr["itemID"])
                            {
                                lGetQty = MainFunction.GetQuantity(nn.IssueUnitID, nn.ItemID);
                                qtySum += (nn.QtyIssue * (lGetQty == 0 ? 1 : lGetQty));
                                lprevqty = nn.PrevQty * MainFunction.GetQuantity(nn.PrevUnitID, nn.ItemID);
                            }
                            if ((qtySum + lprevqty) < ((int)(double)rr["qtyord"] * MainFunction.GetQuantity((int)rr["ordunitid"], (int)rr["itemID"])))
                            {
                                MStatus = 1;
                                break;
                            }
                        }
                        if (MStatus == 1) { break; }
                    }
                    if (MStatus == 0) { MStatus = 2; }


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



                    StrSql = "update indent set status=" + MStatus + ", recstatus = 0 where id=" + order.OrderID
                        + " and destinationid = " + UserInfo.selectedStationID + " and Status = 0 or status = 1";
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

                    StrSql = "Update invmax set maxid=maxid+1 where slno=5 and stationid=0";
                    bool UpdInvmax = MainFunction.SSqlExcuite(StrSql, Trans);
                    int intId = 0;


                    StrSql = "select maxid from invmax where slno=5 and stationid=0";
                    DataSet ds1 = MainFunction.SDataSet(StrSql, "tbl1", Trans);
                    foreach (DataRow rr in ds1.Tables[0].Rows)
                    {
                        intId = (int)rr["maxid"];
                    }

                    int stationslno = 0;
                    StrSql = "select max(stationslno) as stationslno from indentissue where sourceid=" + UserInfo.selectedStationID + "";
                    DataSet ds2 = MainFunction.SDataSet(StrSql, "tbl21", Trans);
                    foreach (DataRow rr in ds2.Tables[0].Rows)
                    {
                        stationslno = MainFunction.NullToInteger(rr["stationslno"].ToString()) + 1;

                    }

                    StrSql = "insert into indentissue (id,IndentID,datetime,OperatorID,status,RefNo,Sourceid,DESTINATIONID,stationslno) values"
                            + "(" + intId + "," + order.OrderID + ",sysdatetime(),"
                            + UserInfo.EmpID + ", 0 ,'" + order.txtRef + "'," + UserInfo.selectedStationID
                            + "," + order.ToStationID + "," + stationslno + ")";
                    bool inser = MainFunction.SSqlExcuite(StrSql, Trans);


                    StrSql = "update indentissue set categoryid=a.categoryid from indent a,indentissue b "
                    + "  where a.id=b.indentid and b.indentid= " + order.OrderID;
                    bool upd2 = MainFunction.SSqlExcuite(StrSql, Trans);

                                         long mTransId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, intId, stationslno, 1, 4, order.ToStationID);
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
                        if (string.IsNullOrEmpty(item.ExpDt) == true)
                        {
                            StrSql = "insert into indentissuedetail (IssueID,ItemID,Quantity,batchno,substituteId,UnitId,expirydate,BatchId,sqno,epr) values"
                                    + "(" + intId + "," + item.SubId + "," + item.Qty
                                    + ",'" + item.BatchNo + "'," + item.ID + "," + item.UnitId
                                    + ",null," + item.Batchid + "," + count + "," + item.Price + ")";
                        }
                        else
                        {
                            StrSql = "insert into indentissuedetail (IssueID,ItemID,Quantity,batchno,substituteId,UnitId,expirydate,BatchId,sqno,epr) values"
                                    + "(" + intId + "," + item.SubId + "," + item.Qty + ",'" + item.BatchNo + "'," + item.ID + ","
                                    + item.UnitId + ",'" + item.ExpDt + "'," + item.Batchid + "," + count + " + 1," + item.Price + ")";
                        }
                        bool inseDtl = MainFunction.SSqlExcuite(StrSql, Trans);


                        StrSql = "Update  BatchStore Set Quantity=Quantity-" + item.DedQty + " where ItemId=" + item.ID
                            + " and BatchId=" + item.Batchid + " and StationId=" + UserInfo.selectedStationID;
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
                            if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, item.ID, item.DedQty, item.BatchNo, item.Batchid) == false)
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


                    


                    StrSql = "select b.itemid,a.quantity,a.batchid,a.unitid,b.costprice from indentissuedetail a,batch b where "
                      + " a.issueid=" + intId + " And b.itemid = a.substituteid And b.batchid = a.batchid";
                    decimal amount = 0;
                    DataSet nxss = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    foreach (DataRow rr in nxss.Tables[0].Rows)
                    {
                        lGetQty = MainFunction.GetQuantity((int)rr["unitid"], (int)rr["itemid"]);

                        amount = amount + (decimal)((double)rr["quantity"] * (double)(lGetQty * (decimal)rr["costprice"]));


                    }

                    bool updamount = MainFunction.SSqlExcuite(" update indentissue set amount=" + amount + " where id=" + intId, Trans);

                    order.ErrMsg = "Issue Number " + stationslno + "  saved sucessfully ";



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
                                One.Price = (decimal)rr["costprice"] * lGetQty;
                                One.UnitId = pt.IssueList[i].IssueUnitID;
                                One.ExpDt = rr["expirydate"].ToString();
                                One.SubId = pt.IssueList[i].substituteid;
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
                                One.Price = (decimal)rr["costprice"] * lGetQty;
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




        public static IndentIssueView PrintOut(int OrderID, int gStationid)
        {
            IndentIssueView ll = new IndentIssueView();
            try
            {
                ll = GetIndentsHeader(OrderID, gStationid);
                 
                                 return ll;
            }
            catch (Exception e) { return ll; };
        }



        public static IndentIssueView GetIndentsHeader(int OrderID, int gStationid)
        {
            IndentIssueView ll = new IndentIssueView();
            try
            {
                string StrSql = "select a.name as sourcename,b.name,c.id,substring(a.name,1,3) + CAST(c.stationslno AS VARCHAR(100)) AS stationslno,"
                              + " c.[datetime],c.deliverydate,c.referenceno,c.status,C.SOURCEID,c.CategoryId,"
                              + " isnull(c.iscocktail,0) as iscocktail  "
                              + " from station a,employee b,indent c where c.sourceid=a.id and  "
                              + " c.operatorid =b.id and c.status =0  and c.destinationid=" + gStationid + " "
                              + " and c.id=" + OrderID
                              + " order by c.stationslno";

                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.IndentNo = (string)rr["stationslno"];
                    ll.IndentFrom = (string)rr["sourcename"];
                    ll.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    ll.DeliveryBy = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    ll.SentBy = (string)rr["name"];
                    ll.RefNo = (string)rr["referenceno"];
                    ll.categoryID = (int)rr["CategoryId"];
                    ll.status = (byte)rr["status"];
                    ll.IndentId = (int)rr["id"];
                    ll.isCocktail = (byte)rr["iscocktail"];
                    ll.sourceid = (int)rr["SOURCEID"];
                    ll.IndentIssueID = 0;
                    ll.QuerySeq = 1;
                    ll.IndentTxtRef = MainFunction.GetName("select name from ItemGroup where id=" + ll.categoryID, "name");
                }


                StrSql = "select a.name as sourcename,b.name,c.id,c.recstatus,substring(a.name,1,3) + CAST(c.stationslno AS VARCHAR(100)) AS stationslno"
                        + ",c.[datetime],c.deliverydate,"
                        + "c.referenceno,c.status,c.sourceid,d.datetime as issdate,e.name as issname, c.categoryid,isnull(c.iscocktail,0)  iscocktail "
                        + " ,cast(d.stationslno as varchar(100)) as stationno, d.id as IssueID "
                        + " from station a,employee b,indent c,INDENTISSUE D ,employee e where c.sourceid=a.id and"
                        + " c.operatorid =b.id and d.operatorid=e.id and C.ID=D.indentID AND c.status =1 and "
                        + "  c.destinationid=" + gStationid + " and c.id= " + OrderID + " order by c.stationslno ";
                n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.IndentNo = (string)rr["stationslno"];
                    ll.IndentFrom = (string)rr["sourcename"];
                    ll.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    ll.DeliveryBy = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    ll.SentBy = (string)rr["name"];
                    ll.IssuedBy = (string)rr["issname"];
                    ll.IssueDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["issdate"]);
                    ll.RefNo = (string)rr["stationno"];
                    ll.categoryID = (int)rr["categoryid"];
                    ll.status = (byte)rr["status"];
                    ll.IndentId = (int)rr["id"];
                    ll.isCocktail = (byte)rr["iscocktail"];
                    ll.sourceid = (int)rr["sourceid"];
                    ll.recstatus = (byte)rr["recstatus"];
                    ll.IndentTxtRef = MainFunction.GetName("select name from ItemGroup where id=" + ll.categoryID, "name");
                    ll.IndentIssueID = (int)rr["IssueID"];
                    ll.QuerySeq = 2;

                }

                StrSql = "select distinct a.name as sourcename,b.name,c.id,c.recstatus,substring(a.name,1,3) + cast(c.stationslno as varchar(100)) as stationslno,"
                    + " c.[datetime],c.deliverydate,c.referenceno,c.status, (select max(datetime) "
                    + " from indentissue z where z.indentid=d.indentid) as issdate, e.name as issname,"
                    + "cast( (select max(stationslno) from indentissue z1 where z1.indentid=d.indentid) as varchar(100)) as stationno,"
                    + " a.id as sourceid,isnull(c.iscocktail,0) iscocktail,d.categoryID, d.id as IssueID "
                    + " from station a,employee b,indent c,INDENTISSUE D ,employee e where c.sourceid=a.id and"
                    + " c.operatorid =b.id and C.ID=D.indentID AND (c.status =2 or c.recstatus>0) and e.id=d.operatorid "
                    + " and c.destinationid=" + gStationid + " and c.id= " + OrderID
                    + "  and datediff(d,c.DateTime,getdate())<=60 order by c.stationslno";
                n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {

                    ll.IndentNo = (string)rr["stationslno"];
                    ll.IndentFrom = (string)rr["sourcename"];
                    ll.IndentDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    ll.DeliveryBy = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    ll.SentBy = (string)rr["name"];
                    ll.IssuedBy = (string)rr["issname"];
                    ll.IssueDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["issdate"]);

                    ll.RefNo = (string)rr["stationno"];
                    ll.categoryID = (int)rr["categoryID"];
                    ll.status = (byte)rr["status"];
                    ll.IndentId = (int)rr["id"];
                    ll.isCocktail = (byte)rr["iscocktail"];
                    ll.sourceid = (int)rr["sourceid"];
                    ll.recstatus = (byte)rr["recstatus"];
                    ll.IndentTxtRef = MainFunction.GetName("select name from ItemGroup where id=" + ll.categoryID, "name");
                    ll.IndentIssueID = (int)rr["IssueID"];
                    ll.QuerySeq = 3;

                }

                ll.IndentFrom = ll.IndentFrom.Substring(0, 3) + "--" + ll.IndentFrom;
                ll.ItemList = GetIndentsDetail(ll.IndentIssueID, gStationid);
                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static List<IndentIssueInsertedItemList> GetIndentsDetail(int IssueID, int gStationid)
        {

            List<IndentIssueInsertedItemList> xx = new List<IndentIssueInsertedItemList>();
            try
            {
                string StrSql = "select b.sqno, m.datetime,a.itemcode,a.name,b.quantity,c.costprice,d.name isspack,C.expirydate,"
               + " b.unitid,a.id as itemid,m.DestinationID,m.CategoryID "
               + " from item a,indentissuedetail b,batch c,packing d,indentissue m "
               + " where a.id=b.substituteid and b.substituteid=c.itemid and m.id = b.issueid and b.batchid=c.batchid and b.unitid=d.id "
               + " and b.issueid=" + IssueID + " order by b.sqno";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentIssueInsertedItemList ll = new IndentIssueInsertedItemList();
                    int lqty = MainFunction.GetQuantity((int)rr["unitid"], (int)rr["itemid"]);
                    ll.SNO = (int)rr["sqno"];
                    ll.Item = (string)rr["itemcode"] + "    " + (string)rr["name"];
                    ll.QtyIssue = (double)rr["quantity"];
                    ll.IssueUnits = (string)rr["isspack"];
                    ll.Price = (Single)(decimal)rr["costprice"] * lqty;
                    ll.Amount = (Single)(decimal)rr["costprice"] * (Single)((double)rr["quantity"] * lqty);
                    xx.Add(ll);
                }

                return xx;
            }
            catch (Exception e) { return xx; };

        }

    } 
} 




