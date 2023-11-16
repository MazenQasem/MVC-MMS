using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data;


namespace MMS2
{

    public class AlterMRPFun
    {
        public static AlterMRP GetItemsList(int stationid)
        {

            AlterMRP sp = new AlterMRP();








            sp.DrugList_Add = new List<Item>();
            sp.ConsumableList_Add = new List<Item>();
            sp.OthersList_Add = new List<Item>();
            String sqlStr = " select id,name,itemcode,drugtype,qoh from MMs_itemmaster  "
                + " where stationid=" + stationid + " and qoh>0 and deleted=0  order by name,drugtype ";

            DataSet ds = MainFunction.SDataSet(sqlStr, "DrugList");
            foreach (DataRow VR in ds.Tables["DrugList"].Rows)
            {
                Item u = new Item();
                u.ID = int.Parse(VR["id"].ToString());
                u.Name = VR["name"].ToString();
                u.ItemCode = VR["itemcode"].ToString();
                u.DrugType = int.Parse(VR["drugtype"].ToString());
                switch (u.DrugType)
                {
                    case 0: sp.DrugList_Add.Add(u); break;
                    case 1: sp.ConsumableList_Add.Add(u); break;
                    case 2: sp.OthersList_Add.Add(u); break;
                }
            }



            return sp;


        }
        public static List<Item> GetItemBatch(int itmID, int stationid)
        {
            try
            {


                List<Item> ItemDetails = new List<Item>();

                String sqlStr = " Select Item.ID itemid,ITEM.ItemCode,item.name as itemName, batch.BatchNo,costprice,ExpiryDate as ExpDate,mrp,batch.TAX,item.drugtype," +
                                 "batch.batchid,conversionqty,unitid ,P.name uomname" +
                                " from Batch,item,packing P ,batchstore bs " +
                                " where P.id=Item.unitid and batch.itemid=item.id and batch.quantity>0 and batch.ItemId= " + itmID +
                                " and batch.BatchID=bs.BatchID " +
                                " and batch.BatchNo=bs.BatchNo " +
                                " and batch.ItemID=bs.ItemID " +
                                " and bs.StationID=" + stationid +
                                " and bs.Quantity>0 ";


                DataSet ds = MainFunction.SDataSet(sqlStr, "SList");
                foreach (DataRow VR in ds.Tables[0].Rows)
                {
                    Item u = new Item();
                    u.ID = int.Parse(VR["itemid"].ToString());
                    u.ItemCode = VR["Itemcode"].ToString();
                    u.Name = VR["ItemName"].ToString();
                    u.ExpriyDate = DateTime.Parse(VR["ExpDate"].ToString());
                    u.OldCP = decimal.Parse(VR["costprice"].ToString());
                    u.OldMRP = decimal.Parse(VR["mrp"].ToString());
                    u.UOM = VR["UOMName"].ToString();
                    u.BatchNo = VR["batchno"].ToString();
                    u.BatchID = VR["batchid"].ToString();
                    u.BatchTax = MainFunction.NullToFloat(VR["Tax"].ToString());
                    u.ConversionQty = int.Parse(VR["conversionqty"].ToString());
                    u.DrugType = int.Parse(VR["drugtype"].ToString());
                    ItemDetails.Add(u);
                }

                return ItemDetails;
            }
            catch (Exception e) { return null; }
        }

