using System;
using System.Collections.Generic;

namespace MMS2
{
    public partial class IndentOrderModel
    {
        public int OrderID { get; set; }
        public string lbldate { get; set; }
        public string lbloperator { get; set; }
        public string lblId { get; set; }
        public string txtRef { get; set; }
        public string dtpBydate { get; set; }
        public string ErrMsg { get; set; }
        public int ItemCategory { get; set; }
        public int ToStationID { get; set; }
        public int SectionID { get; set; }
        public byte Status { get; set; }
        public List<IndentView> IndentList { get; set; }
        public List<TempListMdl> StationList { get; set; }
        public List<IndentInsertedItemList> SelectedItem { get; set; }

        
        public string ToStationName { get; set; }
        public string CurrentStationName { get; set; }

        
        public List<IndentReturnedItemList> ReturnList { get; set; }
        public int mIssueID { get; set; }
        public int IssSlNO { get; set; }
        public int CurrentStation { get; set; }

        
        public string IndentTxtRef { get; set; }
        public List<IndentIssueInsertedItemList> IssueList { get; set; }
        public List<arrItem> arrItem { get; set; }

        
        public List<ReceiptIndentView> ReceiptIndentView { get; set; }
        public string IndentNo { get; set; }         public string Slno { get; set; }
        public string IndentDate { get; set; }
        public string ReferenceNo { get; set; }
        public string IndentBy { get; set; }
        public string IssueOn { get; set; }
        public string IssuedBy { get; set; }
        public string ReceivedAt { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceiptNo { get; set; }
        public int IssueNo { get; set; }
        public int IndentID { get; set; }
        public List<IndentReceiptDtl> ItemReceiptList { get; set; }
        
        public List<ReturnReceiptIndentView> ReturnView { get; set; }
        public List<ReturnReceiptDtl> ReturnItemList { get; set; }
        public int mReturnId { get; set; }
        public string lblReturnNo { get; set; }
        public string lblReturnfrom { get; set; }
        public string lblRetAt { get; set; }
        public string lblRetBy { get; set; }
        public int SourceID { get; set; }
        
    }
    public partial class IndentView
    {
        public int IndentID { get; set; }
        public string StationSlNO { get; set; }
        public string IndentDateTime { get; set; }
        public string DeliveryDate { get; set; }
        public string referenceno { get; set; }
        public byte Status { get; set; }
        public string IndentTo { get; set; }
        public string IndentByName { get; set; }
        public int CategoryID { get; set; }
        public int SectionID { get; set; }
        public int ToStationID { get; set; }


        
        public int RetrunIssueID { get; set; }
        public bool ReturnType { get; set; }
    }
    public partial class IndentDeptView
    {
        public int IssueNo { get; set; }
        public string OrderNo { get; set; }
        public string Department { get; set; }

        public string DateTime { get; set; }
        public string Operator { get; set; }
        public string RefNo { get; set; }

        public int Status { get; set; }
        public int ID { get; set; }



    }
    
                              
               
         
    public partial class IndentInsertedItemList
    {

        
        public int SNO { get; set; }
        public string Item { get; set; }
        public int DistinationQty { get; set; }
        public int QtyReqeust { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public string Remarks { get; set; }
        public int MaxLevel { get; set; }
        public int QOH { get; set; }
        public int MinLevel { get; set; }
        public int ID { get; set; }


        public int DrugType { get; set; }
        public string UnitList { get; set; }
        public int conversionqty { get; set; }
        public int NewUomID { get; set; }



        public string Name { get; set; }
        public string ItemCode { get; set; }

        
        public string Action { get; set; }
        public string ErrMsg { get; set; }
    }

    public partial class IndentReturnedItemList
    {
                 
        public int SNO { get; set; }
        public string Name { get; set; }
        public string BatchNo { get; set; }
        public int BatchID { get; set; }
        public string ExpiryDate { get; set; }

        public int Quantity { get; set; }
        public int QOH { get; set; }
        public int issQty { get; set; }
        public int retQty { get; set; }
        public int converQty { get; set; }

        public string unit { get; set; }
        public int UnitID { get; set; }
        public string Remarks { get; set; }
        public int DrugType { get; set; }


        public int ID { get; set; }





