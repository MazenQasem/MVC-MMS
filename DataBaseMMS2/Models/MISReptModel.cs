using System;
using System.Collections.Generic;


namespace MMS2
{
    public class MISRept
    {
        public int ReportNo { get; set; }
        public string ReportName { get; set; }
        public int gStationID { get; set; }
        public string gIACode { get; set; }
        public string AppPath { get; set; }
        
        public int MSupplierID { get; set; }
        public int MRadioID { get; set; }
        public string ReportStrID { get; set; }     }

         public class CashSummary
    {
        public string Date { get; set; }
        public string Station { get; set; }
        public List<CashSummaryDetail> CashDtl { get; set; }
                 public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string ParaTitle1 { get; set; }
        public string ParaTitle2 { get; set; }

    }
    public class CashSummaryDetail
    {
        public int Sno { get; set; }
        public string Receipt { get; set; }
        public decimal Amount { get; set; }
        public string PIN { get; set; }
        public string Time { get; set; }
        public string User { get; set; }
    }

         public class CashIssueDetail
    {
        public int Sno { get; set; }
        public string OrderNo { get; set; }
        public string OrderDate { get; set; }
        public string Item { get; set; }
        public string Company { get; set; }
        public decimal Price { get; set; }
        public decimal QTY { get; set; }
        public string UOM { get; set; }
        public string Station { get; set; }
    }

         public class PhOperatorWiseDispensesHeader
    {
        public string Title { get; set; }
        public string TitleDate { get; set; }
        public string TitleSheft { get; set; }
        public string TitleStation { get; set; }



        public int ReprotType { get; set; }

        public long GrandCashCount { get; set; }
        public long GrandCompanyCount { get; set; }
        public long GrandTotal { get; set; }

        public List<PhOperatorWiseDispenses> Datalist { get; set; }
    }
    public class PhOperatorWiseDispenses
    {
        public int SNO { get; set; }
        public int ID { get; set; }
        public string prefix { get; set; }
        public string BillStationNo { get; set; }
        public string PEmpoyeeID { get; set; }
        public string EmpCode { get; set; }
        public string OEmployeeID { get; set; }
        public string DateTime { get; set; }
        public Single Prcie { get; set; }          public decimal Quatnity { get; set; }         public string ItemCode { get; set; }
        public string PinNo { get; set; }
        public string Company { get; set; }
        public int Comp_ID { get; set; }
        public int CashCredit { get; set; }
        public byte BillType { get; set; }

        public long CashCount { get; set; }
        public long CompanyCount { get; set; }
        public long Total { get; set; }


    }
         public class ItemWiseIssuesDetailsHeader
    {
        public string Title { get; set; }
        public string TitleDate { get; set; }
        public string TitleStation { get; set; }
        public List<ItemWiseIssuesDetailsDetail> ListDetail { get; set; }


    }

