using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;


namespace MMS2
{
    public class SupplierFun
    {
        public static SupplierS GetSupplierMaster(int StationID,int SupplierIDs = 0)
        {
            try
            {
                SupplierS sp = new SupplierS();



                
                if (SupplierIDs > 0)
                {
                    String sqlStr;
                    sqlStr = " select a.id supplierID, a.name as SupplierName,a.grade,a.startdatetime as [date],a.contactPerson as contactPerson, " +
                             " a.Address1,address2,city,country,pobox,Phone1,telex, Page,cell,Email,fax,active,web_site, " +
                             " a.PaymentType,a.NetDueDays,a.DiscountDays,a.percentage,a.Creditlimit,overseas,code," +
                             " isnull(a.bankid,0) as BankId,isnull(a.AccountNo,'') as AccountNo  " +
                             " from supplier a   where a.id= " + SupplierIDs.ToString();
                    DataSet ds3 = MainFunction.SDataSet(sqlStr, "SupplierDetail");
                    foreach (DataRow RR in ds3.Tables["SupplierDetail"].Rows)
                    {
                        sp.ID = int.Parse(RR["SupplierID"].ToString());
                        sp.Name = RR["SupplierName"].ToString();
                        sp.ContactPerson = RR["contactPerson"] == null ? "" : RR["contactPerson"].ToString();
                        sp.Grade = RR["grade"] == null ? "" : RR["grade"].ToString();
                        sp.Code = RR["code"] == null ? "" : RR["code"].ToString();
                        sp.OverSeas = Boolean.Parse(RR["overseas"].ToString());
                        sp.City = RR["city"] == null ? "" : RR["city"].ToString();
                        sp.Country = RR["country"] == null ? "" : RR["country"].ToString();
                        sp.Box = RR["pobox"] == null ? "" : RR["pobox"].ToString();
                        sp.Address1 = RR["ADDRESS1"] == null ? "" : RR["ADDRESS1"].ToString();
                        sp.Address2 = RR["ADDRESS2"] == null ? "" : RR["ADDRESS2"].ToString();
                        sp.AccountNo = RR["accountNo"] == null ? "" : RR["accountNo"].ToString();
                        sp.Phone1 = RR["phone1"] == null ? "" : RR["phone1"].ToString();
                        sp.Telex = RR["TELEX"] == null ? "" : RR["TELEX"].ToString();
                        sp.Page = RR["PAGE"] == null ? "" : RR["PAGE"].ToString();
                        sp.Cell = RR["CELL"] == null ? "" : RR["CELL"].ToString();
                        sp.Fax = RR["FAX"] == null ? "" : RR["FAX"].ToString();
                        sp.Email = RR["EMAIL"] == null ? "" : RR["EMAIL"].ToString();
                        sp.web_site = RR["web_site"] == null ? "" : RR["web_site"].ToString();

                        sp.Percentage = byte.Parse(RR["percentage"].ToString());
                        sp.NetDueDays = byte.Parse(RR["NetDueDays"].ToString());
                        sp.DiscountDays = byte.Parse(RR["Discountdays"].ToString());
                        sp.CreditLimit = byte.Parse(RR["creditlimit"].ToString());

                        sp.Active = RR["active"] == null ? false : bool.Parse(RR["active"].ToString());
                        sp.BankID = int.Parse(RR["bankid"].ToString());
                        sp.PaymentType = byte.Parse(RR["paymentType"].ToString());

                    }

                                         sp.BankList = new List<BankS>();
                    int FirstRecordOnly = 0;                     sqlStr = " select id,name  from  bank where deleted=0 order by name ";
                    DataSet ds = MainFunction.SDataSet(sqlStr, "BankList");
                    foreach (DataRow VR in ds.Tables["BankList"].Rows)
                    {
                        if (FirstRecordOnly == 0)
                        {
                            BankS Tempu = new BankS();
                            Tempu.ID = 0;
                            Tempu.Name = "n/a";
                            sp.BankList.Add(Tempu);
                            FirstRecordOnly += 1;
                        }

                        BankS u = new BankS();
                        u.ID = int.Parse(VR["id"].ToString());
                        u.Name = VR["Name"].ToString();
                        sp.BankList.Add(u);

                    }

                                         sp.ItemList = new List<MMS_ItemMaster>();
                                         sp.ItemList = MainFunction.CashedAllItems(StationID, " and id in (select top 10 id from item where deleted=0)");
                    
                                                                                                                                                                                             
                     
                                         sp.SelectedSupplierItems = new List<SupplierItems>();

                    sqlStr = " select item.itemcode itemcode,item.name name,Itemid from Supplieritem,Item where SupplierId=" +
                       SupplierIDs + " and Item.ID=SupplierItem.ITemID order by Item.Name";
                    DataSet ds4 = MainFunction.SDataSet(sqlStr, "SelItemList");
                    foreach (DataRow VR in ds4.Tables["SelItemList"].Rows)
                    {
                        SupplierItems Fuu = new SupplierItems();
                        Fuu.ItemID = int.Parse(VR["ITEMID"].ToString());
                        Fuu.Itemname = VR["Name"].ToString();
                        Fuu.Itemcode = VR["ITEMCODE"].ToString();
                        sp.SelectedSupplierItems.Add(Fuu);

                        var itemToRemove = sp.ItemList.SingleOrDefault(r => r.Id == Fuu.ItemID);                         if (itemToRemove != null) { sp.ItemList.Remove(itemToRemove); }
                    }





                                         sp.ManufacturerList = new List<Manufacturer>();
                    sqlStr = " Select id,name from manufacturer where deleted=0 order by name";
                    DataSet ds2 = MainFunction.SDataSet(sqlStr, "ManList");
                    foreach (DataRow VR in ds2.Tables["ManList"].Rows)
                    {
                        Manufacturer ff = new Manufacturer();
                        ff.id = int.Parse(VR["id"].ToString());
                        ff.Name = VR["Name"].ToString();
                        sp.ManufacturerList.Add(ff);

                    }

                                         sp.SelectedManufacturer = new List<SupplierManf>();
                    sqlStr = "select manufacturer.id ID,manufacturer.NAME NAME from Suppliermanufacturer,manufacturer where SupplierId=" + SupplierIDs +
                        " and manufacturer.ID=Suppliermanufacturer.manufacturerID order by manufacturer.Name";
                    DataSet ds5 = MainFunction.SDataSet(sqlStr, "ManList");
                    foreach (DataRow VR in ds5.Tables["ManList"].Rows)
                    {
                        SupplierManf Fuu = new SupplierManf();
                        Fuu.ManufacturerID = int.Parse(VR["id"].ToString());
                        Fuu.name = VR["Name"].ToString();
                        sp.SelectedManufacturer.Add(Fuu);

                        var itemToRemove = sp.ManufacturerList.SingleOrDefault(r => r.id == Fuu.ManufacturerID);                         if (itemToRemove != null) { sp.ManufacturerList.Remove(itemToRemove); }
                    }





                }



                return sp;
            }
            catch (Exception e)
            { SupplierS sp = new SupplierS(); sp.SupplierErr = "GetDetails>>" + e.Message; return sp; }

        }
        public static SupplierS GetSupplierMasterNew(int StationID)
        {
            SupplierS sp = new SupplierS();
            sp.Active = false;             sp.StartdateTime = DateTime.Parse(DateTime.Now.ToShortDateString());             sp.BankID = 0; 
                                                                                                                                  
             
                         sp.BankList = new List<BankS>();
            int FirstRecordOnly = 0;             String sqlStr = " select id,name  from  bank where deleted=0 order by name ";
            DataSet ds = MainFunction.SDataSet(sqlStr, "BankList");
            foreach (DataRow VR in ds.Tables["BankList"].Rows)
            {
                if (FirstRecordOnly == 0)
                {
                    BankS Tempu = new BankS();
                    Tempu.ID = 0;
                    Tempu.Name = "n/a";
                    sp.BankList.Add(Tempu);
                    FirstRecordOnly += 1;
                }

                BankS u = new BankS();
                u.ID = int.Parse(VR["id"].ToString());
                u.Name = VR["Name"].ToString();
                sp.BankList.Add(u);
            }


                         sp.ItemList = new List<MMS_ItemMaster>();
                         sp.ItemList = MainFunction.CashedAllItems(StationID," and id in (select top 10 id from item where deleted=0)");
                                                                                                                     
             

                         sp.ManufacturerList = new List<Manufacturer>();
            sqlStr = " Select id,name from manufacturer where deleted=0 order by name";
            DataSet ds2 = MainFunction.SDataSet(sqlStr, "ManList");
            foreach (DataRow VR in ds2.Tables["ManList"].Rows)
            {
                Manufacturer ff = new Manufacturer();
                ff.id = int.Parse(VR["id"].ToString());
                ff.Name = VR["Name"].ToString();
                sp.ManufacturerList.Add(ff);

            }

                                                    
                                                    
            return sp;


        }
        public static List<SupplierS> GetSupplierList()
        {

            List<SupplierS> sss = new List<SupplierS>();
            String sqlStr = " select Id,name,startdatetime as [Date] ,ContactPerson,active from  supplier where deleted=0 ";

            DataSet ds = MainFunction.SDataSet(sqlStr, "SList");
            foreach (DataRow VR in ds.Tables[0].Rows)
            {
                SupplierS u = new SupplierS();
                u.ID = int.Parse(VR["id"].ToString());
                u.Name = VR["Name"].ToString();
                u.StartdateTime = DateTime.Parse(VR["date"].ToString());
                u.ContactPerson = VR["contactPerson"].ToString();
                u.Active = (VR["active"] != DBNull.Value) ? (Boolean)VR["active"] : false;

                sss.Add(u);
            }

            return sss;
        }
        public static Boolean DeleteSupplier(int SupplierID)
        {
            bool ResultsMsg;
                         String sqlStr = "select id from purchaseorder where  status<=1 and supplierid=" + SupplierID;
            DataSet ds = MainFunction.SDataSet(sqlStr, "CheckTbl");
            if (ds.Tables["CheckTbl"].Rows.Count > 0)
            {
                                 ResultsMsg = false;
            }
            else
            {
                sqlStr = "Update supplier set deleted=1 WHERE  id=" + SupplierID;
                bool COM = MainFunction.SSqlExcuite(sqlStr);
                                 ResultsMsg = true;
            }
            return ResultsMsg;

        }
        public static Boolean SaveSupplier(SupplierS sp, int SupplierID)
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
                        if (sp.OverSeas == false) { OverSeaVal = 0; } else { OverSeaVal = 1; }
                        if (sp.Active == false || sp.Active == null) { IsActive = 0; } else { IsActive = 1; }

