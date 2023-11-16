using System;
using System.Collections.Generic;
using System.Linq;

using System.Data.SqlClient;
using System.Data;

namespace MMS2
{
    public class ItemFun
    {
        
        public static List<TempListMdl> listMdl(String sqlstr, int stationid = 0)
        {
            List<TempListMdl> tnList = new List<TempListMdl>();
            TempListMdl tn = new TempListMdl();
            DataSet ds = MainFunction.SDataSet(sqlstr, "T1");
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                tn = new TempListMdl();
                tn.ID = int.Parse(r["ID"].ToString());
                tn.Name = r["Name"].ToString();
                switch (stationid)
                {
                    case 4: if (tn.ID == 17) { tn.Selected = true; }; break;
                    case 95: if (tn.ID == 17) { tn.Selected = true; }; break;
                    case 103: if (tn.ID == 17) { tn.Selected = true; }; break;
                    case 200: if (tn.ID == 17) { tn.Selected = true; }; break;
                    case 224: if (tn.ID == 17) { tn.Selected = true; }; break;
                    case 228: if (tn.ID == 17) { tn.Selected = true; }; break;
                    case 231: if (tn.ID == 17) { tn.Selected = true; }; break;
                    default:
                        break;
                }

                tnList.Add(tn);
            }
            return tnList;



        }
        public static List<ItemLookUp> ItemLookUpResult(string str)
        {
            List<ItemLookUp> ls = new List<ItemLookUp>();
            DataSet tn = MainFunction.SDataSet(str, "t1");
            foreach (DataRow dr in tn.Tables[0].Rows)
            {
                string Supplier = "";
                DataSet SuppName =
                    MainFunction.SDataSet("select a.name as Name from supplier a,supplieritem b " +
                    " where a.id=b.supplierid and b.itemid=" + MainFunction.NullToInteger(dr["ID"].ToString()), "t2");
                foreach (DataRow Tr in SuppName.Tables[0].Rows)
                { Supplier = Tr["Name"].ToString(); }


                ItemLookUp nls = new ItemLookUp();
                nls.ID = MainFunction.NullToInteger(dr["ID"].ToString());
                nls.ItemName = dr["ItemName"].ToString();
                nls.ManuName = dr["ManuName"].ToString();
                nls.GroupName = dr["GroupName"].ToString();
                nls.SupName = Supplier;
                nls.ItemCode = dr["ItemCode"].ToString();
                nls.SellPrice = MainFunction.NullToDecmial(dr["SellPrice"].ToString());
                ls.Add(nls);
            }
            return ls;

        }
        public static Item ItemLookUpBatches(int ItemID, int StationID)
        {
            var ItemList = new List<MMS_ItemMaster>();
                         ItemList = MainFunction.CashedAllItems(StationID, " and id =" + ItemID);

            Item it = new Item();
            var an = (from m in ItemList
                      where m.Id == ItemID
                      select new
                      {
                          ItemID = m.Id,
                          MinLevel = m.MinLevel,
                          MaxLevel = m.MaxLevel,
                          quantity = m.QOH,
                          ROL = m.ROL,
                          ROQ = m.ROQ,
                          Uom = m.UnitID,
                          ConverQty = m.ConversionQty
                      });

                         it.QOH = 0;
            it.MinLevel = 0;
            it.MaxLevel = 0;
            it.ROL = 0;
            it.ROQ = 0;
            it.StoreUnitID = 0;
            it.StoreConversionQty = 0;
            it.UOM = "";
            it.location = "";



            foreach (var row in an)
            {
                it.ID = row.ItemID;

                it.QOH = getItemQOH(it.ID, StationID) / (Decimal)MainFunction.NullToOne(row.ConverQty.ToString());
                it.MinLevel = Math.Round((decimal)(MainFunction.NullToInteger2(row.MinLevel.ToString()) / (Decimal)MainFunction.NullToOne(row.ConverQty.ToString())), 2);
                it.MaxLevel = Math.Round((decimal)(MainFunction.NullToInteger2(row.MaxLevel.ToString()) / (Decimal)MainFunction.NullToOne(row.ConverQty.ToString())), 2);
                                  
                it.ROL = Math.Round((decimal)(MainFunction.NullToInteger2(row.ROL.ToString()) / (Decimal)MainFunction.NullToOne(row.ConverQty.ToString())), 2);
                                 it.ROQ = Math.Round((decimal)(MainFunction.NullToInteger2(row.ROQ.ToString()) / (Decimal)MainFunction.NullToOne(row.ConverQty.ToString())), 2);
                                 it.UOM = getItemUnit(row.Uom);
                it.StoreConversionQty = row.ConverQty;
                it.location = getItemLocation(row.ItemID, StationID);
            }

            it.BatchInfo = new List<BatchInfo>();
            DataSet BATCHES = MainFunction.SDataSet("Select  ROW_NUMBER() over (order by a.batchno) as slno, a.BatchNo," +
                " convert(varchar(20),a.ExpiryDate,105) as ExpiryDate,b.Quantity,a.sellingprice from batch a,batchstore b " +
                " where b.quantity > 0 and a.ItemId=" + it.ID + " and a.batchno=b.batchno and a.batchid=b.batchid " +
                " and b.stationid=" + StationID, "TW");
            foreach (DataRow RR in BATCHES.Tables[0].Rows)
            {
                BatchInfo BT = new BatchInfo();
                BT.slno = MainFunction.NullToInteger(RR["slno"].ToString());
                BT.BatchNo = RR["BatchNo"].ToString();
                BT.Quantity = MainFunction.NullToInteger(RR["Quantity"].ToString());
                BT.ExpiryDate = RR["ExpiryDate"].ToString();
                BT.SellingPrice = MainFunction.NullToDecmial(RR["sellingprice"].ToString());
                it.BatchInfo.Add(BT);
            }

            return it;

        }
        public static string getItemUnit(int? Uid)
        {
            string UName = "";
            DataSet temp = MainFunction.SDataSet("select name from packing where id=" + Uid, "t1");
            foreach (DataRow rr in temp.Tables[0].Rows)
            {
                UName = rr["name"].ToString();
            }

            return UName;
        }
        public static Decimal getItemQOH(int? Itemid, int stationid)
        {
            Decimal UName = 0;
            DataSet temp = MainFunction.SDataSet("select sum(quantity) as qty from batchstore where itemid=" + Itemid
                + " and stationid=" + stationid, "t1");
            foreach (DataRow rr in temp.Tables[0].Rows)
            {
                UName = MainFunction.NullToDecmial(rr["qty"].ToString());
            }

            return UName;
        }
        public static string getItemLocation(int? Uid, int stationid)
        {
            string UName = "";
            DataSet temp = MainFunction.SDataSet(
                " select r.name as name from itemlocation l,rack r Where l.Itemid = " + Uid + " And l.rackId = r.id and l.stationid=" + stationid +
                " Union select s.name as name from itemlocation l,shelf s Where l.Itemid = " + Uid + " And l.ShelfId = s.id and l.stationid=" + stationid
                , "t1");
            int count = 0;
            foreach (DataRow rr in temp.Tables[0].Rows)
            {
                if (count == 0) { UName = rr["name"].ToString(); }
                else { UName += "--" + rr["name"].ToString(); }
                count += 1;
            }
            return UName;
        }

        
        
        
        public static MMS_ItemMaster ItemSubStoreResults(int Itemid, int stationid)
        {
            MMS_ItemMaster uu = new MMS_ItemMaster();
            try
            {
                                 List<MMS_ItemMaster> ItemList = MainFunction.CashedAllItems(stationid, " and id=" + Itemid);

                                 var n = ItemList.Where(q => q.Id == Itemid && MainFunction.NullToBool(q.Deleted.ToString()) == false).ToList();
                foreach (var VR in n)
                {
                    uu.Id = VR.Id;
                    uu.ItemCode = VR.ItemCode;
                    uu.Name = VR.Name;
                                         uu.ROL = MainFunction.NullToInteger(VR.ROL.ToString());
                    decimal qoh = getItemQOH(VR.Id, stationid) / (Decimal)MainFunction.NullToOne(VR.ConversionQty.ToString());
                                         uu.strQOH = qoh.ToString();
                    uu.ROQ = MainFunction.NullToInteger(VR.ROQ.ToString());
                    uu.Tax = MainFunction.NullToInteger(VR.Tax.ToString());
                    uu.UnitID = MainFunction.NullToInteger(VR.UnitID.ToString());
                    uu.ConversionQty = MainFunction.NullToInteger(VR.ConversionQty.ToString());
                    uu.Strength = VR.Strength;
                    uu.MaxLevel = MainFunction.NullToInteger2(VR.MaxLevel.ToString());
                    uu.MinLevel = MainFunction.NullToInteger2(VR.MinLevel.ToString());
                    uu.ABC = MainFunction.NullToByte(VR.ABC.ToString());
                    uu.FSN = MainFunction.NullToByte(VR.FSN.ToString());
                    uu.VED = MainFunction.NullToByte(VR.VED.ToString());
                    uu.StartDateTime = DateTime.Parse(VR.StartDateTime.ToString("dd MMM yy"));
                    uu.ProfitCenter = VR.ProfitCenter;
                    uu.DrugType = VR.DrugType;
                    switch (uu.DrugType)
                    {
                        case 0: uu.ItemType = "Drug"; break;
                        case 1: uu.ItemType = "Consumable"; break;
                        case 2: uu.ItemType = "Others"; break;
                        default:
                            break;
                    }
                    uu.ItemCategory = MainFunction.GetName("select Name from Itemgroup where id=" + VR.CategoryID, "Name");
                    uu.UOMName = MainFunction.GetName("select Name from Packing where id=" + VR.UnitID, "Name");

                    uu.PackingList = new List<TempListMdl>();
                    DataSet GetPacking = MainFunction.SDataSet("select i.packid,p.name from itempacking i, packing p where i.packid = p.id and i.itemid =" + VR.Id, "Pk");
                    foreach (DataRow rr in GetPacking.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["packid"].ToString());
                        tmp.Name = rr["name"].ToString();
                        uu.PackingList.Add(tmp);
                    }
                }

                return uu;

            }
            catch (Exception)
            {
                uu.ErrMsg = "Error reading Item Details!";
                throw;
            }


        }
        
        public static List<TempListMdl> LoadPacking(int ItemID)
        {
            string ParentName = "";
            int ParentID = 0;
            int CountChield = 0;
            int TotalRecord = 0;
            List<TempListMdl> Packlist = new List<TempListMdl>();

            DataSet st = MainFunction.SDataSet("Select P.Name as PackName,IP.ItemId,IP.Parent,IP.Quantity,IP.PackId " +
                    " from ItemPacking IP,Packing P  where P.Id=IP.PackId and ItemId=" + ItemID + " order by IP.parent", "D1");
            foreach (DataRow RR in st.Tables[0].Rows)
            {
                TempListMdl ls = new TempListMdl();

                TotalRecord = st.Tables[0].Rows.Count - 1;

                if (int.Parse(RR["Quantity"].ToString()) == 0)
                {
                    ParentName = RR["PackName"].ToString();
                    ParentID = int.Parse(RR["PackId"].ToString());
                }
                if (int.Parse(RR["Quantity"].ToString()) > 0)
                {
                    CountChield += 1;
                    DataSet tmt = MainFunction.SDataSet("Select P.Name as PackName,IP.ItemId,IP.Parent,IP.Quantity,IP.PackId " +
                    " from ItemPacking IP,Packing P  where P.Id=IP.PackId and ItemId=" + ItemID + " and ip.packid=" + int.Parse(RR["parent"].ToString())
                    + "order by IP.parent", "D2");
                    foreach (DataRow rs in tmt.Tables[0].Rows)
                    {
                        ls.ID = int.Parse(rs["packid"].ToString());
                        ls.Name = "1 " + rs["PackName"].ToString() + " = " + RR["Quantity"].ToString() + ' ' + RR["PackName"].ToString();
                        Packlist.Add(ls);
                    }
                }
                if (CountChield == 0 && TotalRecord == 0)
                { 
                    ls.ID = ParentID;
                    ls.Name = ParentName;
                    Packlist.Add(ls);

                }

            }
            return Packlist;
        }
        
        public static List<BatchInfo> GetBatchInfo(int ID, int StationID)
        {
            List<BatchInfo> NNN = new List<BatchInfo>();

            DataSet BATCHES = MainFunction.SDataSet("Select  ROW_NUMBER() over (order by a.batchno) as slno, a.BatchNo," +
                   " convert(varchar(20),a.ExpiryDate,105) as ExpiryDate,b.Quantity,a.sellingprice from batch a,batchstore b " +
                   " where  a.ItemId=" + ID + " and a.batchno=b.batchno and a.batchid=b.batchid and b.quantity>0 " +
                   " and b.stationid=" + StationID, "TW");
            foreach (DataRow RR in BATCHES.Tables[0].Rows)
            {
                BatchInfo BT = new BatchInfo();
                BT.slno = MainFunction.NullToInteger(RR["slno"].ToString());
                BT.BatchNo = RR["BatchNo"].ToString();
                BT.Quantity = MainFunction.NullToInteger(RR["Quantity"].ToString());
                BT.ExpiryDate = RR["ExpiryDate"].ToString();
                                 NNN.Add(BT);
            }
            return NNN;

        }
        
        public static Boolean UpdateSubStore(ItemStore sp, int stationid)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();

                    int ConverQty = MainFunction.GetQuantity(sp.UnitID, sp.ItemID);


                    string Sql = " Update itemstore Set MaxLevel=" + sp.MaxLevel + ",MinLevel=" + sp.MinLevel + ",ROL=" +
                    sp.ROL + ",ROQ=" + sp.ROQ + ",ABC=" + sp.ABC + ",FSN=" + sp.FSN + ",unitid = " + sp.UnitID
                    + ", conversionqty = " + ConverQty + " where itemid=" + sp.ItemID + " and stationid=" + stationid;

                    Boolean bt = MainFunction.SSqlExcuite(Sql, Trans);

                    Boolean bt2 = MainFunction.SSqlExcuite("Delete from itemlocation where itemid=" + sp.ItemID + " and stationid=" + stationid, Trans);
                    foreach (ItemLocation rr in sp.ItemLocationS)
                    {
                        string SSql = "Insert into ItemLocation(ItemId,RackId,ShelfID,stationid) Values(" + rr.ItemId + "," + rr.RackId + "," + rr.ShelfId +
                            "," + stationid + ")";
                        Boolean InsrtInotLocation = MainFunction.SSqlExcuite(SSql, Trans);
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

        
        
        public static MMS_ItemMaster ItemMasterResults(int Itemid, int stationid)
        {
            MMS_ItemMaster uu = new MMS_ItemMaster();
            try
            {
                                 List<MMS_ItemMaster> ItemList = MainFunction.CashedAllItems(stationid, " and id=" + Itemid);

                                 var n = ItemList.Where(q => q.Id == Itemid && MainFunction.NullToBool(q.Deleted.ToString()) == false).ToList();
                foreach (var VR in n)
                {
                    uu.Id = VR.Id;
                    uu.ItemCode = VR.ItemCode;
                    uu.Name = VR.Name;
                    uu.CategoryID = VR.CategoryID;
                    uu.PartNumber = VR.PartNumber;
                    uu.catalogueno = VR.catalogueno;
                    uu.ModelNo = VR.ModelNo;
                    uu.EUBbool = VR.EUBbool;
                    uu.ROL = MainFunction.NullToInteger(VR.ROL.ToString());
                    decimal qoh = getItemQOH(VR.Id, stationid) / (Decimal)MainFunction.NullToOne(VR.ConversionQty.ToString());
                                         uu.strQOH = qoh.ToString();
                    uu.ROQ = MainFunction.NullToInteger(VR.ROQ.ToString());
                    uu.Tax = MainFunction.NullToInteger(VR.Tax.ToString());
                    uu.UnitID = MainFunction.NullToInteger(VR.UnitID.ToString());
                    uu.ConversionQty = MainFunction.NullToInteger(VR.ConversionQty.ToString());
                    uu.Strength = VR.Strength;
                    uu.Strength_no = VR.Strength_no;
                    uu.Strength_Unit = VR.Strength_Unit;
                    uu.MaxLevel = MainFunction.NullToInteger2(VR.MaxLevel.ToString());
                    uu.MinLevel = MainFunction.NullToInteger2(VR.MinLevel.ToString());
                    uu.ABC = MainFunction.NullToByte(VR.ABC.ToString());
                    uu.FSN = MainFunction.NullToByte(VR.FSN.ToString());
                    uu.VED = MainFunction.NullToByte(VR.VED.ToString());
                    uu.StartDateTime = DateTime.Parse(VR.StartDateTime.ToString("dd MMM yy"));
                    uu.ProfitCenter = VR.ProfitCenter;
                    uu.ManufacturerId = VR.ManufacturerId;
                    uu.ItemPrefix = VR.ItemPrefix;
                    uu.ProfitCentreID = MainFunction.NullToByte(VR.ProfitCentreID.ToString());

                    uu.DrugType = MainFunction.NullToInteger(VR.DrugType.ToString());

                    uu.Deleted = MainFunction.NullToBool(VR.Deleted.ToString());
                    uu.OpeningBalance = 0;
                    uu.VED = MainFunction.NullToByte(VR.VED.ToString());
                    uu.Schedulebool = VR.Schedulebool;
                    uu.DrugState = MainFunction.NullToByte(VR.DrugState.ToString());

                    uu.iscocktailbool = MainFunction.NullToBool(VR.iscocktailbool.ToString());
                    uu.EUBbool = MainFunction.NullToBool(VR.EUB.ToString());
                    uu.FixedAssetbool = MainFunction.NullToBool(VR.FixedAsset.ToString());
                    uu.NonStockedbool = MainFunction.NullToBool(VR.NonStocked.ToString());
                    uu.BatchStatusbool = MainFunction.NullToBool(VR.BatchStatus.ToString());
                    uu.Narcoticbool = MainFunction.NullToBool(VR.Narcotic.ToString());

                    uu.MRPItembool = MainFunction.NullToBool(VR.MRPItem.ToString());
                    uu.CssdItembool = MainFunction.NullToBool(VR.CssdItem.ToString());
                    uu.CSSDAppbool = MainFunction.NullToBool(VR.CSSDApp.ToString());
                    uu.Consignmentbool = MainFunction.NullToBool(VR.Consignment.ToString());
                    uu.CriticalItembool = MainFunction.NullToBool(VR.CriticalItem.ToString());

                    uu.IndentIssueBool = VR.IndentIssueBool;
                    uu.DepartmentIssueBool = VR.DepartmentIssueBool;

                    uu.Approvalbool = MainFunction.NullToBool(VR.Approval.ToString());
                    uu.DuplicateLabelbool = MainFunction.NullToBool(VR.DuplicateLabel.ToString());
                    uu.Feasibilitybool = MainFunction.NullToBool(VR.Feasibility.ToString());
                    switch (uu.DrugType)
                    {
                        case 0: uu.ItemType = "Drug"; break;
                        case 1: uu.ItemType = "Consumable"; break;
                        case 2: uu.ItemType = "Others"; break;
                        default:
                            break;
                    }
                    switch (uu.IssueType)
                    {
                        case 0: uu.IndentIssueBool = false; uu.DepartmentIssueBool = false; break;
                        case 1: uu.IndentIssueBool = false; uu.DepartmentIssueBool = true; break;
                        case 2: uu.IndentIssueBool = true; uu.DepartmentIssueBool = false; break;
                        case 3: uu.IndentIssueBool = true; uu.DepartmentIssueBool = true; break;
                    }
                    uu.ItemCategory = MainFunction.GetName("select Name from Itemgroup where id=" + VR.CategoryID, "Name");
                    uu.UOMName = MainFunction.GetName("select Name from Packing where id=" + VR.UnitID, "Name");
                    uu.Notes = MainFunction.GetName("select notes as Name from itemnotes where itemid = " + VR.Id, "Name");

                    uu.PackingList = new List<TempListMdl>();
                    DataSet GetPacking = MainFunction.SDataSet("select i.packid,p.name from itempacking i, packing p where i.packid = p.id and i.itemid =" + VR.Id, "Pk");
                    foreach (DataRow rr in GetPacking.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["packid"].ToString());
                        tmp.Name = rr["name"].ToString();
                        uu.PackingList.Add(tmp);
                    }

                    uu.Manufacturerlist = new List<TempListMdl>();
                    DataSet GetManufacturer = MainFunction.SDataSet("select ID,Name from  Manufacturer order by name", "Manufacturer");
                    foreach (DataRow rr in GetManufacturer.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.Manufacturerlist.Add(tmp);
                    }
                    TempListMdl tmpManuf = new TempListMdl();
                    tmpManuf.ID = 0;
                    tmpManuf.Name = "<None>";
                    uu.Manufacturerlist.Add(tmpManuf);


                    uu.ItemCategoryList = new List<TempListMdl>();
                    DataSet GetItemCategoryList = MainFunction.SDataSet("Select ID,Name from ItemGroup where Parent=0 order by name", "ItemCategory");
                    foreach (DataRow rr in GetItemCategoryList.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.ItemCategoryList.Add(tmp);
                    }
                    TempListMdl tmpCat = new TempListMdl();
                    tmpCat.ID = 0;
                    tmpCat.Name = "<None>";
                    uu.ItemCategoryList.Add(tmpCat);


                    uu.ProfitList = new List<TempListMdl>();
                    DataSet GetProfitList = MainFunction.SDataSet("Select ID,Name from department where deleted=0 ORDER BY NAME", "profit");
                    foreach (DataRow rr in GetProfitList.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.ProfitList.Add(tmp);
                    }
                    TempListMdl tmpz = new TempListMdl();
                    tmpz.ID = 0;
                    tmpz.Name = "<None>";
                    uu.ProfitList.Add(tmpz);

                    uu.StationList = new List<TempListMdl>();
                                                                                   DataSet StationList = MainFunction.SDataSet("Select ID,Name from station where deleted=0 and stores=1 and id<>" + stationid +
                                                                " order BY NAME", "Stations");
                    foreach (DataRow rr in StationList.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.StationList.Add(tmp);
                    }


                    uu.SelectedStationList = new List<TempListMdl>();
                    DataSet SelectStationList = MainFunction.SDataSet("Select ID,Name from station where deleted=0 and stores=1 and id<>" + stationid +
                                                                " and id in (select stationid from itemstore where itemid=" + VR.Id + " group by stationid ) " +
                                                                " order BY NAME", "Stations");
                    foreach (DataRow rr in SelectStationList.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.SelectedStationList.Add(tmp);
                    }

                    uu.AllSupplierList = new List<TempListMdl>();
                    DataSet SupplierList = MainFunction.SDataSet("Select ID,Name from supplier " +
                                                                " order BY NAME", "Supplier");
                    foreach (DataRow rr in SupplierList.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.AllSupplierList.Add(tmp);
                    }

                    uu.AllGenericList = new List<TempListMdl>();
                    DataSet GenericList = MainFunction.SDataSet("select id,Name from m_generic order by name ", "Generic");
                    foreach (DataRow rr in GenericList.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.AllGenericList.Add(tmp);
                    }

                    uu.DrugInteractingList = new List<DrugInterAction>();
                    DataSet DrugInter = MainFunction.SDataSet("select a.id as ID,a.Name as Interacting,d.Name as Generic,c.itemid  as ItemID,b.discription " +
                    " from M_generic a,L_drugdruginteraction b,Itemgeneric c,M_generic d " +
                    " where a.id=b.InteractingGenericid and d.id=b.genericID and c.genericid=b.GenericID and c.itemid=" + uu.Id +
                    " order by d.name,a.name ", "drug");
                    foreach (DataRow rr in DrugInter.Tables[0].Rows)
                    {
                        DrugInterAction tmp = new DrugInterAction();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Interacting = rr["Interacting"].ToString();
                        tmp.Generic = rr["Generic"].ToString();
                        tmp.ItemID = MainFunction.NullToInteger(rr["ItemID"].ToString());
                        tmp.Discription = rr["discription"].ToString();
                        uu.DrugInteractingList.Add(tmp);
                    }




                    uu.DiscontinueList = new List<TempListMdl>();
                    for (int i = 0; i < 3; i++)
                    {
                        TempListMdl deletedoptoin = new TempListMdl();
                        switch (i)
                        {
                            case 0:
                                deletedoptoin.ID = i;
                                deletedoptoin.Name = "<None>";
                                break;
                            case 1:
                                deletedoptoin.ID = i;
                                deletedoptoin.Name = "Discontinue";
                                break;
                            case 2:
                                deletedoptoin.ID = i;
                                deletedoptoin.Name = "Discontinue as per Drug Controls inst.";
                                break;
                        }
                        uu.DiscontinueList.Add(deletedoptoin);
                    }

                    if ((bool)uu.Deleted == true)
                    {
                        if (uu.DRUGCONTROL == true)
                        {
                            uu.DiscontinueListID = 2;
                        }
                        else
                        {
                            uu.DiscontinueListID = 1;
                        }
                    }

                    
                    

                    uu.UOMChieldSelected0 = GetPackingChield(0, uu.Id);
                    if (uu.UOMChieldSelected0 != 0)
                    {
                        uu.UOMChieldSelected1 = GetPackingChield(uu.UOMChieldSelected0, uu.Id);
                    }
                    else { uu.UOMChieldSelected1 = 0; }
                    if (uu.UOMChieldSelected1 != 0)
                    {
                        uu.UOMChieldSelected2 = GetPackingChield(uu.UOMChieldSelected1, uu.Id);
                    }
                    else { uu.UOMChieldSelected2 = 0; }
                    if (uu.UOMChieldSelected2 != 0)
                    {
                        uu.UOMChieldSelected3 = GetPackingChield(uu.UOMChieldSelected2, uu.Id);
                    }
                    else { uu.UOMChieldSelected3 = 0; }
                    if (uu.UOMChieldSelected3 != 0)
                    {
                        uu.UOMChieldSelected4 = GetPackingChield(uu.UOMChieldSelected3, uu.Id);
                    }
                    else { uu.UOMChieldSelected4 = 0; }

                    uu.UOMChieldConvQty1 = GetPackingQty(uu.UOMChieldSelected0, uu.Id);
                    uu.UOMChieldConvQty2 = GetPackingQty(uu.UOMChieldSelected1, uu.Id);
                    uu.UOMChieldConvQty3 = GetPackingQty(uu.UOMChieldSelected2, uu.Id);
                    uu.UOMChieldConvQty4 = GetPackingQty(uu.UOMChieldSelected3, uu.Id);




                    uu.UOMChieldList0 = new List<TempListMdl>();
                    DataSet UOMLIST = MainFunction.SDataSet("Select ID,Name from Packing where deleted=0 order by Name", "MainUOM");
                    foreach (DataRow rr in UOMLIST.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.UOMChieldList0.Add(tmp);
                    }

                    uu.UOMChieldList1 = new List<TempListMdl>();
                    TempListMdl NONField1 = new TempListMdl();
                    NONField1.ID = 0;
                    NONField1.Name = "<NONE>";
                    uu.UOMChieldList1.Add(NONField1);
                    DataSet UOMChieldList1 = MainFunction.SDataSet("Select a.name,a.id from packingdetail b,packing a where a.id=b.packid and b.parent=" + uu.UOMChieldSelected0 + " order by Name ", "ChieldList");
                    foreach (DataRow rr in UOMChieldList1.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.UOMChieldList1.Add(tmp);
                    }



                    uu.UOMChieldList2 = new List<TempListMdl>();
                    TempListMdl NONField2 = new TempListMdl();
                    NONField2.ID = 0;
                    NONField2.Name = "<NONE>";
                    uu.UOMChieldList2.Add(NONField2);
                    DataSet UOMChieldList2 = MainFunction.SDataSet("Select a.name,a.id from packingdetail b,packing a where a.id=b.packid and b.parent=" + uu.UOMChieldSelected1 + " order by Name ", "ChieldList");
                    foreach (DataRow rr in UOMChieldList2.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.UOMChieldList2.Add(tmp);
                    }



                    uu.UOMChieldList3 = new List<TempListMdl>();
                    TempListMdl NONField3 = new TempListMdl();
                    NONField3.ID = 0;
                    NONField3.Name = "<NONE>";
                    uu.UOMChieldList3.Add(NONField3);
                    DataSet UOMChieldList3 = MainFunction.SDataSet("Select a.name,a.id from packingdetail b,packing a where a.id=b.packid and b.parent=" + uu.UOMChieldSelected2 + " order by Name ", "ChieldList");
                    foreach (DataRow rr in UOMChieldList3.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.UOMChieldList3.Add(tmp);
                    }


                    uu.UOMChieldList4 = new List<TempListMdl>();
                    TempListMdl NONField4 = new TempListMdl();
                    NONField4.ID = 0;
                    NONField4.Name = "<NONE>";
                    uu.UOMChieldList4.Add(NONField4);
                    DataSet UOMChieldList4 = MainFunction.SDataSet("Select a.name,a.id from packingdetail b,packing a where a.id=b.packid and b.parent=" + uu.UOMChieldSelected3 + " order by Name ", "ChieldList");
                    foreach (DataRow rr in UOMChieldList4.Tables[0].Rows)
                    {
                        TempListMdl tmp = new TempListMdl();
                        tmp.ID = MainFunction.NullToInteger(rr["ID"].ToString());
                        tmp.Name = rr["Name"].ToString();
                        uu.UOMChieldList4.Add(tmp);
                    }
                }

                return uu;

            }
            catch (Exception)
            {
                uu.ErrMsg = "Error reading Item Details!";
                throw;
            }


        }
        public static int GetPackingQty(int PackageID, int ItemID)
        {
            try
            {
                int qty = 0;
                DataSet Get = MainFunction.SDataSet("select quantity from itempacking where parent=" + PackageID + " and itemid=" + ItemID, "th");
                foreach (DataRow rr in Get.Tables[0].Rows)
                {
                    qty = MainFunction.NullToInteger(rr["quantity"].ToString());
                }
                return qty;
            }
            catch (Exception e) { return 0; }
        }
        public static int GetPackingChield(int ParentID, int ItemID)
        {
            try
            {
                int qty = 0;
                DataSet Get = MainFunction.SDataSet("select PackID from itempacking where parent=" + ParentID + " and itemid=" + ItemID, "th");

                foreach (DataRow rr in Get.Tables[0].Rows)
                {
                    qty = MainFunction.NullToInteger(rr["PackID"].ToString());
                }
                if (Get.Tables[0].Rows.Count > 0)
                {
                    return qty;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception e) { return 0; }
        }
        public static string SaveUOM(string UOMNAME, String[] st, int UOMID = 0)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                String MSG = "";
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    int MaxID = 0;
                    DataSet dsvalidate = MainFunction.SDataSet("select Id from packing where name='" + UOMNAME.Trim() + "'", "validate");
                    if (dsvalidate.Tables[0].Rows.Count == 0 && UOMNAME.Length > 0 && UOMID == 0)
                    {
                        if (UOMID == 0)                          {
                            DataSet AddDs = MainFunction.SDataSet("select max(id) as maxid from  packing", "Add");
                            if (AddDs.Tables[0].Rows.Count == 0) { MaxID = 1; }
                            foreach (DataRow RR in AddDs.Tables[0].Rows)
                            {
                                MaxID = MainFunction.NullToInteger(RR["maxid"].ToString());
                                MaxID += 1;
                            }

                            bool AddinPacking = MainFunction.SSqlExcuite("insert into packing(id,name) values(" + MaxID + ",'" + UOMNAME.Trim() + "')", Trans);
                            bool AddinUnit = MainFunction.SSqlExcuite("insert into unit(id,name) values(" + MaxID + ",'" + UOMNAME.Trim() + "')", Trans);


                            for (int i = 0; i < st.Count(); i++)
                            {
                                bool AddinPackDetail = MainFunction.SSqlExcuite("insert into packingdetail(packid,parent) values(" + MaxID + "," + st[i].ToString() + ")", Trans);
                            }

                        }

                    }
                    else if (dsvalidate.Tables[0].Rows.Count > 0 && UOMNAME.Length > 0 && UOMID > 0)                      {
                        MaxID = UOMID;
                        bool delPackingdtl = MainFunction.SSqlExcuite("delete from packingdetail where packid=" + MaxID, Trans);
                        for (int i = 0; i < st.Count(); i++)
                        {
                            bool AddinPackDetail = MainFunction.SSqlExcuite("insert into packingdetail(packid,parent) values(" + MaxID + ",'" + st[i].ToString() + "')", Trans);
                        }

                    }
                    else
                    {
                        if (UOMNAME.Length == 0) { MSG = "you must entr valid text"; }
                        else { MSG = "Item Exist!"; }
                        Con.BeginTransaction().Rollback();
                        Con.Close();
                        return MSG;
                    }

                    if (UOMID == 0) { MSG = "New UOM Added"; }
                    else { MSG = "UOM Updated"; }
                    Trans.Commit();
                    return MSG;


                }



            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                return "Error! can not save.";

            }









        }
        public static bool SavePacking(MMS_ItemMaster tbl, int gSavedById, int gStationID)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                String StrSql = "";
                long Lqty = 0;

                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    try
                    {

                        
                        DataSet CheckBatch = MainFunction.SDataSet("select count(*) as c1 from batch where itemid=" + tbl.Id, "tbl");
                        foreach (DataRow rr in CheckBatch.Tables[0].Rows)
                        {
                            if ((int)rr["c1"] > 0)
                            {
                                tbl.ErrMsg = "this item has batchs can't modify or edit";
                                return false;
                            }
                            else
                            {
                                StrSql = "Insert into CancelItemPacking (itemid,packid,Quantity,Parent,Slno,Conversionqty,CanOperatorid,CanDatetime) "
                                + " select itemid,packid,quantity,parent,slno,conversionqty," + gSavedById + ",getdate() from itempacking where itemid = "
                                + tbl.Id;
                                bool insertINtoCancel = MainFunction.SSqlExcuite(StrSql, Trans);
                            }


                        }


                        bool DeletPacking = MainFunction.SSqlExcuite("Delete from ItemPacking where ItemId=" + tbl.Id, Trans);
                        StrSql = "Insert into itempacking(itemid,packid,parent,quantity,slno) values(" +
                                +tbl.Id + "," + tbl.UOMChieldSelected0 + ",0,0,1)";
                        bool insertFirst = MainFunction.SSqlExcuite(StrSql, Trans);

                        if (tbl.UOMChieldSelected1 > 0)
                        {
                            StrSql = "Insert into itempacking(itemid,packid,parent,quantity,slno) values(" +
                                    +tbl.Id + "," + tbl.UOMChieldSelected1 + "," + tbl.UOMChieldSelected0 + "," + tbl.UOMChieldConvQty1 + ",2)";
                            bool insertSecond = MainFunction.SSqlExcuite(StrSql, Trans);
                        }

                        if (tbl.UOMChieldSelected2 > 0)
                        {
                            StrSql = "Insert into itempacking(itemid,packid,parent,quantity,slno) values(" +
                                    +tbl.Id + "," + tbl.UOMChieldSelected2 + "," + tbl.UOMChieldSelected1 + "," + tbl.UOMChieldConvQty2 + ",3)";
                            bool insertThird = MainFunction.SSqlExcuite(StrSql, Trans);
                        }

                        if (tbl.UOMChieldSelected3 > 0)
                        {
                            StrSql = "Insert into itempacking(itemid,packid,parent,quantity,slno) values(" +
                                    +tbl.Id + "," + tbl.UOMChieldSelected3 + "," + tbl.UOMChieldSelected2 + "," + tbl.UOMChieldConvQty3 + ",4)";
                            bool insertForth = MainFunction.SSqlExcuite(StrSql, Trans);
                        }
                        if (tbl.UOMChieldSelected4 > 0)
                        {
                            StrSql = "Insert into itempacking(itemid,packid,parent,quantity,slno) values(" +
                                    +tbl.Id + "," + tbl.UOMChieldSelected4 + "," + tbl.UOMChieldSelected3 + "," + tbl.UOMChieldConvQty4 + ",5)";
                            bool insertSecond = MainFunction.SSqlExcuite(StrSql, Trans);
                        }


                        Trans.Commit();

                        DataSet GetUomQTY = MainFunction.SDataSet("select packid from itempacking where itemid = " + tbl.Id + " order by slno", "tbl");
                        foreach (DataRow rr in GetUomQTY.Tables[0].Rows)
                        {
                            Lqty = MainFunction.GetQuantity((int)rr["packid"], tbl.Id);
                            bool updatepacking = MainFunction.SSqlExcuite("update itempacking set conversionqty = " + Lqty + " where itemid = " + tbl.Id
                                + " and packid = " + (int)rr["packid"]);
                        }

                        tbl.ErrMsg = "Packing Saved";
                        return true;

                    }
                    catch (Exception e) { Trans.Rollback(); tbl.ErrMsg = "Can't Save:" + e.Message; return false; }
                }
            }
            catch (Exception e)
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                tbl.ErrMsg = "Can't Save:" + e.Message;
                return false;
            }
        }





        public static Boolean UPdateItemMaster(MMS_ItemMaster Tbl, int gSavedById, int gStationID)
        {
            SqlConnection Con = MainFunction.MainConn();
            try
            {
                using (Con)
                {
                    Con.Open();
                    SqlTransaction Trans = Con.BeginTransaction();
                    try
                    {

                        string StrSql = "";
                        if (ValidateItemMaster(Tbl) == true)
                        {
                            int lLowestUnitID = 0;
                            int Deleted = 0;
                            int ControlDeteld = 0;
                            if (Tbl.DiscontinueListID == 2)
                            {
                                Deleted = 1;
                                ControlDeteld = 1;
                            }
                            else if (Tbl.DiscontinueListID == 1)
                            {
                                Deleted = 1;
                                ControlDeteld = 0;
                            }


                            
                            DataSet LowUOMds = MainFunction.SDataSet("Select PackId from ItemPacking where itemid = " + Tbl.Id + " and  slno in (Select Max(slno) from ItemPacking where itemid = " + Tbl.Id + ")", "tbl");
                            foreach (DataRow RR in LowUOMds.Tables[0].Rows)
                            {
                                lLowestUnitID = MainFunction.NullToInteger(RR["PackId"].ToString());
                            }


                            if (Tbl.DepartmentIssueBool == false && Tbl.IndentIssueBool == false)
                            {
                                Tbl.IssueType = 0;
                            }
                            if (Tbl.DepartmentIssueBool == true && Tbl.IndentIssueBool == false)
                            {
                                Tbl.IssueType = 1;
                            }
                            if (Tbl.DepartmentIssueBool == false && Tbl.IndentIssueBool == true)
                            {
                                Tbl.IssueType = 2;
                            }
                            if (Tbl.DepartmentIssueBool == true && Tbl.IndentIssueBool == true)
                            {
                                Tbl.IssueType = 3;
                            }
                            bool InsertInOldItem = MainFunction.SSqlExcuite("Insert into OldItem select * from Item where id = " + Tbl.Id);
                            if (Tbl.ManufacturerId == 0)
                            {
                                StrSql = "Update Item set Name='" + Tbl.Name.Trim().ToUpper()
                               + "',Name1='" + Tbl.Name.Trim().ToUpper() + "'"
                               + ",CategoryId=" + Tbl.CategoryID
                               + ",ItemCode='" + Tbl.ItemCode.Trim().ToUpper()
                               + "',deleted=" + Deleted
                               + ",EUB=" + MainFunction.BoolToInteger(Tbl.EUBbool)
                                                                    + ",UnitId=" + Tbl.UnitID
                               + ",Tax=" + MainFunction.NullToInteger(Tbl.Tax.ToString())
                               + ",Schedule=" + MainFunction.BoolToInteger(Tbl.Schedulebool)
                               + ",MRPItem=" + MainFunction.BoolToInteger(Tbl.MRPItembool)
                               + ",drugtype=" + Tbl.DrugType
                               + ",cssditem=" + MainFunction.BoolToInteger(Tbl.CssdItembool)
                               + ", batchstatus=" + MainFunction.BoolToInteger(Tbl.BatchStatusbool)
                               + ",strength_no=" + MainFunction.NullToDecmial(Tbl.Strength_no.ToString())
                               + ", Narcotic=" + MainFunction.BoolToInteger(Tbl.Narcoticbool)
                               + ",nonstocked=" + MainFunction.BoolToInteger(Tbl.NonStockedbool)
                               + ",itemPrefix='" + MainFunction.NullToString(Tbl.ItemPrefix, true)
                               + "',drugcontrol=" + ControlDeteld
                               + ",consignment= " + MainFunction.BoolToInteger(Tbl.Consignmentbool).ToString()
                               + ", approval=" + MainFunction.BoolToInteger(Tbl.Approvalbool).ToString()
                               + ",profitCentreId=" + Tbl.ProfitCentreID
                               + ",FixedAsset =" + MainFunction.BoolToInteger(Tbl.FixedAssetbool)
                               + ",duplicatelabel = " + MainFunction.BoolToInteger(Tbl.DuplicateLabelbool)
                               + ", issuetype = " + Tbl.IssueType
                               + ", drugstate = " + Tbl.DrugState
                               + ",Strength_unit = '" + MainFunction.NullToString(Tbl.Strength_Unit) + "'"
                               + ",PartNumber = '" + MainFunction.NullToString(Tbl.PartNumber) + "'"
                               + ",ModifiedDatetime = sysdatetime(), ModifiedBy = " + gSavedById
                               + " ,CSSDApp =" + MainFunction.BoolToInteger(Tbl.CSSDAppbool)
                               + ",ModelNo ='" + MainFunction.NullToString(Tbl.ModelNo)
                               + "',Feasibility=" + MainFunction.BoolToInteger(Tbl.Feasibilitybool)
                               + ",CriticalItem=" + MainFunction.BoolToInteger(Tbl.CriticalItembool)
                               + ",ISCOCKTAIL=" + MainFunction.BoolToInteger(Tbl.iscocktailbool)
                               + ",CATALOGUENO='" + MainFunction.NullToString(Tbl.catalogueno) + "'  "
                               + "  where ID=" + Tbl.Id;

                            }
                            else
                            {
                                StrSql = "Update Item set Name='" + Tbl.Name.Trim().ToUpper()
                                + "',Name1='" + Tbl.Name.Trim().ToUpper() + "'"
                                + ",ManufacturerId=" + Tbl.ManufacturerId
                                + ",CategoryId=" + Tbl.CategoryID
                                + ",ItemCode='" + Tbl.ItemCode.Trim().ToUpper()
                                + "',deleted=" + Deleted
                                + ",EUB=" + MainFunction.BoolToInteger(Tbl.EUBbool)
                                                                     + ",UnitId=" + Tbl.UnitID
                                + ",Tax=" + MainFunction.NullToInteger(Tbl.Tax.ToString())
                                + ",Schedule=" + MainFunction.BoolToInteger(Tbl.Schedulebool)
                                + ",MRPItem=" + MainFunction.BoolToInteger(Tbl.MRPItembool)
                                + ",drugtype=" + Tbl.DrugType
                                + ",cssditem=" + MainFunction.BoolToInteger(Tbl.CssdItembool)
                                + ", batchstatus=" + MainFunction.BoolToInteger(Tbl.BatchStatusbool)
                                + ",strength_no=" + MainFunction.NullToDecmial(Tbl.Strength_no.ToString())
                                + ", Narcotic=" + MainFunction.BoolToInteger(Tbl.Narcoticbool)
                                + ",nonstocked=" + MainFunction.BoolToInteger(Tbl.NonStockedbool)
                                + ",itemPrefix='" + MainFunction.NullToString(Tbl.ItemPrefix, true)
                                + "',drugcontrol=" + ControlDeteld
                                + ",consignment= " + MainFunction.BoolToInteger(Tbl.Consignmentbool).ToString()
                                + ", approval=" + MainFunction.BoolToInteger(Tbl.Approvalbool).ToString()
                                + ",profitCentreId=" + Tbl.ProfitCentreID
                                + ",FixedAsset =" + MainFunction.BoolToInteger(Tbl.FixedAssetbool)
                                + ",duplicatelabel = " + MainFunction.BoolToInteger(Tbl.DuplicateLabelbool)
                                + ", issuetype = " + Tbl.IssueType
                                + ", drugstate = " + Tbl.DrugState
                                + ",Strength_unit = '" + MainFunction.NullToString(Tbl.Strength_Unit) + "'"
                                + ",PartNumber = '" + MainFunction.NullToString(Tbl.PartNumber) + "'"
                                + ",ModifiedDatetime = sysdatetime(), ModifiedBy = " + gSavedById
                                + " ,CSSDApp =" + MainFunction.BoolToInteger(Tbl.CSSDAppbool)
                                + ",ModelNo ='" + MainFunction.NullToString(Tbl.ModelNo)
                                + "',Feasibility=" + MainFunction.BoolToInteger(Tbl.Feasibilitybool)
                                + ",CriticalItem=" + MainFunction.BoolToInteger(Tbl.CriticalItembool)
                                + ",ISCOCKTAIL=" + MainFunction.BoolToInteger(Tbl.iscocktailbool)
                                + ",CATALOGUENO='" + MainFunction.NullToString(Tbl.catalogueno) + "'  "

                                + "  where ID=" + Tbl.Id;

                            }

                            bool Update = MainFunction.SSqlExcuite(StrSql, Trans);

                            
                            StrSql = "Update itemstore Set MaxLevel=" + MainFunction.NullToInteger(Tbl.MaxLevel.ToString())
                                   + ",MinLevel=" + MainFunction.NullToInteger(Tbl.MinLevel.ToString()) + ",ROL=" + MainFunction.NullToInteger(Tbl.ROL.ToString())
                                   + ",ROQ=" + MainFunction.NullToInteger(Tbl.ROQ.ToString()) + ",UnitId=" + Tbl.UnitID.ToString()
                                   + ",ABC=" + Tbl.ABC + ",FSN=" + Tbl.FSN + ",VED=" + Tbl.VED + " where ItemId=" + Tbl.Id
                                   + "   and stationid=" + gStationID;
                            bool SaveItemStore = MainFunction.SSqlExcuite(StrSql, Trans);

                            
                            bool DeleteNotes = MainFunction.SSqlExcuite("delete from itemnotes where itemid = " + Tbl.Id, Trans);
                            StrSql = "insert into ItemNotes (itemid,notes) values (" + Tbl.Id + ",'" + Tbl.Notes.Trim() + "')";
                            bool SaveNotes = MainFunction.SSqlExcuite(StrSql, Trans);

                            
                            foreach (var rr in Tbl.SelectedStationList)
                            {
                                DataSet ValidateExitingStore = MainFunction.SDataSet("Select itemid from itemstore where stationid=" + rr.ID + " and itemid=" + Tbl.Id + " ", "table");
                                if (ValidateExitingStore.Tables[0].Rows.Count == 0)
                                {
                                    StrSql = " Insert into itemstore(ItemId,stationid,maxlevel,minlevel,rol,roq,abc,fsn,StartDateTime,Deleted,unitid) Values("
                                    + Tbl.Id + "," + rr.ID + "," + MainFunction.NullToInteger(Tbl.MaxLevel.ToString()) + "," + MainFunction.NullToInteger(Tbl.MinLevel.ToString()) + ","
                                    + MainFunction.NullToInteger(Tbl.ROL.ToString()) + "," + MainFunction.NullToInteger(Tbl.ROQ.ToString()) + "," + Tbl.ABC
                                    + "," + Tbl.FSN + ",sysdatetime(),0," + Tbl.UnitID + ")";

                                    bool insertAreaStore = MainFunction.SSqlExcuite(StrSql, Trans);
                                }

                            }

                            
                            bool DeleteSupplier = MainFunction.SSqlExcuite("Delete from SupplierItem where ItemId=" + Tbl.Id, Trans);
                            if (Tbl.AllSupplierList != null)
                            {
                                if (Tbl.AllSupplierList.Count > 0)
                                {
                                    foreach (var rr in Tbl.AllSupplierList)
                                    {
                                        bool InsertSupplier = MainFunction.SSqlExcuite("Insert into SupplierItem(SupplierId,ItemID) values(" + rr.ID + "," + Tbl.Id + ")", Trans);
                                    }
                                }
                            }
                            
                            bool DeleteManufacturer = MainFunction.SSqlExcuite("Delete from ManufacturerItem where manufacturerid =" + Tbl.ManufacturerId, Trans);
                            bool InsertManufacturer = MainFunction.SSqlExcuite("Insert into ManufacturerITem (Itemid,ManufacturerId,PartNo) Values (" + Tbl.Id
                                + "," + MainFunction.NullToInteger(Tbl.ManufacturerId.ToString()) + ",'" + Tbl.PartNumber + "')", Trans);

                            
                            bool DeleteGeneric = MainFunction.SSqlExcuite("Delete from Itemgeneric where ItemId=" + Tbl.Id, Trans);
                            if (Tbl.AllGenericList != null)
                            {
                                if (Tbl.AllGenericList.Count > 0)
                                {
                                    foreach (var rr in Tbl.AllGenericList)
                                    {
                                        bool InsertGeneric = MainFunction.SSqlExcuite("Insert into Itemgeneric(genericid,itemid) values(" + rr.ID + "," + Tbl.Id + ")", Trans);
                                    }
                                }
                            }
                            
                            bool DeleteItemLoacation = MainFunction.SSqlExcuite("Delete from ItemLocation where ItemId=" + Tbl.Id, Trans);
                            foreach (var rr in Tbl.ItemLocationS)
                            {
                                StrSql = "Insert into ItemLocation(ItemId,RackId,ShelfID,stationid) Values("
                                    + Tbl.Id + "," + rr.RackId + "," + rr.ShelfId + "," + gStationID + ")";
                                bool Savelocation = MainFunction.SSqlExcuite(StrSql, Trans);
                            }

                            Trans.Commit();
                            return true;
                        }
                        else
                        {
                            Trans.Rollback();
                            return false;
                        }
                    }
                    catch (Exception e) { Trans.Rollback(); Tbl.ErrMsg = "Error>>" + e.Message; return false; }
                }
            }
            catch (Exception e)
            {

                if (Con.State == ConnectionState.Open)
                {
                    Con.BeginTransaction().Rollback();
                    Con.Close();
                }
                Tbl.ErrMsg = "Error>>" + e.Message;

                return false;
            }
        }
        public static bool ValidateItemMaster(MMS_ItemMaster it)
        {
            DataSet ItemGroupDs = MainFunction.SDataSet("Select id from ItemGroup where id = " + it.CategoryID
                + " and Fixed =1 and medical = 1 and equipment = 1", "tbl");
            foreach (DataRow RR in ItemGroupDs.Tables[0].Rows)
            {
                if (it.ManufacturerId <= 0) { it.ErrMsg = "Select Manufacturer"; return false; }

            }

            DataSet ItemExsitDs = MainFunction.SDataSet("select count(*) as itemcount from item " +
                " where deleted=0 and itemcode = '" + it.ItemCode.Trim() + "' and id <> " + it.Id, "tbl");
            foreach (DataRow RR in ItemExsitDs.Tables[0].Rows)
            {
                if (MainFunction.NullToInteger(RR["itemcount"].ToString()) > 0)
                {
                    it.ErrMsg = "Item with the entered code already exists"; return false;
                }
            }

            DataSet CheckBatchds = MainFunction.SDataSet("Select i.Categoryid,bt.Batchid from Batch bt,Item i " +
                " where i.id = bt.itemid and i.id = " + it.Id, "tbl");
            foreach (DataRow rr in CheckBatchds.Tables[0].Rows)
            {
                if (MainFunction.NullToInteger(rr["Categoryid"].ToString()) != it.CategoryID)
                {
                    it.ErrMsg = "Item Category Cannot be Changed";
                    return false;
                }

            }

            if (MainFunction.NullToInteger(it.Strength_no.ToString()) > 0 && it.CategoryID == 17)
            {
                it.ErrMsg = "Please enter the Strength!";
                return false;
            }

            if (it.ItemCode.ToString().Length == 0) { it.ErrMsg = "Please Enter Valid Itemcode"; return false; }
            if (it.Name.ToString().Length == 0) { it.ErrMsg = "Please Enter Valid Item Name"; return false; }
            if (it.UnitID == 0) { it.ErrMsg = "Please select the Item Unit!"; return false; }
            if (it.UOMChieldSelected0 == 0) { it.ErrMsg = "Select the packing units"; return false; }
            if (it.UOMChieldSelected1 == 0 && it.UOMChieldConvQty1 > 0)
            {
                it.ErrMsg = "Please Enter the Conversion Qty and the Unit";
                return false;
            }
            if (it.UOMChieldSelected1 > 0 && it.UOMChieldConvQty1 == 0)
            {
                it.ErrMsg = "Please Enter the Conversion Qty and the Unit";
                return false;
            }
            if (it.UOMChieldSelected2 == 0 && it.UOMChieldConvQty2 > 0)
            {
                it.ErrMsg = "Please Enter the Conversion Qty and the Unit";
                return false;
            }
            if (it.UOMChieldSelected2 > 0 && it.UOMChieldConvQty2 == 0)
            {
                it.ErrMsg = "Please Enter the Conversion Qty and the Unit";
                return false;
            }
            if (it.UOMChieldSelected3 == 0 && it.UOMChieldConvQty3 > 0)
            {
                it.ErrMsg = "Please Enter the Conversion Qty and the Unit";
                return false;
            }
            if (it.UOMChieldSelected3 > 0 && it.UOMChieldConvQty3 == 0)
            {
                it.ErrMsg = "Please Enter the Conversion Qty and the Unit";
                return false;
            }

            if (it.NonStockedbool == false)
            {
                if (it.MaxLevel == 0 || it.MinLevel == 0) { it.ErrMsg = "Item is an Inventory Item. Enter the Min-Max Level for the item."; return false; }
                if (it.MaxLevel < it.ROL) { it.ErrMsg = "Enter the Max Level for the item  MaxLevel > ROL "; return false; }
                if (it.ROL == 0) { it.ErrMsg = "Enter the ROL for the item."; return false; }
            }

            DataSet CheckNameds = MainFunction.SDataSet("Select Id from Item where upper(Name)='" + it.Name.ToUpper().Trim() + " " + it.Strength_no.ToString().Trim() + "' "
                + " and deleted=0 and  id<>" + it.Id, "tbl");
            foreach (DataRow rr in CheckNameds.Tables[0].Rows)
            {
                it.ErrMsg = "An item with this name already exists in the inventory,enter different name"; return false;
            }

            if (it.DepartmentIssueBool == false && it.IndentIssueBool == false)
            {
                it.ErrMsg = "Select Issue type.";
                return false;
            }

            return true;


        }

    }
}