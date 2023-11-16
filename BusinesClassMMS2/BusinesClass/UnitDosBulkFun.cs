using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class UnitDoseBulkFun
    {
        public static UnitDoseBulkModel Clear(int Gstationid, int gModuleId, int gBranchCode)
        {
            UnitDoseBulkModel nn = new UnitDoseBulkModel();
            string Sq = "exec MMS_PHARMALIST " + Gstationid + " ," + gModuleId + ", 0 ";
            DataSet ds = MainFunction.SDataSet(Sq, "tbl1");

            List<TempListMdl> PH = new List<TempListMdl>();
            List<TempListMdl> AsstPH = new List<TempListMdl>();

            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl TT = new TempListMdl();
                TT.ID = (int)rr["ID"];
                TT.Name = rr["Name"].ToString();
                if ((int)rr["CategoryID"] != 2)
                {
                    PH.Add(TT);
                }
                else
                {
                    AsstPH.Add(TT);
                }
            }


            List<TempListMdl> StationList = new List<TempListMdl>();
            Sq = "select ID,Name From station where deleted=0";
            DataSet dst = MainFunction.SDataSet(Sq, "tbl");
            foreach (DataRow rr in dst.Tables[0].Rows)
            {
                TempListMdl ll = new TempListMdl();
                ll.ID = (int)rr["ID"];
                ll.Name = rr["Name"].ToString();
                StationList.Add(ll);
            }

            nn.PharmacistList = PH;
            nn.AsstPharmacistList = AsstPH;
            nn.StationList = StationList;

            return nn;
        }
        public static string ProcessingBulk(string dtpDate, int StationID, int PHID, int AssPHID, string bulkDate, User UserInfo)
        {
            try
            {
                int cnt = MainFunction.GetOneVal("select count(*) as c1 from WardCutOffTime where stationid = " + StationID
                    + " and datepart(hh,getdate()) >= datepart(hh,cutofftime)", "c1");
                if (cnt == 0) { return "Bulk Processing for this Station cannot be done before the Cut Off Time."; }

                
                HeaderInfo MainHD = new HeaderInfo();
                MainHD.dtpDate = dtpDate;
                MainHD.StationID = StationID;
                MainHD.PHID = PHID;
                MainHD.AssPHID = AssPHID;
                MainHD.bulkDate = bulkDate;
                MainHD.OperatorID = UserInfo.EmpID;
                MainHD.gStationID = UserInfo.selectedStationID;                 if (InitBatchArray(ref MainHD) == false)
                {
                    return "Error while saving!";
                }



                int OrderStatus = 0;
                int mPartial = 0;
                string str = "select i.id,i.orderstatus from ipprescription i where i.orderstatus < 2 and i.stationid = " + StationID;
                DataSet dst = MainFunction.SDataSet(str, "tbl");
                foreach (DataRow rsTemp in dst.Tables[0].Rows)
                {
                    OrderStatus = 2;
                    mPartial = 0;
                    string Sql = "select totalqty,totalissuedqty,enddatetime,discontinueddatetime,discontinued,bulkprocessdate "
                        + " from ipprescriptiondetail where presid = " + (int)rsTemp["id"];
                    DataSet prdst = MainFunction.SDataSet(Sql, "tbl");
                    foreach (DataRow rsDetailTemp in prdst.Tables[0].Rows)
                    {
                        if ((int)rsDetailTemp["totalqty"] > (int)rsDetailTemp["totalissuedqty"] && (DateTime)rsDetailTemp["discontinueddatetime"] >= DateTime.Parse(dtpDate))
                        {
                            OrderStatus = 1;
                            foreach (DataRow rsPartial in prdst.Tables[0].Rows)
                            {
                                if ((DateTime)rsPartial["bulkprocessdate"] < DateTime.Parse(dtpDate) ||
                                    MainFunction.NullToString(rsPartial["bulkprocessdate"].ToString()) == "")
                                {
                                    mPartial = 1;
                                    break;
                                }
                            }
                            break;  
                        }
                    } 
                    if (OrderStatus == 2) { bool Ex = MainFunction.SSqlExcuite("Update ipprescription set OrderStatus = 2 where id = " + (int)rsTemp["id"]); }
                    else { bool ex2 = MainFunction.SSqlExcuite("Update ipprescription set OrderStatus = 1 where id = " + (int)rsTemp["id"]); }

                    if (mPartial == 1) { bool exp = MainFunction.SSqlExcuite("Update ipprescription set partial = " + mPartial + " where id = " + (int)rsTemp["id"]); }
                    else if (mPartial == 0) { bool exp2 = MainFunction.SSqlExcuite("Update ipprescription set partial = " + mPartial + " where id = " + (int)rsTemp["id"]); }
                }

                return "Process Completed";
            }
            catch (Exception) { return "Error when try to Process the bulk !"; }
        }
        public static bool InitBatchArray(ref HeaderInfo tbl)
        {
            try
            {
                int prevPresId = 0;
                DateTime dtpDate = DateTime.Parse(tbl.dtpDate);
                string dtpProcessDate = MainFunction.DateFormat(dtpDate.ToString(), "dd", "MMM", "yyyy", "hh", "mm");
                string SelDate = MainFunction.DateFormat(dtpDate.ToString(), "dd", "MMM", "yyyy");
                string XSelDate = MainFunction.DateFormat(dtpDate.AddDays(1).ToString(), "dd", "MMM", "yyyy");
                int mQuantity = 0;
                tbl.arrItemList = new List<arrItem>();                  tbl.arrItemList1 = new List<arrItem>();
                string Sql = "select pd.PresID,pd.ItemID,pd.FrequencyID,pd.StartDateTime,pd.DiscontinuedDateTime ,pd.PerDayQty,"
                  + " pd.PerDoseQty,pd.Period,pd.UpdatedPeriod  "
                  + " from wardcutofftime w,ipprescription p,ipprescriptiondetail pd,inpatient ip,bed b "
                  + " where p.ipid = b.ipid and b.status = 5 and"
                  + " p.id = pd.presid and w.stationid = p.stationid and p.ipid = ip.ipid and p.orderstatus < 2 "
                  + " and (pd.processdate < dateadd( hh,datepart(hour,w.cutofftime), '" + SelDate + "') or pd.processdate is null) "
                  + " and p.stationid = " + tbl.StationID + " and p.ordertype <> 2 and pd.totalqty > pd.totalissuedqty "
                  + " and (dateadd(day,pd.period,pd.processdate) >= '" + SelDate
                  + "' or pd.processdate is null or dateadd(day,pd.period,pd.processdate) <  "
                  + " dateadd(dd,1,dateadd(hh,datepart(hour,w.cutofftime),Cast('" + SelDate + "' as Datetime))))  "
                  + " and pd.discontinueddatetime > dateadd( hh,datepart(hour,w.cutofftime), '" + SelDate + "')   "
                  + " and pd.startdatetime < '" + XSelDate + "' order by p.id ";

                DataSet MainPresItems = MainFunction.SDataSet(Sql, "tbl");
                foreach (DataRow rsTemp in MainPresItems.Tables[0].Rows)
                {
                    if (prevPresId != (int)rsTemp["presid"] && prevPresId > 0)
                    {
                        Save(prevPresId, ref tbl, dtpProcessDate);
                        tbl.arrItemList = new List<arrItem>();                      }
                    mQuantity = CheckDose((byte)rsTemp["FrequencyID"], (int)rsTemp["PresID"], (int)rsTemp["ItemID"],
                        MainFunction.DateFormat(tbl.dtpDate, "dd", "MMM", "yyyy"),
                        MainFunction.DateFormat(tbl.dtpDate, "dd", "MMM", "yyyy", "hh", "mm")) * (int)rsTemp["PerDoseQty"];

                    if (mQuantity > 0)
                    {
                        tbl.arrItemList1 = new List<arrItem>();
                        string BatchStr = "SELECT B.BatchNo,b.batchid,B.ItemId as ID,i.unitid,B.Quantity "
                            + " ,i.conversionqty,a.SellingPrice,a.costprice "
                            + " ,packid as unit "
                            + " FROM BATCH  a,batchstore b ,Item I ,itemlowestunit Iunit"
                            + " Where b.StationID = " + tbl.gStationID + " And a.Itemid = b.Itemid  "
                            + " And a.itemid=iunit.itemid "
                            + " And LTrim(RTrim(a.BatchNo)) = LTrim(RTrim(b.BatchNo)) And a.Batchid = b.Batchid "
                            + " AND A.ItemID=I.ID AND B.Quantity>0 AND B.ItemID= " + (int)rsTemp["ItemID"]
                            + " order by a.expirydate,a.startDate ";
                        DataSet rsBatchDst = MainFunction.SDataSet(BatchStr, "tb");
                        if (rsBatchDst.Tables[0].Rows.Count > 0)
                        {
                            foreach (DataRow rsBatch in rsBatchDst.Tables[0].Rows)
                            {
                                if ((int)rsBatch["Quantity"] >= mQuantity)
                                {
                                    arrItem One = new arrItem();
                                    One.ID = (int)rsBatch["ID"];
                                    One.BatchNo = (string)rsBatch["BatchNo"];
                                    One.Batchid = rsBatch["batchid"].ToString();
                                    One.DedQty = mQuantity;
                                    One.Qty = mQuantity;
                                    One.ItemID = (int)rsBatch["ID"];
                                    One.Price = (decimal)rsBatch["SellingPrice"];
                                    One.EPR = (decimal)rsBatch["costprice"];
                                    One.UnitId = (int)rsBatch["unit"];

                                    One.Period = (byte)rsTemp["Period"];
                                    One.UpdatedPeriod = (byte)rsTemp["UpdatedPeriod"];
                                    One.PresID = (int)rsTemp["PresID"];

                                    tbl.arrItemList.Add(One);
                                    mQuantity = 0;                                     break; 
                                }
                                else
                                {
                                    if ((int)rsBatch["Quantity"] >= mQuantity)
                                    {
                                        arrItem One = new arrItem();
                                        One.ID = (int)rsBatch["ID"];
                                        One.BatchNo = (string)rsBatch["BatchNo"];
                                        One.Batchid = rsBatch["batchid"].ToString();
                                        One.DedQty = mQuantity;
                                        One.Qty = mQuantity;
                                        One.ItemID = (int)rsBatch["ID"];
                                        One.Price = (decimal)rsBatch["SellingPrice"];
                                        One.EPR = (decimal)rsBatch["costprice"];
                                        One.UnitId = (int)rsBatch["unit"];

                                        One.Period = (byte)rsTemp["Period"];
                                        One.UpdatedPeriod = (byte)rsTemp["UpdatedPeriod"];
                                        One.PresID = (int)rsTemp["PresID"];

                                        mQuantity = 0;                                         tbl.arrItemList1.Add(One);
                                        break; 
                                    }
                                    else
                                    {
                                        mQuantity = mQuantity - (int)rsBatch["Quantity"];
                                        arrItem One = new arrItem();
                                        One.ID = (int)rsBatch["ID"];
                                        One.BatchNo = (string)rsBatch["BatchNo"];
                                        One.Batchid = rsBatch["batchid"].ToString();
                                        One.DedQty = mQuantity;
                                        One.Qty = mQuantity;
                                        One.ItemID = (int)rsBatch["ID"];
                                        One.Price = (decimal)rsBatch["SellingPrice"];
                                        One.EPR = (decimal)rsBatch["costprice"];
                                        One.UnitId = (int)rsBatch["unit"];

                                        One.Period = (byte)rsTemp["Period"];
                                        One.UpdatedPeriod = (byte)rsTemp["UpdatedPeriod"];
                                        One.PresID = (int)rsTemp["PresID"];

                                        tbl.arrItemList1.Add(One);
                                    }

                                }


                            } 
                            if (mQuantity == 0)
                            {
                                foreach (arrItem XOne in tbl.arrItemList1)
                                {
                                    arrItem One = new arrItem();
                                    One.ID = XOne.ID;
                                    One.BatchNo = XOne.BatchNo;
                                    One.Batchid = XOne.Batchid;
                                    One.DedQty = XOne.DedQty;
                                    One.Qty = XOne.Qty;
                                    One.ItemID = XOne.ItemID;
                                    One.Price = XOne.Price;
                                    One.EPR = XOne.EPR;
                                    One.UnitId = XOne.UnitId;
                                    One.Period = XOne.Period;
                                    One.UpdatedPeriod = XOne.UpdatedPeriod;
                                    One.PresID = XOne.PresID;

                                    tbl.arrItemList.Add(One);

                                }

                            }
                            else
                            {

                                arrItem One = new arrItem();
                                One.ID = (int)rsTemp["ItemID"];
                                One.DedQty = 0;
                                One.Qty = 0;
                                One.Period = (byte)rsTemp["Period"];
                                One.UpdatedPeriod = (byte)rsTemp["UpdatedPeriod"];
                                One.PresID = (int)rsTemp["PresID"];
                                tbl.arrItemList.Add(One);
                            }
                        }
                        else                          {
                            arrItem arrOneItem = new arrItem();
                            arrOneItem.ID = (int)rsTemp["ItemID"];
                            arrOneItem.DedQty = 0;
                            arrOneItem.Qty = 0;
                            arrOneItem.Period = (byte)rsTemp["Period"];
                            arrOneItem.UpdatedPeriod = (byte)rsTemp["UpdatedPeriod"];
                            arrOneItem.PresID = (int)rsTemp["PresID"];
                            tbl.arrItemList.Add(arrOneItem);
                        }
                    }
                    prevPresId = (int)rsTemp["PresID"];
                } 
                if (prevPresId > 0)
                {
                    foreach (arrItem itm in tbl.arrItemList)
                    {
                        if (itm.Qty > 0)
                        {
                            Save(prevPresId, ref tbl, dtpProcessDate);
                            tbl.arrItemList = new List<arrItem>();
                            break;
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
        public static bool Save(int mPresid, ref HeaderInfo tbl, string dtpProcessDate)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    
                    int pStatus = 0;
                    int mOrderID = 0;
                    int mstOrderID = 0;
                    string StrSql = "";
                    long mTranId = 0;
                    int mToStationId = 0;

                    bool UpdateOrderMax = MainFunction.SSqlExcuite("update ordermaxid set maxid=maxid+1 where tableid=22 and stationid=0", Trans);
                    mOrderID = MainFunction.GetOneVal("select maxid from ordermaxid where tableid=22 and  stationid=0", "maxid", Trans);
                    mToStationId = MainFunction.GetOneVal("select b.Stationid from Bed b,ipprescription ip where b.ipid = ip.ipid and b.status = 5", "Stationid", Trans);
                    if (mToStationId == 0) { mToStationId = tbl.gStationID; }

                    bool updateOrderMaxSt = MainFunction.SSqlExcuite("update ordermaxid set maxid=maxid+1 where tableid=22 and stationid=" + mToStationId, Trans);
                    mstOrderID = MainFunction.GetOneVal("select maxid from ordermaxid where tableid=22 and stationid=" + mToStationId, "maxid", Trans);

                    StrSql = "select ip.ipid,b.stationid,ip.operatorid,ip.doctorid,ip.orderdatetime,b.id as bedid,null,getdate(),2,ip.id "
                        + "  from ipprescription ip, bed b where ip.ipid = b.ipid and b.status = 5 and ip.id = " + mPresid;
                    DataSet MainDs = MainFunction.SDataSet(StrSql, "tbl", Trans);
                    if (MainDs.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow rsTemp in MainDs.Tables[0].Rows)
                        {
                            StrSql = " Insert into Drugorder (id,IPID,StationId,OperatorID,DoctorID,DateTime,BedId,Profileid, "
                                + " DispatchedDatetime,Dispatched,ToStationId,stationslno,ordertype,printstatus,prescriptionid,"
                                + " bulkprocessdate,pharmacyoperatorid,pharmacistid,AsstPharmacistid, PresId)"
                                + " values (" + mOrderID + "," + (int)rsTemp["ipid"] + "," + (int)rsTemp["stationid"] + "," + tbl.OperatorID
                                + "," + (int)rsTemp["doctorid"] + ",'" + dtpProcessDate + "'," + (int)rsTemp["bedid"] + ",null,'" + dtpProcessDate + "',3,"
                                + tbl.gStationID + "," + mstOrderID + ",1,0," + (int)rsTemp["id"] + ",'" + dtpProcessDate + "',"
                                + tbl.OperatorID + "," + tbl.PHID + "," + tbl.AssPHID + "," + mPresid + ")";
                            bool InsertDrugOrder = MainFunction.SSqlExcuite(StrSql, Trans);

                        }
                    }
                    else
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        return false;
                    }

                                         mTranId = MainFunction.SaveInTranOrder(Trans, tbl.gStationID, mOrderID, mstOrderID, 1, 6, tbl.gStationID, "IP", mstOrderID);
                    if (mTranId == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        return false;
                    }

                    
                    foreach (arrItem dtl in tbl.arrItemList)
                    {
                        if (dtl.Qty > 0)
                        {
                            StrSql = "Insert into DrugOrderdetailSubstitute (OrderID,ServiceID,DispatchQuantity,Substituteid,BatchNo, "
                                + " price,UnitId,batchid,epr,BillableQty,BillablePrice,BillableUnitId) Values("
                                + mOrderID + "," + dtl.ID + "," + dtl.Qty + "," + dtl.ID + ",'" + dtl.BatchNo + "'," + dtl.Price
                                + "," + dtl.UnitId + "," + dtl.Batchid + "," + dtl.EPR + "," + dtl.Qty + "," + dtl.Price + "," + dtl.UnitId + ")";
                            bool insertDetail = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "Update IPPrescriptiondetail set updatedperiod = (case when updatedperiod -1 = 0 then period else updatedperiod -1 end), "
                                + " processdate = '" + dtpProcessDate + "',totalissuedqty = totalissuedqty + " + dtl.DedQty
                                + ",bulkprocessdate = '" + dtpProcessDate + "' where presid = " + dtl.PresID + " and itemid = " + dtl.ID;
                            bool UPdteDetail = MainFunction.SSqlExcuite(StrSql, Trans);

                            StrSql = "Update Batchstore set Quantity=Quantity-" + dtl.DedQty + " where ItemId=" + dtl.ID + " and BatchNo='" + dtl.BatchNo
                                + "' and batchid=" + dtl.Batchid + " and stationid=" + tbl.gStationID;
                            bool UPdteBatch = MainFunction.SSqlExcuite(StrSql, Trans);

                            if (MainFunction.MazValidateBatchStoreQty(dtl.ID, int.Parse(dtl.Batchid), tbl.gStationID, Trans) == false)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                return false;
                            }

                            
                            
                            if (MainFunction.SaveInTranOrderDetail(Trans, mTranId, dtl.ID, dtl.DedQty, dtl.BatchNo, dtl.Batchid, dtl.EPR, dtl.Price, dtl.EPR) == false)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                return false;
                            }

                        }
                        else { pStatus = 1; }
                    }

                    StrSql = "update ipprescription set OrderStatus=1,ordertype =0,partial =" + pStatus + " where id = " + mPresid;
                    bool updatePrescription = MainFunction.SSqlExcuite(StrSql, Trans);

                    Trans.Commit();
                }

                return true;
            }

            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                return false;

            }




        }
        public static int CheckDose(int Frequencyid, int PresID, int ItemID, string SelDate, string SelDateTime)
        {
            int val = 0;
            string XselDate = DateTime.Parse(SelDate).AddDays(1).ToString();

            string Sql = " select count(*) as c1 from frequencytiming f,ipprescriptiondetail ip,wardcutofftime w,ipprescription p where "
                        + " f.FrequencyId = " + Frequencyid + " and p.id = ip.presid and p.stationid = w.stationid and  ip.presid = " + PresID
                            + " and ip.itemid = " + ItemID + " and "
                        + " ((datepart(hour,f.FrequencyTiming)  > datepart(hour,'" + SelDateTime + "') and "
                        + " (dateadd(hh,datepart(hour,f.FrequencyTiming),cast('" + SelDate + "' as datetime))  >= "
                        + " dateadd(hh,datepart(hour,w.cutofftime),cast('" + SelDate + "' as datetime))) "
                        + " and (ip.discontinueddatetime > dateadd(day,1,(cast('" + SelDate + "' as datetime))) or "
                        + " ip.discontinueddatetime > dateadd(hh,datepart(hour,f.FrequencyTiming),cast('" + SelDate + "' as datetime)))) "
                        + " or "
                        + " (dateadd(hh,datepart(hour,f.FrequencyTiming),cast('" + XselDate + "' as datetime))  <= "
                        + " dateadd(hh,datepart(hour,w.cutofftime),cast('" + XselDate + "' as datetime))) and "
                        + " ip.discontinueddatetime > dateadd(hh,datepart(hour,f.FrequencyTiming),cast('" + XselDate + "' as datetime))) ";
            DataSet nn = MainFunction.SDataSet(Sql, "tbl");
            foreach (DataRow rr in nn.Tables[0].Rows)
            {

                val = (int)rr["c1"];

            }
            return val;

        }
        
        public static List<OrderPrintView> PrintOut(int stationid, string ProcessDate)
        {
            List<OrderPrintView> aLLoRDER = new List<OrderPrintView>();
            try
            {
                string StrSql = " select Ip.AdmitDatetime,max(em.name) name , do.empCode as DCode,min(P.OrderDateTime) orderdatetime "
                + " ,CASE P.OrderType WHEN 0 THEN 'NORMAL (ROUTINE)' WHEN 2 then 'TAKE HOME' ELSE 'STAT' END AS ORDERTYPE "
                + " , isnull(do.firstname,'') + ' ' + isnull(do.middlename,'') + ' ' + isnull(do.lastname,'') as Doctor "
                + " , IP.RegistrationNo, b.Name as Bed,S.Name as Station,e.employeeid,min(d.dispatcheddatetime) dispatcheddatetime "
                + " , IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as patientname,"
                + " IP.Age ,  case Ip.Sex when 2 then 'MALE' else 'FEMALE' end AS SEX, co.code + '-' + co.name as Charge "
                + " ,emp.name as pharmacist,aemp.name as AsstPharmacist,p.stationid "
                + " from employee do, Bed B, Station S,drugorderdetailsubstitute ds"
                + " ,drugorder d,allinpatients ip,ipprescription p,ipprescriptiondetail pd,  Company Co"
                + " ,employee em, frequency g,itemnotes n,employee e,employee emp,employee aemp, item i"
                + " Where co.ID = IP.CompanyId And emp.ID = d.pharmacistid"
                + " and aemp.id=d.AsstPharmacistid and d.ipid = ip.ipid and d.id = ds.orderid and d.prescriptionid >0"
                + " and d.prescriptionid = pd.presid and p.id = pd.presid and  ds.substituteid = pd.itemid"
                + " and pd.frequencyid = g.id and ds.substituteid = i.id and i.id *= n.itemid and s.Id = d.StationId"
                + " and b.ipid = ip.IpId and do.id= ip.doctorid  and d.operatorid = e.id "
                + " and em.id=p.operatorid and p.stationid = " + stationid + " "
                + " and d.bulkprocessdate  = '" + ProcessDate + "'"
                + " GROUP BY "
                + " p.stationid,Ip.AdmitDatetime, P.OrderType, do.empCode "
                + " , isnull(do.firstname,'') + ' ' + isnull(do.middlename,'') + ' ' + isnull(do.lastname,'') "
                + " , IP.RegistrationNo, b.Name,S.Name,e.employeeid, IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') , "
                + " ip.Age, Ip.Sex, co.code + '-' + co.name,aemp.name,emp.name order by b.name,IP.RegistrationNo";


                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    try
                    {
                        OrderPrintView ll = new OrderPrintView();
                        ll.AdmitDatetime = String.Format("{0:dd-MMM-yyyy hh:mm tt}", (DateTime)rr["AdmitDatetime"]);
                        ll.orderdatetime = String.Format("{0:dd-MMM-yyyy hh:mm tt}", (DateTime)rr["orderdatetime"]);
                        ll.dispatcheddatetime = String.Format("{0:dd-MMM-yyyy hh:mm tt}", (DateTime)rr["dispatcheddatetime"]);

                        ll.ORDERTYPE = rr["ORDERTYPE"].ToString();
                        ll.RegistrationNo = rr["RegistrationNo"].ToString();
                        ll.Bed = rr["Bed"].ToString();
                        ll.employeeid = rr["employeeid"].ToString();
                        ll.DCode = rr["DCode"].ToString();
                        ll.SEX = rr["SEX"].ToString();
                        ll.Age = rr["Age"].ToString();
                        ll.stationid = rr["stationid"].ToString();

                        ll.Station = (rr["Station"].ToString().Length > 35 ? rr["Station"].ToString().Substring(0, 35) : rr["Station"].ToString());
                        ll.name = (rr["name"].ToString().Length > 35 ? rr["name"].ToString().Substring(0, 35) : rr["name"].ToString());
                        ll.Doctor = (rr["Doctor"].ToString().Length > 35 ? rr["Doctor"].ToString().Substring(0, 35) : rr["Doctor"].ToString());
                        ll.patientname = (rr["patientname"].ToString().Length > 35 ? rr["patientname"].ToString().Substring(0, 35) : rr["patientname"].ToString());
                        ll.Charge = (rr["Charge"].ToString().Length > 35 ? rr["Charge"].ToString().Substring(0, 35) : rr["Charge"].ToString());
                        ll.Pharmacist = (rr["Pharmacist"].ToString().Length > 35 ? rr["Pharmacist"].ToString().Substring(0, 35) : rr["Pharmacist"].ToString());
                        ll.AsstPharmacist = (rr["AsstPharmacist"].ToString().Length > 35 ? rr["AsstPharmacist"].ToString().Substring(0, 35) : rr["AsstPharmacist"].ToString());
                        aLLoRDER.Add(ll);
                    }
                    catch (Exception E) { }                 }



                return aLLoRDER;
            }
            catch (Exception e) { return aLLoRDER; };
        }
        public static List<OrderPrintDetail> PrintDetailOut(string stationid, string ProcessDate, string RegNo)
        {
            
            List<OrderPrintDetail> itemList = new List<OrderPrintDetail>();

            string SUBSTR2 = "select IP.RegistrationNo,0 as Slno,i.itemcode,i.name as item,pd.strength as Dos, "
            + " cast(pd.perdoseqty as varchar(20)) + ' ' + g.Description as frequency, "
            + " ds.dispatchquantity as Qty, cast(pd.startdatetime as Date) StartDateTime,cast(pd.discontinueddatetime as Date) as enddatetime "
            + " from Station S,drugorderdetailsubstitute ds,drugorder d,allinpatients ip, "
            + " ipprescription p,ipprescriptiondetail pd,frequency g,item i "
            + " Where d.ipid = ip.ipid and d.id = ds.orderid and d.prescriptionid >0  "
            + " and d.prescriptionid = pd.presid and p.id = pd.presid and  ds.substituteid = pd.itemid "
            + " and pd.frequencyid = g.id and ds.substituteid = i.id and s.Id = d.StationId "
            + " and p.stationid =   " + stationid
            + " and ip.registrationno=  " + RegNo
            + " and d.bulkprocessdate  =  '" + ProcessDate + "' "
            + " order by IP.RegistrationNo,d.id ";
            DataSet itemds = MainFunction.SDataSet(SUBSTR2, "tbL");
            int count = 1;
            foreach (DataRow rr in itemds.Tables[0].Rows)
            {
                OrderPrintDetail item = new OrderPrintDetail();
                item.Slno = count;
                item.RegistrationNo = rr["RegistrationNo"].ToString();
                item.itemcode = rr["itemcode"].ToString();
                item.item = rr["item"].ToString();
                item.Dos = rr["Dos"].ToString();
                item.frequency = rr["frequency"].ToString();
                item.Qty = rr["Qty"].ToString();
                item.StartDateTime = rr["StartDateTime"].ToString();
                item.enddatetime = rr["enddatetime"].ToString();
                itemList.Add(item);
                count += 1;
            }

            return itemList;
        }

        public static List<LableDetail> PrintLabels(int stationid, string ProcessDate)
        {
            List<LableDetail> ll = new List<LableDetail>();
            try
            {
                string StrSql = " select ip.RegistrationNo,ds.orderid,e.employeeid,d.dispatcheddatetime, ip.firstname + ' ' + "
                    + " ip.middlename + ' ' + ip.lastname as patientname, "
                    + " isnull(i.strength_unit,'') as Strength_unit,i.name as item,pd.strength,pd.startdatetime,pd.discontinueddatetime as enddatetime "
                    + " ,pd.perdoseqty,g.Description as frequency,g.notes as freqDesc,i.duplicatelabel,BA.EXPIRYDATE,  "
                    + "  n.NOTES,pd.presid,ds.dispatchquantity as DispatchQuantity,"
                    + " u.name as unit,isnull(b.name,'') as bed,r.description as routeofadmin,ds.remarks,st.prefix as stprefix,ds.OrderId "
                    + " from drugorder d "
                    + " inner join drugorderdetailsubstitute ds on d.ID = ds.orderId "
                    + " inner join allinpatients ip on d.ipid = ip.ipid "
                    + " inner join ipprescription p on d.prescriptionid = p.id  "
                    + " inner join ipprescriptiondetail pd on d.prescriptionid = pd.presid  "
                    + " inner join packing u on ds.unitid = u.id  "
                    + " inner join BATCH BA on BA.ITEMID=PD.ITEMID  "
                    + " inner join item i on ds.substituteid = i.id  "
                    + " left join bed b on ip.ipid=b.ipid  "
                    + " left join routeofadministration r on pd.routeofadminid=r.id  "
                    + " left join itemnotes n on i.id=n.itemid  "
                    + " inner join station st on p.stationid = st.id  "
                    + " inner join employee e on d.operatorid=e.id  "
                    + " inner join frequency g on pd.frequencyid = g.id   "
                    + " Where  d.prescriptionid > 0  "
                    + " and  ds.serviceid = pd.itemid   "
                    + " and p.stationid =  " + stationid
                    + " and ds.batchid = ba.batchid and ds.batchno = ba.batchno   "
                    + " and  d.bulkprocessdate = '" + ProcessDate + "' "
                    + " group by ip.RegistrationNo,ds.orderid,e.employeeid,d.dispatcheddatetime, ip.firstname  "
                    + " ,ip.middlename, ip.lastname ,i.name ,pd.strength,pd.startdatetime,pd.discontinueddatetime  "
                    + " ,pd.perdoseqty,g.Description ,  n.NOTES,pd.presid,u.name ,b.name ,r.description ,  "
                    + " ds.Remarks , st.prefix, i.strength_unit, g.notes, i.duplicatelabel, BA.EXPIRYDATE,ds.dispatchquantity  "
                    + " order by b.name,ip.RegistrationNo, ds.orderid  ";


                DataSet n = MainFunction.SDataSet(StrSql, "tbl");

                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    LableDetail dd = new LableDetail();
                    if ((bool)rr["duplicatelabel"] == true)
                    {
                        for (int i = 1; i <= (decimal)rr["DispatchQuantity"]; i++)
                        {
                            dd.PatientName = rr["patientname"].ToString().Substring(0, rr["patientname"].ToString().Length > 30 ? 30 : rr["patientname"].ToString().Length - 1).ToString();
                            dd.PatientName = "Patient :" + dd.PatientName.PadRight(20, ' ').ToString() + "PIN: " + rr["RegistrationNo"].ToString();
                            dd.Bed = "Bed     :" + (rr["stprefix"].ToString() + "," + rr["bed"].ToString()).PadRight(20, ' ').ToString() + "IssDate:" + String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["dispatcheddatetime"]);
                            dd.Medicine = "Medicine:" + rr["item"].ToString().Substring(0, rr["item"].ToString().Length > 50 ? 50 : rr["item"].ToString().Length - 1).ToString();
                            string DateMask = "******";
                            if ((DateTime)rr["EXPIRYDATE"] > DateTime.Now)
                            {
                                dd.Expdate = "ExpDate :" + String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["EXPIRYDATE"]).PadRight(20, ' ').ToString() + "QTY :  1";
                            }
                            else
                            {
                                dd.Expdate = "ExpDate :" + DateMask.PadRight(20, ' ').ToString() + "QTY :  1";
                            }
                            string strength = "Dose    :" + rr["strength"].ToString() + ' ' + rr["Strength_unit"].ToString();
                            string frequency = rr["frequency"].ToString() + ' ' + rr["freqDesc"].ToString();
                            string RouteofAdmin = rr["routeofadmin"].ToString();
                            dd.Dose = strength.PadRight(10) + "  " + frequency.PadRight(20) + "  " + RouteofAdmin.PadRight(25);
                        }
                        ll.Add(dd);
                    }
                    else
                    {
                        dd.PatientName = rr["patientname"].ToString().Substring(0, rr["patientname"].ToString().Length > 30 ? 30 : rr["patientname"].ToString().Length - 1).ToString();
                        dd.PatientName = "Patient :" + dd.PatientName.PadRight(20, ' ').ToString() + "PIN: " + rr["RegistrationNo"].ToString();
                        dd.Bed = "Bed     :" + (rr["stprefix"].ToString() + "," + rr["bed"].ToString()).PadRight(20, ' ').ToString() + "IssDate:" + String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["dispatcheddatetime"]);
                        dd.Medicine = "Medicine:" + rr["item"].ToString().Substring(0, rr["item"].ToString().Length > 50 ? 50 : rr["item"].ToString().Length - 1).ToString();
                        string DateMask = "******";
                        if ((DateTime)rr["EXPIRYDATE"] > DateTime.Now)
                        {
                            dd.Expdate = "ExpDate :" + String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["EXPIRYDATE"]).PadRight(20, ' ').ToString() + "QTY : " + rr["DispatchQuantity"].ToString();
                        }
                        else
                        {
                            dd.Expdate = "ExpDate :" + DateMask.PadRight(20, ' ').ToString() + "QTY : " + rr["DispatchQuantity"].ToString();
                        }
                        string strength = "Dose    :" + rr["strength"].ToString() + ' ' + rr["Strength_unit"].ToString();
                        string frequency = rr["frequency"].ToString() + ' ' + rr["freqDesc"].ToString();
                        string RouteofAdmin = rr["routeofadmin"].ToString();
                        dd.Dose = strength.PadRight(10) + "  " + frequency.PadRight(20) + "  " + RouteofAdmin.PadRight(25);
                        ll.Add(dd);
                    }
                }
                return ll;
            }
            catch (Exception e) { return ll; };
        }








    }

}





