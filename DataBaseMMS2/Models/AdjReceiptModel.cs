using System.Collections.Generic;

namespace MMS2
{
    public partial class AdjReceiptModel
    {
        public List<TempListMdl> StationList { get; set; }
        public List<AdjView> ViewList { get; set; }

        public int StationID { get; set; }
        public int lblStation { get; set; }
        public int ReceiptID { get; set; }
        public string lbldate { get; set; }
        public string lblOperator { get; set; }
        public string txtRefNo { get; set; }
        public string lblNo { get; set; }
        public List<AdjItemsInserted> SelectedItem { get; set; }
        public string ErrMsg { get; set; }
    }
    public partial class AdjView
    {
                 public int AdjRecID { get; set; }
        public int Number { get; set; }
        public string Operator { get; set; }
        public string Date { get; set; }
        public string RefNo { get; set; }
                 public int IssueTo { get; set; }
     }
    
    public class AdjItemsInserted
    {
        
        public int SNO { get; set; }          public string Item { get; set; }
        public string BatchNo { get; set; }
        public double QOH { get; set; }
        public decimal PRate { get; set; }
        public string Expiry { get; set; }
        public decimal Quantity { get; set; }
        public string Reason { get; set; }
        public int ConversionQty { get; set; }
        public int DrugType { get; set; }
        public int BatchID { get; set; }          public int UnitID { get; set; }
        public int CategoryID { get; set; }
        public decimal SellingPrice { get; set; }
        public string UOM { get; set; }
        public string ItemCode { get; set; }          public int ItemID { get; set; }
        public string ErrMsg { get; set; }
        public bool BatchNotFound { get; set; }
        public bool NewBatchFlag { get; set; }
        }


}



