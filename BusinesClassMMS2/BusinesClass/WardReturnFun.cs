using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class WardReturnFun
    {
        public static WardReturnHeader View(int gStationid)
        {
            WardReturnHeader ll = new WardReturnHeader();
            try
            {
                string Sql = " SELECT A.ID,A.IPID,b.registrationno,(B.FirstName+' ' +B.MiddleName+ ' ' + B.LastName) AS PATIENTNAME,"
                    + " d.name as bed,A.DATETIME,c.name,a.status,e.name as station,a.stationslno,"
                    + " isnull(b.vip,0) as vip ,case b.sex when 2 then 'Male' else 'Female' end as Sex,"
                    + " cast(b.age as varchar(10)) + ag.name as Age "
                    + " FROM DRUGRETURN A ,INPATIENT B,employee c,bed d ,station e,agetype ag "
                    + " WHERE d.id=a.bedid and A.IPID=B.IPID and c.id=a.operatorid "
                    + " and b.agetype=ag.id and a.stationid=e.id and a.tostationid = " + gStationid + " order by a.id desc";
                DataSet rsTemp = MainFunction.SDataSet(Sql.ToLower(), "tbl");
                List<WardReturnViewList> XLIST = new List<WardReturnViewList>();
                foreach (DataRow rr in rsTemp.Tables[0].Rows)
                {
                    WardReturnViewList yy = new WardReturnViewList();
                    yy.OrderNO = (int)rr["id"];
                    yy.Pin = rr["registrationno"].ToString();
                    yy.Patient = rr["patientname"].ToString();
                    yy.Bed = rr["bed"].ToString();
                    yy.Station = rr["station"].ToString();

                    yy.DateTime = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "");
                    yy.ReturnBy = rr["name"].ToString();
                    yy.Status = (Byte)rr["status"];
                    yy.SlNO = (int)rr["stationslno"];
                    yy.VIP = (bool)rr["vip"];

                    yy.Sex = rr["Sex"].ToString();
                    yy.Age = rr["Age"].ToString();
                    XLIST.Add(yy);
                }

                ll.ListView = XLIST;

                return ll;
            }
            catch (Exception e) { return ll; }




        }
        public static WardReturnHeader GetDetails(int mOrderID, int Status)
        {
            WardReturnHeader Maintbl = new WardReturnHeader();
            List<WardReturnDetail> ll = new List<WardReturnDetail>();
            try
            {
                decimal curPrice = 0;
                int lItemid = 0;
                if (Status == 0)
                {

                    string StrSql = "select distinct b.serviceid,x.drugorderid,isnull(b.quantity,0) quantity,"
                           + " bt.batchid,bt.batchno,b.remarks,bt.sellingprice as price,bt.costprice,a.name "
                           + " from drugreturn x,drugreturndetail b,item a,batch bt"
                           + " where  x.id = b.orderid and b.serviceid=a.id and b.orderid=" + mOrderID
                           + " and bt.itemid = a.id and bt.batchid = B.BATCHID";
                    DataSet rsRet = MainFunction.SDataSet(StrSql.ToLower(), "tbl");
                    int SNO = 1;

                    foreach (DataRow rr in rsRet.Tables[0].Rows)
                    {
                        if (lItemid == (int)rr["serviceid"])
                        {
                            WardReturnDetail xx = new WardReturnDetail();
                            xx.SNO = SNO;
                                                         xx.BatchNo = rr["BatchNo"].ToString();
                                                         xx.Price = (decimal)rr["price"];
                                                         xx.RetQty = (int)(Int16)rr["quantity"];
                            xx.Remarks = rr["remarks"].ToString();

                            string sqlUOm = "select pk.name as pack, ds.Unitid,p.ConversionQty "
                                + " from drugorder do,  drugorderdetailsubstitute ds, drugreturn a, ItemPacking P, Packing pk"
                                + " where pk.id= ds.Unitid and p.packid=ds.Unitid and ds.serviceId=p.ItemId and do.id = ds.orderid "
                                + " and do.id=a.DrugOrderId  and a.Id =" + mOrderID + " and ds.SubstituteId = " + (int)rr["serviceid"];
                            xx.UOM = MainFunction.GetName(sqlUOm, "pack");

                            xx.ItemID = (int)rr["serviceid"];
                            xx.BatchID = (int)rr["batchid"];
                            xx.CostPrice = (decimal)rr["costprice"];
                            ll.Add(xx);
                            SNO += 1;

                        }
                        else
                        {
                            WardReturnDetail xx = new WardReturnDetail();
                            xx.SNO = SNO;
                            xx.Item = rr["name"].ToString();
                            xx.BatchNo = rr["BatchNo"].ToString();
                            xx.Quantity = (int)(Int16)rr["quantity"];
                            xx.Price = (decimal)rr["price"];
                                                         xx.RetQty = (int)(Int16)rr["quantity"];
                            xx.Remarks = rr["remarks"].ToString();

                            string sqlUOm = "select pk.name as pack, ds.Unitid,p.ConversionQty "
                                + " from drugorder do,  drugorderdetailsubstitute ds, drugreturn a, ItemPacking P, Packing pk"
                                + " where pk.id= ds.Unitid and p.packid=ds.Unitid and ds.serviceId=p.ItemId and do.id = ds.orderid "
                                + " and do.id=a.DrugOrderId  and a.Id =" + mOrderID + " and ds.SubstituteId = " + (int)rr["serviceid"];
                            xx.UOM = MainFunction.GetName(sqlUOm, "pack");

                            xx.ItemID = (int)rr["serviceid"];
                            xx.BatchID = (int)rr["batchid"];
                            xx.CostPrice = (decimal)rr["costprice"];
                            ll.Add(xx);
                            SNO += 1;
                        }

                        lItemid = (int)rr["serviceid"];
                    }

                    foreach (var item in ll)
                    {
                        item.Total = item.Quantity * item.Price;
                        curPrice += item.Total;

                    }
                }
                else
                {
                    string StrSql = "select b.serviceid,b.quantity,b.remarks,b.batchno,b.price as price"
                        + ",a.name from drugreturndetail b, item a"
                        + " where b.serviceid = a.id and b.orderid = " + mOrderID;
                    DataSet rsRet = MainFunction.SDataSet(StrSql.ToLower(), "tbl");
                    int SNO = 1;
                    foreach (DataRow rr in rsRet.Tables[0].Rows)
                    {

                        WardReturnDetail xx = new WardReturnDetail();
                        xx.SNO = SNO;
                        xx.Item = rr["name"].ToString();
                        xx.BatchNo = rr["BatchNo"].ToString();
                        xx.Quantity = (int)(Int16)rr["quantity"];
                        xx.Price = (decimal)rr["price"];
                                                 xx.RetQty = (int)(Int16)rr["quantity"];
                        xx.Remarks = rr["remarks"].ToString();

                        string sqlUOm = "select pk.name as pack, ds.Unitid,p.ConversionQty "
                            + " from drugorder do,  drugorderdetailsubstitute ds, drugreturn a, ItemPacking P, Packing pk"
                            + " where pk.id= ds.Unitid and p.packid=ds.Unitid and ds.serviceId=p.ItemId and do.id = ds.orderid "
                            + " and do.id=a.DrugOrderId  and a.Id =" + mOrderID + " and ds.SubstituteId = " + (int)rr["serviceid"];
                        xx.UOM = MainFunction.GetName(sqlUOm, "pack");

                        xx.ItemID = (int)rr["serviceid"];
                        ll.Add(xx);
                        SNO += 1;
                    }

                    foreach (var item in ll)
                    {
                        item.Total = item.Quantity * item.Price;
                        curPrice += item.Total;

                    }
                }
                Maintbl.Details = ll;
                Maintbl.Total = curPrice;

                return Maintbl;
            }
            catch (Exception e) { Maintbl.ErrMsg = e.Message; return Maintbl; }


        }
        public static WardReturnHeader Save(WardReturnHeader Mdl, User UserData)
        {
            WardReturnHeader ll = new WardReturnHeader();
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    string StrSql = "update DrugReturn set status= 1,receivedby=" + UserData.EmployeeID
                        + ",receiveddatetime= GetDate()  WHERE ID=" + Mdl.OrderNO;
                    bool upd = MainFunction.SSqlExcuite(StrSql, Trans);

                                         long mTransId = MainFunction.SaveInTranOrder(Trans, UserData.selectedStationID
                        , (long)(int)Mdl.OrderNO, (long)(int)Mdl.OrderNO, 0, 7, 0, "", 0, Convert.ToInt64(Mdl.Pin));
                    if (mTransId == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        Mdl.ErrMsg = "Can't Update the Table.";
                        return Mdl;
                    }

                    int DrugOrderID = 0;
                    StrSql = "select DrugOrderId from DrugReturn where id=" + Mdl.OrderNO;
                    DataSet dd = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    foreach (DataRow rr in dd.Tables[0].Rows)
                    {
                        DrugOrderID = (int)rr["DrugOrderId"];
                    }

                    if (Mdl.Details.Count > 0)
                    {
                        bool del = MainFunction.SSqlExcuite("delete from drugreturndetail where orderid = " + Mdl.OrderNO, Trans);

                        foreach (var itm in Mdl.Details)
                        {
                            if (itm.Quantity > 0)
                            {
                                int ItemPacking = MainFunction.GetOneVal("select i.packid from itempacking "
                                    + " i where i.itemid = " + itm.ItemID
                                    + " and i.slno in (select max(j.slno) from itempacking j "
                                    + " where j.itemid = " + itm.ItemID + ")", "packid", Trans);

                                StrSql = "insert into  DrugReturnDetail (orderid,serviceid,quantity,remarks,batchno,price,batchid,unitid,costprice,DrugOrderID)"
                                + " values (" + Mdl.OrderNO + "," + itm.ItemID + "," + itm.Quantity
                                + ",'" + itm.Remarks.Trim() + "','" + itm.BatchNo
                                + "'," + itm.Price + "," + itm.BatchID + "," + ItemPacking
                                + "," + itm.CostPrice + "," + DrugOrderID + ")";

                                bool INser = MainFunction.SSqlExcuite(StrSql, Trans);

                                StrSql = "update batchstore set quantity= quantity+" + itm.Quantity
                                    + " where itemid=" + itm.ItemID + " and"
                                   + " batchno='" + itm.BatchNo + "' and batchid=" + itm.BatchID + "  and stationid=" + UserData.selectedStationID;

                                bool updteBatch = MainFunction.SSqlExcuite(StrSql, Trans);

                                                                                                  bool Trandtl = MainFunction.SaveInTranOrderDetail(Trans, mTransId, itm.ItemID, itm.Quantity, itm.BatchNo, itm.BatchID.ToString(), itm.CostPrice, itm.Price, itm.CostPrice);
                                if (Trandtl == false)
                                {
                                    if (Con.State == ConnectionState.Open)
                                    {
                                        Con.BeginTransaction().Rollback();
                                        Con.Close();
                                    }
                                    Mdl.ErrMsg = "Can't Update the Table.";
                                    return Mdl;

                                }


                            }
                        }



                    }





                    ll.ErrMsg = "Returns Have Been Recorded";
                    Trans.Commit();
                    return ll;
                }
            }
            catch (Exception e) { ll.ErrMsg = "Error when try to save !"; return ll; }



        }

        
        public static RptWardReturnHeader RetPrint(int mOrderID, User UserData)
        {
            RptWardReturnHeader ll = new RptWardReturnHeader();
            try
            {
                int Gstationid = UserData.selectedStationID;
                bool first = true;
                int sno = 1;
                string StrSql = "select i.IPID ,i.registrationno as pinno,s.name,i.firstname+' '+i.lastname+' '+i.middlename as patname,"
                + " b.name as bed,s.name as ward,E.STATIONSLNO AS SLNO,a.name as itemname,"
                + " c.quantity,c.batchno,c.price as price,c.batchid,c.quantity*c.price as TotPrice,"
                + " G.EXPIRYDATE,c.price ,do.name as DoctorName,e.receiveddatetime "
                + " from item a,drugreturndetail c , drugreturn e,BATCH G,inpatient i,bed b,station s ,doctor do"
                + " where s.id=b.stationid and "
                + " i.ipid=e.ipid and b.id=e.bedid and G.BATCHNO=C.BATCHNO and g.batchid=c.batchid"
                + " AND c.batchid=g.batchid and c.orderid=e.id  and  a.id=g.itemid and e.doctorid=do.id "
                + " and c.serviceid=a.id and e.tostationid=" + Gstationid + " and e.id=" + mOrderID;
                DataSet st = MainFunction.SDataSet(StrSql.ToLower(), "tbl");
                List<RptWardReturnDetail> listdtl = new List<RptWardReturnDetail>();
                foreach (DataRow rr in st.Tables[0].Rows)
                {
                    if (first == true)
                    {
                        ll.IPID = (int)rr["ipid"];
                        ll.Slno = "IP " + rr["slno"].ToString() + " INPATIENT ISSUES ";
                        ll.Station = UserData.StationName;
                        ll.Pin = MainFunction.getUHID(UserData.gIACode, rr["pinno"].ToString(), true);
                        ll.DateTime = MainFunction.DateFormat(rr["receiveddatetime"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "");
                        ll.Doctor = rr["doctorname"].ToString();
                        ll.Bed = rr["bed"].ToString();
                        ll.OrderNo = mOrderID;
                        ll.Name = rr["patname"].ToString();
                        ll.Ward = rr["ward"].ToString();
                        first = false;
                    }

                    RptWardReturnDetail nn = new RptWardReturnDetail();
                    nn.BatchID = (int)rr["batchid"];
                    nn.BatchNo = rr["batchno"].ToString();
                    nn.ExpiryDate = MainFunction.DateFormat(rr["expirydate"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-", "");
                    nn.ItemName = rr["itemname"].ToString();
                    nn.price = (decimal)rr["price"];
                    nn.Qty = (int)(Int16)rr["quantity"];
                    nn.SNO = sno;
                    nn.TotalPrice = (decimal)rr["totprice"];
                    listdtl.Add(nn);
                    sno += 1;
                }

                ll.dtl = listdtl;

                return ll;
            }
            catch (Exception e) { return ll; }



        }
    }

}





