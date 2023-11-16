using System;
using System.Collections.Generic;
using System.Data;


namespace MMS2
{
    public class BatchLocatorFun
    {
        public static List<ListBox> LoadCombo(String sqlstr, Boolean withAllOption = false)
        {
            List<ListBox> tnList = new List<ListBox>();
            ListBox tn = new ListBox();
            if (withAllOption == true)
            {
                tn.ID = 0;
                tn.NAME = "ALL";
                tnList.Add(tn);
            }
            DataSet ds = MainFunction.SDataSet(sqlstr, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new ListBox();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.NAME = r["Name"].ToString();
                tnList.Add(tn);
            }
            return tnList;

        }
        public static List<ListBox> LoadCellCombo(String sqlstr)
        {
            List<ListBox> tnList = new List<ListBox>();
            ListBox tn = new ListBox();

            DataSet ds = MainFunction.SDataSet(sqlstr, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new ListBox();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.NAME = r["Name"].ToString();
                tnList.Add(tn);
            }
            return tnList;
        }

                 public static List<BatchLocator> GetTableResults(int TabIndex, int TransTypeOrOption, int TransOrRack, int SheflID, int Gstationid,
            jQueryDataTableParamModel Param, int intID = 0)
        {
            string ExtraTableSource = "";
            string RackTransCond = "";
            string ShelfCond = "";
            string SearchCond = "";

            if (TabIndex == 0)
            {
                if (TransTypeOrOption == 2)
                {
                    ExtraTableSource = " left join dbo.itemlocation as loca on dbo.item.id=loca.itemid and dbo.rack.id=loca.rackid and dbo.shelf.id=loca.shelfid " +
                              " and dbo.batchstore.stationid =loca.stationid";
                    if (TransOrRack > 0)
                    {
                        RackTransCond = " AND (loca.RackId = " + TransOrRack + " ) ";
                    }
                    if (SheflID > 0)
                    {
                        ShelfCond = " AND (loca.ShelfId = " + SheflID + " ) ";
                    }
                }
                else
                {
                    RackTransCond = "";
                    ShelfCond = " ";
                }


            }

            if (string.IsNullOrEmpty(Param.sSearch) == false)
            {
                SearchCond = "   and  ( DBO.ITEM.NAME LIKE '%" + Param.sSearch + "%' " +
                             " OR DBO.ITEM.ITEMCODE like  '%" + Param.sSearch + "%' " +
                             " OR CAST(dbo.batch.expirydate AS NVARCHAR(50)) LIKE '%" + Param.sSearch + "%' " +
                             " OR dbo.BATCH.BATCHNO LIKE '%" + Param.sSearch + "%' " +
                             " OR DBO.RACK.NAME LIKE '%" + Param.sSearch + "%' " +
                             " OR DBO.shelf.NAME LIKE '%" + Param.sSearch + "%' )  ";
            }




            string RowOrder = "";
            switch (Param.sSortDir_0)
            {
                case "asc":
                    {
                        if (Param.iSortingCols == 1) { RowOrder = " DBO.ITEM.NAME"; }
                        if (Param.iSortingCols == 2) { RowOrder = " DBO.ITEM.ITEMCODE"; }
                        if (Param.iSortingCols == 3) { RowOrder = " DBO.BATCH.BATCH.BATCHNO"; }
                        if (Param.iSortingCols == 4) { RowOrder = " DBO.RACK.NAME"; }
                        if (Param.iSortingCols == 5) { RowOrder = " DBO.SHELF.NAME"; }
                        RowOrder += " ASC ";
                        break;
                    }
                case "desc":
                    {
                        if (Param.iSortingCols == 1) { RowOrder = " DBO.ITEM.NAME"; }
                        if (Param.iSortingCols == 2) { RowOrder = " DBO.ITEM.ITEMCODE"; }
                        if (Param.iSortingCols == 3) { RowOrder = " DBO.BATCH.BATCH.BATCHNO"; }
                        if (Param.iSortingCols == 4) { RowOrder = " DBO.RACK.NAME"; }
                        if (Param.iSortingCols == 5) { RowOrder = " DBO.SHELF.NAME"; }
                        RowOrder += " DESC ";
                        break;
                    }
                default: RowOrder = " DBO.ITEM.NAME ASC"; break;
            }






            string sqlstrAll = "SELECT dbo.item.Name AS ItemName,dbo.item.ItemCode , dbo.batch.batchno AS BatchNo, dbo.rack.Name AS Rack, " +
              " dbo.shelf.Name AS Shelf, isnull(dbo.batch.expirydate,'') AS ExpDate,dbo.batchstore.quantity AS Qty, " +
              " dbo.batch.costprice AS CP,dbo.item.ID,dbo.Batch.BatchID, dbo.batch.sellingprice AS SP, dbo.batchstore.StationId, " +
              " dbo.batchstore.itemid AS ItemID,dbo.rack.id as RackID,dbo.shelf.ID as ShelfID, " +
              " Row_Number() OVER (  " +
              " ORDER BY  " + RowOrder + " ) AS RowNum    ";
            string FromWhereSt = " FROM dbo.rack LEFT OUTER JOIN   " +
               " dbo.shelf RIGHT OUTER JOIN   " +
               " dbo.BatchLocator ON dbo.shelf.Id = dbo.BatchLocator.ShelfID ON dbo.rack.Id = dbo.BatchLocator.RackID RIGHT OUTER JOIN   " +
               " dbo.item INNER JOIN   " +
               " dbo.batch INNER JOIN   " +
               " dbo.batchstore ON dbo.batch.itemid = dbo.batchstore.itemid AND dbo.batch.batchid = dbo.batchstore.BatchId ON   " +
               " dbo.item.id = dbo.batchstore.itemid ON dbo.BatchLocator.Itemid = dbo.Batchstore.Itemid   " +
               " And dbo.BatchLocator.Batchid = dbo.Batchstore.Batchid  " + ExtraTableSource +
               " WHERE  (dbo.batchstore.StationId = " + Gstationid + ") AND (dbo.batchstore.quantity > 0)  " + SearchCond + RackTransCond + ShelfCond;

            sqlstrAll += FromWhereSt;
            var startRow = Param.iDisplayStart;
            var endRow = Param.iDisplayStart + Param.iDisplayLength;
            string CoverString = "";


            CoverString = "select * from (" + sqlstrAll + ") as a  where a.rownum>= " + startRow + "  and a.rownum<=" + endRow;


            DataSet sd = MainFunction.SDataSet(CoverString, "tbL");


                         Param.iTotalRecords = sd.Tables[0].Rows.Count;


                         Param.iTotalDisplayRecords = MainFunction.GetTotalRecordCount("SELECT count(*) as totCount " + FromWhereSt, "totcount");


            List<BatchLocator> bt = new List<BatchLocator>();
            foreach (DataRow r in sd.Tables[0].Rows)
            {
                BatchLocator btd = new BatchLocator();
                DateTime dt = (DateTime)r["expdate"];

                btd.ID = int.Parse(r["ID"].ToString());
                btd.SNO = int.Parse(r["RowNum"].ToString());
                btd.ItemCode = r["Itemcode"].ToString();
                btd.ItemName = r["itemName"].ToString();
                btd.BatchNo = r["batchno"].ToString();
                btd.BatchID = int.Parse(r["batchid"].ToString());
                btd.Rack = r["rack"].ToString();
                btd.Shelf = r["shelf"].ToString();
                btd.expdate = dt.ToString("dd MMM yy");
                btd.Quantity = MainFunction.NullToInteger(r["Qty"].ToString());
                btd.Cost = Math.Round(MainFunction.NullToDecmial(r["CP"].ToString()), 3);
                btd.SelPrice = Math.Round(MainFunction.NullToDecmial(r["SP"].ToString()), 3);
                btd.StationID = int.Parse(r["stationid"].ToString());
                btd.ItemID = int.Parse(r["ID"].ToString());
                bt.Add(btd);
            }
            return bt;
        }

