        
namespace MMS2
{
    using System;
    using System.Collections.Generic;
    
    public partial class MMS_ItemMaster
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Name1 { get; set; }
        public string Strength { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public int ManufacturerId { get; set; }
        public Nullable<int> MaxLevel { get; set; }
        public Nullable<int> MinLevel { get; set; }
        public Nullable<int> ROL { get; set; }
        public Nullable<int> ROQ { get; set; }
        public Nullable<int> QOH { get; set; }
        public Nullable<byte> ABC { get; set; }
        public Nullable<byte> FSN { get; set; }
        public Nullable<byte> VED { get; set; }
        public Nullable<decimal> SellingPrice { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> Enddatetime { get; set; }
        public string ItemCode { get; set; }
        public Nullable<bool> Deleted { get; set; }
        public Nullable<bool> EUB { get; set; }
        public string ProfitCenter { get; set; }
        public Nullable<int> UnitID { get; set; }
        public Nullable<int> Tax { get; set; }
        public Nullable<bool> Schedule { get; set; }
        public Nullable<bool> MRPItem { get; set; }
        public Nullable<int> ConversionQty { get; set; }
        public int DrugType { get; set; }
        public Nullable<int> DeletedBY { get; set; }
        public Nullable<bool> CssdItem { get; set; }
        public Nullable<bool> BatchStatus { get; set; }
        public Nullable<decimal> Strength_no { get; set; }
        public Nullable<bool> Narcotic { get; set; }
        public Nullable<bool> NonStocked { get; set; }
        public string ItemPrefix { get; set; }
        public Nullable<bool> Consignment { get; set; }
        public Nullable<bool> Approval { get; set; }
        public Nullable<short> ProfitCentreID { get; set; }
        public Nullable<bool> FixedAsset { get; set; }
        public Nullable<byte> IssueType { get; set; }
        public Nullable<byte> DrugState { get; set; }
        public string Strength_Unit { get; set; }
        public Nullable<bool> DuplicateLabel { get; set; }
        public string PartNumber { get; set; }
        public Nullable<int> CSSDApp { get; set; }
        public Nullable<bool> DRUGCONTROL { get; set; }
        public string ModelNo { get; set; }
        public Nullable<bool> CriticalItem { get; set; }
        public Nullable<bool> Feasibility { get; set; }
        public Nullable<byte> iscocktail { get; set; }
        public string catalogueno { get; set; }
        public int stationid { get; set; }
    }
}
