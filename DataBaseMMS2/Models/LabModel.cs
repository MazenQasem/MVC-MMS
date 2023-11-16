using System.Collections.Generic;

namespace MMS2
{
    public partial class LabModel
    {

        public List<TempListMdl> ProfileList { get; set; }
        public List<TempListMdl> ItemList { get; set; }
        public List<TempListMdl> UnitList { get; set; }
        public int ProfileID { get; set; }
        public int ProfileUnitID { get; set; }
        public int MaxID { get; set; }
        public List<ProfileItems> SelectedItems { get; set; }
        public string ErrMsg { get; set; }

        public List<InitGrid> IssueViewList { get; set; }

        
        public string GRN { get; set; }
        public string OrderDate { get; set; }
        public string DateTime { get; set; }
        public int IssueNo { get; set; }
        public string RecDate { get; set; }
        public string Operator { get; set; }

    }
    public partial class ProfileItems
    {
        public int SNO { get; set; }
        public string Drug { get; set; }
        public int Qty { get; set; }
        public string Unit { get; set; }
        public int UnitID { get; set; }
        public int ID { get; set; }
        public string ErrMsg { get; set; }

        
        public int MaxID { get; set; }
        public int ProUOMID { get; set; }

        
        public int QOH { get; set; }
    }
    
    public partial class InitGrid
    { 
             public int IssueId { get; set; }
        public string IssueNo { get; set; }
        public string ItemName { get; set; }
        public string Department { get; set; }
        
        public string Operator { get; set; }
        public string DateTime { get; set; }
        public string RefNo { get; set; }

        
        public int DepartmentID { get; set; }
        public string ErrMsg { get; set; }
        public List<ProfileItems> SelectedItems { get; set; }
        public List<arrItem> arrItem { get; set; }
        
        public List<ReceiptItems> ReciptList { get; set; }
        public int checkStatus { get; set; }
    }
    public partial class ItemDtl
    {
        public string Item { get; set; }
        public string rQty { get; set; }
        public int QOH { get; set; }
        public string ErrMsg { get; set; }
    }
    public partial class LabReceiptView
    {
        public string ReceiptNo { get; set; }
        public string IssueNo { get; set; }
        public string DateTime { get; set; }
        public string Operator { get; set; }
        public int ID { get; set; }
    }
    public partial class ReceiptItems
    {
        public int SNO { get; set; }
        public string Item { get; set; }
        public string UOM { get; set; }
        public string temp { get; set; }
        public int OrdQty { get; set; }
        
        public int ReceivedQty { get; set; }
        public int PrvRecivedQty { get; set; }
        public int UnitID { get; set; }
        public int BatchID { get; set; }
        public double CostPrice { get; set; }
        public double SellPrice { get; set; }

        public int Qty { get; set; }
        public decimal EPR { get; set; }
        public decimal SP { get; set; }
        
        public int ID { get; set; }
        public string ErrMsg { get; set; }
        public int lGetQty { get; set; }

    
    
    
    }
}



