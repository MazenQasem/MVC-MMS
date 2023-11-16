using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IndentOrderFun
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

                string StrSql = "Select a.id,a.StationSlNo,a.datetime,a.deliverydate,a.referenceno,a.status,b.name as "
                             + " destination,c.name,a.categoryid,isnull(a.sectionid,0) as sectionid,a.destinationid from indent a,station b,employee c where a.destinationid=b.id "
                             + " and a.operatorid=c.id  and a.status < 3 and a.sourceid=" + pm.gStationid + ""
                             + " and a.DateTime >= '" + pm.Sdate + "' and a.DateTime < '" + pm.Edate + "'"
                             + " order by a.Status,a.Id desc";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentView iv = new IndentView();
                    iv.IndentID = (int)rr["id"];
                    string st = MainFunction.GetName("select upper(substring(name,1,2)) as name from station where id=" + pm.gStationid, "name");
                    iv.StationSlNO = st + rr["StationSlNo"].ToString();
                    iv.IndentDateTime = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
                    iv.DeliveryDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    iv.referenceno = rr["referenceno"].ToString();
                    iv.Status = (Byte)rr["status"];
                    iv.IndentTo = rr["destination"].ToString();
                    iv.IndentByName = rr["name"].ToString();
                    iv.CategoryID = (int)rr["categoryid"];
                    iv.SectionID = (int)rr["sectionid"];
                    iv.ToStationID = (int)rr["destinationid"];
                    ll.Add(iv);
                }

                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static List<IndentInsertedItemList> InsertItem(int ItemId, int Gstationid, int ToStationID, List<IndentInsertedItemList> ExistList = null)
        {
            List<IndentInsertedItemList> ll = new List<IndentInsertedItemList>();
            IndentInsertedItemList FakeToHoldErr = new IndentInsertedItemList();
            try
            {
                string Str = "Select I.ID,I.Name,isnull(p.ConversionQty,1) ConversionQty ,p.PackID UnitID,i.drugtype,U.Name as Unit,isnull(s.maxlevel,0) maxlevel"
                         + " ,isnull(s.MinLevel,0) MinLevel from Item I,itemstore s,packing U,itempacking P  Where i.ID = s.itemid "
                         + " and i.ID=p.ItemID   and u.ID=p.PackID   and s.stationid = " + Gstationid + " and i.id=" + ItemId + " "
                         + " and p.Parent=0 ";
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
                        IndentInsertedItemList l = new IndentInsertedItemList();
                        l.SNO = sno;
                        l.ID = (int)rr["ID"];
                        l.Name = (string)rr["Name"];
                        l.conversionqty = (int)rr["ConversionQty"];
                        l.UnitID = (int)rr["UnitID"];
                        l.UnitName = (string)rr["Unit"];
                        l.DrugType = (int)rr["drugtype"];
                        l.MaxLevel = (int)rr["maxlevel"] / (int)rr["ConversionQty"];
                        l.MinLevel = (int)rr["MinLevel"] / (int)rr["ConversionQty"];
                        l.Remarks = " ";
                        l.QtyReqeust = 0;
                        Str = "Select I.ID,I.Name,i.unitid,i.drugtype,U.Name as Unit,sum(s.quantity) as qoh "
                          + " from Item I,packing U,batchstore s where U.ID=I.UnitID and i.id=s.itemid and s.stationid=" + ToStationID
                          + " and i.id=" + ItemId + " group by I.ID,I.Name,I.ConversionQty,i.unitid,i.drugtype,U.Name";
                        DataSet nw = MainFunction.SDataSet(Str, "tbl2");
                        if (nw.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow nn in nw.Tables[0].Rows)
                            {
                                if (ToStationID != 5)
                                {
                                    l.DistinationQty = (int)nn["qoh"] / (int)l.conversionqty;
                                }
                            }
                        }
                        else
                        {
                            l.DistinationQty = 0;
                        }

                        Str = "select ISNULL(sum(quantity),0) as qoh from batchstore where itemid = " + ItemId
                            + " and stationid = " + Gstationid;
                        DataSet NQ = MainFunction.SDataSet(Str, "tbl3");
                        if (NQ.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow xx in NQ.Tables[0].Rows)
                            {
                                l.QOH = (int)xx["qoh"] / (int)l.conversionqty;
                            }
                        }
                        else
                        {

                            l.QOH = 0;
                        }

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
                if (CheckEdit(order) == true)
                {
                    
                    using (Con)
                    {
                        Con.Open();
                        SqlTransaction Trans = Con.BeginTransaction();

                        if (order.OrderID == 0)                           {
                            int intId = 0;
                            int intstSlNo = 0;

                            bool n1 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=4 and stationid=0", Trans);
                            DataSet d1 = MainFunction.SDataSet("select MaxId from invmax where slno=4 and stationid=0", "tb1", Trans);
                            foreach (DataRow rr in d1.Tables[0].Rows)
                            {
                                intId = (int)rr["MaxId"];
                            }

                            bool n2 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=4 and stationid=" + UserInfo.selectedStationID, Trans);
                            DataSet d2 = MainFunction.SDataSet("select MaxId from invmax where slno=4 and stationid=" + UserInfo.selectedStationID, "tb1", Trans);
                            foreach (DataRow rr in d2.Tables[0].Rows)
                            {
                                intstSlNo = (int)rr["MaxId"];
                            }

                            string StrSql = "Insert into indent(Id,datetime,operatorid,sourceId,destinationId,deliveryDate,referenceNo "
                                          + " ,status,StationSlno,recstatus,categoryID,sectionid)"
                                          + " values (" + intId + ",sysdatetime()," + UserInfo.EmpID + "," + UserInfo.selectedStationID + ","
                                          + " " + order.ToStationID + ",'" + order.dtpBydate + "',"
                                          + " '" + order.txtRef + "',0," + intstSlNo + ",0," + order.ItemCategory + "," + order.SectionID + " )";

                            bool ins = MainFunction.SSqlExcuite(StrSql, Trans);
                            int count = 1;
                            foreach (var it in order.SelectedItem)
                            {
                                StrSql = "insert into indentdetail(Indentid,itemid,quantity,remarks,unitId,slno) values"
                                       + "(" + intId + "," + it.ID + "," + it.QtyReqeust + ",'" + it.Remarks
                                       + "'," + it.UnitID + "," + count + ")";
                                bool insdtl = MainFunction.SSqlExcuite(StrSql, Trans);
                                count += 1;
                            }
                            order.ErrMsg = "Indent No " + intstSlNo + " saved sucessfully ";
                        }
                        else                          {

                            string StrSql = " Insert into oldindent(indentid,sourceid,destinationid,referenceno,cancelledby,cancelleddttime) "
                                        + " select id,sourceid,destinationid,referenceno,operatorid,datetime from"
                                        + " indent where id=" + order.OrderID;
                            bool SaveOld = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "insert into oldindentdetail(Indentid,itemid,quantity,remarks,unitid)"
                                + " select Indentid,itemid,quantity,remarks,unitid  from indentdetail"
                                + " where indentid=" + order.OrderID;
                            bool SaveOldDetail = MainFunction.SSqlExcuite(StrSql, Trans);


                            StrSql = "update indent set"
                                + " datetime=sysdatetime(),"
                                + " operatorid=" + UserInfo.EmpID + ","
                                + " sourceId=" + UserInfo.selectedStationID + ","
                                + " sectionId=" + order.SectionID + ","
                                + " destinationId=" + order.ToStationID + ","
                                + " deliveryDate='" + order.dtpBydate + "',"
                                + " referenceNo='" + order.txtRef + "',"
                                + " status=0 where id =" + order.OrderID;
                            bool SaveEdit = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "Delete from indentdetail where indentid=" + order.OrderID;
                            bool DeleteOldDetail = MainFunction.SSqlExcuite(StrSql, Trans);

                            int count = 1;
                            foreach (var it in order.SelectedItem)
                            {
                                StrSql = "insert into indentdetail(Indentid,itemid,quantity,remarks,unitId,slno) values"
                                       + "(" + order.OrderID + "," + it.ID + "," + it.QtyReqeust + ",'" + it.Remarks
                                       + "'," + it.UnitID + "," + count + ")";
                                bool insdtl = MainFunction.SSqlExcuite(StrSql, Trans);
                                count += 1;
                            }

                            order.ErrMsg = "changes saved sucessfully ";

                        }

                        Trans.Commit();
                        return order;
                    }
                }
                else
                {
                    order.ErrMsg = "No Change found to Save";
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

        public static List<IndentInsertedItemList> ViewDetails(int orderID)
        {
            List<IndentInsertedItemList> ll = new List<IndentInsertedItemList>();
            try
            {
                string StrSql = " select a.name,b.itemid ID,a.conversionqty,a.drugtype,b.quantity, "
                              + " b.remarks,c.name as units,b.unitid,e.name as dunit,a.itemcode from"
                              + " item a,indentdetail b,packing C,indent d,packing e where b.itemid=a.id and"
                              + " c.Id=a.UnitId and b.indentid=d.id and e.id=b.unitid and "
                              + " b.indentid=" + orderID + " order by slno";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentInsertedItemList dd = new IndentInsertedItemList();
                    dd.SNO = sno;
                    dd.ID = (int)rr["ID"];
                    dd.Name = (string)rr["name"];
                    dd.conversionqty = (int)rr["conversionqty"];
                    dd.UnitID = (int)rr["unitid"];
                    dd.UnitName = (string)rr["units"];
                    dd.DrugType = (int)rr["drugtype"];
                    dd.MaxLevel = 0;
                    dd.MinLevel = 0;
                    dd.ItemCode = (string)rr["itemcode"];
                    dd.Remarks = (string)rr["remarks"];
                    dd.QtyReqeust = (int)(double)rr["quantity"];
                    sno += 1;
                    ll.Add(dd);
                }
                return ll;
            }
            catch (Exception e) { ll[0].ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }
        public static bool CheckEdit(IndentOrderModel NewData)
        {
            IndentOrderModel M = new IndentOrderModel();
            List<IndentInsertedItemList> OriginalDtl = new List<IndentInsertedItemList>();
            try
            {
                
                string str = "select deliveryDate,referenceNo,destinationID,categoryid,sectionid "
                    + " from indent where id=" + NewData.OrderID;
                DataSet nn = MainFunction.SDataSet(str, "tbl1");
                foreach (DataRow rr in nn.Tables[0].Rows)
                {
                    M.dtpBydate = MainFunction.DateFormat(rr["deliveryDate"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-");
                    M.txtRef = (string)rr["referenceNo"];
                    M.ToStationID = (int)rr["destinationID"];
                    M.ItemCategory = (int)rr["categoryid"];
                    M.SectionID = (int)rr["sectionid"];

                }

                if (M.dtpBydate != NewData.dtpBydate) { return true; }
                if (M.txtRef != MainFunction.NullToString(NewData.txtRef)) { return true; }
                if (M.ToStationID != NewData.ToStationID) { return true; }
                if (M.ItemCategory != NewData.ItemCategory) { return true; }
                if (M.SectionID != NewData.SectionID) { return true; }

                
                string StrSql = " select a.name,b.itemid ID,a.conversionqty,a.drugtype,b.quantity, "
                              + " b.remarks,c.name as units,b.unitid,e.name as dunit,a.itemcode from"
                              + " item a,indentdetail b,packing C,indent d,packing e where b.itemid=a.id and"
                              + " c.Id=a.UnitId and b.indentid=d.id and e.id=b.unitid and "
                              + " b.indentid=" + NewData.OrderID + " order by slno";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentInsertedItemList dd = new IndentInsertedItemList();
                    dd.SNO = sno;
                    dd.Name = (string)rr["name"];
                    dd.ID = (int)rr["ID"];
                    dd.QtyReqeust = (int)(double)rr["quantity"];
                    dd.UnitID = (int)rr["unitid"];
                    dd.UnitName = (string)rr["units"];
                    dd.Remarks = (string)rr["remarks"];
                    dd.MaxLevel = 0;
                    dd.MinLevel = 0;
                    sno += 1;
                    OriginalDtl.Add(dd);
                }

                List<IndentInsertedItemList> NewList = NewData.SelectedItem;
                if (ListMatch(OriginalDtl, NewList) == false)
                {
                    return true;
                }


                return false;
            }
            catch (Exception e) { return false; }

        }

        public static bool ListMatch(List<IndentInsertedItemList> l1, List<IndentInsertedItemList> l2)
        {
            try
            {
                if (l1.Count != l2.Count) { return false; }

                bool match = false;

                foreach (var itm in l1)
                {
                    foreach (var itm2 in l2)
                    {
                        if (itm.ID == itm2.ID &&
                            itm.QtyReqeust == itm2.QtyReqeust &&
                            itm.UnitID == itm2.UnitID &&
                            itm.Remarks == itm2.Remarks)
                        {
                            match = true; break;
                        }
                        else
                        {
                            match = false;
                        }
                    }
                    if (match == false) { return false; }

                }
                return match;
            }
            catch (Exception e) { return false; }


        }

        public static IndentOrderModel PrintOut(int OrderID, int gStationid)
        {
            IndentOrderModel ll = new IndentOrderModel();
            try
            {
                string StrSql = "Select a.id,a.StationSlNo,a.datetime,a.deliverydate,a.referenceno,a.status,b.name as "
                             + " destination,c.name,a.categoryid,isnull(a.sectionid,0) as sectionid,a.destinationid from indent a,station b,employee c where a.destinationid=b.id "
                             + " and a.operatorid=c.id  and a.status < 3 and a.id=" + OrderID
                             + " order by a.Status,a.Id desc";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.OrderID = (int)rr["id"];
                    string st = MainFunction.GetName("select upper(substring(name,1,2)) as name from station where id=" + gStationid, "name");
                    ll.lblId = st + rr["StationSlNo"].ToString();
                    ll.dtpBydate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["deliverydate"]);
                    ll.ReferenceNo = rr["referenceno"].ToString();
                    ll.Status = (Byte)rr["status"];
                    ll.ToStationName = rr["destination"].ToString();

                }
                List<IndentInsertedItemList> lst = ViewDetails(OrderID);

                ll.SelectedItem = lst;
                return ll;
            }
            catch (Exception e) { return ll; };
        }
    }

}