                        if (SupplierID == 0)
                        {
                            sp.Active = false;
                            sqlStr = "Insert into supplier(active,Name,contactperson,grade,address1,address2,city,country,POBox," +
                                             "Phone1,telex,Page,cell,fax,email,web_site,deleted,startdatetime," +
                                             "PaymentType,NetDueDays,DiscountDays,percentage,Creditlimit,vendorstatus,overseas,code,BankId,AccountNo)" +
                                             " values(" + IsActive + ",'" + sp.Name + "','" + sp.ContactPerson + "','" + sp.Grade + "','" + sp.Address1 + "','" + sp.Address2 + "'," +
                                             "'" + sp.City + "','" + sp.Country + "','" + sp.Box + "','" + sp.Phone1 + "','" + sp.Phone2 + "','" + sp.Page + "'," +
                                             "'" + sp.Cell + "','" + sp.Fax + "','" + sp.Email + "','" + sp.web_site + "',0,sysdatetime() ," +
                                             "" + sp.PaymentType + "," + sp.NetDueDays + "," + sp.DiscountDays + "," + sp.Percentage + "," + sp.CreditLimit +
                                             ",0," + OverSeaVal + ",'" + sp.Code + "'," + sp.BankID + ",'" + sp.AccountNo + "')";
                        }
                        else
                        {                              bool DelItemsList = MainFunction.SSqlExcuite("delete from supplieritem where supplierid=" + SupplierID, Trans);
                            bool DelManufact = MainFunction.SSqlExcuite("delete from suppliermanufacturer where supplierid=" + SupplierID, Trans);

                            sqlStr = "Update supplier set name='" + sp.Name + "',contactperson='" + sp.ContactPerson + "'," +
                            "grade='" + sp.Grade + "',address1='" + sp.Address1 + "',address2='" + sp.Address2 + "',city='" + sp.City + "'," +
                            "country='" + sp.Country + "',pobox='" + sp.Box + "',phone1='" + sp.Phone1 + "',telex='" + sp.Phone2 + "',page='" + sp.Page +
                            "',cell='" + sp.Cell + "',fax='" + sp.Fax + "',email='" + sp.Email + "',web_site='" + sp.web_site + "',code='" + sp.Code +
                            "' where id= " + SupplierID;
                        }
                        bool COM = MainFunction.SSqlExcuite(sqlStr, Trans);
                        int NewSupplierID;
                        
