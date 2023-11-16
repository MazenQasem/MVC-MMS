using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;


namespace MMS2
{
    public class RolReCalculationFun
    {
        public static ItemAvgSale LoadItem()
        {
            ItemAvgSale sp = new ItemAvgSale();
                                                                                                                                              sp.ItemGroupList = new List<ItemGroup>();
                                                                                                       string SQL = "select ID,Name from itemgroup where deleted=0 order by name";
            DataSet nn = MainFunction.SDataSet(SQL, "tbl1");
            foreach (DataRow rr in nn.Tables[0].Rows)
            {
                ItemGroup itm = new ItemGroup();
                itm.ID = (int)rr["ID"];
                itm.Name = rr["Name"].ToString();
                sp.ItemGroupList.Add(itm);

            }






            sp.ShowFactor = 15;              sp.ShowMonth = 2; 
            DataSet xn = MainFunction.SDataSet("select month from avgsalemonths", "Tb");
            foreach (DataRow VR in xn.Tables["Tb"].Rows)
            {
                sp.ShowMonth = int.Parse(VR["month"].ToString());
            }

            MMS_ItemMaster fakeitems = new MMS_ItemMaster();
            fakeitems.Id = 0;
            fakeitems.Name = "n/a";
            List<MMS_ItemMaster> FakeList = new List<MMS_ItemMaster>();
            FakeList.Add(fakeitems);
            sp.ItemList = FakeList;

            return sp;
        }
        public static ItemStore GetQOH(int itemid, int StationID)
        {
            ItemStore itm = new ItemStore();
                         var CashedItem = MainFunction.CashedAllItems(StationID, " and ID=" + itemid);
            var nn = (from a in CashedItem
                      where a.Id == itemid
                      select new
                      {
                          QOH = a.QOH,
                          ROQ = a.ROQ,
                          ROL = a.ROL
                      });

            foreach (var a in nn.ToList())
            {
                itm.QOH = MainFunction.NullToInteger2(a.QOH.ToString());
                itm.ROQ = MainFunction.NullToInteger2(a.ROQ.ToString());
                itm.ROL = MainFunction.NullToInteger2(a.ROL.ToString());
            }
            return itm;

        }
        public static List<MMS_ItemMaster> GetCategoryItem(int categoryid, string srch, int StationID)
        {
                         var CASHEDITEM = MainFunction.CashedAllItems(StationID, " and categoryid=" + categoryid + " and name like'%" + srch + "%'");
            var TBL = (from a in CASHEDITEM
                       where a.CategoryID == categoryid
                       where a.Name.Contains(srch)
                       select new

                       {
                           ID = a.Id,
                           Name = a.Name
                       });

            List<MMS_ItemMaster> NewList = new List<MMS_ItemMaster>();
            foreach (var a in TBL.ToList())
            {
                MMS_ItemMaster itm = new MMS_ItemMaster();
                itm.Id = a.ID;
                itm.Name = a.Name;
                NewList.Add(itm);
            }


                          
                                                                                                        
            return NewList;

        }

        public static Boolean Save(ItemAvgSale sp, int EmpID)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    int ctr, NoOfDays, NoOfDaysStk;
                    string OrdTable, OrdDetTable, FieldName, strInsert;
                    long avgPatient, ct1, maxleveldays, minleveldays;

                    NoOfDaysStk = sp.ShowFactor;
                    if (NoOfDaysStk == 0) { NoOfDaysStk = 1; }
                    ctr = 0;
                    ct1 = 1;
                    NoOfDays = 0;
                    avgPatient = 0;
                    maxleveldays = 0;
                    minleveldays = 0;
                    DateTime StartDate, EndDate;
                    StartDate = DateTime.Now.Date;
                    StartDate = new DateTime(
                                StartDate.AddMonths(-sp.ShowMonth).Year,
                                StartDate.AddMonths(-sp.ShowMonth).Month,
                                StartDate.AddDays(1).Day);

                    while (StartDate.Date < DateTime.Now.Date)
                    {
                        EndDate = StartDate.AddDays(1);
                        DataSet ds = MainFunction.SDataSet("select count(*) as PtCount from allinpatients where admitdatetime < '" + EndDate + "' " +
                                    " and (dischargedatetime >'" + EndDate + "' or dischargedatetime is null)", "tg");
                        foreach (DataRow n in ds.Tables["tg"].Rows)
                        {
                            avgPatient = avgPatient + long.Parse(n["PtCount"].ToString());
                        }
                        StartDate = StartDate.AddDays(1);
                        ct1 += 1;
                    }

                    avgPatient = avgPatient / ct1;

                    strInsert = "select maxleveldays,minleveldays,roldays from maxlevel where " + avgPatient + " >= avgfrom and " + avgPatient + " <= avgto";
                    DataSet dsx = MainFunction.SDataSet(strInsert, "tg2");

