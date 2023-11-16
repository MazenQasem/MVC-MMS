using System;
using System.Collections.Generic;

namespace MMS2
{
    public partial class Item
    {
        
        public int StationID { get; set; }
                 public Nullable<int> StoreUnitID { get; set; }
        public Nullable<Decimal> MaxLevel { get; set; }
        public Nullable<Decimal> MinLevel { get; set; }
        public Nullable<Decimal> QOH { get; set; }
        public Nullable<Decimal> ROL { get; set; }
        public Nullable<Decimal> ROQ { get; set; }
        public Nullable<byte> ABC { get; set; }
        public Nullable<byte> FSN { get; set; }
        public Nullable<byte> ved { get; set; }
        public Nullable<int> StoreTax { get; set; }
        public Nullable<int> StoreConversionQty { get; set; }
        public String location { get; set; }
        public List<BatchInfo> BatchInfo { get; set; }
        
       

    }
    public class ItemLookUp
    {
        public int ID { get; set; }
        public String ItemName { get; set; }
        public String GroupName { get; set; }
        public String ItemCode { get; set; }
        public String ManuName { get; set; }
        public String SupName { get; set; }
        public Decimal SellPrice { get; set; }

    }
    public class BatchInfo
    {
        public int slno { get; set; }         public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public String ExpiryDate { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
    }
    public partial class MMS_ItemMaster 
    {
        public string ItemType { get; set; }
        public string ItemCategory { get; set; }
        public string UOMName { get; set; }
        public string ErrMsg { get; set; }
        public List<TempListMdl> PackingList { get; set; }
        public int PackingListHolder { get; set; }
       
        public decimal OpeningBalance { get; set; }
        public List<TempListMdl> Manufacturerlist { get; set; }
        public List<TempListMdl> ItemCategoryList { get; set; }
        public List<TempListMdl> ProfitList { get; set; }
        
        public bool iscocktailbool { get; set; }
        public bool EUBbool { get; set; }
        public bool FixedAssetbool { get; set; }
        public bool NonStockedbool { get; set; }
        public bool BatchStatusbool { get; set; }
        public bool Narcoticbool { get; set; }

        public bool MRPItembool { get; set; }
        public bool CssdItembool { get; set; }
        public bool CSSDAppbool { get; set; }
        public bool Consignmentbool { get; set; }
        public bool CriticalItembool { get; set; }

        public bool Approvalbool { get; set; }
        public bool DepartmentIssueBool { get; set; }
        public bool IndentIssueBool { get; set; }
        public bool DuplicateLabelbool { get; set; }
        public bool Feasibilitybool { get; set; }
        public bool Schedulebool { get; set; }

        public List<TempListMdl> DiscontinueList { get; set; }
        public int DiscontinueListID { get; set; }
        public List<TempListMdl> StationList { get; set; }
        public List<TempListMdl> SelectedStationList { get; set; }



        public List<TempListMdl> UOMChieldList0 { get; set; }
        public int UOMChieldSelected0 { get; set; }
        public int UOMChieldConvQty0 { get; set; }
        
        public List<TempListMdl> UOMChieldList1 { get; set; }
        public int UOMChieldSelected1 { get; set; }
        public int UOMChieldConvQty1 { get; set; }

        public List<TempListMdl> UOMChieldList2 { get; set; }
        public int UOMChieldSelected2 { get; set; }
        public int UOMChieldConvQty2 { get; set; }

        public List<TempListMdl> UOMChieldList3 { get; set; }
        public int UOMChieldSelected3 { get; set; }
        public int UOMChieldConvQty3 { get; set; }

        public List<TempListMdl> UOMChieldList4 { get; set; }
        public int UOMChieldSelected4 { get; set; }
        public int UOMChieldConvQty4 { get; set; }

        public List<TempListMdl> AllSupplierList { get; set; }
        public List<TempListMdl> AllGenericList { get; set; }
        public List<ItemLocation> ItemLocationS { get; set; }

        public List<DrugInterAction> DrugInteractingList { get; set; }
        
        public string strQOH { get; set; }
        public String Notes { get; set; }
         
        


    }

    public partial class ItemStore
    {
        public List<ItemLocation> ItemLocationS { get; set; }
        public string ErrMsg { get; set; }
    
    }


    public partial class DrugInterAction
    {
        public int ID { get; set; }
        public String Interacting { get; set; }
        public String Generic { get; set; }
        public int ItemID { get; set; }
        public string Discription { get; set; }
   
    }
   
    
}