        public static Boolean Save(AlterMRP sp, int SavedBy)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                if (CheckData(sp))
                {
                    using (Con)
                    {
                        Con.Open();
                        SqlTransaction Trans = Con.BeginTransaction();

                        List<Item> UpdateMrPList = sp.UpdatedMRPList_Add;
                        String sqlStr = "";
                        for (int i = 0; i < sp.UpdatedMRPList_Add.Count(); i++)
                        {
                            if (sp.UpdatedMRPList_Add[i].NewMRP > 0)
                            {
                                var Tax = 1 + 0.01 * sp.UpdatedMRPList_Add[i].BatchTax;
                                var NewMRP = (sp.UpdatedMRPList_Add[i].NewMRP * decimal.Parse(Tax.ToString())) / sp.UpdatedMRPList_Add[i].ConversionQty;
                                sqlStr = "Update Item Set sellingprice= " + NewMRP + " where id=" + sp.UpdatedMRPList_Add[i].ID;
                                bool Excute = MainFunction.SSqlExcuite(sqlStr, Trans);
                                if (sp.UpdatedMRPList_Add[i].ExpriyDate.ToString().Length > 0)
                                {

                                    sqlStr = "update batch set sellingprice=" + NewMRP + " ,mrp=" + sp.UpdatedMRPList_Add[i].NewMRP / sp.UpdatedMRPList_Add[i].ConversionQty +
                                        " , ExpiryDate='" + sp.UpdatedMRPList_Add[i].ExpriyDate + "' where itemid=" + sp.UpdatedMRPList_Add[i].ID +
                                        " and batchno='" + sp.UpdatedMRPList_Add[i].BatchNo.Trim() + "' " +
                                        " and batchid=" + sp.UpdatedMRPList_Add[i].BatchID + "";
                                }
                                else
                                {
                                    sqlStr = "update batch set sellingprice=" + NewMRP + " ,mrp=" + sp.UpdatedMRPList_Add[i].NewMRP / sp.UpdatedMRPList_Add[i].ConversionQty +
                                        " , ExpiryDate='" + sp.UpdatedMRPList_Add[i].ExpriyDate + "' where itemid=" + sp.UpdatedMRPList_Add[i].ID +
                                        " and batchno='" + sp.UpdatedMRPList_Add[i].BatchNo.Trim() + "' " +
                                        " and batchid=" + sp.UpdatedMRPList_Add[i].BatchID + "";
                                }
                                Excute = MainFunction.SSqlExcuite(sqlStr, Trans);


                                if (sp.UpdatedMRPList_Add[i].ExpriyDateOld.ToString().Length > 0 &&

    sp.UpdatedMRPList_Add[i].ExpriyDateOld.ToString().Contains("0001") != true
   )
                                {
                                    sqlStr = "insert into altermrp (itemid,batchid,datetime,oldcp,oldmrp,newcp,newmrp,operatorid,OldExpDate,NewExpDate) values (" +
                                        sp.UpdatedMRPList_Add[i].ID + "," +
                                        sp.UpdatedMRPList_Add[i].BatchID + "," +
                                        " sysdatetime()," +
                                        sp.UpdatedMRPList_Add[i].OldCP / sp.UpdatedMRPList_Add[i].ConversionQty + "," +
                                        sp.UpdatedMRPList_Add[i].OldMRP / sp.UpdatedMRPList_Add[i].ConversionQty + ",0," +
                                        NewMRP + "," +
                                        SavedBy + "," +
                                        "'" + sp.UpdatedMRPList_Add[i].ExpriyDateOld + "'," +
                                        "'" + sp.UpdatedMRPList_Add[i].ExpriyDate + "')";
                                }
                                else
                                {
                                    sqlStr = "insert into altermrp (itemid,batchid,datetime,oldcp,oldmrp,newcp,newmrp,operatorid) values (" +
                                          sp.UpdatedMRPList_Add[i].ID + "," +
                                          sp.UpdatedMRPList_Add[i].BatchID + "," +
                                          " sysdatetime()," +
                                          sp.UpdatedMRPList_Add[i].OldCP / sp.UpdatedMRPList_Add[i].ConversionQty + "," +
                                          sp.UpdatedMRPList_Add[i].OldMRP / sp.UpdatedMRPList_Add[i].ConversionQty + ",0," +
                                          NewMRP + "," +
                                          SavedBy + ")";
                                }

                                Excute = MainFunction.SSqlExcuite(sqlStr, Trans);
                            }
                        }




                        Trans.Commit();
                        return true;
                    }
                }
                else
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                    }
                    sp.ErrMsg = "Saving>> Server Side validate Error!" + sp.ErrMsg;
                    return false;
                }

            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                sp.ErrMsg = "Saving>>" + e.Message;
                return false;
            }
        }
        public static Boolean CheckData(AlterMRP sp)
        {


            return true;
        }

    }
}