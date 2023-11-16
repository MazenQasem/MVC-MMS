using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class IndentRetReceiptFun
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
        public static List<ReturnReceiptIndentView> GetIndents(ParamTable pm)
        {
            List<ReturnReceiptIndentView> ll = new List<ReturnReceiptIndentView>();
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

                string StrSql = " Select R.Id as ReturnNo,R.DateTime as ReturnDt,O.Name as ReturnedBy,S.Name as RetFrom, "
                                + " REC.RECBY,REC.RECDT,R.Status "
                                + " from IndentReturn R "
                                + " LEFT JOIN Station S ON R.sourceID=S.Id  "
                                + " LEFT JOIN Employee O ON R.OperatorId=O.Id "
                                + " LEFT JOIN  "
              + " (Select R.ReturnId ,R.DateTime as RecDt,O.Name as RecBy from ReturnReceipt R,employee o where R.OperatorId=O.Id ) AS REC "
                                + " ON R.ID=REC.RETURNID "
                                + " where R.destinationId = " + pm.gStationid
                                + " and R.Status < 3 and  "
                                + " ((r.status = 2 and r.datetime >= getdate()-30) or (r.status <> 2) )  "
                                + " order by  R.Status   ";



                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    ReturnReceiptIndentView iv = new ReturnReceiptIndentView();
                    iv.ReturnNo = rr["ReturnNo"].ToString();
                    iv.ReturnDate = MainFunction.DateFormat(rr["ReturnDt"].ToString(), "dd", "MMM", "yyyy");
                    iv.ReturnBy = rr["ReturnedBy"].ToString();
                    iv.ReturnFrom = rr["RetFrom"].ToString();
                    iv.ReceivedBy = rr["RECBY"].ToString();
                    iv.ReceivedAt = rr["RECDT"].ToString();
                    if (string.IsNullOrEmpty(iv.ReceivedAt) == false)
                    {
                        iv.ReceivedAt = MainFunction.DateFormat(rr["RECDT"].ToString(), "dd", "MMM", "yyyy");
                    }
                    iv.Status = (byte)rr["Status"];

                    ll.Add(iv);
                }






                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static IndentOrderModel ViewDetails(int mReturnId, int Gstationid, string OperatorName, int MStatus)
        {
            IndentOrderModel MainReturn = new IndentOrderModel();

            try
            {
                string StrSql = "Select S.Name as SourceSt,r.sourceid,E.Name as RetBy,R.DateTime as RetDt from "
                + " Station S,employee E,IndentReturn R where R.Id=" + mReturnId + " and R.SourceID=S.Id "
                + " and E.ID=R.OperatorID and r.destinationid=" + Gstationid;
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    if (MStatus > 0)
                    {
                        StrSql = "Select E.Name as Operator,R.DateTime as RecDt "
                            + " from ReturnReceipt R,Employee E  where E.ID=R.OperatorId";
                        DataSet Sxx = MainFunction.SDataSet(StrSql, "tbl");
                        foreach (DataRow xx in Sxx.Tables[0].Rows)
                        {
                            MainReturn.lbloperator = xx["Operator"].ToString();
                            MainReturn.lbldate = MainFunction.DateFormat(xx["RecDt"].ToString(), "dd", "MM", "yyyy");
                        }
                    }
                    else
                    {

                        MainReturn.lbloperator = OperatorName;
                        MainReturn.lbldate = MainFunction.DateFormat(DateTime.Now.ToString(), "dd", "MM", "yyyy", "hh", "mm", "ss");
                    }
                    MainReturn.lblReturnNo = mReturnId.ToString();
                    MainReturn.lblReturnfrom = rr["SourceSt"].ToString();
                    MainReturn.lblRetAt = MainFunction.DateFormat(rr["RetDt"].ToString(), "dd", "MM", "yyyy", "hh", "mm", "ss");
                    MainReturn.lblRetBy = rr["RetBy"].ToString();
                    MainReturn.SourceID = (int)rr["sourceid"];
                }

                StrSql = "Select p.name as unit,R.ItemId,R.Quantity,(I.itemcode + '-' + i.name) as Item,"
                + " R.BatchNo,B.ExpiryDate,i.conversionqty,Bs.Quantity as QOH,B.ExpiryDate,R.Remarks,"
                + " r.unitid,r.batchid  "
                + " from IndentReturnDetail R,Batch B,Item I,batchstore bs,packing p "
                + " where bs.batchno=b.batchno and r.unitid = p.id and b.itemid=bs.itemid and "
                + " R.ReturnID=" + mReturnId + " and R.ItemId=B.ItemId and B.BatchNo=R.BatchNo "
                + " and b.batchid=r.batchid and R.ItemId=I.Id and b.batchid = bs.batchid "
                + " and bs.stationid=" + Gstationid;
                DataSet ItQty = MainFunction.SDataSet(StrSql, "tbl");
                int sno = 1;
                List<ReturnReceiptDtl> ll = new List<ReturnReceiptDtl>();
                foreach (DataRow rr in ItQty.Tables[0].Rows)
                {
                    ReturnReceiptDtl yy = new ReturnReceiptDtl();
                    yy.SNO = sno;
                    yy.Item = rr["Item"].ToString();
                    yy.BatchNo = rr["BatchNo"].ToString();
                    yy.ExpiryDate = MainFunction.DateFormat(rr["ExpiryDate"].ToString(), "dd", "MM", "yy");
                    yy.QOH = (double)((int)rr["QOH"] / (int)rr["conversionqty"]);
                    yy.QtyRet = (double)(int)rr["Quantity"];
                    yy.RetRemarks = rr["Remarks"].ToString();
                    yy.QtyRec = (double)(int)rr["Quantity"];
                    yy.RecRemarks = "";
                    yy.Unit = rr["unit"].ToString();
                    yy.ConversionQty = MainFunction.GetQuantity((int)rr["unitid"], (int)rr["ItemId"]);                      yy.BatchID = (int)rr["batchid"];
                    yy.UnitID = (int)rr["unitid"];
                    yy.ItemID = (int)rr["ItemId"];

                    sno += 1;
                    ll.Add(yy);
                }
                MainReturn.ReturnItemList = ll;
                if (MStatus > 0)
                {
                    foreach (var item in MainReturn.ReturnItemList)
                    {
                        StrSql = "Select R.ItemID,r.batchid,R.Quantity,R.BatchNO,R.Remarks "
                            + " from ReturnReceiptDetail R where R.ReturnID = " + mReturnId;
                        DataSet re = MainFunction.SDataSet(StrSql, "tb");
                        foreach (DataRow rr in re.Tables[0].Rows)
                        {
                            if ((int)rr["ItemID"] == item.ItemID && rr["BatchNO"].ToString() == item.BatchNo)
                            {
                                item.QtyRec = (double)(int)rr["Quantity"];
                                item.RecRemarks = rr["Remarks"].ToString();
                                item.BatchID = (int)rr["batchid"];
                                goto Exit_loop;
                            }
                        }
                    Exit_loop:
                        sno = 0;                     }
                }

                return MainReturn;
            }
            catch (Exception e)
            {
                MainReturn.ErrMsg = "Error when try to load details: " + e.Message;
                return MainReturn;
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
                    int MSTATUS = order.Status;
                    int mReturnId = order.mReturnId;
                    int Gstationid = UserInfo.selectedStationID;
                    int SourceId = order.SourceID;
                    int gSavedById = UserInfo.EmpID;
                    long mTranId = 0;

                    
                    foreach (var itm in order.ReturnItemList)
                    {
                        if (itm.QtyRet > itm.QtyRec) { MSTATUS = 1; }
                        else { MSTATUS = 2; }
                    }

                    string StrSql = "Update IndentReturn Set Status=" + MSTATUS
                        + " where Id=" + mReturnId + " and status=0";
                    int Rec = MainFunction.SSqlExcuiteRecordNumber(StrSql, Trans);
                    if (Rec == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Error while saving!";
                        return order;
                    }

                    StrSql = "Insert into ReturnReceipt (ReturnId,DateTime,OperatorId,Stationid) "
                        + "  Values(" + mReturnId + ",sysdatetime()," + gSavedById + "," + Gstationid + " )";

                    bool INs1 = MainFunction.SSqlExcuite(StrSql, Trans);

                    mTranId = MainFunction.SaveInTranOrder(Trans, Gstationid, mReturnId, mReturnId, 0, 17, SourceId);
                    if (mTranId == 0)
                    {
                        if (Con.State == ConnectionState.Open)
                        {
                            Con.BeginTransaction().Rollback();
                            Con.Close();
                        }
                        order.ErrMsg = "Error while saving!";
                        return order;
                    }

                    foreach (var itm in order.ReturnItemList)
                    {
                        if (itm.QtyRet > 0)
                        {
                            StrSql = "Insert into ReturnReceiptDetail(ReturnID,ItemId,Quantity,BatchNo,"
                                + " Remarks,batchid) Values("
                            + mReturnId + "," + itm.ItemID + "," + itm.QtyRec + ",'" + itm.BatchNo + "','"
                            + itm.RecRemarks.Trim() + "'," + itm.BatchID + ")";
                            bool insdetail = MainFunction.SSqlExcuite(StrSql, Trans);


                            StrSql = "Update batchstore Set Quantity=Quantity+" + itm.QtyRec * itm.ConversionQty
                                + " where ItemId=" + itm.ItemID
                                + " and BatchNo='" + itm.BatchNo.Trim()
                                + "'and batchid=" + itm.BatchID
                                + " and stationid = " + Gstationid;
                            bool updatebatchstore = MainFunction.SSqlExcuite(StrSql, Trans);


                            StrSql = "select costprice,sellingprice from batch where batchid = " + itm.BatchID;
                            DataSet costdst = MainFunction.SDataSet(StrSql, "tbl", Trans);
                            if (costdst.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow rr in costdst.Tables[0].Rows)
                                {

                                    if (MainFunction.SaveInTranOrderDetail(Trans, mTranId, itm.ItemID, (long)(itm.QtyRec * itm.ConversionQty),
                                        itm.BatchNo, itm.BatchID.ToString(), (decimal)rr["costprice"], (decimal)rr["sellingprice"], (decimal)rr["costprice"]) == false)
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
                            else
                            {
                                if (MainFunction.SaveInTranOrderDetail(Trans, mTranId, itm.ItemID, (long)(itm.QtyRec * itm.ConversionQty),
                                      itm.BatchNo, itm.BatchID.ToString()) == false)
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
                    }


                    order.ErrMsg = "Return No " + mReturnId + " saved sucessfully ";


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

    }

}





