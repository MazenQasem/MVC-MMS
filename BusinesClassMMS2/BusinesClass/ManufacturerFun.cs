using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;


namespace MMS2
{
    public class ManufacturerFun
    {
        public static List<ManufacturerS> GetManufacturerList()
        {

            List<ManufacturerS> sss = new List<ManufacturerS>();
            String sqlStr = " select id,name,startdatetime as [Date] ,ContactPerson,active from  Manufacturer where deleted=0 ";

            DataSet ds = MainFunction.SDataSet(sqlStr, "MList");
            foreach (DataRow VR in ds.Tables[0].Rows)
            {
                ManufacturerS Mu = new ManufacturerS();
                Manufacturer u = new Manufacturer();
                u.id = int.Parse(VR["id"].ToString());
                u.Name = VR["Name"].ToString();
                u.StartdateTime = DateTime.Parse(VR["date"].ToString());
                u.ContactPerson = VR["contactPerson"].ToString();
                u.Active = (VR["active"] != DBNull.Value) ? (Boolean)VR["active"] : false;
                Mu.Manufacturerx = u;
                sss.Add(Mu);
            }

            return sss;
        }
        public static ManufacturerS GetManufacturerMasterNew(int StationID)
        {
            ManufacturerS sp = new ManufacturerS();
            Manufacturer MM = new Manufacturer();
            MM.Active = false;              MM.StartdateTime = DateTime.Parse(DateTime.Now.ToShortDateString());             MM.OverSeas = false;
            sp.Manufacturerx = MM;

                          

                         sp.ItemList = new List<MMS_ItemMaster>();
                                      sp.ItemList = MainFunction.CashedAllItems(StationID, " and id in (select top 10 id from item where deleted=0)");

                                                                                                                     
             

                          sp.SupplierList = new List<SupplierS>();
             string sqlStr = " Select id,name from supplier where deleted=0 order by name";
            DataSet ds2 = MainFunction.SDataSet(sqlStr, "ManList");
            foreach (DataRow VR in ds2.Tables["ManList"].Rows)
            {
                SupplierS Fuu = new SupplierS();
                Fuu.ID = int.Parse(VR["id"].ToString());
                Fuu.Name = VR["Name"].ToString();
                sp.SupplierList.Add(Fuu);

            }


            return sp;


        }
        public static ManufacturerS GetManufacturerMaster(int StationID,int ManufacturereIDs = 0)
        {
            try
            {
                ManufacturerS sp = new ManufacturerS();



                
                if (ManufacturereIDs > 0)
                {
                    String sqlStr;
                    sqlStr = " select a.id ManufacturerID, a.name as ManufacturerName,a.grade,a.startdatetime as [date],a.contactPerson as contactPerson, " +
                             " a.Address1,address2,city,country,pobox,Phone1, Page,cell,Email,fax,active,web_site, " +
                             " a.PaymentType,a.NetDueDays,a.DiscountDays,a.percentage,a.Creditlimit,overseas,code" +
                             " from manufacturer a   where a.id= " + ManufacturereIDs.ToString();
                    DataSet ds3 = MainFunction.SDataSet(sqlStr, "ManufacturereDetail");
                    foreach (DataRow RR in ds3.Tables["ManufacturereDetail"].Rows)
                    {
                        Manufacturer MM=new Manufacturer();
                        MM.id = int.Parse(RR["ManufacturerID"].ToString());
                        MM.Name = RR["ManufacturerName"].ToString();
                        MM.ContactPerson = RR["contactPerson"] == null ? "" : RR["contactPerson"].ToString();
                        MM.Grade = RR["grade"] == null ? "" : RR["grade"].ToString();
                        MM.Code = RR["code"] == null ? "" : RR["code"].ToString();

                        MM.OverSeas = MainFunction.NullToBool(RR["overseas"].ToString());
                        MM.City = RR["city"] == null ? "" : RR["city"].ToString();
                        MM.Country = RR["country"] == null ? "" : RR["country"].ToString();
                        MM.POBox =MainFunction.NullToInteger(RR["pobox"].ToString());
                        MM.Address1 = RR["ADDRESS1"] == null ? "" : RR["ADDRESS1"].ToString();
                        MM.Address2 = RR["ADDRESS2"] == null ? "" : RR["ADDRESS2"].ToString();

                        MM.Phone1 = RR["phone1"] == null ? "" : RR["phone1"].ToString();

                        MM.Page = RR["PAGE"] == null ? "" : RR["PAGE"].ToString();
                        MM.Cell = RR["CELL"] == null ? "" : RR["CELL"].ToString();
                        MM.Fax = RR["FAX"] == null ? "" : RR["FAX"].ToString();
                        MM.Email = RR["EMAIL"] == null ? "" : RR["EMAIL"].ToString();
                        MM.web_site = RR["web_site"] == null ? "" : RR["web_site"].ToString();

                        MM.Percentage = MainFunction.NullToByte(RR["percentage"].ToString());
                        MM.NetDueDays = MainFunction.NullToByte(RR["NetDueDays"].ToString());
                        MM.DiscountDays = MainFunction.NullToByte(RR["Discountdays"].ToString());
                        MM.CreditLimit = MainFunction.NullToSingleOrReal(RR["creditlimit"].ToString());

                        MM.Active = MainFunction.NullToBool(RR["active"].ToString());

                        MM.PaymentType = MainFunction.NullToByte(RR["paymentType"].ToString());
                        sp.Manufacturerx = MM;

                    }

                                         sp.ItemList = new List<MMS_ItemMaster>();
                                         sp.ItemList = MainFunction.CashedAllItems(StationID," and id in (select top 10 id from item where deleted=0)");
                                                                                                                                                                                             
                     
                                         sp.SelectedManufacturerItems = new List<ManufactrureItems>();

                    sqlStr = " select item.itemcode itemcode,item.name name,Itemid from manufactureritem,Item where manufactureritem.ManufacturerId=" +
                       ManufacturereIDs + " and Item.ID=ManufacturerItem.ITemID order by Item.Name";
                    DataSet ds4 = MainFunction.SDataSet(sqlStr, "SelItemList");
                    foreach (DataRow VR in ds4.Tables["SelItemList"].Rows)
                    {
                        ManufactrureItems Fuu = new ManufactrureItems();
                        Fuu.ItemID = int.Parse(VR["ITEMID"].ToString());
                        Fuu.ItemName = VR["Name"].ToString();
                        Fuu.ItemCode = VR["ITEMCODE"].ToString();
                        sp.SelectedManufacturerItems.Add(Fuu);

                        var itemToRemove = sp.ItemList.SingleOrDefault(r => r.Id == Fuu.ItemID);                         if (itemToRemove != null) { sp.ItemList.Remove(itemToRemove); }
                    }




                                         sp.SupplierList = new List<SupplierS>();
                    sqlStr = " Select id,name from supplier where deleted=0 order by name";
                    DataSet ds2 = MainFunction.SDataSet(sqlStr, "ManList");
                    foreach (DataRow VR in ds2.Tables["ManList"].Rows)
                    {
                        SupplierS Fuu = new SupplierS();
                        Fuu.ID = int.Parse(VR["id"].ToString());
                        Fuu.Name = VR["Name"].ToString();
                        sp.SupplierList.Add(Fuu);

                    }

                                         sp.SelectedSupplier = new List<ManufacturerSupplier>();
                    sqlStr = "select supplier.id ID,supplier.NAME NAME from Suppliermanufacturer,supplier where Suppliermanufacturer.manufacturerID=" + ManufacturereIDs +
                        " and supplier.ID=Suppliermanufacturer.SupplierID order by supplier.Name";
                    DataSet ds5 = MainFunction.SDataSet(sqlStr, "ManList");
                    foreach (DataRow VR in ds5.Tables["ManList"].Rows)
                    {
                        ManufacturerSupplier Fuu = new ManufacturerSupplier();
                        Fuu.SupplierID = int.Parse(VR["id"].ToString());
                        Fuu.name = VR["Name"].ToString();
                        sp.SelectedSupplier.Add(Fuu);

                        var itemToRemove = sp.SupplierList.SingleOrDefault(r => r.ID == Fuu.SupplierID);                         if (itemToRemove != null) { sp.SupplierList.Remove(itemToRemove); }
                    }
                }
                return sp;
            }
            catch (Exception e)
            { ManufacturerS sp = new ManufacturerS(); sp.MessageShow = "GetDetails>>" + e.Message; return sp; }
        }

        
        public static Boolean DeleteManufacturer(int ManufacturerID,List<ManufacturerSupplier> SupplierID)
        {
            bool ResultsMsg;
            String sqlStr;
             
            if (SupplierID !=null &&   SupplierID.Count > 0)
            {
                sqlStr = "select id from purchaseorder where  status<=1 and supplierid=" + SupplierID[0].SupplierID;
                DataSet ds = MainFunction.SDataSet(sqlStr, "CheckTbl");
                if (ds.Tables["CheckTbl"].Rows.Count > 0)
                {
                                         ResultsMsg = false;
                }
                else
                {
                    sqlStr = "Update manufacturer set deleted=1 WHERE  id=" + ManufacturerID;
                    bool COM = MainFunction.SSqlExcuite(sqlStr);
                                         ResultsMsg = true;
                }
            }
            else
            {
                sqlStr = "Update manufacturer set deleted=1 WHERE  id=" + ManufacturerID;
                bool COM = MainFunction.SSqlExcuite(sqlStr);
                                 ResultsMsg = true;
            }
            return ResultsMsg;
        }
        public static Boolean SaveManufacturer(ManufacturerS sp, int ManufacturerID)
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


                         
                        int OverSeaVal;
                        int IsActive;
                        String sqlStr = "";
                        if (sp.Manufacturerx.OverSeas == false) { OverSeaVal = 0; } else { OverSeaVal = 1; }
                        if (sp.Manufacturerx.Active == false ) { IsActive = 0; } else { IsActive = 1; }

