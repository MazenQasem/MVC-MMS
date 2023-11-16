using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class DeptOrderFun
    {
        public static IndentOrderModel ClearIndent(int gStation)
        {
            IndentOrderModel nn = new IndentOrderModel();
            List<TempListMdl> station = new List<TempListMdl>();
            DataSet ds = MainFunction.SDataSet("select id,name from station where stores=1 and deleted=0 and id <>" + gStation + " order by name", "tbl");
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

                string StrSql = "Select a.id,a.StationSlNo,a.datetime,a.referenceno,a.status,b.name as "
                             + " destination,b.id as destinationid,c.name,a.categoryid,isnull(a.sectionid,0) sectionid "
                             + " from departmentorder a,station b,Employee c where a.destinationid=b.id "
                             + " and a.operatorid=c.id and a.DateTime >= '" + pm.Sdate + "' and a.datetime<='" + pm.Edate + "'"
                             + " and a.stationid=" + pm.gStationid
                             + " order by a.Status,a.Id desc";


                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentView iv = new IndentView();
                    iv.IndentID = (int)rr["id"];
                    string st = MainFunction.GetName("select upper(substring(name,1,2)) as name from station where id=" + pm.gStationid, "name");
                    iv.StationSlNO = st + rr["StationSlNo"].ToString();
                    iv.IndentDateTime = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
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
        public static List<IndentInsertedItemList> ViewDetails(int orderID)
        {
            List<IndentInsertedItemList> ll = new List<IndentInsertedItemList>();
            try
            {
                string StrSql = "select a.itemcode + '     ' + a.name as name,b.itemid ID,a.conversionqty,a.drugtype,b.quantity,b.remarks,c.name as units,"
                    + " b.unitid,e.name as dunit "
                    + " from item a,departmentorderdetail b,packing C,departmentorder d,packing e where b.itemid=a.id and"
                    + " c.Id=a.UnitId and b.orderid=d.id and e.id=b.unitid and "
                    + " b.orderid=" + orderID + " order by slno";



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
                                                              dd.Remarks = (string)rr["remarks"];
                    dd.QtyReqeust = (int)rr["quantity"];
                    sno += 1;
                    ll.Add(dd);
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
                if (CheckEdit(order) == true)
                {
                    
                    using (Con)
                    {
                        Con.Open();
                        SqlTransaction Trans = Con.BeginTransaction();

                        if (order.OrderID == 0)                           {   
                            int intId = 0;
                            int intstSlNo = 0;
                            int departmentId = 0;
                            bool n1 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=31 and stationid=0", Trans);
                            DataSet d1 = MainFunction.SDataSet("select MaxId from invmax where slno=31 and stationid=0", "tb1", Trans);
                            foreach (DataRow rr in d1.Tables[0].Rows)
                            {
                                intId = (int)rr["MaxId"];
                            }
                                                                                                                                                                                                                                       bool n2 = MainFunction.SSqlExcuite("update invmax set maxid=" + intId + " where slno=31 and stationid=" + UserInfo.selectedStationID, Trans);
                            intstSlNo = intId;
                            DataSet dept1 = MainFunction.SDataSet("select departmentid from station where id =" + UserInfo.selectedStationID, "tb1", Trans);
                            if (dept1.Tables[0].Rows.Count == 0)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                                order.ErrMsg = "Department is not being mapped to your station";
                                return order;
                            }
                            else
                            {
                                foreach (DataRow rr in dept1.Tables[0].Rows)
                                {

                                    departmentId = (int)rr["departmentid"];
                                }


                            }

                            string StrSql = "Insert into departmentorder(Id,datetime,operatorid,stationid,destinationId,"
                                          + " referenceNo,status,StationSlno,categoryID,deptid,sectionid)"
                                          + " values (" + intId + ",sysdatetime()," + UserInfo.EmpID + "," + UserInfo.selectedStationID + ","
                                          + " " + order.ToStationID + ",'" + order.txtRef + "',0," + intstSlNo + "," + order.ItemCategory + ","
                                          + " " + departmentId + "," + order.SectionID + " )";

                            bool ins = MainFunction.SSqlExcuite(StrSql, Trans);
                            int count = 1;
                            foreach (var it in order.SelectedItem)
                            {
                                StrSql = "insert into departmentorderdetail(Orderid,itemid,quantity,remarks,unitId,slno) values"
                                       + "(" + intId + "," + it.ID + "," + it.QtyReqeust + ",'" + it.Remarks
                                       + "'," + it.UnitID + "," + count + ")";
                                bool insdtl = MainFunction.SSqlExcuite(StrSql, Trans);
                                count += 1;
                            }
                            order.ErrMsg = "Department Order No " + intstSlNo + " saved sucessfully ";
                        }
                        else                          {
                            string StrSql = "update Departmentorder set"
                                + " datetime=sysdatetime(),"
                                + " operatorid=" + UserInfo.EmpID + ","
                                + " stationid=" + UserInfo.selectedStationID + ","
                                + " categoryid=" + order.ItemCategory + ","
                                + " sectionId=" + order.SectionID + ","
                                + " destinationId=" + order.ToStationID + ","
                                + " referenceNo='" + order.txtRef + "',"
                                + " status=0 where id =" + order.OrderID;

                            bool SaveEdit = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "Delete from DepartmentOrderDetail where orderid=" + order.OrderID;
                            bool DeleteOldDetail = MainFunction.SSqlExcuite(StrSql, Trans);

                            int count = 1;
                            foreach (var it in order.SelectedItem)
                            {
                                StrSql = "insert into departmentorderdetail(Orderid,itemid,quantity,remarks,unitid,slno) values"
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
        public static bool CheckEdit(IndentOrderModel NewData)
        {
            IndentOrderModel M = new IndentOrderModel();
            List<IndentInsertedItemList> OriginalDtl = new List<IndentInsertedItemList>();
            try
            {
                
                string str = "select referenceNo,destinationID,categoryid,sectionid "
                    + " from departmentorder where id=" + NewData.OrderID;
                DataSet nn = MainFunction.SDataSet(str, "tbl1");
                foreach (DataRow rr in nn.Tables[0].Rows)
                {
                    M.txtRef = (string)rr["referenceNo"];
                    M.ToStationID = (int)rr["destinationID"];
                    M.ItemCategory = (int)rr["categoryid"];
                    M.SectionID = (int)rr["sectionid"];

                }

                if (M.txtRef != MainFunction.NullToString(NewData.txtRef)) { return true; }
                if (M.ToStationID != NewData.ToStationID) { return true; }
                if (M.ItemCategory != NewData.ItemCategory) { return true; }
                if (M.SectionID != NewData.SectionID) { return true; }

                
                string StrSql = " select a.name,b.itemid ID,a.conversionqty,a.drugtype,b.quantity, "
                              + " b.remarks,c.name as units,b.unitid,e.name as dunit,a.itemcode "
                              + " from item a,departmentorderdetail b,packing C,departmentorder d,packing e where b.itemid=a.id and"
                              + " c.Id=a.UnitId and b.orderid=d.id and e.id=b.unitid and "
                              + " b.orderid=" + NewData.OrderID + " order by slno";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    IndentInsertedItemList dd = new IndentInsertedItemList();
                    dd.SNO = sno;
                    dd.Name = (string)rr["name"];
                    dd.ID = (int)rr["ID"];
                    dd.QtyReqeust = (int)rr["quantity"];
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
                string StrSql = "Select a.id,a.StationSlNo,a.datetime,a.referenceno,a.status,b.name as "
                          + " destination,b.id as destinationid,c.name,a.categoryid,isnull(a.sectionid,0) sectionid "
                          + " from departmentorder a,station b,Employee c where a.destinationid=b.id "
                          + " and a.operatorid=c.id and a.id=" + OrderID
                          + " order by a.Status,a.Id desc";


                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.OrderID = (int)rr["id"];
                    string st = MainFunction.GetName("select upper(substring(name,1,2)) as name from station where id=" + gStationid, "name");
                    ll.lblId = st + rr["StationSlNo"].ToString();
                    ll.dtpBydate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["datetime"]);
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





