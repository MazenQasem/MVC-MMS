using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class MSFIssueFun
    {
        public static List<ViewList> GetList(ParamTable pm)
        {
            List<ViewList> ll = new List<ViewList>();
            try
            {
                string Sql = " select a.Type,ID,DoctorID,PPINNo,PName,DateTime,VisitID,Dispacthed,b.TRec ,b.DRec  from "
                + "( Select Type,ID,DoctorID,PPINNo,PName,DateTime,VisitID,Dispacthed From ( Select 'Medication' as Type,A.ID, A.DoctorID, "
                + " A.IssueAuthorityCode + '.' + Replicate('0',10-len(convert(varchar,A.RegistrationNo))) + "
                + " Convert(varchar,A.RegistrationNo) as PPINNo,B.FirstName+' ' + B.MiddleName + ' ' + B.LastName as PName, "
                + " A.VisitID , A.DateTime, Isnull(A.Dispacthed,'') as Dispacthed "
                + " From MSF_Prescription A Inner Join Patient B On A.RegistrationNo = B.RegistrationNo "
                + "  Where DateTime Between '" + pm.Sdate + "' And '" + DateTime.Parse(pm.Edate).AddDays(1) + "' and A.StationID = " + pm.gStationid + " And A.Deleted = 0 "
                + " Union All "
                + "  Select 'Procedure' as Type,A.ID,A.DoctorID,A.IssueAuthorityCode + '.' + "
                + " Replicate('0',10-len(convert(varchar,A.RegNo))) + Convert(varchar,A.RegNo) as PPINNo,B.FirstName+' ' + "
                + " B.MiddleName + ' ' + B.LastName as PName, A.VisitID, A.OrderDate, '' as Dispacthed "
                + " From OPOrder A Inner Join Patient B On A.RegNo = B.RegistrationNo Where OrderDate Between  "
                + " '" + pm.Sdate + "' And '" + DateTime.Parse(pm.Edate).AddDays(1) + "'and A.StationID = " + pm.gStationid + "  aND A.DELETED = 0) t ) as a "
                + " inner join (select aaa.Type,aaa.OrderID,pt.Firstname + ' ' + pt.MiddleName + ' ' + pt.FamilyName as ptName,aaa.isa,aaa.regno,aaa.dT,ttYpe= Case when aaa.Type=1 then 'Medication' else 'Procedure' end "
                + " ,aaa.Trec,aaa.drec"
                + " from (Select 1 as Type,A.OrderID,IsNull(TRec,0) as TRec,IsNull(DRec,0) as DRec ,a.dT ,a.isa,a.regno "
                + " From (Select OrderID,Count(*) as TRec ,max(msm.DateTime) as dT,msm.IssueAuthorityCode as isa,msm.RegistrationNo as regno "
                + " From MSF_PrescriptionDetail msf left join MSF_Prescription msm on msf.OrderID=msm.ID   Group By OrderID,msm.RegistrationNo ,msm.IssueAuthorityCode) as A "
                + " Left Outer Join   (Select OrderID,Count(*) as DRec   From MSF_PrescriptionDetail Where Dispatched = 1 Group By OrderID)as B  On A.OrderID = B.OrderID "
                + " Union All Select 2 as Type,A.OrderID,IsNull(TRec,0) as TRec,IsNull(DRec,0) as DRec ,a.dT,a.isa ,a.RegNo "
                + " From  (Select OrderID,Count(*) as TRec ,max(opm.OrderDate) as dT ,opm.IssueAuthorityCode as isa,opm.RegNo  From OPOrderdetail opd left join oporder opm on opd.OrderId=opm.Id  Group By OrderID,opm.RegNo,opm.IssueAuthorityCode ) as A "
                + " Left Outer Join  (Select OrderID,Count(*) as DRec From OPOrderdetail Where Billed = 1 Group By OrderID)as B "
                + " On A.OrderID = B.OrderID ) as aaa left join patient pt on aaa.regno=pt.Registrationno "
                + " where aaa.dT>='" + pm.Sdate + "' and aaa.dt<'" + DateTime.Parse(pm.Edate).AddDays(1) + "'  ) as b on a.id=b.orderid  Order By DateTime Desc ";
                DataSet ds = MainFunction.SDataSet(Sql, "tbl");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ViewList dtl = new ViewList();
                    dtl.ID = (int)rr["ID"];
                    dtl.PIN = rr["PPINNo"].ToString();
                    dtl.PTName = rr["PName"].ToString();
                    dtl.DateTime = rr["DateTime"].ToString();
                    dtl.Type = rr["Type"].ToString();
                    dtl.DRec = (int)rr["DRec"];
                    dtl.TRec = (int)rr["TRec"];
                    ll.Add(dtl);
                }

                return ll;
            }
            catch (Exception e) { return ll; }
        }
        public static MSFIssueMdl ViewDetails(ParamTable Param)
        {
            MSFIssueMdl IndDetail = new MSFIssueMdl();
            List<InsertItems> ll = new List<InsertItems>();
            try
            {
                string StrSql = "";
                int DocID = 0;
                if (Param.RecType == "Medication")
                {
                    StrSql = "select a.id,a.visitdatetime,b.FirstName + ' ' + b.MiddleName + ' ' + b.LastName  as PName, b.Age,c.Name as AgeType,  s.Name as sex, "
                    + " c1.Code + ' ' + c1.Name as CName, IsNull(b.address1,'') + ',' + IsNull(b.address2,'') as PAddress, "
                    + " d.ID as DID,d.EmpCode + ' - ' + d.Name as DName "
                    + " From clinicalvisit a inner join Patient b On a.RegistrationNo = b.RegistrationNo  and a.IssueAuthorityCode = b.IssueAuthorityCode "
                    + " Inner Join AgeType c On b.AgeType = c.ID Inner Join Sex s On b.sex = s.id Inner Join Company c1 on b.CompanyID = c1.ID "
                    + " Inner Join Doctor d On a.DoctorID = d.ID Inner join MSF_Prescription MS On a.RegistrationNo = MS.RegistrationNo and a.id=ms.visitid "
                    + " Where a.RegistrationNo =" + MainFunction.getRegNumber(Param.RegNo) + "  And MS.ID = " + Param.OrderID + "  Order By a.visitdatetime Desc ";
                    IndDetail.IsProcedure = false;
                }
                else
                {
                    StrSql = "select distinct b.FirstName + ' ' + b.MiddleName + ' ' + b.LastName  as PName, b.Age,c.Name as AgeType,  s.Name as sex, "
                    + " c1.Code + ' ' + c1.Name as CName, IsNull(b.address1,'') + ',' + IsNull(b.address2,'') as PAddress, "
                    + " d.ID as DID,d.EmpCode + ' - ' + d.Name as DName "
                    + " From clinicalvisit a inner join Patient b On a.RegistrationNo = b.RegistrationNo  and a.IssueAuthorityCode = b.IssueAuthorityCode "
                    + " Inner Join AgeType c On b.AgeType = c.ID Inner Join Sex s On b.sex = s.id Inner Join Company c1 on b.CompanyID = c1.ID "
                    + " Inner Join Doctor d On a.DoctorID = d.ID Inner join OPOrder O1 On a.RegistrationNo = O1.RegNo and a.id=O1.visitid "
                    + " Where a.RegistrationNo =" + MainFunction.getRegNumber(Param.RegNo)
                    + " and  O1.id= " + Param.OrderID
                    + "  Order By dname ";

                                                                                                                                                                                            IndDetail.IsProcedure = true;
                }
                DataSet DS = MainFunction.SDataSet(StrSql, "tbl");
                IndDetail.VisitListDate = new List<TempListMdl>();
                IndDetail.VisitListDoc = new List<TempListMdl>();
                foreach (DataRow rr in DS.Tables[0].Rows)
                {
                    IndDetail.OrderID = Param.OrderID;
                    IndDetail.PINNO = Param.RegNo;
                    IndDetail.Name = rr["PName"].ToString();
                    IndDetail.Age = rr["Age"].ToString() + "  " + rr["AgeType"].ToString();
                    IndDetail.Sex = rr["sex"].ToString();
                    IndDetail.Address = rr["PAddress"].ToString();
                    IndDetail.CompanyName = rr["CName"].ToString();
                    TempListMdl Docist = new TempListMdl();
                    Docist.ID = (int)rr["DID"];
                    Docist.Name = rr["DName"].ToString();
                    IndDetail.VisitListDoc.Add(Docist);
                    IndDetail.RegNo = MainFunction.getRegNumber(Param.RegNo);
                                     }




                if (Param.RecType == "Medication")
                {
                    string Sw = "Select top 1 c.doctorid DID  From clinicalvisit c inner join MSF_Prescription m On c.RegistrationNo = m.RegistrationNo "
                    + " Where c.RegistrationNo =" + MainFunction.getRegNumber(Param.RegNo)
                    + " And M.ID = " + Param.OrderID
                    + " and c.id=m.visitid "
                    + "   Order By c.ID Desc ";
                    IndDetail.CmbVisitListDoc = MainFunction.GetOneVal(Sw, "DID");
                    DocID = IndDetail.CmbVisitListDoc;
                }
                else
                {
                    string Sw = "select top 1 a.doctorid DID from clinicalvisit a Inner join OPOrder O1 On a.RegistrationNo = O1.RegNo  "
                    + " where RegistrationNo=" + MainFunction.getRegNumber(Param.RegNo)
                    + " and O1.id= " + Param.OrderID
                    + " and a.id=o1.visitid "
                    + " order by a.visitdatetime desc";
                    IndDetail.CmbVisitListDoc = MainFunction.GetOneVal(Sw, "DID");
                    DocID = IndDetail.CmbVisitListDoc;
                }

                if (Param.DocID > 0) { DocID = Param.DocID; }

                
                string StrSql2 = "";
                if (Param.RecType == "Medication")
                {
                    StrSql2 = "Select c.ID, c.VisitDateTime From clinicalvisit c inner join MSF_Prescription m On c.RegistrationNo = m.RegistrationNo "
                    + " Where c.RegistrationNo =" + MainFunction.getRegNumber(Param.RegNo) + " And c.DoctorID = " + DocID
                    + " and c.id=m.visitid And M.ID = " + Param.OrderID + "  Order By c.ID Desc ";
                }
                else
                {
                    StrSql2 = " Select c.ID, c.VisitDateTime From clinicalvisit c inner join OPOrder m On c.RegistrationNo = m.RegNo "
                    + " Where c.RegistrationNo = " + MainFunction.getRegNumber(Param.RegNo) + " And c.DoctorID = " + DocID
                    + " and c.id=m.visitid And m.Id = " + Param.OrderID + "  Order By c.Id Desc ";
                }
                DataSet VisitDS = MainFunction.SDataSet(StrSql2, "tbl");
                IndDetail.CmbVisitListDate = 0;
                foreach (DataRow rr in VisitDS.Tables[0].Rows)
                {
                    TempListMdl Viist = new TempListMdl();
                    Viist.ID = (int)rr["ID"];
                    Viist.Name = rr["VisitDateTime"].ToString();
                    if (IndDetail.CmbVisitListDate == 0) { IndDetail.CmbVisitListDate = (int)rr["ID"]; }
                    IndDetail.VisitListDate.Add(Viist);
                }


                                 IndDetail.ItemsList = new List<InsertItems>();
                string ItemStrSql = "";
                if (Param.RecType == "Medication")
                {
                    ItemStrSql = " Select a.OrderID,a.ItemID,a.qty,a.PackID,a.Dispatched ,b.Name as uom ,i.ItemCode ,i.Name "
                + " From MSF_PrescriptionDetail a,Packing b,item i "
                + " where  orderid = " + Param.OrderID + " And a.PackID = b.ID And a.Itemid = I.ID ";
                }
                else
                {
                    ItemStrSql = "Select B.ItemID,C.Code,C.Name,ReqDoctorId From OPOrder A Inner Join OPOrderDetail B On A.Id = B.OrderId "
                    + "  Inner Join OtherProcedures C On B.ItemID = C.ID Where A.RegNo = " + MainFunction.getRegNumber(Param.RegNo)
                    + " And A.VisitId = " + IndDetail.CmbVisitListDate + " And "
                    + "  A.DoctorId = " + DocID + "  And A.StationID =" + Param.gStationid + " And A.Id = " + Param.OrderID + " Order By C.Name ";
                }
                DataSet ItemDS = MainFunction.SDataSet(ItemStrSql, "tbl");
                foreach (DataRow rsTemp in ItemDS.Tables[0].Rows)
                {
                    if (Param.RecType == "Medication")
                    {
                        long QOH = 0;
                        string rsQohStr = "Select i.ID, i.Name as Name,b.batchid,i.drugtype,s.conversionqty,s.tax,"
                  + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.BatchNo, b.ExpiryDate,i.ITEMCODE,i.CSSDItem "
                  + " FROM Item i,ItemStore s ,packing u,batch b, BatchStore Bs "
                  + " where s.unitid=u.id and b.itemid=i.id "
                  + " and i.id=" + (int)rsTemp["ItemID"]
                  + " and I.Id = s.ItemId and b.batchid=bs.batchid "
                  + " and S.Stationid = " + Param.gStationid
                  + " and bs.itemid=i.id "
                  + " and B.BatchNo = Bs.BatchNo "
                  + " and Bs.StationId = " + Param.gStationid
                  + " and Bs.Quantity > 0 "
                  + " Order By B.Startdate";
                        DataSet rsQoh = MainFunction.SDataSet(rsQohStr, "tb");
                        foreach (DataRow rsQohR in rsQoh.Tables[0].Rows) { QOH += (int)rsQohR["Quantity"]; }
                        InsertItems item = new InsertItems();
                        item.ItemID = (int)rsTemp["ItemID"];
                        item.ItemCode = rsTemp["ItemCode"].ToString();
                        item.ItemName = rsTemp["ItemCode"].ToString() + " - " + rsTemp["Name"].ToString();
                        item.QtyIssue = (int)rsTemp["qty"];
                        item.IssueUnits = rsTemp["uom"].ToString();
                        item.BatchNo = "";
                        item.ExpiryDate = "";
                        item.QOH = (int)(long)QOH;
                        item.DispatchQty = (Boolean)rsTemp["Dispatched"];
                        item.Total = 0;
                        item.Price = 0;
                        item.ConversionQty = 1;
                        item.UnitID = (int)rsTemp["PackID"];
                        item.ReadOnly = true;
                        IndDetail.ItemsList.Add(item);

                    }
                    else
                    {

                        InsertItems item = new InsertItems();
                        item.ItemID = (int)rsTemp["ItemID"];
                        item.ItemName = rsTemp["Code"].ToString() + " - " + rsTemp["Name"].ToString();
                        item.ReadOnly = true;
                        IndDetail.ItemsList.Add(item);
                    }
                }

                
                string Docstr = "";
                if (Param.RecType == "Medication")
                {
                    Docstr = "Select ReqDoctorId From MSF_Prescription Where ID = " + Param.OrderID;
                }
                else
                {
                    Docstr = "Select ReqDoctorId From oporder Where ID = " + Param.OrderID;
                }
                DataSet ReQQ = MainFunction.SDataSet(Docstr, "tbL");
                foreach (DataRow Doctor in ReQQ.Tables[0].Rows)
                {
                    IndDetail.CmbReqDoctorID = (int)Doctor["ReqDoctorId"];
                }

                return IndDetail;
            }
            catch (Exception e) { IndDetail.ErrMsg = "Error when try to load details: " + e.Message; return IndDetail; }


        }
        public static MSFIssueMdl NewOrder(ParamTable Param)
        {
            MSFIssueMdl IndDetail = new MSFIssueMdl();
            List<InsertItems> ll = new List<InsertItems>();
            try
            {
                string StrSql = "";

                StrSql = "select a.id,a.visitdatetime,b.FirstName + ' ' + b.MiddleName + ' ' + b.LastName  as PName, b.Age,c.Name as AgeType,  s.Name as sex, "
                + " c1.Code + ' ' + c1.Name as CName, IsNull(b.address1,'') + ',' + IsNull(b.address2,'') as PAddress, "
                + " d.ID as DID,d.EmpCode + ' - ' + d.Name as DName , a.RegistrationNo ,a.IssueAuthorityCode "
                + " From clinicalvisit a inner join Patient b On a.RegistrationNo = b.RegistrationNo  and a.IssueAuthorityCode = b.IssueAuthorityCode "
                + " Inner Join AgeType c On b.AgeType = c.ID Inner Join Sex s On b.sex = s.id Inner Join Company c1 on b.CompanyID = c1.ID "
                + " Inner Join Doctor d On a.DoctorID = d.ID  "
                + " Where a.RegistrationNo =" + Param.RegNo + " And DateDiff(day,a.VisitDateTime,getdate())<=10  Order By a.visitdatetime Desc ";
                IndDetail.IsProcedure = false;

                DataSet DS = MainFunction.SDataSet(StrSql, "tbl");
                IndDetail.VisitListDate = new List<TempListMdl>();
                IndDetail.VisitListDoc = new List<TempListMdl>();
                if (DS.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow rr in DS.Tables[0].Rows)
                    {
                        IndDetail.OrderID = Param.OrderID;
                        IndDetail.PINNO = MainFunction.getUHID(rr["IssueAuthorityCode"].ToString(), rr["RegistrationNo"].ToString(), true);
                        IndDetail.Name = rr["PName"].ToString();
                        IndDetail.Age = rr["Age"].ToString() + "  " + rr["AgeType"].ToString();
                        IndDetail.Sex = rr["sex"].ToString();
                        IndDetail.Address = rr["PAddress"].ToString();
                        IndDetail.CompanyName = rr["CName"].ToString();
                        TempListMdl Docist = new TempListMdl();
                        Docist.ID = (int)rr["DID"];
                        Docist.Name = rr["DName"].ToString();
                        IndDetail.VisitListDoc.Add(Docist);
                                             }
                }
                else
                {

                    IndDetail.ErrMsg = "There is no visit within 10 days for this patient!";

                }

                return IndDetail;
            }
            catch (Exception e) { IndDetail.ErrMsg = "Error when try to load details: " + e.Message; return IndDetail; }


        }
        public static MSFIssueMdl GetVisitDate(ParamTable Param)
        {
            MSFIssueMdl IndDetail = new MSFIssueMdl();
            try
            {
                int DocID = Param.DocID;
                
                string StrSql2 = "";
                if (Param.RecType == "Medication")
                {
                    StrSql2 = "Select c.ID, c.VisitDateTime From clinicalvisit c  "
                    + " Where c.RegistrationNo =" + Param.RegNo + " And c.DoctorID = " + DocID
                    + " Order By c.ID Desc ";
                }
                else
                {
                    StrSql2 = " Select c.ID, c.VisitDateTime From clinicalvisit c  "
                    + " Where c.RegistrationNo = " + Param.RegNo + " And c.DoctorID = " + DocID
                    + " Order By c.Id Desc ";
                }
                DataSet VisitDS = MainFunction.SDataSet(StrSql2, "tbl");
                IndDetail.CmbVisitListDate = 0;
                IndDetail.VisitListDate = new List<TempListMdl>();
                foreach (DataRow rr in VisitDS.Tables[0].Rows)
                {
                    TempListMdl Viist = new TempListMdl();
                    Viist.ID = (int)rr["ID"];
                    Viist.Name = rr["VisitDateTime"].ToString();
                    if (IndDetail.CmbVisitListDate == 0) { IndDetail.CmbVisitListDate = (int)rr["ID"]; }
                    IndDetail.VisitListDate.Add(Viist);
                }
                return IndDetail;
            }
            catch (Exception e) { IndDetail.ErrMsg = "Error when try to load details: " + e.Message; return IndDetail; }

        }
        public static List<InsertItems> InsertItem(List<InsertItems> ll, int ItemID, int Gstationid)
        {
            List<InsertItems> tt = new List<InsertItems>();
            try
            {
                if (ll != null && ll.Count > 0)
                {
                    foreach (InsertItems ii in ll)
                    {
                        ii.UomList = ii.UomList.Replace("\"", "'");
                        tt.Add(ii);
                    }
                }  
                decimal tax = 0;
                int qoh = 0;
                long lLargeQty = 0;
                decimal cprice = 0;
                string Sql = "Select top 1 i.ID,i.Name,b.batchid,i.drugtype,isnull(s.conversionqty,0) as ConverstionQty,isnull(s.tax,0) as TAX,"
                    + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.BatchNo, b.ExpiryDate,i.ITEMCODE,i.CSSDItem "
                    + " FROM Item i,ItemStore s ,packing u,batch b, BatchStore Bs "
                    + " where s.unitid=u.id and b.itemid=i.id "
                    + " and i.id=" + ItemID
                    + " and I.Id = s.ItemId and b.batchid=bs.batchid "
                    + " and S.Stationid = " + Gstationid
                    + " and bs.itemid=i.id "
                    + " and B.BatchNo = Bs.BatchNo "
                    + " and Bs.StationId = " + Gstationid
                    + " and Bs.Quantity <> 0 "
                    + " Order By B.Startdate";

                DataSet Dst = MainFunction.SDataSet(Sql, "tbl");
                foreach (DataRow rsItem in Dst.Tables[0].Rows)
                {
                    qoh = MainFunction.GetOneVal("select QOH from mms_itemmaster where stationid=" + Gstationid + " and id=" + ItemID, "QOH");
                    tax = decimal.Parse(rsItem["TAX"].ToString()) * decimal.Parse("0.01");
                    cprice = (decimal)rsItem["mrp"] +
                            ((decimal)rsItem["mrp"] * (decimal)tax) *
                            (int)rsItem["ConverstionQty"];
                    lLargeQty = qoh / MainFunction.GetQuantity((int)rsItem["unitid"], ItemID);
                    InsertItems nn = new InsertItems();
                    nn.ItemID = ItemID;
                                         nn.ItemCode = rsItem["ITEMCODE"].ToString();
                    nn.ItemName = rsItem["ITEMCODE"].ToString() + " - " + rsItem["Name"].ToString();
                    nn.UomList = MainFunction.ListToSelect2Col("select p.id as ID,p.name as Name from packing p,itempacking i where i.packid=p.id and i.itemid=" + ItemID);
                    nn.BatchNo = rsItem["BatchNo"].ToString();
                    nn.ExpiryDate = MainFunction.DateFormat(rsItem["ExpiryDate"].ToString(), "dd", "MM", "yyyy");
                    nn.QOH = (int)lLargeQty;
                    nn.Price = Math.Round(cprice, 2);
                    nn.UnitID = (int)rsItem["unitid"];
                    nn.ConversionQty = (int)rsItem["ConverstionQty"];
                    nn.BatchID = (int)rsItem["batchid"];
                    nn.ReadOnly = false;
                    nn.ErrMsg = "";
                    tt.Add(nn);
                }
                return tt;
            }
            catch (Exception e)
            {
                if (tt.Count == 0)
                {
                    InsertItems nn = new InsertItems();
                    nn.ErrMsg = "this Item can't be load!";
                    tt.Add(nn);
                }
                return tt;
            }


        }
        public static InsertItems GetQtyItem(InsertItems ll)
        {
            try
            {
                if (ll.QtyIssue == 0) { return ll; }
                else if (ll.QtyIssue > ll.QOH) { ll.QtyIssue = ll.QOH; return ll; }
                else
                {

                    int lconvqty = MainFunction.GetQuantity(int.Parse(ll.IssueUnits), ll.ItemID);
                    int lGetQty = lconvqty;
                    int lSmallQty = lconvqty * ll.QOH;
                    decimal curSp = ll.Price * lGetQty;

                    if (ll.QtyIssue * lGetQty > lSmallQty) { ll.QtyIssue = ll.QtyIssue / lGetQty; }

                    if (ll.QOH * ll.ConversionQty < ll.QtyIssue * lGetQty)
                    { ll.QtyIssue = ll.QOH / lGetQty; }

                    ll.Total = ll.QtyIssue * lGetQty * ll.Price;
                }
                return ll;
            }
            catch (Exception e) { ll.ErrMsg = "can't convert item unit!"; return ll; }
        }
        public static MessageModel Save(MSFIssueMdl order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            MessageModel Msg = new MessageModel();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    int Gstationid = UserInfo.selectedStationID;
                    int gOperatorid = UserInfo.EmpID;
                    int MastOrderID = 0;
                    if (order.IsProcedure == false)
                    {
                        bool Upd1 = MainFunction.SSqlExcuite("update ordermaxid set maxid=maxid+1 where tableid=27 and stationid=0", Trans);
                        MastOrderID = MainFunction.GetOneVal("select maxid from ordermaxid where tableid=27 and  stationid=0 ", "maxid", Trans);
                        bool updSt = MainFunction.SSqlExcuite("update ordermaxid set maxid=maxid+1 where tableid=27 and stationid=" + Gstationid, Trans);
                        int mOrderID = MainFunction.GetOneVal("select maxid from ordermaxid where tableid=22 and stationid=" + Gstationid, "maxid", Trans);

                        string StrSql = "insert into MSF_Prescription(Id,RegistrationNo,IssueAuthorityCode,DoctorId,StationId,DateTime,VisitId,OperatorID, "
                            + " Deleted,Dispacthed,DeletedOperator,ReqDoctorId) "
                            + " values (" + MastOrderID + " ," + MainFunction.getRegNumber(order.PINNO) + ",'" + MainFunction.getAuthorityCode(order.PINNO) + "',"
                            + order.CmbVisitListDoc + "," + Gstationid + " ,sysdatetime()," + order.CmbVisitListDate + " ," + gOperatorid + ",0,0,0,"
                            + order.CmbReqDoctorID + ")";
                        bool InserMain = MainFunction.SSqlExcuite(StrSql, Trans);

                        foreach (InsertItems item in order.ItemsList)
                        {
                            if (item.ItemID > 0 && item.QtyIssue > 0)
                            {
                                string ItemStrSql = "Insert into MSF_PrescriptionDetail(OrderId,ItemId,qty,PackID,Dispatched,Deleted,DeletedOperator) "
                                + " Values( " + MastOrderID + "," + item.ItemID + "," + item.QtyIssue + "," + item.UnitID + ",0,0,0) ";
                                bool InsertItem = MainFunction.SSqlExcuite(ItemStrSql, Trans);
                            }
                        }
                    }
                    else
                    {
                        string StrSql = "Insert Into OPOrder(OrderDate,RegNo,DoctorId,VisitId,StationID,IssueAuthorityCode,OPERATORID,ReqDoctorId) "
                        + " Values (sysdatetime()," + MainFunction.getRegNumber(order.PINNO) + ", " + order.CmbVisitListDoc + "," + order.CmbVisitListDate
                        + "," + Gstationid + ",'" + MainFunction.getAuthorityCode(order.PINNO) + "'," + gOperatorid + "," + order.CmbReqDoctorID + ") ";
                        bool insMain = MainFunction.SSqlExcuite(StrSql, Trans);

                        MastOrderID = MainFunction.GetOneVal("Select IsNull(Max(Id),0)  As OrderID From OPOrder", "OrderID", Trans);

                        foreach (InsertItems item in order.ItemsList)
                        {

                            string ItemStrSql = "Insert Into OPOrderDetail(OrderId,Itemid,Quantity,Billed) "
                            + " Values( " + MastOrderID + "," + item.ItemID + ",0,0) ";
                            bool InsertItem = MainFunction.SSqlExcuite(ItemStrSql, Trans);
                        }
                    }
                    Trans.Commit();
                    Msg.Message = " Order No. " + MastOrderID + " saved successfully";
                    Msg.id = MastOrderID;
                    Msg.isSuccess = true;
                    return Msg;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Msg.Message = "Error while saving!";
                Msg.id = 0;
                Msg.isSuccess = false;
                return Msg;
            }

        }

        public static MessageModel DeleteOrderD(ParamTable order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            MessageModel Msg = new MessageModel();
            try
            {
                
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    int Gstationid = UserInfo.selectedStationID;
                    int gOperatorid = UserInfo.EmpID;
                    int MastOrderID = order.OrderID;

                    if (order.RecType == "0")
                    {
                        bool Upd1 = MainFunction.SSqlExcuite("Update MSF_Prescription Set Deleted = 1, DeletedOperator=" + gOperatorid
                            + ", DeleteReason='" + order.Reason.Trim() + "',DeletedDateTime = sysdatetime() Where ID = " + MastOrderID, Trans);
                        bool Upd2 = MainFunction.SSqlExcuite("Update MSF_PrescriptionDetail Set Deleted = 1, DeletedOperator=" + gOperatorid
                            + " Where OrderID =" + MastOrderID, Trans);
                    }
                    else
                    {
                        DataSet ds = MainFunction.SDataSet("select * From OPOrderDetail where Billed=1 and OrderID =" + MastOrderID, "tb", Trans);
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            Msg.Message = "Sorry this Order already billed!!";
                            Msg.id = 0;
                            Msg.isSuccess = false;
                            return Msg;
                        }
                        else
                        {
                            bool Upd1 = MainFunction.SSqlExcuite("Update OPOrder Set Deleted = 1, DeletedOperator=" + gOperatorid
                                + ", DeleteReason='" + order.Reason.Trim() + "'  Where ID = " + MastOrderID, Trans);
                            bool Upd2 = MainFunction.SSqlExcuite("Update OPOrderDetail Set Deleted = 1, DeletedDateTime = sysdatetime(),DeletedOperator="
                                + gOperatorid + " Where OrderID =" + MastOrderID, Trans);
                        }
                    }
                    Trans.Commit();
                    Msg.Message = " Order Deleted successfully";
                    Msg.isSuccess = true;
                    return Msg;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Msg.Message = "Error while Delete!";
                Msg.id = 0;
                Msg.isSuccess = false;
                return Msg;
            }

        }

        public static List<MSFreportHeader> PrintOut(List<ParamTable> Param, User UserInfo)
        {
            List<MSFreportHeader> tp = new List<MSFreportHeader>();
            int OrderType = 2; int OrderID = 0; int RecIndex = 0;

            foreach (ParamTable rec in Param)
            {
                string StrSql = "";
                if (rec.RecType == "0")                  {
                    StrSql = "  Select 'Medication' AS MtYPE, CountryName,VisitDateTime,E1.EmpCode as OCode, "
                        + " E1.FirstName + ' ' + E1.MiddleName + ' ' + E1.LastName as OName, "
                        + " D1.EmpCode as ReqCode,D1.Name as RDName,MorO, TT.Id,C1.Code as CCode,C1.Name as CName, "
                        + " IsNull(Dp.DeptCode,' ') as DeptCode,RevCentre, RegistrationNo,TT.Age,AgeType,TT.Sex,PName,DocCode, DName, "
                        + " TT.DateTime , ItemCode, IName, Qty, TT.empcode, OpName, SName"
                        + " FROM ( Select  CT.Name as CountryName,CL1.VisitDateTime,A.OperatorId,1 as MorO,"
                        + " A.Id,ReqDoctorId,P.CompanyId,I.ProfitCentreId as RevCentre,A.RegistrationNo,P.Age,A1.Name as AgeType, "
                        + " S2.Name as Sex,P.FirstName + ' ' + P.MiddleName + ' ' + P.LastName as PName,D.EmpCode as DocCode, "
                        + " D.Name as DName,A.DateTime,I.ItemCode,I.Name as IName,B.Qty,E.EmpCode,E.Name as OpName,S.Name as SName"
                        + " From MSF_Prescription A "
                        + " Inner Join MSF_PrescriptionDetail B On A.ID = B.OrderID "
                        + " Inner Join Patient P On A.RegistrationNo = P.RegistrationNo "
                        + " Inner join Doctor D On A.DoctorId = D.Id "
                        + "  Inner Join CLINICALVISIT CL1 On A.VisitId = CL1.Id "
                        + " LEFT OUTER Join Employee E On A.OperatorID = E.Id "
                        + " Inner Join Station S On A.StationID = S.Id "
                        + " Inner Join AgeType A1 On P.AgeType = A1.Id "
                        + " Inner Join Sex S2 On P.Sex = S2.Id "
                        + " Left Outer Join Nationality CT On P.Nationality = CT.Id "
                        + " Inner Join Item I On B.ItemId = I.Id "
                        + " Where A.ID =" + rec.OrderID + "  And B.Deleted = 0  )TT "
                        + " Left Outer Join Department Dp On TT.RevCentre = Dp.Id "
                        + " Inner Join Company C1 On TT.CompanyId = C1.Id "
                        + " Inner Join Doctor D1 On TT.ReqDoctorId = D1.Id "
                        + " Inner Join Employee E1 On TT.OperatorID = E1.ID "
                        + " Order By MorO,Dp.DeptCode,ItemCode ";
                }                 else
                {
                    StrSql = " Select 'Other Procedure'  AS MtYPE,CountryName,VisitDateTime,E1.EmpCode as OCode, "
                        + " E1.FirstName + ' ' + E1.MiddleName + ' ' + E1.LastName as OName,"
                        + " D1.EmpCode as ReqCode,D1.Name as RDName,MorO, TT.Id, C1.Code as CCode,C1.Name as CName,"
                        + " IsNull(Dp.DeptCode,'') as DeptCode, RevCentre, RegistrationNo,TT.Age,AgeType,TT.Sex,PName,DocCode, DName,"
                        + " TT.DateTime , ItemCode, IName, case when qty=0 then 1 else qty end as Qty, TT.empcode, OpName, sname"
                        + " FROM ( Select CT.Name as CountryName,CL1.VisitDateTime,A.OperatorId,2 as MorO, A.Id, A.ReqDoctorId,P.CompanyId, "
                        + " O.DepartmentID as RevCentre,A.RegNo as RegistrationNo,P.Age,A1.Name as AgeType,S2.Name as Sex, "
                        + " P.FirstName + ' ' + P.MiddleName + ' ' + P.LastName as PName,D.EmpCode as DocCode, D.Name as DName, "
                        + " A.OrderDate as DateTime,O.Code as ItemCode,O.Name as IName,B.Quantity as Qty,E.EmpCode,E.Name as OpName, "
                        + " S.Name as SName  From OPOrder A "
                        + " Inner Join OPOrderDetail B On A.ID = B.OrderId "
                        + " Inner Join Patient P On A.RegNo = P.RegistrationNo "
                        + " Inner join Doctor D On A.DoctorId = D.Id "
                        + " Inner Join CLINICALVISIT CL1 On A.VisitId = CL1.Id "
                        + " LEFT OUTER Join Employee E On A.OperatorID = E.Id  "
                        + " Inner Join Station S On A.StationID = S.Id "
                        + " Inner Join AgeType A1 On P.AgeType = A1.Id "
                        + " Inner Join Sex S2 On P.Sex = S2.Id "
                        + " Left Outer Join Nationality CT On P.Nationality = CT.Id "
                        + " Inner Join OtherProcedures O On B.ItemId = O.Id "
                        + " Where A.ID = " + rec.OrderID + "  and B.Deleted = 0 )TT "
                        + " Left Outer Join Department Dp On TT.RevCentre = Dp.Id  "
                        + " Inner Join Company C1 On TT.CompanyId = C1.Id  "
                        + " Inner Join Doctor D1 On TT.ReqDoctorId = D1.Id  "
                        + " Inner Join Employee E1 On TT.OperatorID = E1.ID  "
                        + " Order By MorO,Dp.DeptCode,ItemCode  ";
                }
                DataSet getMain = MainFunction.SDataSet(StrSql, "tbl");
                int Seq = 0;
                foreach (DataRow rr in getMain.Tables[0].Rows)
                {
                    Seq += 1;
                    if (OrderID != 0 && OrderID != rec.OrderID)                     {
                        OrderID = 0;
                        OrderType = 2;                           RecIndex += 1;
                        Seq = 1;
                    }

                                                                                                                                                  if (OrderID == rec.OrderID && OrderType == int.Parse(rec.RecType) && Seq == 20)                     {
                        OrderID = 0;
                        OrderType = 2;                           RecIndex += 1;
                    }

                    if (OrderID != rec.OrderID && OrderType != int.Parse(rec.RecType))
                    {
                        MSFreportHeader dtl = new MSFreportHeader();
                        dtl.Page = RecIndex + 1;
                        dtl.PatientPin = MainFunction.getUHID(UserInfo.gIACode, rr["RegistrationNo"].ToString(), true);
                        dtl.PatientName = rr["PName"].ToString();
                        dtl.StationName = rr["SName"].ToString().Length > 50 ? rr["SName"].ToString().Substring(0, 50) : rr["SName"].ToString();
                        dtl.Age = rr["Age"].ToString() + "   " + rr["AgeType"].ToString();
                        dtl.Sex = rr["Sex"].ToString();
                        dtl.Nationality = rr["CountryName"].ToString();
                        dtl.Company = rr["CCode"].ToString() + " - " + rr["CName"].ToString();
                        dtl.Company = dtl.Company.Length > 50 ? dtl.Company.Substring(0, 50) : dtl.Company;
                        dtl.DoctorName = rr["DocCode"].ToString() + " - " + rr["DName"].ToString();
                        dtl.DoctorName = dtl.DoctorName.Length > 50 ? dtl.DoctorName.Substring(0, 50) : dtl.DoctorName;
                        dtl.ReqDate = rr["DateTime"].ToString();
                        dtl.MSFNo = rr["Id"].ToString();
                        dtl.OrderTypeName = rr["MtYPE"].ToString();
                        if (rec.OtherType == "0")
                        {
                            dtl.Type = " [ X ] Emergency  ";
                        }
                        else
                        { dtl.Type = " [ X ] Routine  "; }

                        dtl.rDetail = new List<MSFreportDetail>();
                        MSFreportDetail ItemDetail = new MSFreportDetail();
                        ItemDetail.Seq = Seq;
                        ItemDetail.DeptCode = rr["DeptCode"].ToString();
                        ItemDetail.ItemCode = rr["ItemCode"].ToString();
                        ItemDetail.ItemName = rr["IName"].ToString();
                        ItemDetail.Qty = rr["Qty"].ToString();
                        dtl.TotQty += (int)rr["Qty"];
                        dtl.ReqDoctorName = rr["ReqCode"].ToString() + rr["RDName"].ToString();
                        dtl.ReqDoctorName = dtl.ReqDoctorName.Length > 30 ? dtl.ReqDoctorName.Substring(0, 30) : dtl.ReqDoctorName;
                        dtl.OperatorName = rr["OpName"].ToString().Length > 30 ? rr["OpName"].ToString().Substring(0, 30) : rr["OpName"].ToString();
                        dtl.rDetail.Add(ItemDetail);
                        OrderID = rec.OrderID;
                        OrderType = int.Parse(rec.RecType);
                        tp.Add(dtl);
                    }
                    else
                    {
                        MSFreportDetail ItemDetail = new MSFreportDetail();
                        ItemDetail.Seq = Seq;
                        ItemDetail.DeptCode = rr["DeptCode"].ToString();
                        ItemDetail.ItemCode = rr["ItemCode"].ToString();
                        ItemDetail.ItemName = rr["IName"].ToString();
                        ItemDetail.Qty = rr["Qty"].ToString();
                        tp[RecIndex].TotQty += (int)rr["Qty"];
                        tp[RecIndex].rDetail.Add(ItemDetail);

                    }

                }

            }

            return tp;

        }

        public static List<MSFPrintList> GetPrintList(ParamTable Parm, User UserInfo)
        {
            List<MSFPrintList> GPlist = new List<MSFPrintList>();
            string regNo = Parm.RegNo;
            DateTime DTPicker1 = DateTime.Parse(Parm.Sdate);
            DateTime DTPicker2 = DateTime.Parse(Parm.Edate);
            int Gstationid = UserInfo.selectedStationID;
            string strsql2 = " select a.Type,ID,DoctorID,PPINNo,PName,DateTime,VisitID,Dispacthed,b.TRec ,b.DRec  from "
            + "( Select Type,ID,DoctorID,PPINNo,PName,DateTime,VisitID,Dispacthed From ( Select 'Medication' as Type,A.ID, A.DoctorID, "
            + " A.IssueAuthorityCode + '.' + Replicate('0',10-len(convert(varchar,A.RegistrationNo))) + "
            + " Convert(varchar,A.RegistrationNo) as PPINNo,B.FirstName+' ' + B.MiddleName + ' ' + B.LastName as PName, "
            + " A.VisitID , A.DateTime, Isnull(A.Dispacthed,'') as Dispacthed "
            + " From MSF_Prescription A Inner Join Patient B On A.RegistrationNo = B.RegistrationNo "
            + "  Where DateTime Between '" + DTPicker1 + "' And '" + DTPicker2.AddDays(1) + "'  "
            + " and A.StationID = " + Gstationid + " And A.Deleted = 0 and a.RegistrationNo=" + regNo
            + " Union All "
            + "  Select 'Procedure' as Type,A.ID,A.DoctorID,A.IssueAuthorityCode + '.' + "
            + " Replicate('0',10-len(convert(varchar,A.RegNo))) + Convert(varchar,A.RegNo) as PPINNo,B.FirstName+' ' + "
            + " B.MiddleName + ' ' + B.LastName as PName, A.VisitID, A.OrderDate, '' as Dispacthed "
            + " From OPOrder A Inner Join Patient B On A.RegNo = B.RegistrationNo Where OrderDate Between  "
            + " '" + DTPicker1 + "' And '" + DTPicker2.AddDays(1) + "' and A.StationID = " + Gstationid + "  and a.regno=" + regNo + " and A.DELETED = 0) t ) as a "
            + " inner join (select aaa.Type,aaa.OrderID,pt.Firstname + ' ' + pt.MiddleName + ' ' + pt.FamilyName as ptName,aaa.isa,aaa.regno,aaa.dT,ttYpe= Case when aaa.Type=1 then 'Medication' else 'Procedure' end "
            + " ,aaa.Trec,aaa.drec"
            + " from (Select 1 as Type,A.OrderID,IsNull(TRec,0) as TRec,IsNull(DRec,0) as DRec ,a.dT ,a.isa,a.regno "
            + " From (Select OrderID,Count(*) as TRec ,max(msm.DateTime) as dT,msm.IssueAuthorityCode as isa,msm.RegistrationNo as regno "
            + " From MSF_PrescriptionDetail msf left join MSF_Prescription msm on msf.OrderID=msm.ID  WHERE MSM.REGISTRATIONNO=" + regNo
            + "  Group By OrderID,msm.RegistrationNo ,msm.IssueAuthorityCode) as A "
            + " Left Outer Join   (Select OrderID,Count(*) as DRec   From MSF_PrescriptionDetail Where Dispatched = 1 Group By OrderID)as B  On A.OrderID = B.OrderID "
            + " Union All Select 2 as Type,A.OrderID,IsNull(TRec,0) as TRec,IsNull(DRec,0) as DRec ,a.dT,a.isa ,a.RegNo "
            + " From  (Select OrderID,Count(*) as TRec ,max(opm.OrderDate) as dT ,opm.IssueAuthorityCode as isa,opm.RegNo  From OPOrderdetail opd left join oporder opm on opd.OrderId=opm.Id "
            + " WHERE OPM.REGNO=" + regNo + "  Group By OrderID,opm.RegNo,opm.IssueAuthorityCode ) as A "
            + " Left Outer Join  (Select OrderID,Count(*) as DRec From OPOrderdetail Where Billed = 1 Group By OrderID)as B "
            + " On A.OrderID = B.OrderID ) as aaa left join patient pt on aaa.regno=pt.Registrationno  "
            + " where aaa.dT>='" + DTPicker1 + "' and aaa.dt<'" + DTPicker2.AddDays(1) + "'  and aaa.regno=" + regNo + " ) as b on a.id=b.orderid  Order By DateTime Desc ";

            DataSet ds = MainFunction.SDataSet(strsql2, "tbl");
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                MSFPrintList ll = new MSFPrintList();
                ll.OrderNo = rr["ID"].ToString();
                ll.OrderType = rr["Type"].ToString();
                ll.PIN = rr["PPINNo"].ToString();
                ll.Name = rr["PName"].ToString();
                ll.OrderDate = MainFunction.DateFormat(rr["DateTime"].ToString(), "dd", "MM", "yyyy", "hh", "mm", "ss");
                GPlist.Add(ll);
            }



            return GPlist;



        }





    }

}