                        int NewManufacturerID;
                        
                        if (ManufacturerID == 0)
                        {
                            NewManufacturerID = MainFunction.GetInTransValue("max(id)", "manufacturer", "", Trans);
                        }
                        else
                        {
                            NewManufacturerID = ManufacturerID;
                        }


                        if (ManufacturerID == 0)
                        {
                            sp.Manufacturerx.Active = false;
                            sqlStr = "Insert into manufacturer(ID,active,Name,contactperson,grade,address1,address2,city,country,POBox," +
                                             "Phone1,phone2,Page,cell,fax,email,web_site,deleted,startdatetime," +
                                             "PaymentType,NetDueDays,DiscountDays,percentage,Creditlimit,vendorstatus,overseas,code)" +
                                             " values(" + NewManufacturerID + "," + IsActive + ",'" + sp.Manufacturerx.Name + "','" + sp.Manufacturerx.ContactPerson + "','" + sp.Manufacturerx.Grade + "','" + sp.Manufacturerx.Address1 + "','" + sp.Manufacturerx.Address2 + "'," +
                                             "'" + sp.Manufacturerx.City + "','" + sp.Manufacturerx.Country + "','" + sp.Manufacturerx.POBox + "','" + sp.Manufacturerx.Phone1 + "','" + sp.Manufacturerx.Phone2 + "','" + sp.Manufacturerx.Page + "'," +
                                             "'" + sp.Manufacturerx.Cell + "','" + sp.Manufacturerx.Fax + "','" + sp.Manufacturerx.Email + "','" + sp.Manufacturerx.web_site + "',0,sysdatetime() ," +
                                             "" + MainFunction.NullToSingleOrReal(sp.Manufacturerx.PaymentType.ToString()) + "," + MainFunction.NullToSingleOrReal(sp.Manufacturerx.NetDueDays.ToString()) 
                                             + "," +MainFunction.NullToSingleOrReal(sp.Manufacturerx.DiscountDays.ToString()) + "," +MainFunction.NullToSingleOrReal(sp.Manufacturerx.Percentage.ToString()) 
                                             + "," +MainFunction.NullToSingleOrReal(sp.Manufacturerx.CreditLimit.ToString()) +
                                             ",0," + OverSeaVal + ",'" + sp.Manufacturerx.Code + "')";

                                                                                  }
                        else
                        {  
                            bool DelItemsList = MainFunction.SSqlExcuite("Delete from manufactureritem where manufacturerid=" + ManufacturerID, Trans);
                                                         bool DelManufact = MainFunction.SSqlExcuite("delete from suppliermanufacturer where ManufacturerID=" + ManufacturerID, Trans);

                            sqlStr = "Update manufacturer set name='" + sp.Manufacturerx.Name + "',contactperson='" + sp.Manufacturerx.ContactPerson + "'," +
                            "grade='" + sp.Manufacturerx.Grade + "',address1='" + sp.Manufacturerx.Address1 + "',address2='" + sp.Manufacturerx.Address2 + "',city='" + sp.Manufacturerx.City + "'," +
                            "country='" + sp.Manufacturerx.Country + "',pobox=" + sp.Manufacturerx.POBox + ",phone1='" + sp.Manufacturerx.Phone1 + "',phone2='" + sp.Manufacturerx.Phone2 + "',page='" + sp.Manufacturerx.Page +
                            "',cell='" + sp.Manufacturerx.Cell + "',fax='" + sp.Manufacturerx.Fax + "',email='" + sp.Manufacturerx.Email + "',web_site='" + sp.Manufacturerx.web_site + "',code='" + sp.Manufacturerx.Code + "'," +
                            "overseas=" +  OverSeaVal + " where id= " + ManufacturerID;
                        }
                        bool COM = MainFunction.SSqlExcuite(sqlStr, Trans);
                        

                        
                        try
                        {
                            if (sp.SelectedManufacturerItems != null)
                            {                                   if (sp.SelectedManufacturerItems.Count > 0)
                                {
                                    IList<ManufactrureItems> ss = sp.SelectedManufacturerItems;
                                    foreach (ManufactrureItems vr in sp.SelectedManufacturerItems)
                                    {
                                        COM = MainFunction.SSqlExcuite("Insert into manufactureritem(manufacturerid,itemid,PartNo) " +
                                            " values(" + NewManufacturerID + "," + vr.ItemID + ",'" + vr.PartNo + "')",
                                          Trans);
                                    }
                                }

                            };
                        }
                        catch (Exception e) { ;}  


                        
                        try
                        {
                            if (sp.SelectedSupplier != null)
                            {
                                if (sp.SelectedSupplier.Count > 0 && String.IsNullOrEmpty(sp.SelectedSupplier.ToString()) == false)
                                {
                                    List<ManufacturerSupplier> mm = sp.SelectedSupplier;
                                    foreach (ManufacturerSupplier vr in sp.SelectedSupplier)
                                    {
                                        COM = MainFunction.SSqlExcuite("Insert into Suppliermanufacturer(supplierid,manufacturerid) values(" + vr.SupplierID + "," + NewManufacturerID + ")",
                                          Trans);
                                    }
                                }
                            }
                        }
                        catch (Exception e) { ;}  

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
                    sp.MessageShow = "Saving>> Server Side validate Error!" + sp.MessageShow;
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
                sp.MessageShow = "Saving>>" + e.Message;
                return false;
            }
        }
        public static Boolean CheckData(ManufacturerS sp)
        {

                         DataSet ds = MainFunction.SDataSet("select code from manufacturer where code='" + sp.Manufacturerx.Code.Trim() + "' and id not in (0," + sp.Manufacturerx.id + ")", "ManufacturerCodes");
            if (ds.Tables[0].Rows.Count > 0) { sp.MessageShow = "THIS CODE ALREADY USED"; return false; }
            if (sp.Manufacturerx.Percentage > 100) { sp.MessageShow = "Discount Percent cannot be greater than 100!"; return false; }
            if (sp.Manufacturerx.PaymentType == 1 && (sp.Manufacturerx.Percentage == 0)) { sp.MessageShow = "Complete Payment Detail information!"; return false; }
            if (sp.Manufacturerx.PaymentType == 2 && ((sp.Manufacturerx.Percentage == 0) || (sp.Manufacturerx.NetDueDays == 0) || (sp.Manufacturerx.DiscountDays == 0))) { sp.MessageShow = "Complete Payment Detail information!"; return false; }
            return true;
        }

        
        public static List<TempListMdl> GetCategoryItem(string srch, int StationID)
        {
                         var CASHEDITEM = MainFunction.CashedAllItems(StationID, " and name like'%" + srch + "%'");
            var TBL = (from a in CASHEDITEM
                       where a.Name.Contains(srch)
                       select new
                       {
                           ID = a.Id,
                           Name = a.Name
                       });

             List<TempListMdl> NewList = new List<TempListMdl>();
            foreach (var a in TBL.ToList())
            {
                TempListMdl itm = new TempListMdl();
                itm.ID = a.ID;
                itm.Name = a.Name;
                NewList.Add(itm);
            }
            return NewList;
        }

    }
}