        public static BatchLocator LoadTransTypeList()
        {
            BatchLocator BT = new BatchLocator();
                                                                                                                                               


            BT.TransTypeLIst = new List<TransactionType>();
            TransactionType Lt = new TransactionType();
            Lt.id = 0;
            Lt.description = "select Type";
            BT.TransTypeLIst.Add(Lt);

                                                                                           
            string SQL = "select ID,Description as Name from TransactionType where type='R' and id in (1,5,15,27) order by id";
            DataSet NN = MainFunction.SDataSet(SQL, "tbl1");
            foreach (DataRow rr in NN.Tables[0].Rows)
            {
                Lt = new TransactionType();
                Lt.id = (int)rr["ID"];
                Lt.description = rr["Name"].ToString();
                BT.TransTypeLIst.Add(Lt);

            }



            BT.TransNoList = new List<ListBox>();
            ListBox nnt = new ListBox();
            nnt.ID = 0;
            nnt.NAME = "ALL";
            BT.TransNoList.Add(nnt);

            BT.Racklist = new List<ListBox>();
            nnt = new ListBox();
            nnt.ID = 0;
            nnt.NAME = "ALL";
            BT.Racklist.Add(nnt);

            BT.Shelflist = new List<ListBox>();
            nnt = new ListBox();
            nnt.ID = 0;
            nnt.NAME = "ALL";
            BT.Shelflist.Add(nnt);

            return BT;

        }
        public static List<ListBox> GetTransNoList(int tabBatLoc, int Gstationid, int TransType = 0)
        {
            string sqlstr = "";
            List<ListBox> Tlist = new List<ListBox>();
            if (TransType == 1)              {
                if (tabBatLoc == 1)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from AdjustReceipt where LocatorStatus=0 and stationid=" + Gstationid + " Order by Id desc ";

                }
                else if (tabBatLoc == 2)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from AdjustReceipt where LocatorStatus=1 and stationid=" + Gstationid + " Order by Id desc";
                }
            }

