using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class DrugOrderIssueFun
    {
        public static DrugOrderIssueModel ClearIndent(int Gstationid, int gModuleId, int gBranchCode)
        {
            DrugOrderIssueModel nn = new DrugOrderIssueModel();
                                                                 

            string Sq = "exec MMS_PHARMALIST " + Gstationid + " ," + gModuleId + ", 0 ";
            DataSet ds = MainFunction.SDataSet(Sq, "tbl1");

            List<TempListMdl> PH = new List<TempListMdl>();
            List<TempListMdl> AsstPH = new List<TempListMdl>();

            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl TT = new TempListMdl();
                TT.ID = (int)rr["ID"];
                TT.Name = rr["Name"].ToString();
                if ((int)rr["CategoryID"] == 1)
                {
                    PH.Add(TT);
                }
                else
                {
                    AsstPH.Add(TT);
                }
            }

            nn.PharmacistList = PH;
            nn.AsstPharmacistList = AsstPH;

            return nn;
        }
        public static List<DrugOrderIssueView> GetOrders(ParamTable pm)
        {
            List<DrugOrderIssueView> ll = new List<DrugOrderIssueView>();
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

                string StrSql = "SELECT distinct Station.Prefix,Drugorder.ID  as OrderNo,Drugorder.IPID as IPID, " +
                " inpatient.IssueAuthorityCode + '.' + replicate('0',10- len( inpatient.registrationno)) + cast(inpatient.registrationno as varchar(15)) as RegNo , " +
                " rTrim(InPatient.FirstName) + ' ' + rTrim(InPatient.MiddleName) + ' ' + lTrim(InPatient.LastName)  as PatientName " +
                " , Bed.Name as BedNo, Drugorder.DateTime as DateTime, " +
                " employee.Name  as Operator,Station.Name as Station, cast(Dispatched as integer) as Status,isnull(IsPartial,0) as IsPartial, " +
                " printstatus,Drugorder.stationslno,cast(Drugorder.ordertype as integer) as OrderType,Drugorder.istakehome as takehome  " +
                " FROM employee,Bed,InPatient,Station,  Drugorder  " +
                " WHERE InPatient.IPID = Drugorder.IPID AND Bed.ID = Drugorder.BedID AND employee.ID = Drugorder.OperatorID " +
                " and Station.id=drugorder.stationid  and (Dispatched =1 or dispatched = 2)and Drugorder.tostationid=" + pm.gStationid +
                " and (Drugorder.prescriptionid =0 or Drugorder.prescriptionid is null)   " +
                " and Drugorder.DateTime >='" + pm.Sdate + "' And Drugorder.DateTime <'" + pm.Edate + "'  " +
                " and drugorder.ipid=(case " + pm.IPID + " when 0 then drugorder.ipid else " + pm.IPID + " end) " +
                " Order By datetime desc ";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    DrugOrderIssueView iv = new DrugOrderIssueView();
                    iv.OrderID = (int)rr["OrderNo"];
                    iv.OrderNo = rr["Prefix"].ToString() + "---" + rr["stationslno"].ToString();
                    iv.PINNO = (int)rr["IPID"];
                    iv.registrationno = rr["RegNo"].ToString();
                    iv.PatientName = rr["PatientName"].ToString();
                    iv.BedNo = rr["BedNo"].ToString();
                    iv.DateTime = String.Format("{0:dd-MMM-yyyy hh:mm:ss}", (DateTime)rr["DateTime"]);
                    iv.Operator = rr["Operator"].ToString();
                    iv.Station = rr["Station"].ToString();
                    iv.Status = (int)rr["Status"];
                    iv.isPartial = MainFunction.NullToInteger(rr["IsPartial"].ToString());
                    iv.PrintStatus = MainFunction.NullToBool(rr["printstatus"].ToString());
                    iv.StationSlno = (int)rr["stationslno"];
                    iv.type = (int)rr["OrderType"];
                    iv.TakeHome = MainFunction.NullToInteger(rr["takehome"].ToString());


                    ll.Add(iv);
                }

                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static DrugOrderIssueModel ViewDetails(int orderID, int gStationID)
        {
            DrugOrderIssueModel dd = new DrugOrderIssueModel();
            decimal lquantity = 0;
            decimal lquantity1 = 0;
            int i = 0;
            long ltimes = 0;
            bool mDispatched = false;
            int lconvqty = 0;
            int mServiceid = 0;
            try
            {
                string StrSql = " SELECT  Bed.ID,Bed.Name as Bed, InPatient.IPID ,isnull(inpatient.companyid,0) as CompanyID," +
                " inpatient.IssueAuthorityCode + '.' + replicate('0',10- len( inpatient.registrationno)) + cast(inpatient.registrationno as varchar(15)) as RegNo ," +
                " (InPatient.Title) + ' ' + (InPatient.FirstName) + ' ' + (InPatient.MiddleName) + ' ' + (InPatient.LastName) as PatientName," +
                " CAST(InPatient.Age AS VARCHAR(3)) + '  ' + AGETYPE.NAME AS AGE, " +
                " case InPatient.Sex when 2 then 'Male' when 1 then 'Female' else InPatient.SexOthers end as Sex," +
                " Station.Name ,Drugorder.DateTime,Drugorder.ID, " +
                " Drugorder.Dispatched,d.Name as Doctor,d.id as DocID,employee.name as operatorname ,isnull(inpatient.VIP,0) as VIP," +
                " isnull(Drugorder.pharmacistid,0) as pharmacistid,isnull(Drugorder.AsstPharmacistid,0) as AsstPharmacistid ,  " +
                " Drugorder.ordertype as OrderType " +
                " From  Station,InPatient, Drugorder,employee d,Employee, Bed ,AGETYPE " +
                " WHERE d.ID = Drugorder.DoctorID AND Drugorder.IPID = INPATIENT.IPID AND Station.ID = Drugorder.StationID " +
                " And Bed.ID = Drugorder.BedID and Drugorder.operatorid=employee.id   AND Drugorder.ID = " + orderID +
                " AND iNPATIENT.AGETYPE *= AGETYPE.ID " +
                " ORDER BY Drugorder.Dispatched ";
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");

                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    dd.OrderID = orderID;
                    dd.IPID = (int)rr["IPID"];
                    dd.lblPinNo = rr["RegNo"].ToString();
                    dd.lblBed = rr["Bed"].ToString();
                    dd.lblWard = rr["Name"].ToString();
                    dd.lblOrderNo = orderID.ToString();
                    dd.lblName = rr["PatientName"].ToString();
                    dd.lblDateTime = MainFunction.DateFormat(rr["DateTime"].ToString(), "dd", "MMM", "yyyy", "hh", "mm", "ss", "-", ":");
                    dd.lblSex = rr["Sex"].ToString();
                    dd.lblAge = rr["AGE"].ToString();
                    dd.lblDoctor = rr["Doctor"].ToString();
                    dd.lblOperator = rr["operatorname"].ToString();
                    dd.CmbPHID = (int)rr["pharmacistid"];
                    dd.CmbAssPHID = (int)rr["AsstPharmacistid"];
                    dd.CompanyID = (int)rr["CompanyID"];
                    dd.OrderType = MainFunction.NullToBool(rr["OrderType"].ToString());

                    if (MainFunction.NullToInteger(rr["Dispatched"].ToString()) == 2) { dd.mDispatched = true; mDispatched = true; dd.mAddNew = false; }
                    else
                    { dd.mDispatched = false; mDispatched = false; dd.mAddNew = true; }


                    if (MainFunction.NullToInteger(rr["VIP"].ToString()) == 1) { dd.VIP = true; } else { dd.VIP = false; }
                }

                
                string DrugSql = "select name as description from M_Generic a,v_inpatientdrugallergies b "
                    + " where b.watdrugid=a.id and b.ipid=" + dd.IPID;
                DataSet DrugDs = MainFunction.SDataSet(DrugSql, "tbl2");
                List<TempListMdl> DrugList = new List<TempListMdl>();
                int Sq = 1;
                foreach (DataRow rr in DrugDs.Tables[0].Rows)
                {
                    TempListMdl tt = new TempListMdl();
                    tt.ID = Sq;
                    tt.Name = rr["description"].ToString();
                    Sq++;
                    DrugList.Add(tt);
                }
                dd.DrugAllergiesList = DrugList;

                
                string FoodSql = " select name from foodrawitems a,inpatientfoodallergies b "
                    + " where b.foodallergyid=a.id and b.ipid=" + dd.IPID;
                DataSet FoodDs = MainFunction.SDataSet(FoodSql, "tbl2");
                List<TempListMdl> FoodList = new List<TempListMdl>();
                Sq = 1;
                foreach (DataRow rr in FoodDs.Tables[0].Rows)
                {
                    TempListMdl tt = new TempListMdl();
                    tt.ID = Sq;
                    tt.Name = rr["name"].ToString();
                    Sq++;
                    FoodList.Add(tt);
                }
                dd.FoodAllergiesList = FoodList;

                

                

                
                string StrSql2 = "";
                if (dd.OrderType == false)
                {
                    StrSql2 = "Select a.ServiceId,isnull(a.Quantity,0) as Quantity,a.Remarks,a.sqno,c.name as Units,b.Name as Item,b.qoh from " +
                    " DrugOrderDetail a,Item b,packing C where a.Serviceid=b.Id and a.UnitId=c.Id  and a.OrderId=" + orderID + " order by a.sqno";
                                     }
                else
                {
                    StrSql2 = " Select  a.ServiceId,isnull(a.Quantity,0) as Quantity,a.Remarks,a.routeofadmin,b.strength," +
                        " cast(b.strength_no as integer) as strength_no,a.frequency_id,a.duration_id, " +
                        " a.duration_no,a.sqno,c.name as Units, " +
                        " b.itemcode + '-' + b.Name as Item,sum(bs.quantity)qoh, cast(b.strength_no as integer) as actualStrength,d.description as repeatpattern, " +
                        " e.description as duration  " +
                        " ,isnull(s.minlevel,0) as MinLevel  " +
                        " from  DrugOrderDetail a,Item b,packing C,repeatPattern d,DurationComponent e, ItemStore s, Batchstore bs " +
                        "  Where b.ID = a.ServiceId And c.ID = a.UnitId And e.ID = a.duration_id And b.ID = s.itemid And " +
                        " b.ID = s.itemid And s.StationID = " + gStationID + "  And bs.StationID = " + gStationID +
                        "  and d.id = a.frequency_id  and a.OrderId= " + orderID +
                        " group by a.ServiceId,a.Quantity,a.Remarks,a.routeofadmin,b.strength,b.strength_no,a.frequency_id,a.duration_id, " +
                        " a.duration_no,a.sqno,c.name,  b.ItemCode , b.Name, d.Description, e.Description,s.minlevel order by a.sqno  ";
                                     }


                DataSet itm = MainFunction.SDataSet(StrSql2, "tbl1");

                List<OrdersItems> lvwItems = new List<OrdersItems>();


                foreach (DataRow rr in itm.Tables[0].Rows)
                {
                    OrdersItems OI = new OrdersItems();
                    OI.LvwPrescription = dd.OrderType;
                    if (dd.OrderType == false)
                    {
                        OI.ItemID = (int)rr["ServiceId"];
                        OI.ItemName = rr["Item"].ToString();
                        OI.Strength = "";
                        OI.Qty = MainFunction.NullToInteger(rr["Quantity"].ToString());
                        OI.QOH = MainFunction.NullToInteger(rr["qoh"].ToString());
                        lquantity = OI.Qty;

                    }
                    else
                    {
                        OI.ItemID = (int)rr["ServiceId"];
                        OI.ItemName = rr["Item"].ToString() + "  " + rr["actualStrength"].ToString();
                        OI.Strength = rr["strength_no"].ToString() + "  " + rr["strength"].ToString();
                        OI.Qty = MainFunction.NullToInteger(rr["Quantity"].ToString());
                        OI.UOM = rr["Units"].ToString();
                    }

                    lvwItems.Add(OI);
                }
                dd.lvwItem = lvwItems;

                
                int lItemid = 0;

                if (dd.OrderType == false)
                {
                    StrSql2 = " Select distinct a.ServiceId,isnull(a.Quantity,0) Quantity,a.Remarks,a.sqno,c.name as Units,b.ItemCode + '-' + b.Name as Item, "
                        + " isnull(s.conversionqty,1) conversionqty,c.id as unitid,isnull(a.unitid,0) as wunitid,0 as qoh, "
                        + " isnull((select isnull(sellingprice,0) sellingprice from batch where batchid in "
                        + " (select max(batchid) from batch where itemid = b.id)),0) as sellingprice,b.CSSDItem  "
                        + " ,isnull(s.minlevel,0) as MinLevel  "
                        + " FROM DrugOrderDetail a,Item b,itemstore s,packing C "
                        + " where s.itemid = b.id and s.stationid = " + gStationID + " and a.Serviceid=b.Id and "
                        + " s.UnitId=c.Id and a.OrderId=" + orderID + "  order by a.sqno ";

                }
                else
                {

                    StrSql2 = " Select distinct a.ServiceId,isnull(a.Quantity,0) Quantity,a.Remarks,a.routeofadmin,a.strength,a.strength_no, "
                        + " a.frequency_id,a.duration_id,a.duration_no,b.strength_no as actualstrength,d.description as repeatpattern, "
                        + " e.description as duration,a.sqno,c.name as Units,b.itemcode + '-' +b.Name as Item, "
                        + " isnull(s.conversionqty,1) conversionqty,c.id as unitid,isnull(a.unitid,0) as wunitid,0 as qoh, "
                        + " isnull((select isnull(sellingprice,0) sellingprice from batch where batchid in "
                        + " (select max(batchid) from batch where itemid = b.id)),0) as sellingprice,"
                        + " isnull((select Expirydate from batch where batchid in "
                        + " (select max(batchid) from batch where itemid = b.id)),0) as Expiry,"
                        + " b.CSSDItem "
                        + " ,isnull(s.minlevel,0) as MinLevel  "
                        + " FROM DrugOrderDetail a,Item b,itemstore s,packing C,repeatpattern d,durationcomponent e  "
                        + " where e.id=a.duration_id and d.id=a.frequency_id  and s.itemid = b.id and s.stationid = " + gStationID + " and a.Serviceid=b.Id and "
                        + " s.UnitId=c.Id and a.OrderId=" + orderID + "  order by a.sqno ";

                }
                DataSet ItmDs = MainFunction.SDataSet(StrSql2, "tbl4");
                int Seq = 0;
                List<DrugInsertedItems> ItemList = new List<DrugInsertedItems>();
                if (ItmDs.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rsDetail in ItmDs.Tables[0].Rows)
                    {
                        i = 1; Seq++;
                        if (lItemid != (int)rsDetail["ServiceId"])
                        {
                            if (dd.OrderType == true)
                            {
                                ltimes = getTimes((int)rsDetail["frequency_id"], (int)rsDetail["duration_id"]);
                                lquantity = (int)rsDetail["Quantity"] * ltimes;
                                if (lquantity == 0)
                                {
                                    lquantity = (int)rsDetail["Quantity"] / (int)rsDetail["conversionqty"];
                                }
                                lquantity1 = lquantity;
                                i = i + 1;
                            }
                        }
                        lItemid = (int)rsDetail["ServiceId"];

                        if (mDispatched == false)
                        {
                            string ustring = "";
                            int rsTempUnitID = 0;
                            int rsTempQoh = 0;
                            string rsTempUnitName = "";
                            if ((int)rsDetail["wunitid"] == 0)
                            {
                                ustring = "select isnull(i.qoh,0) qoh,p.id as unitid,p.name from packing p,mms_itemmaster i where i.id=" + (int)rsDetail["ServiceId"]
                                         + " and i.unitid=p.id and i.stationid = " + gStationID;
                            }
                            else
                            {
                                ustring = "select isnull(i.qoh,0) qoh,p.id as unitid,p.name from packing p,mms_itemmaster i where "
                                    + " p.id =" + (int)rsDetail["wunitid"] + " and i.id  = " + (int)rsDetail["ServiceId"]
                                    + " and i.stationid = " + gStationID;
                            }
                            DataSet rsTemp = MainFunction.SDataSet(ustring, "tb");
                            foreach (DataRow rtemp in rsTemp.Tables[0].Rows)
                            {
                                rsTempUnitName = rtemp["name"].ToString();
                                rsTempQoh = (int)rtemp["qoh"];
                                rsTempUnitID = (int)rtemp["unitid"];
                            }

                            lconvqty = MainFunction.GetQuantity((int)rsDetail["wunitid"], (int)rsDetail["ServiceId"]);
                            DrugInsertedItems fgOrderDetail = new DrugInsertedItems();



                            if (mServiceid == (int)rsDetail["ServiceId"])
                            {
                                fgOrderDetail.SNO = Seq;
                                fgOrderDetail.DrugName = rsDetail["Item"].ToString();
                                fgOrderDetail.Unit = rsTempUnitName;
                                fgOrderDetail.ReqQty = (int)lquantity;
                                fgOrderDetail.Expirydate = MainFunction.DateFormat(rsDetail["Expiry"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-");
                                fgOrderDetail.QOH = Math.Round((decimal)(rsTempQoh / lconvqty), 2);
                                if (lquantity1 > rsTempQoh)
                                {
                                    fgOrderDetail.lquantity = rsTempQoh;
                                }
                                else
                                {
                                    fgOrderDetail.lquantity = lquantity1;
                                }
                                fgOrderDetail.DispatchedQty = (int)fgOrderDetail.lquantity;
                                fgOrderDetail.Substitute = rsDetail["Item"].ToString();
                                fgOrderDetail.ItemID = (int)rsDetail["ServiceId"];
                                fgOrderDetail.TQOH = Math.Round((decimal)(rsTempQoh / lconvqty), 2);
                                fgOrderDetail.Remarks = rsDetail["Remarks"].ToString();
                                fgOrderDetail.Total = lquantity * (decimal)rsDetail["sellingprice"] * lconvqty;
                                fgOrderDetail.Price = (decimal)rsDetail["sellingprice"] * lconvqty;
                                fgOrderDetail.UnitID = rsTempUnitID;
                                fgOrderDetail.ConversionQty = lconvqty;
                                fgOrderDetail.MinLevel = (int)rsDetail["MinLevel"];
                                                             }
                            else
                            {
                                fgOrderDetail.SNO = Seq;
                                fgOrderDetail.DrugName = rsDetail["Item"].ToString();
                                fgOrderDetail.Unit = rsTempUnitName;
                                fgOrderDetail.ReqQty = (int)lquantity;
                                fgOrderDetail.Expirydate = MainFunction.DateFormat(rsDetail["Expiry"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-");
                                fgOrderDetail.QOH = Math.Round((decimal)(rsTempQoh / lconvqty), 2);
                                if (lquantity > rsTempQoh)
                                {
                                    fgOrderDetail.lquantity = rsTempQoh;
                                }
                                else
                                {
                                    fgOrderDetail.lquantity = lquantity;
                                }
                                fgOrderDetail.DispatchedQty = (int)fgOrderDetail.lquantity;
                                fgOrderDetail.Substitute = rsDetail["Item"].ToString();
                                fgOrderDetail.ItemID = (int)rsDetail["ServiceId"];
                                fgOrderDetail.TQOH = Math.Round((decimal)(rsTempQoh / lconvqty), 2);
                                fgOrderDetail.Remarks = rsDetail["Remarks"].ToString();
                                fgOrderDetail.Total = lquantity * (decimal)rsDetail["sellingprice"] * lconvqty;
                                fgOrderDetail.Price = (decimal)rsDetail["sellingprice"] * lconvqty;
                                fgOrderDetail.UnitID = rsTempUnitID;
                                fgOrderDetail.ConversionQty = lconvqty;
                                fgOrderDetail.MinLevel = (int)rsDetail["MinLevel"];
                            }
                            lquantity1 = lquantity1 - rsTempQoh;
                            if (lquantity1 <= 0) { lquantity1 = 0; }
                            fgOrderDetail.CSSD = MainFunction.NullToInteger(rsDetail["CSSDItem"].ToString());
                            mServiceid = (int)rsDetail["ServiceId"];
                            ItemList.Add(fgOrderDetail);
                        }
                    }
                    if (mDispatched == true)
                    {
                        string XStrSql = "select e.serviceid,e.batchid,e.dispatchquantity,e.substituteid,"
                            + "  e.batchno,p.name as packname,i.name as itemname,i1.name as substitutename, "
                            + " a.remarks,bt.expirydate,"
                            + " isnull((select sellingprice from batch  where batchid in (select max(batchid) from batch where itemid = bt.itemid)),0) as sellingprice "
                            + " from drugorderdetail a,batch bt,drugorderdetailsubstitute e,packing p,item i,item i1  "
                            + "  where e.serviceid = i.id and e.batchno = bt.batchno and e.batchid=bt.batchid "
                            + " and e.substituteid = bt.itemid and e.substituteid = i1.id and e.unitid = p.id "
                            + " and a.orderid = e.orderid and a.serviceid = e.serviceid  "
                            + " and e.orderid = " + orderID + " order by sqno";
                        DataSet xxtemp = MainFunction.SDataSet(XStrSql, "tb");
                        Seq = 0;
                        foreach (DataRow xrr in xxtemp.Tables[0].Rows)
                        {
                            DrugInsertedItems fgOrderDetail = new DrugInsertedItems();
                            fgOrderDetail.SNO = Seq;
                            fgOrderDetail.DrugName = xrr["itemname"].ToString();
                            fgOrderDetail.Unit = xrr["packname"].ToString();
                            fgOrderDetail.BatchNo = xrr["batchno"].ToString();
                            fgOrderDetail.Expirydate = MainFunction.DateFormat(xrr["expirydate"].ToString(), "dd", "MMM", "yyyy", "", "", "", "-");
                            fgOrderDetail.DispatchedQty = (int)(decimal)xrr["dispatchquantity"];
                            fgOrderDetail.Substitute = xrr["substitutename"].ToString();
                            fgOrderDetail.ItemID = (int)xrr["serviceid"];
                            fgOrderDetail.Remarks = xrr["remarks"].ToString();
                            fgOrderDetail.BatchID = (int)xrr["batchid"];
                            fgOrderDetail.Total = lquantity * (decimal)xrr["sellingprice"] * lconvqty;
                            fgOrderDetail.Price = (decimal)xrr["sellingprice"] * lconvqty;


                            ItemList.Add(fgOrderDetail);
                            Seq++;
                        }
                    }
                    dd.ItemList = ItemList;
                }
                else                  {
                    StrSql2 = "select a.name from item a,drugorderdetail c where c.serviceid=a.id  and"
                        + " c.serviceid not in(select itemid from itemstore where stationid=" + gStationID
                        + ") and c.orderid=" + orderID;
                    DataSet NotExistItemDs = MainFunction.SDataSet(StrSql2, "tb4");
                    if (NotExistItemDs.Tables[0].Rows.Count > 0)
                    {
                        string TempTxt = "";
                        foreach (DataRow rr in NotExistItemDs.Tables[0].Rows)
                        {
                            TempTxt += rr["name"].ToString() + " ; ";
                        }
                        dd.ErrMsg = "The following items ordered are not present in your stores" + TempTxt;
                    }
                }


                                 foreach (DrugInsertedItems xx in dd.ItemList)
                {
                    dd.Amount += (decimal)xx.Total;

                }

                                 if (dd.ItemList.Count > 1)
                {
                    bool checkInterAction = DrugInteraction(ref dd, 0, "DrugOrderdetailSubstitute", "substituteid", 0);
                }
                                 bool checkFoodInteracting = FoodInteraction(ref dd);
                return dd;
            }
            catch (Exception e) { dd.ErrMsg = "Error when try to load details: " + e.Message; return dd; }


        }
        public static long getTimes(long frequency_id, long duration_id)
        {
            long ltimes = 1;
            long lId = 0;
            bool continueLoop = true;
            do
            {
                string StrSql = "select a.value,a.id from repeatpattern a where a.d_id=" + frequency_id + "";
                DataSet xx = MainFunction.SDataSet(StrSql, "tb");
                if (xx.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in xx.Tables[0].Rows)
                    {
                        if (lId == frequency_id) { continueLoop = false; break; }
                        ltimes = ltimes * (int)(decimal)rr["value"];
                        frequency_id = (int)rr["id"];
                        lId = duration_id;
                    }
                }
                else
                {
                    continueLoop = false; break;
                }
            } while (continueLoop);
            return ltimes;
        }
        public static DrugSubList GetSubListDetail(int ItemID, int gStationid)
        {
            DrugSubList ll = new DrugSubList();
            string ss = "select a.QOH ,a.conversionqty as CQty ,a.UNITID ,isnull(a.cssditem,0) cssditem,p.name as UOM," +
            "(Select sellingprice from batch where BatchID in (Select max(batchid) from batch where itemid=a.ID)) as SellingPrice " +
            " from mms_itemmaster a left join packing p on a.UnitID=p.ID " +
            " where a.id=" + ItemID + " and a.stationid= " + gStationid;
            DataSet Ds = MainFunction.SDataSet(ss, "tbl");
            foreach (DataRow rr in Ds.Tables[0].Rows)
            {

                ll.ItemID = ItemID;
                ll.QOH = MainFunction.NullToInteger(rr["QOH"].ToString());
                ll.ConvQty = MainFunction.NullToInteger(rr["CQty"].ToString());
                ll.SellingPrice = (decimal)rr["SellingPrice"];
                ll.UnitID = (int)rr["UNITID"];
                ll.UnitName = rr["UOM"].ToString();
                ll.CSSD = MainFunction.NullToInteger(rr["cssditem"].ToString());
            }

                         List<TempListMdl> nn = new List<TempListMdl>();
            DataSet ds = MainFunction.SDataSet("Select P.Id ID,P.Name Name from ItemPacking I,Packing P where P.Id=I.PackId and itemid=" + ItemID + " order by name", "tb");
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl n = new TempListMdl();
                n.ID = (int)rr["ID"];
                n.Name = rr["Name"].ToString();
                nn.Add(n);
            }
            ll.PackingList = nn;

            return ll;

        }
        public static DrugOrderIssueModel Save(DrugOrderIssueModel order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                if (Validate(ref order) == true)
                {
                    
                    if (InitBatchArray(ref order, UserInfo.selectedStationID) == false)
                    {
                        order.ErrMsg = "Insufficient Stock";
                        return order;
                    }
                    
                    using (Con)
                    {
                        int CssdItem = 0;
                        int isPartial = 0;
                        decimal TotalReqQty = 0;
                        decimal TotalISSQty = 0;
                        bool updateremoteorder = false;

                        foreach (DrugInsertedItems rr in order.ItemList)
                        {
                            if (rr.CSSD == 1) { CssdItem = 1; } else { CssdItem = 0; }
                            TotalReqQty += rr.ReqQty;
                            TotalISSQty += rr.DispatchedQty;
                        }
                        if (TotalReqQty > TotalISSQty) { isPartial = 1; }

                        Con.Open();
                        SqlTransaction Trans = Con.BeginTransaction();

                        string StrSql = "Update DrugOrder set PharmacyOperatorID =" + UserInfo.EmpID
                        + " , Dispatched =" + 2 + ",DispatchedDateTime=sysdatetime(),CSSDItem = " + CssdItem
                        + " , pharmacistid=" + order.CmbPHID + " , AsstPharmacistid= " + order.CmbAssPHID + " ,IsPartial=" + isPartial
                        + "  where ID = " + order.OrderID;

                        int exeadconnSrv = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                        if (exeadconnSrv > 0)
                        {
                            updateremoteorder = true;
                        }
                        else
                        {
                            updateremoteorder = false;
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            order.ErrMsg = "Error while saving!";
                            return order;
                        }
                        Int64 xns = Convert.ToInt64(MainFunction.getRegNumber(order.lblPinNo));

                                                 long mTransId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, (long)order.OrderID, (long)order.OrderID,
                            1, 6, 0, "", 0, (long)xns);
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
                        foreach (arrItem item in order.ArrItems)
                        {
                            if (item.Qty > 0)
                            {
                                string StrSql2 = "Insert into DrugOrderDetailSubstitute (OrderID,DispatchQuantity,SubstituteId,BatchNo,"
                                    + " ServiceId,Price,Unitid,batchid,epr,billableqty,billableprice,billableunitid) Values("
                                    + order.OrderID + "," + item.Qty + "," + item.ID + ",'" + item.BatchNo + "'," + item.SubId
                                    + "," + item.Price + "," + item.UnitId + "," + item.Batchid + "," + item.EPR + "," + item.Qty + ","
                                    + item.Price + "," + item.UnitId + ")";

                                bool ExcutInsertDrugOrderSubstitute = MainFunction.SSqlExcuite(StrSql2, Trans);

                                                                 StrSql2 = "Update DrugorderDetail set dispatchQuantity=" + item.Qty + ",SubstituteId=" + item.ID
                                    + ",Batchno='" + item.BatchNo + "', price=" + item.Price + ",batchid = " + item.Batchid
                                    + " where serviceid=" + item.SubId + " and orderid=" + order.OrderID;
                                bool ExcutInsertDrugorderDetail = MainFunction.SSqlExcuite(StrSql2, Trans);

                                StrSql2 = "Update batchstore set Quantity=Quantity-" + item.DedQty
                                    + " where ItemId=" + item.ID + " and BatchNo='" + item.BatchNo + "' and batchid="
                                    + item.Batchid + " and stationid= " + UserInfo.selectedStationID;
                                bool ExcutUpdateBatchStore = MainFunction.SSqlExcuite(StrSql2, Trans);

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




                                                                 StrSql = "select costprice,sellingprice from batch where batchid =" + item.Batchid;
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
                            }
                        }
                        Trans.Commit();
                        order.ErrMsg = "changes saved sucessfully ";
                        return order;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(order.ErrMsg) == true)
                    {
                        order.ErrMsg = "No Change found to Save";
                    }
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


        public static bool InitBatchArray(ref DrugOrderIssueModel pt, int gStationId, SqlTransaction Trans = null)
        {
            try
            {
                List<arrItem> arrItem = new List<arrItem>();
                decimal mQuantity = 0;
                int ConvertQty = 1;
                foreach (DrugInsertedItems row in pt.ItemList)
                {
                    mQuantity = row.DispatchedQty;
                    if (row.ConversionQty == 1) { ConvertQty = 1; } else { ConvertQty = row.ConversionQty; }
                    if (mQuantity > 0)
                    {
                        string SS = "SELECT B.BatchNo,b.batchid,B.ItemId as ID,i.unitid,B.Quantity,i.conversionqty,a.SellingPrice,a.costprice "
                                   + " FROM BATCH  a,batchstore b ,Item I "
                                   + " Where b.StationID = " + gStationId + " And a.Itemid = b.Itemid  "
                                   + " And LTrim(RTrim(a.BatchNo)) = LTrim(RTrim(b.BatchNo)) And a.Batchid = b.Batchid "
                                   + " AND A.ItemID=I.ID AND B.Quantity>0 AND B.ItemID= " + row.ItemID
                                   + " order by a.expirydate,a.startDate ";
                        DataSet bt = MainFunction.SDataSet(SS, "tbl", Trans);
                        foreach (DataRow rsBatch in bt.Tables[0].Rows)
                        {
                            if (((int)rsBatch["Quantity"] / ConvertQty) >= mQuantity)
                            {

                                arrItem One = new arrItem();
                                One.ID = (int)rsBatch["ID"];
                                One.BatchNo = (string)rsBatch["BatchNo"];
                                One.Batchid = rsBatch["batchid"].ToString();
                                One.Qty = mQuantity;
                                One.UnitId = row.UnitID;
                                One.DedQty = (int)mQuantity * ConvertQty;
                                One.Price = (decimal)rsBatch["SellingPrice"] * ConvertQty;
                                One.EPR = (decimal)rsBatch["costprice"] * ConvertQty;
                                One.SubId = row.SubItemID == 0 ? (int)rsBatch["ID"] : row.SubItemID;
                                arrItem.Add(One);
                                mQuantity = 0;                                 break;                             }
                            else if (((int)rsBatch["Quantity"] / row.ConversionQty) < 1 || mQuantity == 0)
                            {
                                ;                             }
                            else
                            {
                                mQuantity = mQuantity - ((int)rsBatch["Quantity"] / ConvertQty);
                                arrItem One = new arrItem();
                                One.ID = (int)rsBatch["ID"];
                                One.BatchNo = (string)rsBatch["BatchNo"];
                                One.Batchid = rsBatch["batchid"].ToString();
                                One.Qty = mQuantity;
                                One.UnitId = row.UnitID;
                                One.DedQty = (int)mQuantity * ConvertQty;
                                One.Price = (decimal)rsBatch["SellingPrice"] * ConvertQty;
                                One.EPR = (decimal)rsBatch["costprice"] * ConvertQty;
                                One.SubId = row.SubItemID == 0 ? (int)rsBatch["ID"] : row.SubItemID;
                                arrItem.Add(One);
                            }
                        }
                    }

                }

                pt.ArrItems = arrItem;

                return true;
            }
            catch (Exception e)
            {
                pt.ErrMsg = "Error when try to read the Item Batch";
                return false;
            }


        }

        public static bool Validate(ref DrugOrderIssueModel Order)
        {
            try
            {
                var result = true;
                                 string Str = "Select Count(*) as Cn from OtOrder where PatientType=1  "
                    + " and datediff(hour,otstartdatetime,getdate())<=2 and Released<>1 "
                    + " and IPIDOPID=" + Order.IPID;
                int Cn = MainFunction.GetOneVal(Str, "Cn");
                if (Cn > 0) { Order.ErrMsg = "Patient is in Operation Theator.Cannot dispatch"; return false; }

                                 string TempListMinLevel = "";
                foreach (DrugInsertedItems xxt in Order.ItemList)
                {
                    if (MainFunction.CheckQOH_Min(xxt.ItemID, xxt.DrugName, (int)(long)xxt.ConversionQty, xxt.MinLevel,
                        (long)(int)xxt.QOH, (int)(decimal)xxt.DispatchedQty) == true)
                    {
                        Order.MinLvlFlag = true;
                        TempListMinLevel = TempListMinLevel + ',' + xxt.DrugName;
                    }


                                         Str = "Select count(*) as Cn from Item I, Antibiotic_Justification_Drugorder a Where a.Accepted<>1 AND a.Accepted<>2 AND "
                    + "  a.ServiceId = i.Id and a.OrderId = " + Order.OrderID + " and ServiceId in (" + xxt.ItemID + ") ";
                    int ANTI = MainFunction.GetOneVal(Str, "Cn");
                    if (ANTI > 0) { Order.ErrMsg = "Please Accept The Antibiotics  Medicine First!"; return false; }

                }
                if (Order.MinLvlFlag == true)
                {
                    Order.MinLvlStr = TempListMinLevel;
                    result = false;
                }

                Str = "select cast(dispatched as int) dispatched  from drugorder where id =" + Order.OrderID;
                int IssueAlready = MainFunction.GetOneVal(Str, "dispatched");
                if (IssueAlready >= 2) { Order.ErrMsg = "Indent has been already issued"; return false; }

                return result;
            }
            catch (Exception e) { return false; }
        }
        public static bool DrugInteraction(ref DrugOrderIssueModel MainMdl, int strType, string strTableName, string strColName, int duration)
        {
            try
            {

                int intRow = 1;
                string strBrandItem = "";
                string StrDrugInteraction = "";
                bool blnDrugInteraction = false;
                int lngItemId = 0;
                string StrSql = "";
                if (MainMdl.ItemList.Count == 1) { return false; }

                foreach (DrugInsertedItems FlexGrid in MainMdl.ItemList)
                {
                    lngItemId = FlexGrid.ItemID;
                    strBrandItem = "";                     intRow = 1;
                    foreach (DrugInsertedItems FlexGridIN in MainMdl.ItemList)
                    {
                        if (lngItemId != FlexGridIN.ItemID)
                        {
                            if (intRow == 1)
                            {
                                if (FlexGridIN.DispatchedQty > 0)
                                {
                                    strBrandItem = FlexGridIN.ItemID.ToString();
                                }
                            }
                            else
                            {

                                if (FlexGridIN.DispatchedQty > 0)
                                {
                                    if (strBrandItem.Length > 0)
                                    {
                                        strBrandItem += "," + FlexGridIN.ItemID.ToString();
                                    }
                                    else
                                    {
                                        strBrandItem += FlexGridIN.ItemID.ToString();
                                    }
                                }

                            }


                            intRow++;
                        }
                    }


                    if (duration > 0)
                    {
                        if (strBrandItem.Length > 0)
                        {
                            StrSql = "select distinct a." + strColName + " from " + strTableName + " a,drugorder b ";
                            StrSql = StrSql + " where b.id=a.OrderID and b.datetime>getdate()- " + duration + "  AND A.SUBSTITUTEID NOT IN ( " + strBrandItem + ") ";
                        }
                        else
                        {
                            StrSql = "select distinct a." + strColName + " from " + strTableName + " a,drugorder b ";
                            StrSql = StrSql + " where b.id=a.OrderID and b.datetime>getdate()- " + duration + "  ";
                        }
                        DataSet rsdescDS = MainFunction.SDataSet(StrSql, "tbl");
                        foreach (DataRow rsdesc in rsdescDS.Tables[0].Rows)
                        {
                            if (strBrandItem.Length > 0)
                            { strBrandItem += "," + rsdesc[strColName].ToString(); }
                            else
                            { strBrandItem += rsdesc[strColName].ToString(); }


                        }
                    }
                    if (strBrandItem.Length == 0) { return blnDrugInteraction; }
                    StrSql = "";                     if (strType == 1)
                    {
                        StrSql = "Select l.Genericid,l.InteractingGenericid, l.Discription,i.Name as ItemName from l_DrugDrugInteraction l,Item i, ";
                        StrSql = StrSql + " (Select Itemid,GenericId from ItemGeneric where Itemid = " + lngItemId + ") a, ";
                        StrSql = StrSql + " (Select Itemid,GenericId from ItemGeneric where Itemid in (" + strBrandItem + ")) b";
                        StrSql = StrSql + " Where l.GenericId = a.GenericId And l.InteractingGenericId = b.GenericId";
                        StrSql = StrSql + " and i.id = b.Itemid";
                        StrSql = StrSql + " Union all";
                        StrSql = StrSql + " Select l.Genericid,l.InteractingGenericid, l.Discription,i.Name as ItemName from l_DrugDrugInteraction l,Item i,";
                        StrSql = StrSql + " (Select Itemid,GenericId from ItemGeneric where Itemid in (" + strBrandItem + ")) a,";
                        StrSql = StrSql + " (Select Itemid,GenericId from ItemGeneric where Itemid = " + lngItemId + ") b";
                        StrSql = StrSql + " Where l.GenericId = a.GenericId And l.InteractingGenericId = b.GenericId";
                        StrSql = StrSql + " and i.id = a.Itemid";
                    }
                    else
                    {
                        StrSql = "select Distinct a.name,c.id,c.Name,b.Discription,b.Name as ItemName ";
                        StrSql = StrSql + "  from M_Generic a,M_Generic c,";
                        StrSql = StrSql + " (select Distinct (a.genericID),a.InteractingGenericId, ";
                        StrSql = StrSql + " a.discription,c.Name from l_DrugDrugInteraction a,Itemgeneric b ,Item c, ItemGeneric Uq ";
                        StrSql = StrSql + " where c.id=b.itemid and b.itemid in (" + strBrandItem + ") and b.genericid=a.genericid and uq.itemid = " + lngItemId + " ";
                        StrSql = StrSql + " and a.InteractingGenericId = uq.genericid )b ";
                        StrSql = StrSql + " Where a.ID = b.genericid And c.ID = b.InteractingGenericId ";
                    }

                    DataSet rsdescDS2 = MainFunction.SDataSet(StrSql, "tbl2");
                    foreach (DataRow rsdesc in rsdescDS2.Tables[0].Rows)
                    {
                        if (StrDrugInteraction != "")
                        {
                            StrDrugInteraction += " ::  " +
                                            rsdesc["ItemName"].ToString() + "  --- " + rsdesc["Discription"].ToString();
                        }
                        else
                        {
                            StrDrugInteraction = "   Interacts with : " + rsdesc["ItemName"].ToString() + "  --- " + rsdesc["Discription"].ToString();
                        }
                        FlexGrid.DrugInteraction = true;
                        FlexGrid.DrugInteractionMSG = StrDrugInteraction;
                    }

                    StrDrugInteraction = "";


                } 
                return true;
            }
            catch (Exception e) { return true; }


        }
        public static bool FoodInteraction(ref DrugOrderIssueModel MainMdl)
        {
            try
            {
                string StrDrugInteraction = "";
                int lngItemId = 0;
                string StrSql = "";
                if (MainMdl.ItemList.Count == 1) { return false; }

                foreach (DrugInsertedItems FlexGrid in MainMdl.ItemList)
                {
                    lngItemId = FlexGrid.ItemID;
                    StrSql = " select Distinct a.name,c.id,c.Name,b.discription,b.Name as ItemName,b.itemcode " +
                             " from M_Generic a,M_Generic c, " +
                             " ( select Distinct (a.genericID),a.InteractingFoodId,  a.discription,c.Name,i.itemcode " +
                             " from l_DrugFoodInteraction a,Itemgeneric b ,Foodinteraction_VW c,item i " +
                             " where c.id=a.interactingfoodid and b.itemid  =" + lngItemId + "  " +
                             " and b.genericid=a.genericid and b.itemid=i.id )b " +
                             " Where a.ID = b.genericid And c.ID = b.InteractingFoodId ";


                    DataSet rsdescDS2 = MainFunction.SDataSet(StrSql, "tbl2");
                    foreach (DataRow rsdesc in rsdescDS2.Tables[0].Rows)
                    {
                        if (StrDrugInteraction != "")
                        {
                            StrDrugInteraction += " ::  " +
                                            rsdesc["ItemName"].ToString() + "  --- " + rsdesc["Discription"].ToString();
                        }
                        else
                        {
                            StrDrugInteraction = "   Interacts with : " + rsdesc["ItemName"].ToString() + "  --- " + rsdesc["Discription"].ToString();
                        }
                        FlexGrid.FoodInteraction = true;
                        FlexGrid.FoodInteractionMSG = StrDrugInteraction;
                    }

                    StrDrugInteraction = "";


                } 
                return true;
            }
            catch (Exception e) { return true; }


        }



        public static DrugOrderPrintView PrintOut(int mOrderID, int Gstationid)
        {
            DrugOrderPrintView ll = new DrugOrderPrintView();
            try
            {
                string StrSql = " select distinct em.name name , do.empCode as DCode,d.DateTime orderdatetime " +
                " ,CASE d.OrderType WHEN 0 THEN 'NORMAL' WHEN 2 then 'TAKE HOME' ELSE 'STAT' END AS ORDERTYPE " +
                " , isnull(do.firstname,'') + ' ' + isnull(do.middlename,'') + ' ' + isnull(do.lastname,'') as Doctor " +
                " , IP.RegistrationNo, b.Name as Bed,S.Name as Station,e.employeeid,d.dispatcheddatetime dispatcheddatetime " +
                " , IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as patientname, " +
                " IP.Age ,  case Ip.Sex when 2 then 'MALE' else 'FEMALE' end AS SEX, co.code + '-' + co.name as Charge " +
                " ,emp.name as pharmacist,aemp.name as AsstPharmacist,d.id as OrderNo " +
                " from employee do, Bed B, Station S,drugorderdetailsubstitute ds " +
                " ,drugorder d,inpatient ip,  Company Co " +
                " ,employee em,employee e,employee emp,employee aemp, item i " +
                " Where co.ID = IP.CompanyId And emp.ID = d.pharmacistid " +
                " and aemp.id=d.AsstPharmacistid and d.ipid = ip.ipid and d.id = ds.orderid " +
                " and ds.substituteid = i.id and s.Id = d.StationId " +
                " and b.id = d.bedid and do.id= ip.doctorid  and d.operatorid = e.id " +
                " and d.operatorid=em.id " +
                " and D.tostationid=" + Gstationid + " " +
                " and d.id=" + mOrderID;
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ll.OrderNo = rr["OrderNo"].ToString();
                    ll.Name = rr["name"].ToString().Substring(0, rr["name"].ToString().Length - 1).ToString();
                    ll.Doctor = rr["DCode"].ToString() + " - " + rr["Doctor"].ToString().Substring(0, rr["Doctor"].ToString().Length - 1);
                    ll.Orderdatetime = String.Format("{0:dd-MMM-yyyy hh:mm tt}", (DateTime)rr["orderdatetime"]);
                    ll.DispatchDateTime = String.Format("{0:dd-MMM-yyyy hh:mm tt}", (DateTime)rr["dispatcheddatetime"]);
                    ll.OrderType = rr["ORDERTYPE"].ToString();
                    ll.PinNo = rr["RegistrationNo"].ToString();
                    ll.Bed = rr["Bed"].ToString();
                    ll.Station = rr["Station"].ToString();
                    ll.EmployeeID = rr["employeeid"].ToString();
                    ll.PatientName = rr["patientname"].ToString().Substring(0, rr["patientname"].ToString().Length - 1);
                    ll.Age = rr["Age"].ToString();
                    ll.Sex = rr["SEX"].ToString();
                    ll.Company = rr["Charge"].ToString();
                    ll.Pharmacist = rr["pharmacist"].ToString().Substring(0, rr["pharmacist"].ToString().Length - 1);
                    ll.AsstPharmacist = rr["AsstPharmacist"].ToString().Substring(0, rr["AsstPharmacist"].ToString().Length - 1);
                }


                
                List<DrugOrderPrintDetail> itemList = new List<DrugOrderPrintDetail>();
                string SUBSTR2 = " select em.name name , do.empCode as DCode,d.DateTime orderdatetime " +
                " ,CASE d.OrderType WHEN 0 THEN 'NORMAL' WHEN 2 then 'TAKE HOME' ELSE 'STAT' END AS ORDERTYPE " +
                " , isnull(do.firstname,'') + ' ' + isnull(do.middlename,'') + ' ' + isnull(do.lastname,'') as Doctor " +
                " , IP.RegistrationNo, b.Name as Bed,S.Name as Station,e.employeeid,d.dispatcheddatetime dispatcheddatetime " +
                " , IsNull(ip.firstname,'') + ' ' + IsNull(ip.middlename,'')  + ' ' + IsNull(ip.lastname ,'') as patientname, " +
                " IP.Age ,  case Ip.Sex when 2 then 'MALE' else 'FEMALE' end AS SEX, co.code + '-' + co.name as Charge " +
                " ,emp.name as pharmacist,aemp.name as AsstPharmacist " +
                " , I.ITEMCODE,I.NAME AS ITEM,isnull(G.EXPIRYDATE,'') as EXPIRYDATE " +
                " , ds.dispatchquantity as QTY,PACK.NAME as UOM,ds.price as price,ds.dispatchquantity * ds.price as TotPrice " +
                " from employee do, Bed B, Station S,drugorderdetailsubstitute ds " +
                " ,drugorder d,inpatient ip,  Company Co " +
                " ,employee em,employee e,employee emp,employee aemp, item i " +
                " ,BATCH G,PACKING PACK " +
                " Where co.ID = IP.CompanyId And emp.ID = d.pharmacistid " +
                    " and aemp.id=d.AsstPharmacistid and d.ipid = ip.ipid and d.id = ds.orderid " +
                    " and ds.substituteid = i.id and s.Id = d.StationId " +
                    " and b.id = d.bedid and do.id= ip.doctorid  and d.operatorid = e.id " +
                    " and d.operatorid=em.id " +
                    " AND DS.BATCHID=G.BATCHID AND DS.BATCHNO=G.BATCHNO AND DS.substituteid=G.ITEMID " +
                    " AND DS.UNITID=PACK.ID " +
                    " and D.tostationid=" + Gstationid + " " +
                    " and d.id=" + mOrderID;
                DataSet itemds = MainFunction.SDataSet(SUBSTR2, "tbL");
                int count = 1;
                foreach (DataRow rr in itemds.Tables[0].Rows)
                {
                    DrugOrderPrintDetail item = new DrugOrderPrintDetail();
                    item.slno = count;
                    item.ItemCode = rr["ITEMCODE"].ToString();
                    item.ItemName = rr["ITEM"].ToString().Substring(0, rr["ITEM"].ToString().Length - 1);
                    item.ExpiryDate = String.Format("{0:dd-MMM-yyyy}", (DateTime)rr["EXPIRYDATE"]);
                    item.Qty = String.Format("{0:0.00}", rr["QTY"].ToString());
                    item.Unit = rr["UOM"].ToString();
                    item.price = rr["price"].ToString();
                    item.price = String.Format("{0:N}", decimal.Parse(item.price));
                    item.TotPrice = rr["TotPrice"].ToString();
                    item.TotPrice = String.Format("{0:N}", decimal.Parse(item.TotPrice));
                    itemList.Add(item);
                    count += 1;
                }
                ll.Items = itemList;
                return ll;
            }
            catch (Exception e) { return ll; };
        }
        public static DrugOrderPrintView PrintOutlbl(int mOrderID, int Gstationid)
        {
            DrugOrderPrintView ll = new DrugOrderPrintView();
            try
            {
                string StrSql = " select ip.RegistrationNo,ds.orderid,e.employeeid,d.dispatcheddatetime, "
                    + " ip.firstname + ' ' + ip.middlename + ' ' + ip.lastname as patientname, "
                    + " isnull(i.strength_unit,'') as Strength_unit,i.name as item,"
                    + " i.duplicatelabel,BA.EXPIRYDATE,  n.NOTES, "
                    + " sum(ds.dispatchquantity) as DispatchQuantity, "
                    + " u.name as unit,isnull(b.name,'') as bed,ds.remarks,st.prefix as stprefix,ds.OrderId "
                    + " from drugorderdetailsubstitute ds, "
                    + " drugorder d,allinpatients ip,  packing u,BATCH BA, itemnotes n,employee e,item i,bed b,  station St "
                    + " Where d.ipid = ip.ipid And d.ID = ds.orderId  and ip.ipid = b.ipid and ds.unitid = u.id and ds.substituteid = i.id "
                    + " and i.id *= n.itemid  and d.tostationid = " + Gstationid
                    + " and BA.ITEMID=ds.ServiceId  and ds.batchid = ba.batchid and ds.batchno = ba.batchno AND d.tostationid = st.id "
                    + " and d.operatorid = e.id  and ds.OrderId= " + mOrderID
                    + " group by ip.RegistrationNo,ds.orderid,e.employeeid,d.dispatcheddatetime, ip.firstname "
                    + " ,ip.middlename, ip.lastname ,i.name , n.NOTES , u.Name, b.Name, ds.Remarks, St.prefix, "
                    + " i.strength_unit, i.duplicatelabel, BA.Expirydate, ds.dispatchquantity  "
                    + " order by b.name,ip.RegistrationNo, ds.orderid";

                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                List<DrugLableDetail> ddlist = new List<DrugLableDetail>();
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    DrugLableDetail dd = new DrugLableDetail();
                                                                                    
                                                                                                                                                                                             
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


                    ddlist.Add(dd);
                }
                ll.DrugLabels = ddlist;
                return ll;
            }
            catch (Exception e) { return ll; };
        }
    }

}