        public string ItemCode { get; set; }



        
        public string Action { get; set; }
        public string ErrMsg { get; set; }

    }
    
    public partial class IndentIssueView
    {
        public string IndentNo { get; set; }
        public string IndentFrom { get; set; }
        public string IndentDate { get; set; }
        public string DeliveryBy { get; set; }
        public string SentBy { get; set; }

        public string IssuedBy { get; set; }
        public string IssueDate { get; set; }
        public string RefNo { get; set; }
        public int categoryID { get; set; }
        public byte status { get; set; }

        public int IndentId { get; set; }
        public byte isCocktail { get; set; }
        public int sourceid { get; set; }
        public byte recstatus { get; set; }
        public byte QuerySeq { get; set; } 
        public string IndentTxtRef { get; set; }

                 public int IndentIssueID { get; set; } 
        public List<IndentIssueInsertedItemList> ItemList { get; set; }


    }

    public partial class IndentIssueInsertedItemList
    {

        
        public int SNO { get; set; }
        public string Item { get; set; }
        public string Batch { get; set; }
        public string Expiry { get; set; }
        public double QOH { get; set; }

        public double QtyIssue { get; set; }         public string IssueUnits { get; set; }         public int ItemID { get; set; }         public int substituteid { get; set; }
        public int DrugType { get; set; }  
        public int lstsub { get; set; }          public int IssueUnitID { get; set; }
        public int conversionqty { get; set; }
        public int PrevQty { get; set; }
        public int OrderQty { get; set; }

        public int PrevUnitID { get; set; }         public int IndentUnitID { get; set; }         public int batchQty { get; set; }          public int totqty { get; set; }         public int MinLevel { get; set; } 

        
        public string Action { get; set; }         public string ErrMsg { get; set; }         public bool ForEdit { get; set; } 
        
        public int lLargeQty { get; set; }
        public int lSmallOrderQty { get; set; }
        public Single Price { get; set; }

        
        public Single Amount { get; set; }
    }

          
                                             
               
         
    public partial class ReceiptIndentView
    {
        public string IndentNo { get; set; }         public string Slno { get; set; }
        public string IndentDate { get; set; }
        public string ReferenceNo { get; set; }
        public string IndentBy { get; set; }
        public string IssueOn { get; set; }
        public string IssuedBy { get; set; }
        public string ReceivedAt { get; set; }
        public string ReceivedBy { get; set; }
        public byte Status { get; set; }
        public int IssueNo { get; set; }
        public int IndentID { get; set; }
        public byte isCocktail { get; set; }
    }

    public partial class IndentReceiptDtl
    {
        public int SNO { get; set; }
        public string Item { get; set; }
        public double OrderQty { get; set; }
        public string OrderUnitName { get; set; }

        public string BatchNo { get; set; }
        public double IssuedQty { get; set; }
        public string IssueUnitName { get; set; }

        public double ReceivedQty { get; set; }
        public decimal SellingPrice { get; set; }
        public string ReceivedUnitName { get; set; }

        public int UnitID { get; set; }
        public int BatchID { get; set; }
        public int ItemID { get; set; }

        public string ItemCode { get; set; }



        
        public string Action { get; set; }
        public string ErrMsg { get; set; }

        public int StationSlno { get; set; }
        public int MaxSlno { get; set; }

    }


    public partial class ReturnReceiptIndentView
    {
        public string ReturnNo { get; set; }         public string ReturnDate { get; set; }
        public string ReturnBy { get; set; }
        public string ReturnFrom { get; set; }
        public string ReceivedBy { get; set; }
        public string ReceivedAt { get; set; }
        public byte Status { get; set; }
    }
    public partial class ReturnReceiptDtl
    {
        public int SNO { get; set; }
        public string Item { get; set; }
        public string BatchNo { get; set; }
        public string ExpiryDate { get; set; }
        public double QOH { get; set; }
        public double QtyRet { get; set; }
        public string RetRemarks { get; set; }
        
        public double QtyRec { get; set; }
        public string RecRemarks { get; set; }
        public string Unit { get; set; }
     
        public int ConversionQty { get; set; }
        public int BatchID { get; set; }
        public int UnitID { get; set; }
        public int ItemID { get; set; }
        public string Action { get; set; }
        public string ErrMsg { get; set; }

    }

}