                    foreach (DataRow n in dsx.Tables["tg2"].Rows)
                    {

                        maxleveldays = long.Parse(n["maxleveldays"].ToString());
                        minleveldays = long.Parse(n["minleveldays"].ToString());
                        if (int.Parse(n["roldays"].ToString()) == 0)
                        {
                            NoOfDaysStk = int.Parse(n["NoOfDaysStk"].ToString());
                        }
                        else
                        {
                            NoOfDaysStk = int.Parse(n["roldays"].ToString());
                        }

                    }

                    Boolean bt = MainFunction.SSqlExcuite("Delete from itemavgsale", Trans);
                    bt = MainFunction.SSqlExcuite("Insert into itemavgsale (itemid,stationid) select itemid,stationid from itemstore where stationid = " + sp.Stationid, Trans);
                    bt = MainFunction.SSqlExcuite("UPDATE ItemAvgSale SET Month1 = 0, Month2 = 0, Month3 = 0", Trans);

                    ctr = 0;
                    StartDate = DateTime.Now.Date; 
                    while (ctr <= sp.ShowMonth)
                    {
                        OrdTable = " TransOrder_" + StartDate.AddMonths(-ctr).Year + "_" + StartDate.AddMonths(-ctr).Month;
                        OrdDetTable = " TransOrderDetail_" + StartDate.AddMonths(-ctr).Year + "_" + StartDate.AddMonths(-ctr).Month;

                        if (MainFunction.CheckTranTable(OrdTable, OrdDetTable) == true)
                        {

                            if (ctr > 2) { FieldName = "Month3"; } else { FieldName = "Month" + (ctr + 1); }


                            if (sp.AllItem == true)
                            {
                                strInsert = "update itemavgsale set itemavgsale." + FieldName + " = itemavgsale." + FieldName + " + x.quantity FROM ItemAvgSale,  " +
                                    " (select c.itemid, sum(c.quantity) as quantity from " + OrdTable + " B, " + OrdDetTable + " C  " +
                                    " where B.IssType = 1 and  B.IdKey = C.IdKey AND B.StationId = " + sp.Stationid +
                                    "  and b.datetime >= '" + DateTime.Now.Date.AddMonths(-sp.ShowMonth).Date + "' and b.datetime < '" + DateTime.Now.Date.AddDays(1).Date +
                                    "' group by c.itemid ) x  Where ItemAvgSale.Itemid = X.Itemid ";

                            }
                            else
                            {
                                strInsert = "update itemavgsale set itemavgsale." + FieldName + " = itemavgsale." + FieldName + " + x.quantity FROM ItemAvgSale,  " +
                                    " (select c.itemid, sum(c.quantity) as quantity from " + OrdTable + " B, " + OrdDetTable + " C " +
                                    " where B.IssType = 1 and  B.IdKey = C.IdKey AND B.StationId = " + sp.Stationid + "  " +
                                    " and b.datetime >= '" + DateTime.Now.Date.AddMonths(-sp.ShowMonth).Date + "' and b.datetime < '" + DateTime.Now.Date.AddDays(1).Date +
                                    "' and c.itemid = " + sp.ItemListID + " group by c.itemid ) x " +
                                    " Where itemavgsale.itemid = " + sp.ItemListID + " and ItemAvgSale.Itemid = X.Itemid ";

                            }

                            bt = MainFunction.SSqlExcuite(strInsert, Trans);

                        }

                        ctr = ctr + 1;

                    }


                    NoOfDays = sp.ShowMonth * 30;

                    if (sp.AllItem == true)
                    {
                        strInsert = "UPDATE ItemAvgSale SET Average = (Month1 + Month2 + Month3) / cast(" + NoOfDays + " as float)";
                        bt = MainFunction.SSqlExcuite(strInsert, Trans);

                        strInsert = "UPDATE ItemStore SET ItemStore.ROL = B.Average * " + NoOfDaysStk + ", itemstore.maxlevel = B.Average * " + maxleveldays +
                            " , itemstore.minlevel = b.average * " + minleveldays + " " +
                            " FROM ItemStore, ItemAvgSale B " +
                            " WHERE ItemStore.ItemId = B.ItemId AND ItemStore.StationId = " + sp.Stationid;
                        bt = MainFunction.SSqlExcuite(strInsert, Trans);

                    }
                    else
                    {
                        strInsert = "UPDATE ItemAvgSale SET Average = (Month1 + Month2 + Month3) / cast(" + NoOfDays + " as float) where itemid = " + sp.ItemListID;
                        bt = MainFunction.SSqlExcuite(strInsert, Trans);

                        strInsert = "UPDATE ItemStore SET ItemStore.ROL = B.Average * " + NoOfDaysStk + ", itemstore.maxlevel = B.Average * " + maxleveldays +
                            " , itemstore.minlevel = b.average * " + minleveldays +
                            " FROM ItemStore, ItemAvgSale B " +
                            " WHERE ItemStore.ItemId = B.ItemId AND itemstore.itemid=" + sp.ItemListID + " and ItemStore.StationId = " + sp.Stationid;
                        bt = MainFunction.SSqlExcuite(strInsert, Trans);

                    }

                    Trans.Commit();
                    return true;
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

    }

}