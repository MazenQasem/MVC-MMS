using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Text;


namespace MMS2
{

    public class DirectIpIssueFun
    {
                                                      
                  
                                             
         
         
        public static List<Selec2List> LoadItemsDirectIPIssue(int Cat, int StationId, string ItemCode)         {

            var doctors = new List<Selec2List>();
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("select   distinct ID,  ItemCode +' - '+ Name as Name , drugtype  from mms_itemmaster  where deleted=0  and categoryid= '" + Cat + "'    and StationID=  " + StationId + "   and QOH > 0 and (itemcode like  '%" + ItemCode + "%'   or name like  '%" + ItemCode + "%' )   order by name  ");
                                                                                    doctors = MainFunction.ExecuteSQLAndReturnDataTable(query.ToString()).DataTableToList<Selec2List>();
                return doctors;
            }
            catch (Exception ex)
            {
                return doctors;
            }


        }

        public static bool InitBatchArray(ref DirectIpSaveModel pt, int gStationId, SqlTransaction Trans = null)
        {
            try
            {
                List<arrItemDirectIp> arrItem = new List<arrItemDirectIp>();


                decimal mQuantity = 0;
                int lGetQty = 0;
                int lItemid = 0;
                double IssueQty = 0;



                for (int i = 0; i < pt.IssueList.Count; i++)
                {

                    lGetQty = MainFunction.GetQuantityDirectIpIssue((int?)pt.IssueList[i].IssueUnitID, (int)pt.IssueList[i].ID);
                    IssueQty = (double)pt.IssueList[i].PrevQty;
                    mQuantity = (decimal)IssueQty * lGetQty;


                    if (IssueQty > 0)
                    {
                        string SS = "Select b.mrp,b.costprice,Bs.Quantity,B.ItemId AS Id,i.itemcode,i.name,B.BatchNo,b.batchid, b.expirydate,B.TAX   "
                            + " FROM batch b, BatchStore Bs,mms_itemmaster i  "
                        + " where B.ItemId = '" + pt.IssueList[i].ID + "' and Bs.ItemId = '" + pt.IssueList[i].ID + "' and B.BatchNo = Bs.BatchNo  "
                        + " and B.Batchid = Bs.Batchid  and bs.itemid=i.id and b.itemid = i.id  "
                        + " and Bs.StationId = '" + gStationId + "' "
                        + " and Bs.Quantity > 0  ";
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
                                arrItemDirectIp One = new arrItemDirectIp();
                                int tax1 = 0;
                                string TextTax = rr["TAX"].ToString();
                                                                 if (TextTax == "0.00" || TextTax == "0")
                                {
                                    tax1 = 0;
                                }
                                else
                                {
                                    tax1 = Convert.ToInt32(TextTax == "" ? "0" : TextTax);
                                }

                                
                                One.ID = (int)rr["ID"];
                                One.BatchNo = (string)rr["BatchNo"].ToString();
                                One.Batchid = (string)rr["Batchid"].ToString();
                                One.Qty = (decimal)(mQuantity / lGetQty);
                                One.DedQty = (int)(decimal)(One.Qty * lGetQty);
                                One.UnitId = (int)pt.IssueList[i].unitId;
                                One.Tax = tax1;
                                One.mrp = (decimal)((decimal)rr["MRP"] * (int)lGetQty) * Convert.ToInt32(IssueQty);
                                One.cprice = (decimal)((decimal)rr["costprice"] * (int)lGetQty);
                                One.TotalAmount = (decimal)((decimal)rr["MRP"] * (int)lGetQty) + ((decimal)rr["MRP"] * (int)lGetQty * (int)One.Tax / 100);
                                One.Price = (decimal)One.TotalAmount;
                                One.Remarks = (string)pt.IssueList[i].Remarks;
                                One.BillableQty = (decimal)One.Qty;
                                One.BillableUnitId = (int)One.UnitId;
                                One.BillablePrice = (decimal)One.TotalAmount;

                                arrItem.Add(One);
                                
                                goto _ExitBatchLoop;
                            }
                            else
                            {
                                arrItemDirectIp One = new arrItemDirectIp();

                                int tax1 = 0;
                                string TextTax = rr["TAX"].ToString();
                                                                 if (TextTax == "0.00" || TextTax == "0")
                                {
                                    tax1 = 0;
                                }
                                else
                                {
                                    tax1 = Convert.ToInt32(TextTax == "" ? "0" : TextTax);
                                }

                                mQuantity = (decimal)mQuantity - (decimal)rr["Quantity"];
                                One.ID = (int)rr["ID"];
                                One.BatchNo = (string)rr["BatchNo"].ToString();
                                One.Batchid = (string)rr["Batchid"].ToString();
                                One.Qty = (decimal)(mQuantity / lGetQty);
                                One.DedQty = (int)(decimal)(One.Qty * lGetQty);
                                One.UnitId = (int)pt.IssueList[i].unitId;
                                One.Tax = tax1;
                                One.mrp = (decimal)((decimal)rr["MRP"] * (int)lGetQty);
                                One.cprice = (decimal)((decimal)rr["costprice"] * (int)lGetQty);
                                One.TotalAmount = (decimal)((decimal)rr["MRP"] * (int)lGetQty) + ((decimal)rr["MRP"] * (int)lGetQty * (int)One.Tax / 100);
                                One.Price = (decimal)One.TotalAmount;
                                One.Remarks = (string)pt.IssueList[i].Remarks;
                                One.BillableQty = One.Qty;
                                arrItem.Add(One);
                                                                                                                                                                                                                                                                                                                                                                                                                                             
                            }





                        _MoVe_NextBatCH: ;
                        }

                    _ExitBatchLoop: ;

                    }                     lItemid = pt.IssueList[i].ID;
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



        public static DirectIpSaveModel Save(DirectIpSaveModel order, User UserInfo)
        {

            SqlConnection Con = MainFunction.MainConn();
            try
            {

                
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
                    bool Up1 = MainFunction.SSqlExcuite("update ordermaxid set maxid=maxid+1 where tableid=22 and stationid=0", Trans);
                    DataSet RUp1 = MainFunction.SDataSet("select maxid from ordermaxid where tableid=22 and  stationid=0", "tbl", Trans);
                    foreach (DataRow rr in RUp1.Tables[0].Rows)
                    { mOrderID = (int)rr["MaxID"]; }


                    int mstOrderID = 0;
                    bool Up2 = MainFunction.SSqlExcuite("update ordermaxid set maxid=maxid+1 where tableid=22 and stationid = " + UserInfo.selectedStationID, Trans);
                    DataSet RUp2 = MainFunction.SDataSet("select maxid from ordermaxid where tableid=22 and stationid = " + UserInfo.selectedStationID, "tbl", Trans);
                    foreach (DataRow rr in RUp2.Tables[0].Rows)
                    { mstOrderID = (int)rr["maxid"]; }


                    int cssditem = 0;
                    for (int i = 0; i < order.IssueList.Count; i++)
                    {

                        if (order.IssueList[i].CSSDItem == true)
                        {
                            cssditem = 1;
                        }

                    }

                    StringBuilder StrSqlq = new StringBuilder();
                    StrSqlq.Append("Insert into DrugOrder  ");
                    StrSqlq.Append("(id,IPID,StationId,OperatorID,DoctorID,DateTime,BedId,Profileid,DispatchedDatetime,Dispatched,ToStationId,stationslno,ordertype,printstatus,CSSDItem) Values ");
                    StrSqlq.Append("(");
                    int ordertype = order.CatId == 17 ? 1 : 0;                      StrSqlq.Append("  '" + mOrderID + "'");                     StrSqlq.Append(", '" + order.IpId.ToString() + "' ");                     StrSqlq.Append(", '" + UserInfo.selectedStationID.ToString() + "' ");                     StrSqlq.Append(", '" + UserInfo.EmpID + "' ");                     StrSqlq.Append(", '" + order.DocID + "' ");                     StrSqlq.Append(", sysdatetime() ");                     StrSqlq.Append(", '" + order.BedId + "' ");                     StrSqlq.Append(", '0' ");                     StrSqlq.Append(",  sysdatetime() ");                     StrSqlq.Append(", '3' ");                     StrSqlq.Append(", '" + UserInfo.selectedStationID.ToString() + "' ");                     StrSqlq.Append(", '" + mstOrderID + "' ");                     StrSqlq.Append(", '" + ordertype + "' ");                     StrSqlq.Append(", '1' ");                     StrSqlq.Append(", '" + cssditem + "' ");                     StrSqlq.Append(")");

                    bool Insert = MainFunction.SSqlExcuite(StrSqlq.ToString(), Trans);
                    long Pino = Int64.Parse(order.IpId.ToString());
                                         long mTransId = MainFunction.SaveInTranOrder(Trans, UserInfo.selectedStationID, mOrderID, mstOrderID, 1, 6, UserInfo.selectedStationID, "IP", mstOrderID, Pino);

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
                    string StrSql = "";
                    foreach (var item in order.arrItem)
                    {
                                                 int unitID = 0;
                        foreach (var issuelist in order.IssueList)
                        {
                            if (item.ID == issuelist.ID)
                            {
                                unitID = issuelist.IssueUnitID;
                            }
                        }
                         

                        StringBuilder Str = new StringBuilder();
                        Str.Append("Insert into DrugOrderDetailSubstitute  ");
                        Str.Append("(OrderID,ServiceID,DispatchQuantity,Substituteid,BatchNo,price,UnitId,batchid,epr,remarks,BillableQty,BillableUnitId,BillablePrice) Values (");

                        Str.Append(" '" + mOrderID + "'  ");
                        Str.Append(" ,'" + item.ID + "'   ");
                        Str.Append(" , '" + item.Qty + "'  ");
                        Str.Append(" , '" + item.ID + "'  ");
                        Str.Append(", '" + item.BatchNo + "' ");
                        Str.Append(", '" + item.Price + "'   ");                          Str.Append(" ,'" + unitID + "'   ");
                        Str.Append(", '" + item.Batchid + "'   ");
                        Str.Append(", '" + item.cprice + "'   ");
                        Str.Append(", '" + item.Remarks + "'   ");
                        Str.Append(" ,'" + item.BillableQty + "'   ");
                        Str.Append(", '" + unitID + "'   ");
                        Str.Append(", '" + item.BillablePrice + "'   ");
                        Str.Append("  ) ");

                        bool inseDtl = MainFunction.SSqlExcuite(Str.ToString(), Trans);


                        StrSql = "Update  BatchStore Set Quantity=Quantity-" + item.DedQty + " where ItemId=" + item.ID
                            + " and BatchId=" + item.Batchid + " and batchno='" + item.BatchNo + "' "
                            + " and StationId=" + UserInfo.selectedStationID;

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
                                if (MainFunction.SaveInTranOrderDetail(Trans, mTransId, item.ID, item.DedQty, item.BatchNo, item.Batchid, (decimal)rr["costprice"], (decimal)rr["sellingprice"], (decimal)rr["costprice"]) == false)
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
                            string StrSql123 = "Insert into TransOrderDetail_" + DateTime.Now.Year + "_" + DateTime.Now.Month + "(IdKey,ItemId,Quantity,Batchno,BatchId,CP,MRP,EPR) Values(" + Trans + "," + item.ID + "," + item.DedQty + ",'" + item.BatchNo + "'," + item.Batchid + "," + item.cprice + "," + item.mrp + "," + item.cprice + ")";
                            bool nn = MainFunction.SSqlExcuite(StrSql123, Trans);

                            if (nn == false)
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
                    order.ErrMsg = "Issue Number " + mstOrderID + "  saved sucessfully ";

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


        public static DirectIpSaveModel ViewDetails(int mDeptOrderid, int gStationId)
        {
            DirectIpSaveModel ll = new DirectIpSaveModel();
            try
            {

                string StrSql = "  	select  I.Name, I.QOH as Qty,OD.Price,OD.DispatchQuantity as PrevQty, (OD.DispatchQuantity * OD.Price) as Amount, pk.Name as UnitName,OD.BillableQty,OD.BillableUnitId"
                                  + "    ,OD.remarks,OD.UnitId,OD.ServiceId	  from DrugOrderDetailSubstitute OD"
                                  + "    left join  mms_itemmaster I  on  I.ID=OD.substituteid"
                                   + "   left join DrugOrder DO on OD.OrderId = Do.ID  "
                                   + "   left join packing pk on pk.ID = OD.UnitId "
                                   + "   where OD.OrderId = '" + mDeptOrderid + "'   and I.stationid = " + gStationId + "  and DO.StationID = " + gStationId + "  and DO.ID = '" + mDeptOrderid + "' ";
                                                                                                                                        
                DataSet items = MainFunction.SDataSet(StrSql, "tbl1");
                List<IndentIssueInsertedItemListDirectIp> IssueList = new List<IndentIssueInsertedItemListDirectIp>();
                int sn = 1;
                foreach (DataRow rr in items.Tables[0].Rows)
                {
                    IndentIssueInsertedItemListDirectIp xx = new IndentIssueInsertedItemListDirectIp();
                    xx.SNO = sn;
                    xx.Name = rr["Name"].ToString();
                    xx.quantity = Convert.ToInt32(rr["Qty"]);
                    xx.mrp = Convert.ToDouble(rr["price"]);
                    xx.PrevQty = Convert.ToInt32(rr["PrevQty"]);
                    xx.Unit = rr["UnitName"].ToString();
                    xx.Amount = float.Parse(rr["Amount"].ToString());
                    xx.BillQty = Convert.ToInt32(rr["BillableQty"]);
                    xx.BillUnit = rr["UnitName"].ToString();
                    xx.ID = Convert.ToInt32(rr["ServiceId"]);
                    xx.Remarks = rr["remarks"].ToString();


                                                                                                                                                                                                                 IssueList.Add(xx);
                    sn += 1;

                }

                string StrSql1 = " Select I.DoctorId,I.OperatorId,I.IPID,B.Name as BedName,P.FirstName+ P.MiddleName+P.LastName as Name,P.Sex,P.SexOthers,P.Age,O.Name as Operator, "
                                     + "  D.Name as Doctor,I.DateTime as [DateTime],i.profileid,I.stationslno,isnull(p.vip,0) as VIP "
                                     + "  from DrugOrder I,Employee O,Doctor D,Inpatient P,Bed B where  B.Id=I.BedId and O.ID=I.OperatorId and P.IPId=I.IPID and D.ID=I.DoctorId "
                                     + "  and I.Id='" + mDeptOrderid + "' ";
                DataSet n = MainFunction.SDataSet(StrSql1, "tbl1");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                                                                                    
                    ll.IpId = rr["IPID"].ToString();                                          ll.OperatorId = rr["OperatorId"].ToString();                     ll.DocID = Convert.ToInt32(rr["DoctorId"]);                     ll.DateTime = rr["DateTime"].ToString();                                                                                                                                                                                             ll.BedName = rr["BedName"].ToString();                     ll.OperatorName = rr["Operator"].ToString();                     ll.Sex = Convert.ToInt32(rr["Sex"]);                     ll.Age = Convert.ToInt32(rr["Age"]); 
                    ll.OrderID = Convert.ToInt32(mDeptOrderid); 
                }



                ll.IssueList = IssueList;

                return ll;
            }
            catch (Exception e) { ll.ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }


        public static DirectIpSaveModel PrintOut(int mDeptOrderid, int gStationId)
        {
            DirectIpSaveModel ll = new DirectIpSaveModel();
            try
            {

                string StrSql = "  	select  I.Name, I.QOH as Qty,OD.Price,OD.DispatchQuantity as PrevQty, (OD.DispatchQuantity * OD.Price) as Amount, pk.Name as UnitName,OD.BillableQty,OD.BillableUnitId"
                                  + "    ,OD.remarks,OD.UnitId,OD.ServiceId	  from DrugOrderDetailSubstitute OD"
                                  + "    left join  mms_itemmaster I  on  I.ID=OD.substituteid"
                                   + "   left join DrugOrder DO on OD.OrderId = Do.ID  "
                                   + "   left join packing pk on pk.ID = OD.UnitId "
                                   + "   where OD.OrderId = '" + mDeptOrderid + "'   and I.stationid = " + gStationId + "  and DO.StationID = " + gStationId + "  and DO.ID = '" + mDeptOrderid + "' ";
                                                                                                                                        
                DataSet items = MainFunction.SDataSet(StrSql, "tbl1");
                List<IndentIssueInsertedItemListDirectIp> IssueList = new List<IndentIssueInsertedItemListDirectIp>();
                int sn = 1;
                foreach (DataRow rr in items.Tables[0].Rows)
                {
                    IndentIssueInsertedItemListDirectIp xx = new IndentIssueInsertedItemListDirectIp();
                    xx.SNO = sn;
                    xx.Name = rr["Name"].ToString();
                    xx.quantity = Convert.ToInt32(rr["Qty"]);
                    xx.mrp = Convert.ToInt32(rr["price"]);
                    xx.PrevQty = Convert.ToInt32(rr["PrevQty"]);
                    xx.Unit = rr["UnitName"].ToString();
                    xx.Amount = Convert.ToInt32(rr["Amount"]);
                    xx.BillQty = Convert.ToInt32(rr["BillableQty"]);
                    xx.BillUnit = rr["UnitName"].ToString();
                    xx.ID = Convert.ToInt32(rr["ServiceId"]);
                    xx.Remarks = rr["remarks"].ToString();


                                                                                                                                                                                                                 IssueList.Add(xx);
                    sn += 1;

                }

                string StrSql1 = " Select (P.issueauthoritycode +'.' + REPLICATE('0',10-(LEN(CONVERT(Varchar(10),P.registrationno)))) + CONVERT(Varchar(10),P.registrationno)) as RegNo , I.DoctorId,I.OperatorId,I.IPID, B.Name as BedName,P.FirstName+' '+ P.MiddleName+' '+P.LastName as Name,P.Sex,P.SexOthers,P.Age,O.Name as Operator,"
                                     + "  D.Name as Doctor,I.DateTime as [DateTime],i.profileid,I.stationslno,isnull(p.vip,0) as VIP "
                                     + "  from DrugOrder I,Employee O,Doctor D,Inpatient P,Bed B where  B.Id=I.BedId and O.ID=I.OperatorId and P.IPId=I.IPID and D.ID=I.DoctorId "
                                     + "  and I.Id='" + mDeptOrderid + "' ";
                DataSet n = MainFunction.SDataSet(StrSql1, "tbl1");
                foreach (DataRow rr in n.Tables[0].Rows)
                {
                                                                                    
                    ll.IpId = rr["IPID"].ToString();                                          ll.OperatorId = rr["OperatorId"].ToString();                     ll.DocID = Convert.ToInt32(rr["DoctorId"]);                     ll.DateTime = rr["DateTime"].ToString();                                                                                                                                                                         
                    ll.PatientName = rr["Name"].ToString();
                    ll.DoctorName = rr["Doctor"].ToString();                     ll.PinNo = rr["RegNo"].ToString();                     ll.BedName = rr["BedName"].ToString();                     ll.OperatorName = rr["Operator"].ToString();                     ll.Gender = (string)(Convert.ToInt32(rr["Sex"]) == 1 ? "Female" : "Male");                     ll.Age = Convert.ToInt32(rr["Age"]); 
                    ll.OrderID = Convert.ToInt32(mDeptOrderid); 
                }



                ll.IssueList = IssueList;

                return ll;
            }
            catch (Exception e) { ll.ErrMsg = "Error when try to load details: " + e.Message; return ll; }


        }



        public static List<IndentIssueInsertedItemListDirectIp> InsertItemDirectIp(int ItemId, int Gstationid, List<IndentIssueInsertedItemListDirectIp> ExistList = null)
        {
            List<IndentIssueInsertedItemListDirectIp> ll = new List<IndentIssueInsertedItemListDirectIp>();
            IndentIssueInsertedItemListDirectIp FakeToHoldErr = new IndentIssueInsertedItemListDirectIp();
            try
            {

                string Str = "";
                                 Str = "Select  i.ID,i.Name,b.batchid,i.drugtype,s.conversionqty,s.tax, "
         + " b.mrp,b.sellingprice,Bs.Quantity,s.unitid,u.name as unit, b.BatchNo, b.ExpiryDate,i.ITEMCODE,i.CSSDItem "
         + " FROM mms_itemmaster i,ItemStore s ,packing u,batch b, BatchStore Bs "
         + " where s.unitid=u.id and b.itemid=i.id "
        + "  and i.id= '" + ItemId + "' "
         + " and I.Id = s.ItemId and b.batchid=bs.batchid "
        + "  and S.Stationid = " + Gstationid + "  "
        + "  and i.stationid= " + Gstationid + ""
        + "  and bs.itemid=i.id "
        + "  and B.BatchNo = Bs.BatchNo "
        + "  and Bs.StationId = " + Gstationid + " "
        + "  and Bs.Quantity <> 0 "
        + "  Order By B.Startdate";

                DataSet ds = MainFunction.SDataSet(Str, "tbl1");

                                 int QOH = 0;
                foreach (DataRow xrr in ds.Tables[0].Rows)
                {
                    QOH += (int)xrr["quantity"];
                }


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
                        IndentIssueInsertedItemListDirectIp l = new IndentIssueInsertedItemListDirectIp();
                                                                          decimal lLargeQty = (decimal)(int)QOH / (decimal)(int)MainFunction.GetQuantityDirectIpIssue((int)rr["unitId"], (int)rr["Id"]);

                        l.SNO = sno;
                                                 l.ID = (int)rr["Id"];
                        l.Name = (string)rr["Name"].ToString().Replace("\"", "");
                        l.BatchId = (int)rr["BatchId"];
                        l.DrugType = (int)rr["drugtype"];
                        l.conversionqty = (int)rr["Conversionqty"];
                        l.tax = ((rr["tax"] == DBNull.Value) ? (int)0 : (int)rr["tax"]);
                        l.mrp = double.Parse(rr["mrp"].ToString());

                        double cprice = (l.mrp + (double)(l.mrp * ((double)(int)l.tax * 0.01)) * (double)(int)l.conversionqty);
                        l.sellingprice = cprice;
                        l.quantity = lLargeQty;
                        l.unitId = (int)rr["unitId"];
                        l.Unit = (string)rr["Unit"];
                        l.BatchNo = (string)rr["BatchNo"];
                        l.expirydate = DateTime.Parse(rr["expirydate"].ToString());
                        l.itemcode = (string)rr["itemcode"];
                        l.CSSDItem = Boolean.Parse(rr["CSSDItem"].ToString());

                        if (sno > 1)
                        {
                            ExistList.Add(l);
                            ll = ExistList;
                        }
                        else
                        {
                            ll.Add(l);
                        }
                        break;                      }
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

        public static List<TempListMdl> ItemUOMList(int ItemID)
        {
            List<TempListMdl> nn = new List<TempListMdl>();
            DataSet ds = MainFunction.SDataSet("Select P.Id ID,P.Name Name ,I.ConversionQty from ItemPacking I,Packing P where P.Id=I.PackId and itemid=" + ItemID + " order by name", "tb");
            foreach (DataRow rr in ds.Tables[0].Rows)
            {
                TempListMdl n = new TempListMdl();
                n.ID = (int)rr["ID"];
                n.Name = rr["Name"].ToString();
                n.ConversionQty = (int)rr["ConversionQty"];
                nn.Add(n);
            }
            return nn;


        }

                                                               
         
                                                                                                                     


                                                                                                                                       
                                                                                                                                                         
                                                                                                                                                                                                               
                                                                                                                                       

         




    }


}
