using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class LabReceiptFun
    {
        public static LabModel Clear()
        {
            LabModel nn = new LabModel();
            return nn;
        }
        public static List<LabReceiptView> GetIssues()
        {
            List<LabReceiptView> ll = new List<LabReceiptView>();
            try
            {
                string StrSql = "select i.id,i.stationslno,p.stationslno as issueno,i.datetime,o.name as operator "
                    + " from preparationreceipt i,employee o,invlabissue p where i.issueid = p.id and i.operatorid = o.id";

                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    LabReceiptView iv = new LabReceiptView();
                    iv.ID = (int)rr["id"];
                    iv.ReceiptNo = rr["stationslno"].ToString();
                    iv.IssueNo = rr["issueno"].ToString();
                    iv.DateTime = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy");
                    iv.Operator = rr["operator"].ToString();
                    ll.Add(iv);
                }
                return ll;
            }
            catch (Exception e) { return ll; };

        }

        public static List<InitGrid> GetIssuesList(string StrSql)
        {
            List<InitGrid> ll = new List<InitGrid>();
            try
            {
                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    InitGrid iv = new InitGrid();
                                         iv.IssueId = (int)rr["id"];
                    iv.IssueNo = rr["stationslno"].ToString();
                    iv.DateTime = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy");
                    ll.Add(iv);
                }
                return ll;
            }
            catch (Exception e) { return ll; };

        }
        public static List<ReceiptItems> GetItems(string Str)
        {
            List<ReceiptItems> ll = new List<ReceiptItems>();

            try
            {
                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                int sno = 1;
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ReceiptItems l = new ReceiptItems();
                    l.SNO = sno;
                    l.Item = rr["itemname"].ToString();
                    l.ID = (int)rr["itemid"];
                    l.OrdQty = (int)rr["qty"];
                    l.UOM = rr["packname"].ToString();
                    l.UnitID = (int)rr["unitid"];
                    l.ReceivedQty = (int)rr["RecQty"];
                    l.PrvRecivedQty = (int)rr["PrvRecQty"];
                    l.CostPrice = Convert.ToDouble(rr["CostPrice"].ToString());
                    l.CostPrice = Math.Round(l.CostPrice, 2);
                    l.SellPrice = Convert.ToDouble(rr["SellingPrice"].ToString());
                    l.SellPrice = Math.Round(l.SellPrice, 2);
                    l.lGetQty = MainFunction.GetQuantity(l.UnitID, l.ID);
                    ll.Add(l);
                    sno += 1;
                }

                return ll;
            }
            catch (Exception e)
            {
                ReceiptItems f = new ReceiptItems();
                f.ErrMsg = e.Message;
                ll.Add(f);
                return ll;

            }
        }

        public static InitGrid Save(InitGrid order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
                using (Con)
                {

                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    DateTime todate = DateTime.Now;
                    int Gstationid = UserInfo.selectedStationID;
                    int MSTATUS = order.checkStatus;
                    int txtOrderNo = order.IssueId;
                    int gSavedById = UserInfo.EmpID;
                    string dtpReceiptDate = order.DateTime;

                    bool Update = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=27 and stationid=0", Trans);
                    int mMaxId = MainFunction.GetOneVal("select MaxId from invmax where slno=27 and stationid=0", "MaxId", Trans);
                    bool Update2 = MainFunction.SSqlExcuite("update invmax set maxid = maxid +1 where slno = 27 and stationid = " + Gstationid, Trans);
                    int mstOrderID = MainFunction.GetOneVal("select MaxId from invmax where slno = 27 and stationid= " + Gstationid, "MaxId", Trans);
                    bool Update3 = MainFunction.SSqlExcuite("update invlabissue set status = " + MSTATUS + " where id = " + txtOrderNo, Trans);

                    string StrSql = "Insert into preparationreceipt(id,issueid,stationid,receiptdate,datetime "
                    + " ,operatorid,stationslno) values (" + mMaxId + "," + txtOrderNo + ",'" + Gstationid
                    + "','" + dtpReceiptDate + "',sysdatetime()," + gSavedById + "," + mstOrderID + ")";
                    bool ins = MainFunction.SSqlExcuite(StrSql, Trans);

                                         long mTranId = MainFunction.SaveInTranOrder(Trans, Gstationid, mMaxId, mMaxId, 0, 27);
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

                    int sumqty = 0;
                    foreach (var itm in order.ReciptList)
                    {
                        if (itm.Qty > 0)
                        {
                                                         string Str = "Select itemid,BatchId from batch where itemid=" + itm.ID
                                    + " and ABS(CostPrice - " + itm.EPR + ") <= 0.05 "
                                    + " and ABS(SellingPrice - " + itm.SP + ") <= 0.05 ";
                            bool laddbatch = false;
                            int Batchid = 1;
                            DataSet rsTemp = MainFunction.SDataSet(Str, "tb", Trans);
                            if (rsTemp.Tables[0].Rows.Count == 0)
                            {
                                laddbatch = true;
                            }
                            else
                            {
                                foreach (DataRow rr in rsTemp.Tables[0].Rows)
                                {
                                    Batchid = (int)rr["BatchId"];
                                }
                            }
                            if (laddbatch == true)
                            {

                                                                                                  Str = "Insert into Batch(ItemId,BatchNo,ExpiryDate,Quantity,CostPrice,Tax,"
                                + " SellingPrice,StartDate,MRP,PurRate,unitepr) Values("
                                + itm.ID + ",'',null," + itm.Qty + "," + itm.EPR + ", 0,"
                                + itm.SP + ",sysdatetime()," + itm.SP + "," + itm.EPR
                                + "," + itm.EPR + ")";
                                bool ins1 = MainFunction.SSqlExcuite(Str, Trans);

                                int v = MainFunction.GetOneVal("Select max(batchId) AS BID FROM Batch", "BID", Trans);
                                if (v > 0)
                                {
                                    Batchid = v;
                                }

                            }
                            else
                            {
                                Str = "UPDATE Batch SET Quantity = Quantity + " + itm.Qty + ","
                                      + "CostPrice = " + itm.EPR + ","
                                      + "SellingPrice = " + itm.SP + ","
                                      + "Mrp = " + itm.SP + ","
                                      + "Tax = 0, "
                                      + "PurRate=" + itm.EPR
                                      + ",unitepr=" + itm.EPR
                                      + " WHERE BatchId = " + Batchid;
                                bool ins1 = MainFunction.SSqlExcuite(Str, Trans);
                            }

                            
                            Str = "Update Item Set SellingPrice=" + itm.SP + " where Id=" + itm.ID;
                            bool ins2 = MainFunction.SSqlExcuite(Str, Trans);

                            Str = "Insert into  preparationreceiptdetail(ReceiptId,ItemID,Qty,"
                                    + " unitid,costprice,sellingprice,batchid)"
                                    + " Values(" + mMaxId + "," + itm.ID + "," + itm.ReceivedQty
                                    + "," + itm.UnitID + "," + itm.EPR + "," + itm.SellPrice + "," + Batchid + ")";
                            bool ins3 = MainFunction.SSqlExcuite(Str, Trans);

                            string btno = "";
                            bool savintransdtl = MainFunction.SaveInTranOrderDetail(Trans, mTranId, itm.ID
                                 , itm.Qty, btno, Batchid.ToString(), itm.EPR, itm.SP, itm.EPR);
                            if (savintransdtl == false)
                            {
                                if (Con.State == ConnectionState.Open)
                                {
                                    Con.BeginTransaction().Rollback();
                                    Con.Close();
                                }
                                order.ErrMsg = "Error while saving!";
                                return order;


                            }

                            if (laddbatch == true)
                            {

                                Str = "Insert into BatchStore (BatchId,ItemId,BatchNo,Quantity,Stationid) Values("
                                    + Batchid + "," + itm.ID + ",''," + itm.Qty + "," + Gstationid + ")";
                            }
                            else
                            {
                                int btid = MainFunction.GetOneVal("SELECT BatchId FROM batchstore WHERE BatchId = " + Batchid
                                    + " AND StationId = " + Gstationid, "BatchId", Trans);
                                if (btid == 0)
                                {

                                    StrSql = "Insert into BatchStore (BatchId,ItemId,BatchNo,Quantity,Stationid) Values("
                                        + Batchid + "," + itm.ID + ",''," + itm.Qty + "," + Gstationid + ")";
                                }
                                else
                                {
                                    StrSql = "UPDATE BatchStore SET Quantity = Quantity + " + itm.Qty
                                        + " WHERE BatchId = " + Batchid + " AND StationId = " + Gstationid;
                                }
                            }
                            bool ins4 = MainFunction.SSqlExcuite(Str, Trans);

                        }


                    }



                    order.ErrMsg = "Record(s) Saved Successfully";
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