                        if (SupplierID == 0)
                        {
                            NewSupplierID = MainFunction.GetInTransValue("max(id)", "supplier", "", Trans);
                        }
                        else
                        {
                            NewSupplierID = SupplierID;
                        }

                        
                        try
                        {
                            if (sp.SelectedSupplierItems != null)
                            {                                   if (sp.SelectedSupplierItems.Count > 0)
                                {
                                    IList<SupplierItems> ss = sp.SelectedSupplierItems;
                                    foreach (SupplierItems vr in sp.SelectedSupplierItems)
                                    {
                                        COM = MainFunction.SSqlExcuite("Insert into SupplierItem(supplierid,itemid) values(" + NewSupplierID + "," + vr.ItemID + ")",
                                          Trans);
                                    }
                                }

                            };
                        }
                        catch (Exception e) { ;}  


                        
                        try
                        {
                            if (sp.SelectedManufacturer != null)
                            {
                                if (sp.SelectedManufacturer.Count > 0 && String.IsNullOrEmpty(sp.SelectedManufacturer.ToString()) == false)
                                {
                                    List<SupplierManf> mm = sp.SelectedManufacturer;
                                    foreach (SupplierManf vr in sp.SelectedManufacturer)
                                    {
                                        COM = MainFunction.SSqlExcuite("Insert into Suppliermanufacturer(supplierid,manufacturerid) values(" + NewSupplierID + "," + vr.ManufacturerID + ")",
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
                    sp.SupplierErr = "Saving>> Server Side validate Error!" + sp.SupplierErr;
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
                sp.SupplierErr = "Saving>>" + e.Message;
                return false;
            }
        }
        public static Boolean CheckData(SupplierS sp)
        {

                         DataSet ds = MainFunction.SDataSet("select code from supplier where code='" + sp.Code.Trim() + "' and id not in (0," + sp.ID + ")", "SupplierCodes");
            if (ds.Tables[0].Rows.Count > 0) { sp.SupplierErr="THIS CODE ALREADY USED";return false; }

            if (sp.PaymentType == 1 && (sp.Percentage == 0)) { sp.SupplierErr = "Complete Payment Detail information!"; return false; }
            if (sp.PaymentType == 2 && ((sp.Percentage == 0) || (sp.NetDueDays == 0) || (sp.DiscountDays == 0))) { sp.SupplierErr = "Complete Payment Detail information!"; return false; }
            return true;
        }

    }
}