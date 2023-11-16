using System.Collections.Generic;

namespace MMS2
{
    public class ProcMstrComMdl
    {
        public int DeptID { get; set; }
        public int ProcID { get; set; }
        public int ProcType { get; set; }
        public bool Discountinue { get; set; }
        public List<TempListMdl> DeptList { get; set; }
        public List<TempListMdl> ProcList { get; set; }
        public List<TempListMdl> ProcTypeList { get; set; }
        public List<ItemInsertedList> ItemList { get; set; }
        public bool NewRecord { get; set; }
        public int lOrderId { get; set; }
        public string ErrMsg { get; set; }


    }

    public class ItemInsertedList
    {
        public int SNO { get; set; }
        public string ItemName { get; set; }
        public string Units { get; set; }
        public int Quantity { get; set; }
        public int ItemType { get; set; }           public int UnitID { get; set; }
        public int ItemID { get; set; }
        public string ErrMsg { get; set; }
    }


    
    public class ProcIssueMain
    {
        
        
        public string txtBillNo { get; set; }
        public string lblTotCost { get; set; }
        public string lblPin { get; set; }
        public string lblPatient { get; set; }
        public string lblAge { get; set; }
        public string lblSex { get; set; }
        public string lblOrdDate { get; set; }
        public string lblProcQty { get; set; }
        public string lblDateTime { get; set; }
        public string dtpFromDate { get; set; }
        public string lblOperator { get; set; }
        public int intProcType { get; set; }
        public int lngBillId { get; set; }
        public byte IsInvDone {get;set;}
        

        public List<TempListMdl> DeptList { get; set; }
        public int cmbDepartment { get; set; }

        public List<TempListMdl> ProcList { get; set; }
        public int cmbProcedures { get; set; }

        public List<TempListMdl> BillList { get; set; }
        public int cmbBillNo { get; set; }

        
        public List<ProcIssueItemList> ItemList { get; set; }
        public int InsertItemID { get; set; }
        public int InsertTYpe { get; set; }

        
        public List<arrItem> arrItem { get; set; }
    }
    public class ProcIssueItemList
    {
        public int SNO { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int QOH { get; set; }
        public string Units { get; set; }
        public int Quantity { get; set; }
        public decimal Ucost { get; set; }
        public string OrderedItem { get; set; }
        
        public int UnitID { get; set; }
        public int IssItemID { get; set; }
        public int ItemType { get; set; }           
        public int ItemID { get; set; }
        public string ErrMsg { get; set; }
                                                                
    
    }

}



