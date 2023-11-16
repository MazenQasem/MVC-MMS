        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class Item
    {
        public int ID { get; set; }
        public string ItemCode { get; set; }
        public string Name { get; set; }
        public string Name1 { get; set; }
        public string ArabicCode { get; set; }
        public string ArabicName { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
        public string Strength { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public int ManufacturerID { get; set; }
        public Nullable<bool> EUB { get; set; }
        public string ProfitCenter { get; set; }
        public Nullable<int> UnitID { get; set; }
        public Nullable<float> Tax { get; set; }
        public Nullable<bool> Schedule { get; set; }
        public Nullable<bool> MRPItem { get; set; }
        public int ConversionQty { get; set; }
        public int DrugType { get; set; }
        public Nullable<int> TempID { get; set; }
        public Nullable<bool> CSSDItem { get; set; }
        public Nullable<bool> BatchStatus { get; set; }
        public Nullable<decimal> Strength_No { get; set; }
        public Nullable<byte> CapitalRevenue { get; set; }
        public Nullable<bool> Narcotic { get; set; }
        public Nullable<bool> NonStocked { get; set; }
        public string ItemPrefix { get; set; }
        public Nullable<bool> DrugControl { get; set; }
        public Nullable<bool> Consignment { get; set; }
        public Nullable<bool> Approval { get; set; }
        public Nullable<short> ProfitCentreID { get; set; }
        public Nullable<bool> FixedAsset { get; set; }
        public Nullable<byte> IssueType { get; set; }
        public Nullable<byte> DrugState { get; set; }
        public string Strength_Unit { get; set; }
        public Nullable<bool> CriticalItem { get; set; }
        public Nullable<bool> DuplicateLabel { get; set; }
        public string PartNumber { get; set; }
        public Nullable<int> CSSDApp { get; set; }
        public string ModelNo { get; set; }
        public Nullable<int> OperatorId { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDatetime { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<int> DeletedBy { get; set; }
        public Nullable<bool> Feasibility { get; set; }
        public Nullable<int> Uploaded { get; set; }
        public string OrcItemCode { get; set; }
        public string CatalogueNo { get; set; }
        public Nullable<int> ItemForm { get; set; }
        public string Description { get; set; }
        public Nullable<byte> IsCocktail { get; set; }
        public string Ora_code { get; set; }
        public Nullable<bool> IsUnified { get; set; }
        public Nullable<System.DateTime> Date_Unified { get; set; }
        public string Prev_Ora_Code { get; set; }
    }
}
