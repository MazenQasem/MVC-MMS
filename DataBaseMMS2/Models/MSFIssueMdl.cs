using System.Collections.Generic;

namespace MMS2
{
    public class MSFIssueMdl
    {
        
        public int OrderID { get; set; }

        public string Address { get; set; }
        public string CompanyName { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public string Age { get; set; }
        public List<ViewList> ViewList { get; set; }
        public List<InsertItems> ItemsList { get; set; }
        public string ErrMsg { get; set; }
        public List<TempListMdl> VisitListDate { get; set; }
        public List<TempListMdl> VisitListDoc { get; set; }


        
        public string PINNO { get; set; }
        public int CmbReqDoctorID { get; set; }
        public bool IsProcedure { get; set; }         public int CmbVisitListDate { get; set; }
        public int CmbVisitListDoc { get; set; }
        public string RegNo { get; set; }
    }
    public class ViewList
    {
        public int ID { get; set; }
        public string PIN { get; set; }
        public string PTName { get; set; }
        public string DateTime { get; set; }
        public string Type { get; set; }
        public int DRec { get; set; }
        public int TRec { get; set; }
    }
    public class InsertItems
    {
        public int ItemID { get; set; }
        public string ItemCode { get; set; }

        public string ItemName { get; set; }
        public int QtyIssue { get; set; }
        public string IssueUnits { get; set; }
        public string BatchNo { get; set; }
        public string ExpiryDate { get; set; }
        public int QOH { get; set; }
        public bool DispatchQty { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public int UnitID { get; set; }
        public int ConversionQty { get; set; }
        public int BatchID { get; set; }

        public string UomList { get; set; }
        public string ErrMsg { get; set; }
        public bool ReadOnly { get; set; }


    }

    
    public class MSFreportHeader
    {
        
        public int Page { get; set; }
        public int OrderID { get; set; }
        public int OrderType { get; set; }
        
        public string PatientPin { get; set; }          public string PatientName { get; set; }
        public string Nationality { get; set; }
        public string Company { get; set; }
        public string DoctorName { get; set; }
        public string ReqDate { get; set; }
        public string Type { get; set; }         public string MSFNo { get; set; }
        public string StationName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string OrderTypeName { get; set; }
        public List<MSFreportDetail> rDetail { get; set; }
        public int TotQty { get; set; }
        public string OperatorName { get; set; }
        public string ReqDoctorName { get; set; }
    }
    public class MSFreportDetail
    {
        public int Seq { get; set; }
        public string DeptCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Qty { get; set; }
    
    }

    public class MSFPrintList
    {
        public string OrderNo { get; set; }
        public string PIN { get; set; }
        public string Name { get; set; }
        public string OrderDate { get; set; }
        public string OrderType { get; set; }
    
    
    }
}



