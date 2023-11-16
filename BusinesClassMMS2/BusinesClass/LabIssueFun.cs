using System;
using System.Collections.Generic;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class LabIssueFun
    {
        public static LabModel Clear()
        {
            LabModel nn = new LabModel();
            return nn;
        }
        public static List<InitGrid> GetIssues()
        {
            List<InitGrid> ll = new List<InitGrid>();
            try
            {
                string StrSql = "select i.id,i.stationslno,s.name as deptname,o.name as operator, "
                    + " i.datetime,a.name as itemname "
                    + " from invlabissue i,item a,department s,employee o "
                    + " where i.departmentid = s.id and i.operatorid = o.id and i.itemid = a.id "
                    + " order by i.datetime desc";

                DataSet n = MainFunction.SDataSet(StrSql, "tbl");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    InitGrid iv = new InitGrid();
                    iv.IssueId = (int)rr["id"];
                    iv.IssueNo = rr["stationslno"].ToString();
                    iv.ItemName = rr["itemname"].ToString();
                    iv.Department = rr["deptname"].ToString();
                    iv.DateTime = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy");
                    iv.Operator = rr["operator"].ToString();
                    ll.Add(iv);
                }
                return ll;
            }
            catch (Exception e) { return ll; };

        }

        public static List<TempListMdl> GetItems(string Str)
        {
            List<TempListMdl> ll = new List<TempListMdl>();

            try
            {
                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    TempListMdl l = new TempListMdl();
                    l.ID = (int)rr["ID"];
                    l.Name = rr["Name"].ToString();
                    ll.Add(l);
                }

                return ll;
            }
            catch (Exception e)
            {
                TempListMdl f = new TempListMdl();
                f.ErrMsg = e.Message;
                ll.Add(f);
                return ll;

            }
        }
        public static List<ItemDtl> GetItemsDtl(string Str)
        {
            List<ItemDtl> ll = new List<ItemDtl>();

            try
            {
                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ItemDtl l = new ItemDtl();
                    l.Item = rr["Item"].ToString();
                    l.rQty = rr["rQty"].ToString();
                    l.QOH = (int)rr["QOH"];
                    ll.Add(l);
                }

                return ll;
            }
            catch (Exception e)
            {
                ItemDtl f = new ItemDtl();
                f.ErrMsg = e.Message;
                ll.Add(f);
                return ll;

            }
        }
        public static List<ProfileItems> InsertIssueItem(string Str, int Stationid)
        {
            List<ProfileItems> ll = new List<ProfileItems>();

            try
            {
                int Sno = 1;
                DataSet ds = MainFunction.SDataSet(Str, "tbl1");
                foreach (DataRow rr in ds.Tables[0].Rows)
                {
                    ProfileItems l = new ProfileItems();
                    l.SNO = Sno;
                    l.Drug = rr["name"].ToString();
                    l.Unit = rr["unit"].ToString();
                    l.Qty = 0;
                    l.UnitID = (int)rr["unitid"];
                    l.ID = (int)rr["itemid"];

                    
                                                                                   ll.Add(l);
                    Sno += 1;
                }

                return ll;
            }
            catch (Exception e)
            {
                ProfileItems f = new ProfileItems();
                f.ErrMsg = e.Message;
                ll.Add(f);
                return ll;

            }
        }
        public static InitGrid ViewDetails(int mOrderID, int gStationid)
        {
            InitGrid IndDetail = new InitGrid();
            List<ProfileItems> ll = new List<ProfileItems>();

            try
            {
                string StrSql = "select i.id,i.stationslno,i.unitid,u.name as unit,i.refno,a.name as itemname "
                + ", i.departmentid,i.itemid,i.qty,o.name as operator,i.datetime "
                + " from invlabissue i,employee o,packing u,item a "
                + " where i.unitid = u.id and i.itemid = a.id and i.operatorid = o.id and i.id = " + mOrderID;
                DataSet n = MainFunction.SDataSet(StrSql, "tbl1");
                int sno = 1;
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                    
                    IndDetail.IssueId = (int)rr["id"];
                    IndDetail.IssueNo = rr["stationslno"].ToString();
                    IndDetail.RefNo = rr["refno"].ToString();
                    IndDetail.DepartmentID = (int)rr["departmentid"];
                    IndDetail.Operator = rr["operator"].ToString();
                    IndDetail.DateTime = MainFunction.DateFormat(rr["datetime"].ToString(), "dd", "MM", "yyyy");

                    
                    ProfileItems dd = new ProfileItems();
                    dd.SNO = sno;
                    dd.Drug = rr["itemname"].ToString();
                    dd.Qty = (int)rr["qty"];
                    dd.Unit = rr["unit"].ToString();
                    dd.UnitID = (int)rr["unitid"];
                    dd.ID = (int)rr["itemid"];
                    sno += 1;
                    ll.Add(dd);
                }
                IndDetail.SelectedItems = ll;

                return IndDetail;

            }
            catch (Exception e)
            { IndDetail.ErrMsg = "Error when try to load details: " + e.Message; return IndDetail; }
        }

        public static InitGrid Save(InitGrid order, User UserInfo)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                
                foreach (var itm in order.SelectedItems)
                {

                    DataSet rsTemp = MainFunction.SDataSet("select lp.itemid,lp.qty,lp.unitid from labpreparationdetail lp, labpreparation l "
                    + " where lp.preparationid = l.id and l.stationid = " + UserInfo.selectedStationID
                    + " and l.itemid = " + itm.ID, "tbl");
                    foreach (DataRow rr in rsTemp.Tables[0].Rows)
                    {
                        int QOH = MainFunction.GetOneVal("select isnull(QOH,0) QOH from MMS_itemMaster where id="
                            + (int)rr["itemid"] + " and stationid=" + UserInfo.selectedStationID, "QOH");
                        int lGetQty = MainFunction.GetQuantity((int)rr["unitid"], (int)rr["itemid"]) * (int)itm.Qty * (int)rr["qty"];
                        if (lGetQty > QOH)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            order.ErrMsg = "Quantity larger than the QOH!";
                            return order;
                        }

                    }
                }



                
                using (Con)
                {

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
                    int mOrderID = 0;
                    bool Up1 = MainFunction.SSqlExcuite("update invmax set maxid=maxid+1 where slno=26 and stationid=0", Trans);
                    DataSet RUp1 = MainFunction.SDataSet("Select MaxID from invmax where slno=26 and stationid=0", "tbl", Trans);
                    foreach (DataRow rr in RUp1.Tables[0].Rows)
                    { mOrderID = (int)rr["MaxID"]; }

                    int mstOrderID = 0;
                    bool Up2 = MainFunction.SSqlExcuite("Update invmax set maxid = maxid +1 where slno = 26 and stationid = " + UserInfo.selectedStationID, Trans);
                    DataSet RUp2 = MainFunction.SDataSet("select maxid from invmax where slno = 26 and stationid = " + UserInfo.selectedStationID, "tbl", Trans);
                    foreach (DataRow rr in RUp2.Tables[0].Rows)
                    { mstOrderID = (int)rr["maxid"]; }

                    string StrSql = "Insert into invlabissue(ID,[datetime],OperatorID,refno,departmentid"
                    + ",itemid,qty,unitid,stationid,stationslno) Values("
                    + mOrderID + ",sysdatetime()," + UserInfo.EmpID + ",'" + order.RefNo.Trim() + "',"
                    + order.DepartmentID + "," + order.SelectedItems[0].ID + ","
                    + order.SelectedItems[0].Qty + "," + order.SelectedItems[0].UnitID
                    + "," + UserInfo.selectedStationID + "," + mstOrderID + ")";
                    bool Ins1 = MainFunction.SSqlExcuite(StrSql, Trans);

                    long mTranId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, mOrderID, mstOrderID, 1, 26);

                    foreach (var itm in order.arrItem)
                    {
                        StrSql = "insert into invlabissuedetail(issueid,itemid,qty,unitid,batchid,epr) values ("
                       + mOrderID + "," + itm.ID + "," + itm.Qty + "," + itm.UnitId + "," + itm.Batchid
                       + "," + itm.EPR + ")";
                        bool ins2 = MainFunction.SSqlExcuite(StrSql, Trans);

                        StrSql = "Update batchstore set Quantity=Quantity-" + itm.DedQty
                            + " where ItemId=" + itm.ID + " and batchid = " + itm.Batchid
                            + " and  stationid=" + UserInfo.selectedStationID;
                        bool up3 = MainFunction.SSqlExcuite(StrSql, Trans);

                        if (MainFunction.MazValidateBatchStoreQty(itm.ID, int.Parse(itm.Batchid), UserInfo.selectedStationID, Trans) == false)
                        {
                            if (Con.State == ConnectionState.Open)
                            {
                                Con.BeginTransaction().Rollback();
                                Con.Close();
                            }
                            order.ErrMsg = "Can't Issue Stock for some Item!";
                            return order;

                        }


                                                                           
                                                  
                        StrSql = "select costprice,sellingprice from batch where batchid = " + itm.Batchid;
                        DataSet btcost = MainFunction.SDataSet(StrSql, "tr", Trans);
                        if (btcost.Tables[0].Rows.Count == 0)
                        {
                            if (MainFunction.SaveInTranOrderDetail(Trans, mTranId, itm.ID, itm.DedQty,
                                itm.BatchNo, itm.Batchid, itm.Cost, itm.EPR) == false)
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
                        else
                        {
                            foreach (DataRow bb in btcost.Tables[0].Rows)
                            {

                                if (MainFunction.SaveInTranOrderDetail(Trans,
                                    mTranId, itm.ID, itm.DedQty, itm.BatchNo, itm.Batchid, (decimal)bb["costprice"]
                                    , (decimal)bb["sellingprice"], (decimal)bb["costprice"]) == false)
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




        public static bool InitBatchArray(ref InitGrid pt, int gStationId, SqlTransaction Trans = null)
        {
            try
            {
                List<arrItem> arrItem = new List<arrItem>();


                decimal mQuantity = 0;
                int lGetQty = 0;
                int lItemid = 0;
                for (int i = 0; i < pt.SelectedItems.Count; i++)
                {
                    string Str = "select lp.itemid,isnull(lp.qty,0) qty,lp.unitid "
                        + "  from labpreparationdetail lp, labpreparation l"
                        + " where lp.preparationid = l.id and l.stationid = " + gStationId
                        + " and l.itemid = " + pt.SelectedItems[i].ID;
                    DataSet rsTemp = MainFunction.SDataSet(Str, "tbL", Trans);
                    foreach (DataRow xx in rsTemp.Tables[0].Rows)
                    {

                        lGetQty = MainFunction.GetQuantity((int?)xx["unitid"], (int)xx["itemid"]);
                        mQuantity = (decimal)(pt.SelectedItems[i].Qty * lGetQty * (int)xx["qty"]);

                        string SS = "Select B.BatchNo,b.batchid,B.ItemId as ID,c.Quantity,isnull(B.costprice,0) costprice,expirydate,b.mrp,b.purRate "
                        + " from batch B,batchstore c "
                        + " where b.itemid=c.itemid and b.batchid=c.batchid and b.batchno=c.batchno "
                        + " and c.stationid=" + gStationId + " and c.Quantity > 0 and b.itemid=" + (int)xx["itemid"]
                        + " order by b.expirydate,B.startDate";
                        DataSet bt = MainFunction.SDataSet(SS, "tbl", Trans);

                        foreach (DataRow rr in bt.Tables[0].Rows)
                        {
                            if ((int)rr["ID"] == lItemid)
                            {
                                if ((int)rr["Quantity"] - mQuantity == 0)                                  {

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
                                One.Price = (decimal)rr["mrp"] * lGetQty;
                                One.Cost = (decimal)rr["costprice"] * lGetQty;
                                One.EPR = (decimal)rr["costprice"] * lGetQty;
                                One.UnitId = (int)xx["unitid"];
                                One.ExpDt = rr["expirydate"].ToString();

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
                                One.Price = (decimal)rr["mrp"] * lGetQty;
                                One.Cost = (decimal)rr["costprice"] * lGetQty;
                                One.EPR = (decimal)rr["costprice"] * lGetQty;
                                One.DedQty = (int)rr["Quantity"];
                                One.UnitId = (int)xx["unitid"];
                                One.ExpDt = rr["expirydate"].ToString();
                                arrItem.Add(One);
                            }





                        _MoVe_NextBatCH: ;
                        }

                    _ExitBatchLoop: ;


                        lItemid = (int)xx["itemid"];
                    } 
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



    } 
} 