    public class ItemWiseIssuesDetailsDetail
    {
        public string BillNo { get; set; }
        public string PinNo { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public decimal Quantity { get; set; }


    }
         public class ItemLedgerHeader
    {
        public string Title { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Station { get; set; }
        public string Units { get; set; }
        public int TotalReceipt { get; set; }
        public int TotalIssues { get; set; }
        public List<ItemLedgerDetail> TransDetail { get; set; }
        public string ErrMsg { get; set; }
    }
    public class ItemLedgerDetail
    {
        public string TransCode { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public long Issue { get; set; }
        public long Receipt { get; set; }
        public long Stock { get; set; }
    }
         public class ItemLedgerPriceHeader
    {
        public string Title { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Station { get; set; }
        public string Units { get; set; }
        public string Category { get; set; }
        public int TotalReceipt { get; set; }
        public int TotalIssues { get; set; }
        public List<ItemLedgerPriceDetail> TransDetail { get; set; }
        public List<ItemLedgerPriceSummary> TransSummary { get; set; }
        public string ErrMsg { get; set; }
        
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string gStationname { get; set; }
        public int Gstationid { get; set; }
        public int CatID { get; set; }
        public int RadioID { get; set; }
        public string RadioText { get; set; }
        public int ItemID { get; set; }
        public int Idkey { get; set; }
    }
    public class ItemLedgerPriceDetail
    {
        public string TransCode { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public decimal Issue { get; set; }
        public decimal Receipt { get; set; }
        public decimal Stock { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }




        public string ExpDate { get; set; }
        public decimal UnitCost { get; set; }
        public decimal Value { get; set; }
        public int DeptID { get; set; }
        public string DeptName { get; set; }
        public decimal IssValue { get; set; }
        public decimal RecValue { get; set; }


                                                                                                                                       
        public string Title { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Station { get; set; }
        public string Units { get; set; }
        public string Category { get; set; }
        public int TotalReceipt { get; set; }
        public int TotalIssues { get; set; }

    }
    public class ItemLedgerPriceSummary
    {
        public int DeptID { get; set; }
        public string Department { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
    }
    
    public class ExpiryDateReportHeader
    {
        public string Title { get; set; }
        public string Category { get; set; }

        public decimal TotalValue { get; set; }
        public List<ExpiryDateReportDetail> TransDetail { get; set; }
        public string ErrMsg { get; set; }
        public bool WithPrice { get; set; }
    }
    public class ExpiryDateReportDetail
    {
        public int SNO { get; set; }
        public string Category { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public Decimal QOH { get; set; }
        public string BatchNo { get; set; }
        public string ExpiryDate { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Value { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }

                                                                                                                                                                                                                            }
    
    public class ISBSHeader
    {
        public string Title { get; set; }
        public string Supplier { get; set; }


        public List<ISBSDetail> TransDetail { get; set; }
        public string ErrMsg { get; set; }

    }
    public class ISBSDetail
    {
        public int SNO { get; set; }
        
        public int ID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int conversionqty { get; set; }
        public string Unit { get; set; }

        public decimal totiss { get; set; }
        public decimal qty1 { get; set; }
        public decimal qty2 { get; set; }
        public decimal qty3 { get; set; }
        public decimal qty4 { get; set; }
        public decimal qty5 { get; set; }

        public decimal totiss_inp { get; set; }
        public int inp_conversionqty { get; set; }
        public string inp_unit { get; set; }

        public decimal TotalStock { get; set; }
        public decimal TotalIssuance { get; set; }
        public decimal monthOrder { get; set; }


                                             
                                                      
                                                               

    }




    
    public class ItemMasterListHeader
    {
        public string Title { get; set; }
        public string ErrMsg { get; set; }
        public string Category { get; set; }
        public string Station { get; set; }
        public List<ItemMasterListDetail> TransDetail { get; set; }
        public decimal TotalCost { get; set; }
        public int ShowExpirydate { get; set; }
        public int ShowCost { get; set; }
    }
    public class ItemMasterListDetail
    {
        public int SNO { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public decimal QOH { get; set; }
        public string RackShelf { get; set; }
        public decimal TotalCost { get; set; }


                                                                                                                     
    }
    
    public class IPWiseIssueHeader
    {
        public string ErrMsg { get; set; }
        public List<IPWiseIssueDetail> TransDetail { get; set; }

    }
    public class IPWiseIssueDetail
    {
        public string PInfo { get; set; }

        public string Type { get; set; }
        public int SNO { get; set; }
        public string TranDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal QOH { get; set; }
        public string Unit { get; set; }

    }

    
    public class ErrMedModel
    {

        public int GROUPID { get; set; }
        public string GroupName { get; set; }
        public string Title { get; set; }
        public string Ccount { get; set; }

    }
    public class ActiveProfileMdl
    {

        public string State { get; set; }
        public string emp { get; set; }
        public string PinNO { get; set; }
        public string bed { get; set; }
        public string xemp { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }

        public Int32 PINNO2 { get; set; }
        public Int32 topline { get; set; }


    }

    public class AdjustIssueRptMdl
    {
        public string Title { get; set; }
        public string Station { get; set; }
        public decimal TotalAmount { get; set; }
        public bool chkWithAmount { get; set; }
        public List<AdjustIssueRptMdlDtl> TransDetail { get; set; }
        public string ErrMsg { get; set; }
    }
    public class AdjustIssueRptMdlDtl
    {
        public string IssueNo { get; set; }
        public string RefNo { get; set; }
        public string Date { get; set; }
        public string Item { get; set; }
        public string Qty { get; set; }
        public string Unit { get; set; }
        public string Remarks { get; set; }
        public string Amount { get; set; }
        public string UserId { get; set; }
        public string Cost { get; set; }
        public string ErrMsg { get; set; }
    }

    public class AdjustRecRptMdl
    {
        public string Title { get; set; }
        public string Station { get; set; }
        public decimal TotalAmount { get; set; }
        public bool chkWithAmount { get; set; }
        public List<AdjustRecRptMdlDtl> TransDetail { get; set; }
        public string ErrMsg { get; set; }
    }
    public class AdjustRecRptMdlDtl
    {
        public string Date { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string OriginalQty { get; set; }
        public string AdjQty { get; set; }
        public string Amount { get; set; }
        public string NewQty { get; set; }
        public string UserID { get; set; }
        public string DocNo { get; set; }
        public string CostPrice { get; set; }
        public string ErrMsg { get; set; }


    }
    
    public class ADTRptMdl
    {
        public string Title { get; set; }
        public string Station { get; set; }
        public List<ADTRptMdlDtl> TransDetail { get; set; }
        public string ErrMsg { get; set; }

    }
    public class ADTRptMdlDtl
    {
        public string slno { get; set; }
        public string bed { get; set; }
        public string pin { get; set; }
        public string name { get; set; }
        public string age { get; set; }
        public string sex { get; set; }
        public string AdmDate { get; set; }
        public string station { get; set; }
        public string ErrMsg { get; set; }


    }

    
    public class AntiUsedHDRMdl
    {
        public string AsOfDate { get; set; }
        public List<AntiUsedMdl> ItemList { get; set; }
        public string ErrMsg { get; set; }
    }
    public class AntiUsedMdl
    {
        public string PIN { get; set; }
        public string Name { get; set; }
        public string Doctor { get; set; }
        public string Item { get; set; }
        public string Station { get; set; }
        public string DateTime { get; set; }
        public string Status { get; set; }
        public string ErrMsg { get; set; }
   
    }
}