            if (TransType == 5)              {
                if (tabBatLoc == 1)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from IndentReceipt where LocatorStatus=0 and stationid=" + Gstationid + " Order by Id desc";

                }
                else if (tabBatLoc == 2)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from IndentReceipt where LocatorStatus=1 and stationid=" + Gstationid + " Order by Id desc";

                }
            }

            if (TransType == 15)              {
                if (tabBatLoc == 1)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from PurchaseReceipt where LocatorStatus=0 and stationid=" + Gstationid + " Order by Id desc";

                }
                else if (tabBatLoc == 2)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from PurchaseReceipt where LocatorStatus=1 and stationid=" + Gstationid + " Order by Id desc";
                }
            }

            if (TransType == 27)              {
                if (tabBatLoc == 1)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from PreparationReceipt where LocatorStatus=0 and stationid=" + Gstationid + " Order by Id desc";

                }
                else if (tabBatLoc == 2)
                {
                    sqlstr = "SELECT ID,'No: '+cast(StationSlNo as varchar(5))+'   Dt: '+cast(datetime as varchar(12)) as Name from PreparationReceipt where LocatorStatus=1 and stationid=" + Gstationid + " Order by Id desc";

                }
            }

            if (TransType > 0)
            {
                Tlist = LoadCombo(sqlstr, true);
            }
            return Tlist;

        }


        public static List<BatchLocator> GetTransNoQueryNew(int TabIndex, int TransTypeOrOption, int TransOrRack, int SheflID, int Gstationid)
        {
            string Sql1 = "";
            string Sql2 = "";
            if (TabIndex == 1)
            {
                Sql1 = "SELECT  i.name as ItemName,i.itemcode as ItemCode,c.batchno as BatchNo,isnull(c.expirydate,'') as ExpDate," +
                             " d.quantity as Qty,c.costprice as CP,c.sellingPrice as SP,d.batchid as BatchID,i.id as ID ";
                if (TransTypeOrOption == 1)
                {
                    if (TransOrRack == 0)
                    {
                        Sql2 = " from AdjustReceipt A,AdjustReceiptDetail B,Item I,Batch C,BatchStore D " +
                               " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.AdjReceiptID " +
                               " and B.ItemID=I.ID and B.BatchID=C.BatchID and D.BatchID=C.BatchID and " +
                               " D.StationID= " + Gstationid + " ";

                    }
                    else
                    {
                        Sql2 = " from AdjustReceipt A,AdjustReceiptDetail B,Item I,Batch C,BatchStore D " +
                               " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.AdjReceiptID " +
                               " and B.ItemID=I.ID and B.BatchID=C.BatchID and D.BatchID=C.BatchID and " +
                               " D.StationID= " + Gstationid + " and a.id=" + TransOrRack;
                    }
                }
                else if (TransTypeOrOption == 5)
                {
                    if (TransOrRack == 0)
                    {
                        Sql2 = " from IndentReceipt A,IndentReceiptDetail B,Item I,Batch C,BatchStore D   " +
                             " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.ReceiptID " +
                             " and B.ItemID=I.ID and D.StationID=" + Gstationid +
                             " and D.BatchID=C.BatchID and  D.BatchID=B.BatchID ";
                    }
                    else
                    {
                        Sql2 = " from IndentReceipt A,IndentReceiptDetail B,Item I,Batch C,BatchStore D   " +
                             " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.ReceiptID  " +
                             " and B.ItemID=I.ID and D.StationID=" + Gstationid + " and a.id=" + TransOrRack +
                             " and D.BatchID=C.BatchID and  D.BatchID=B.BatchID ";

                    }

                }
                else if (TransTypeOrOption == 15)
                {
                    if (TransOrRack == 0)
                    {
                        Sql2 = "from PurchaseReceipt A,PurchaseReceiptDetail B,Item I,Batch C,BatchStore D  " +
                             " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.ReceiptID " +
                             " and B.ItemID=I.ID and B.BatchID=C.BatchID and D.BatchID=C.BatchID  and D.StationID= " + Gstationid;
                    }
                    else
                    {
                        Sql2 = "from PurchaseReceipt A,PurchaseReceiptDetail B,Item I,Batch C,BatchStore D  " +
                               " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.ReceiptID " +
                               " and B.ItemID=I.ID and B.BatchID=C.BatchID and D.BatchID=C.BatchID  " +
                               " and D.StationID= " + Gstationid + " and a.id=" + TransOrRack;
                    }

                }
                else if (TransTypeOrOption == 27)
                {
                    if (TransOrRack == 0)
                    {
                        Sql2 = " from PreparationReceipt A,PreparationReceiptDetail B,Item I,Batch C,BatchStore D " +
                             " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.ReceiptID " +
                             " and B.ItemID=I.ID and D.StationID=" + Gstationid + " and D.BatchID=C.BatchID and " +
                             " D.BatchID=B.BatchID ";

                    }
                    else
                    {
                        Sql2 = " from PreparationReceipt A,PreparationReceiptDetail B,Item I,Batch C,BatchStore D " +
                             " Where a.LocatorStatus = 0 And a.StationID = " + Gstationid + " And a.ID = b.ReceiptID " +
                             " and B.ItemID=I.ID and D.StationID=" + Gstationid + " and D.BatchID=C.BatchID and " +
                             " D.BatchID=B.BatchID and a.id= " + TransOrRack;

                    }


                }
            }
            else if (TabIndex == 2)
            {

                if (TransTypeOrOption == 1)
                {
                    Sql1 = "SELECT  i.name as ItemName,i.itemcode as ItemCode,c.batchno as BatchNo,isnull(c.expirydate,'') as ExpDate," +
                               " d.quantity as Qty,c.costprice as CP,c.sellingPrice as SP,d.batchid as BatchID," +
                               " d.StationId as StationID, " +
                               " dbo.BatchLocator.RackID as RackID,  " +
                               " dbo.rack.Name AS Rack,  " +
                               " dbo.BatchLocator.ShelfID AS ShelfID, " +
                               " dbo.shelf.Name AS Shelf, " +
                               " b.adjreceiptid AS receiptid," +
                               " i.id as ID";
                    if (TransOrRack == 0)
                    {
                        Sql2 = " FROM item i INNER JOIN  adjustreceiptdetail b " +
                             " INNER JOIN  adjustreceipt a ON b.adjreceiptid = a.id  " +
                             " INNER JOIN  dbo.batchstore D  " +
                        " INNER JOIN  batch  C ON D.BatchId = C.batchid ON B.itemid = C.itemid  " +
                        " AND  B.batchid = C.batchid ON I.id = B.itemid " +
                        " LEFT OUTER JOIN  dbo.BatchLocator " +
                        " RIGHT OUTER JOIN  dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id  " +
                        " RIGHT OUTER JOIN  dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id ON D.BatchId = dbo.BatchLocator.BatchID  " +
                        " WHERE     (d.StationId = " + Gstationid + ") AND (a.LocatorStatus = 1) ";

                    }
                    else
                    {
                        Sql2 = " FROM item i INNER JOIN  adjustreceiptdetail b " +
                             " INNER JOIN  adjustreceipt a ON b.adjreceiptid = a.id  " +
                             " INNER JOIN  dbo.batchstore D  " +
                        " INNER JOIN  batch  C ON D.BatchId = C.batchid ON B.itemid = C.itemid  " +
                        " AND  B.batchid = C.batchid ON I.id = B.itemid " +
                        " LEFT OUTER JOIN  dbo.BatchLocator " +
                        " RIGHT OUTER JOIN  dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id  " +
                        " RIGHT OUTER JOIN  dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id ON D.BatchId = dbo.BatchLocator.BatchID  " +
                        " WHERE     (d.StationId = " + Gstationid + ") AND (a.LocatorStatus = 1) " +
                        " and (a.ID = " + TransOrRack + " )";
                    }

                }
                else if (TransTypeOrOption == 5)
                {
                    Sql1 = "SELECT  i.name as ItemName,i.itemcode as ItemCode,c.batchno as BatchNo,isnull(c.expirydate,'') as ExpDate," +
                               " d.quantity as Qty,c.costprice as CP,c.sellingPrice as SP,d.batchid as BatchID," +
                               " d.StationId as StationID, " +
                               " dbo.BatchLocator.RackID as RackID,  " +
                               " dbo.rack.Name AS Rack,  " +
                               " dbo.BatchLocator.ShelfID AS ShelfID, " +
                               " dbo.shelf.Name AS Shelf, " +
                               " 0 AS receiptid," +
                               " i.id as ID";
                    if (TransOrRack == 0)
                    {
                        Sql2 = " FROM  dbo.BatchLocator RIGHT OUTER JOIN " +
                        " dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id RIGHT OUTER JOIN " +
                        " dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id RIGHT OUTER JOIN " +
                        " dbo.indentreceipt a INNER JOIN " +
                        " dbo.indentreceiptdetail b ON a.id = b.receiptid INNER JOIN " +
                        " dbo.batch c ON b.BatchId = c.batchid INNER JOIN " +
                        " dbo.batchstore d ON c.batchid = d.BatchId INNER JOIN " +
                        " dbo.item i ON b.ItemId = i.id ON dbo.BatchLocator.BatchID = d.BatchId " +
                        " Where (b.receiptid = " + TransOrRack + " ) And (d.StationID = " + Gstationid + ") " +
                        " and a.LocatorStatus=1";
                    }
                    else
                    {
                        Sql2 = " FROM  dbo.BatchLocator RIGHT OUTER JOIN " +
                        " dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id RIGHT OUTER JOIN " +
                        " dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id RIGHT OUTER JOIN " +
                        " dbo.indentreceipt a INNER JOIN " +
                        " dbo.indentreceiptdetail b ON a.id = b.receiptid INNER JOIN " +
                        " dbo.batch c ON b.BatchId = c.batchid INNER JOIN " +
                        " dbo.batchstore d ON c.batchid = d.BatchId INNER JOIN " +
                        " dbo.item i ON b.ItemId = i.id ON dbo.BatchLocator.BatchID = d.BatchId " +
                        " Where (b.receiptid = " + TransOrRack + " ) And (d.StationID = " + Gstationid + ") " +
                        " and a.LocatorStatus=1";

                    }
                }
                else if (TransTypeOrOption == 15)
                {
                    Sql1 = "SELECT  i.name as ItemName,i.itemcode as ItemCode,c.batchno as BatchNo,isnull(c.expirydate,'') as ExpDate," +
                             " d.quantity as Qty,c.costprice as CP,c.sellingPrice as SP,d.batchid as BatchID," +
                             " d.StationId as StationID, " +
                             " dbo.BatchLocator.RackID as RackID,  " +
                             " dbo.rack.Name AS Rack,  " +
                             " dbo.BatchLocator.ShelfID AS ShelfID, " +
                             " dbo.shelf.Name AS Shelf, " +
                             " 0 AS receiptid," +
                             " i.id as ID";
                    if (TransOrRack == 0)
                    {
                        Sql2 = " FROM dbo.item i INNER JOIN " +
                        " dbo.purchasereceiptdetail  b INNER JOIN " +
                        " dbo.purchasereceipt a ON b.ReceiptID = a.ID INNER JOIN " +
                        " dbo.batchstore d INNER JOIN " +
                        " dbo.batch c ON d.BatchId = c.batchid ON b.ItemId = c.itemid AND  " +
                        " b.BatchId =c.batchid ON i.id = b.ItemId LEFT OUTER JOIN " +
                        " dbo.BatchLocator RIGHT OUTER JOIN " +
                        " dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id RIGHT OUTER JOIN " +
                        " dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id ON d.BatchId = dbo.BatchLocator.BatchID " +
                        " Where (d.StationID = " + Gstationid + " )  and (a.LocatorStatus = 1 ) ";


                    }
                    else
                    {
                        Sql2 = " FROM dbo.item i INNER JOIN " +
                     " dbo.purchasereceiptdetail  b INNER JOIN " +
                     " dbo.purchasereceipt a ON b.ReceiptID = a.ID INNER JOIN " +
                     " dbo.batchstore d INNER JOIN " +
                     " dbo.batch c ON d.BatchId = c.batchid ON b.ItemId = c.itemid AND  " +
                     " b.BatchId =c.batchid ON i.id = b.ItemId LEFT OUTER JOIN " +
                     " dbo.BatchLocator RIGHT OUTER JOIN " +
                     " dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id RIGHT OUTER JOIN " +
                     " dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id ON d.BatchId = dbo.BatchLocator.BatchID " +
                     " Where (d.StationID = " + Gstationid + " )  and (a.LocatorStatus = 1 ) " +
                     " and (a.ID = " + TransOrRack + " ) ";
                    }
                }
                else if (TransTypeOrOption == 27)
                {
                    Sql1 = "SELECT  i.name as ItemName,i.itemcode as ItemCode,c.batchno as BatchNo,isnull(c.expirydate,'') as ExpDate," +
                            " d.quantity as Qty,c.costprice as CP,c.sellingPrice as SP,d.batchid as BatchID," +
                            " d.StationId as StationID, " +
                            " dbo.BatchLocator.RackID as RackID,  " +
                            " dbo.rack.Name AS Rack,  " +
                            " dbo.BatchLocator.ShelfID AS ShelfID, " +
                            " dbo.shelf.Name AS Shelf, " +
                            " 0 AS receiptid," +
                            " i.id as ID";
                    if (TransOrRack == 0)
                    {

                        Sql2 = " FROM       dbo.preparationreceipt a INNER JOIN " +
                         " dbo.item i INNER JOIN " +
                         " dbo.preparationreceiptdetail b ON i.id = b.itemid INNER JOIN " +
                         " dbo.batchstore d INNER JOIN " +
                         " dbo.batch c ON d.BatchId = c.batchid ON b.batchid = c.batchid AND " +
                         " b.itemid = c.itemid ON a.id =b.receiptid LEFT OUTER JOIN " +
                         " dbo.BatchLocator RIGHT OUTER JOIN " +
                         " dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id RIGHT OUTER JOIN " +
                         " dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id ON d.BatchId = dbo.BatchLocator.BatchID " +
                         " WHERE     (d.StationId = " + Gstationid + " ) AND (a.LocatorStatus = 1)";

                    }
                    else
                    {
                        Sql2 = " FROM       dbo.preparationreceipt a INNER JOIN " +
                       " dbo.item i INNER JOIN " +
                       " dbo.preparationreceiptdetail b ON i.id = b.itemid INNER JOIN " +
                       " dbo.batchstore d INNER JOIN " +
                       " dbo.batch c ON d.BatchId = c.batchid ON b.batchid = c.batchid AND " +
                       " b.itemid = c.itemid ON a.id =b.receiptid LEFT OUTER JOIN " +
                       " dbo.BatchLocator RIGHT OUTER JOIN " +
                       " dbo.shelf ON dbo.BatchLocator.ShelfID = dbo.shelf.Id RIGHT OUTER JOIN " +
                       " dbo.rack ON dbo.BatchLocator.RackID = dbo.rack.Id ON d.BatchId = dbo.BatchLocator.BatchID " +
                       " WHERE     (d.StationId = " + Gstationid + " ) AND (a.LocatorStatus = 1)" +
                       " and (a.ID = " + TransOrRack + " )";
                    }

                }


            }
            return GetTableResults2New(Sql1, Sql2, TabIndex, TransTypeOrOption, TransOrRack, SheflID, Gstationid);
        }

        public static List<BatchLocator> GetTableResults2New(string sqlstrAll, string FromWhereSt, int TabIndex, int TransTypeOrOption, int TransOrRack, int SheflID, int Gstationid,
                 int intID = 0)
        {
            string RowOrder = " i.NAME";

            sqlstrAll = sqlstrAll +
              " ,Row_Number() OVER (  " +
              " ORDER BY  " + RowOrder + " ) AS RowNum    ";

            sqlstrAll += FromWhereSt;
            string CoverString = "";


            CoverString = "select * from (" + sqlstrAll + ") as a  ";


            DataSet sd = MainFunction.SDataSet(CoverString, "tbL");

            List<BatchLocator> bt = new List<BatchLocator>();
            foreach (DataRow r in sd.Tables[0].Rows)
            {
                BatchLocator btd = new BatchLocator();
                DateTime dt = (DateTime)r["expdate"];

                btd.ID = int.Parse(r["ID"].ToString());
                btd.SNO = int.Parse(r["RowNum"].ToString());
                btd.ItemCode = r["Itemcode"].ToString();
                btd.ItemName = r["itemName"].ToString();
                btd.BatchNo = r["batchno"].ToString();
                btd.BatchID = int.Parse(r["batchid"].ToString());
                if (TabIndex == 2)
                {
                    btd.Rack = r["rack"].ToString();
                    btd.RackID = MainFunction.NullToInteger(r["RackID"].ToString());
                    btd.Shelf = r["shelf"].ToString();
                    btd.ShelfID = MainFunction.NullToInteger(r["shelfID"].ToString());
                    btd.receiptid = MainFunction.NullToInteger(r["receiptid"].ToString());
                    btd.StationID = MainFunction.NullToInteger(r["stationid"].ToString());
                }
                else
                {

                    btd.Rack = "";
                    btd.RackID = 0;
                    btd.Shelf = "";
                    btd.ShelfID = 0;
                    btd.receiptid = 0;
                    btd.StationID = 0;

                }

                btd.expdate = dt.ToString("dd MMM yy");
                btd.Quantity = MainFunction.NullToInteger(r["Qty"].ToString());
                btd.Cost = Math.Round(MainFunction.NullToDecmial(r["CP"].ToString()), 3);
                btd.SelPrice = Math.Round(MainFunction.NullToDecmial(r["SP"].ToString()), 3);

                btd.ItemID = int.Parse(r["ID"].ToString());
                bt.Add(btd);
            }
            return bt;
        }



                                                                                                                                                         
                                                                                                                                                                                                                        
         
                                                                                                                                                
                                                                                 
                                                               
         

                                    
                                                                                                                                                                                                      
                                                                                                                              
                                                                                                                                                                                                                                                                                                                                             
                                                                                                                                                                                                                                 

                                                                                                                                                                                                                                                                     
                                                                                          
                                                                                                                                       
         

                           
                                             

                                                               

                                                                                                                                                                                             





                                    
                                    

         

         

                  

                  

                                             
                                                                                                                                                         
                                                      
         
                                    
                                             
    }




}