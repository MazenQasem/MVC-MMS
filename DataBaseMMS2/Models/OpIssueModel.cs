using System;
using System.Collections.Generic;

namespace MMS2
{
    public partial class Patient
    {
        public String OrgnizationIssueCode { get; set; }
        public string OperatorName { get; set; }
        public string PatientName { get; set; }
        public string SexTitle { get; set; }
        public string AgeTitle { get; set; }
        public string ErrMsg { get; set; }
        public bool CheckEmpPin { get; set; }
        public bool BillisCredit { get; set; }          public int DiscountTypeID { get; set; }
        public int disAuthoriseID { get; set; }
        public String DisReason { get; set; }
        public int mDeptId { get; set; }
        public string CategoryName { get; set; }
        public string CompanyName { get; set; }
        public string GradeName { get; set; }
        public int mLOAConsultation { get; set; }
        public int mAuthorityid { get; set; }
        public int gDedPerType { get; set; }
        public decimal mDeductper { get; set; }
        public int mDeducttype { get; set; }
        public long gOPPHAmtLmt { get; set; }
        public string lblDeductableText { get; set; }
        public decimal LoaLimit { get; set; }
        public long mDedAmt { get; set; }
        public bool mApproval { get; set; }
        public int revisitdays { get; set; }
        public string lblcategory { get; set; }
        public string lblCompany { get; set; }
        public string lblgrade { get; set; }
        public string lbldate { get; set; }
        public string lblLOAAmt { get; set; }
        public string lblLoaDays { get; set; }
        public string lblLoaBal { get; set; }
        public string TxtLOAletterno { get; set; }
        public string TxtInsCardno { get; set; }
        public string lblIdExpiry { get; set; }
        public string TxtCompanyRemarks { get; set; }
        public bool PatientNotFound { get; set; }
        public decimal lblNetAmount { get; set; }
        public decimal lblBalDeposit { get; set; }
        public decimal lblCreditBillAmount { get; set; }
        public decimal lblCreditDiscount { get; set; }
        public decimal lbldedamt { get; set; }
        public decimal lbldonationAMT { get; set; }
        public decimal txtAmountToBeCollected { get; set; }
        public decimal lblBalance { get; set; }

        public int gStationID { get; set; }
        public long mPresid { get; set; }
        public int DispatchDoctorID { get; set; }

        
        public bool PrescriptionDialog { get; set; }
        public string PrescriptionDialogTxt { get; set; }
        public bool BatchExpiryDialog { get; set; }
        public string BatchExpiryDialogTXT { get; set; }
        public bool QOHMinLevelFlag { get; set; }
                          


        public decimal mdisper { get; set; }
        public List<PHAlertMsg> PHalerts { get; set; }
        public List<TempListMdl> Allergey { get; set; }
        public List<TempListMdl> OtherAllergey { get; set; }

        public List<InsertedItemsList> InsertItemValue { get; set; }

        
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public bool mStaff { get; set; }
        public string strExpBatch { get; set; }
        public List<InsertedItemsList> arrItem { get; set; }     

        
        public string DoctorName { get; set; }
        public string DepositChkNo { get; set; }
        public int COBILLNO { get; set; }
        public string sPrefix { get; set; }

        
        public string lblBillNo { get; set; }
        public string DisDoctorName { get; set; }


        
        public int PrintType { get; set; }
        public long MaxBillNo { get; set; }
        public long csMaxBillNo { get; set; }

       
        
    }

    public class PHAlertMsg
    {
        public int Slno { get; set; }
        public string Message { get; set; }
        public string Operator { get; set; }
        public string Station { get; set; }

    }

    public partial class Prescription
    {
        public long OrderNo { get; set; }
        public string DateTime { get; set; }
        public string Operator { get; set; }
        public string Station { get; set; }
        public int DoctorID { get; set; }
        public int Status { get; set; }
        public int VisitId { get; set; }
        public int MPrescriptionF { get; set; }
        public bool corder { get; set; }
        public string ErrMsg { get; set; }
    }

    public partial class PrescriptionDetail
    {
        public int drugid { get; set; }
        public string Name { get; set; }
        public int strength { get; set; }
        public string RouteofAdmin { get; set; }
        public string frequency { get; set; }
        public long issquantity { get; set; }
        public string Duration { get; set; }
        public int OrderNo { get; set; }
        public long lquantity { get; set; }
        public string GenericName { get; set; }
        public string DateTime { get; set; }
        public string ErrMsg { get; set; }

    }
    public partial class Diagnosis
    {
        public int VisitId { get; set; }
        public string ICDCode { get; set; }
        public string Description { get; set; }
        public string ErrMsg { get; set; }

    }
    public partial class Item
    {
        public string ErrMsg { get; set; }
        public bool AlertDrugInteraction { get; set; }
        public bool AlertItemIssuedAlerdy { get; set; }
        public string StrWhole { get; set; }
        public List<InsertedItemsList> InsertItemValue{get;set;}
        public bool ErrorFlag { get; set; }
    }
    public partial class ItemSelectParam
    {
        public bool optCrPat { get; set; }
        public int mDeptId { get; set; }
        public int mAuthorityid { get; set; }
        public int cmbCompany { get; set; }
        public int mCategoryid { get; set; }
        public int mGradeId { get; set; }
        public int mItemID { get; set; }
        public String mItemName { get; set; }
        public int RegNo { get; set; }
        public bool blnDrugInteraction { get; set; }
        public string StrWhole { get; set; }
        public String gIACode { get; set; }
                 public long mQty { get; set; }
        public long mPrescription { get; set; }
        public int substitutetype { get; set; }
        public int listview { get; set; }
        public int StationID { get; set; }
        public List<InsertedItemsList> InsertItemValue { get; set; }
        public string ErrMsg { get; set; }
        public int GenericID { get; set; }
        public int OrderItemId { get; set; }
        public string OrderedItemName { get; set; }
    }
    public partial class ListofItemSelecParm
    {
    public List<ItemSelectParam> PresDrugList{get;set;}

    
    }
    public partial class InsertedItemsList
{

    
        public int SNO { get; set; }
        public string DrugName { get; set; }
        public string BatchNo { get; set; }          public int qoh { get; set; }
       
        public string Highunit { get; set; }
        public decimal price { get; set; }
        public float tax { get; set; }          
        public decimal qty { get; set; }
        

        public string UnitList { get; set; }
        public decimal amount { get; set; }
        public string NewUomName { get; set; }           public int DispatchQty { get; set; }
        
        public int qoh2 { get; set; }
        public int Drugtype { get; set; }
        public long conversionqty { get; set; }          public string batchid { get; set; }           
        public int purqty { get; set; }
        public int lsmallqty { get; set; }
        public long NewUomID { get; set; }
        public long PrescriptionID { get; set; }
    
        public int Deductabletype { get; set; }
        public long DeductablePerAmounttype { get; set; }
        public decimal DeductablePerAmount { get; set; }
        public int DiscountPerAmountType { get; set; }
        
        public decimal DiscountPerAmount { get; set; }
        public string OrderedItem { get; set; }
        public int OrderedItemid { get; set; }
        public string temp3 { get; set; }
        
        public string Name { get; set; }         public string ItemCode { get; set; }         public int ID { get; set; }

        public decimal Epr { get; set; }
        public int DedQty { get; set; }
        public decimal ItemTotal { get; set; }
        
    public string Action { get; set; }
    public string ErrMsg { get; set; }
    
    public decimal disamt { get; set; }
    public decimal dedAmt { get; set; }

        
    public int GenericID { get; set; }
        
    public int MinLevel { get; set; }
}
    public partial class AppItemService
    {
        public long appid { get; set; }
        public long ServiceId { get; set; }
        public long Itemid { get; set; }
        public long DeptId { get; set; }
    }
    public class SelectedItemsList
    {
        public int OrderdItemID { get; set; }


    }
    public partial class Deposit
    {
        public int DepositID { get; set; }
        public string DepositChkNo { get; set; }
        public int RegNo { get; set; }
        public decimal NetBalance { get; set; }
        public decimal IPBalance { get; set; }
        public String ErrMsg { get; set; }
        public bool optCrPat { get; set; }
        public string gIACode { get; set; }
        public string NotifyMsg { get; set; }

    }

    
    public class SearchTable
    { 
        public  int ID {get;set;}
        public  string PatientName {get;set;}
        public  string DoctorName {get;set;}
        public  string OperatorName {get;set;}
        public  int slno { get; set; }
        public  int BillNo {get;set;}
        public  int Creditbillid {get;set;}
        public  decimal amount {get;set;}
        public  bool Canceled {get;set;}
        public  string canceledby {get;set;}
        public  int compcredit {get;set;}
        public  int billtype {get;set;}
        public  string Date { get; set; }
        public  string ErrMsg { get; set; }
    }

    
    public class PrnIssueMaster
    {
        public int BillID { get; set; }
        public string RegNo { get; set; }
        public string PTName { get; set; }
        public string Duplicate { get; set; }
        public string bno { get; set; }
        
        public string cashbillno { get; set; }
        public string creditbillno { get; set; }
        public string BillType { get; set; }
        
        public string CANCELLED { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeID { get; set; }
        public string dtpIssueDt { get; set; }
        
        public string DoctorName { get; set; }
        public string DisDoctorName { get; set; }
        public string PaidTitle { get; set; }
        public string CashierName { get; set; }

        public decimal BillAmount { get; set; }
        public decimal DisPer { get; set; }
        public decimal Dis { get; set; }
        public decimal NetAmt { get; set; }
        public decimal deductper { get; set; }
        public decimal deductableamt { get; set; }
        
        public decimal balance { get; set; }
        
        public decimal DonationAmt { get; set; }
        
             
        public List<PrnIssueDetail> ItemList { get; set; }

        public string ErrMsg { get; set; }
    }
    public class PrnIssueDetail
    {
        public int BillID { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int quantity { get; set; }
        public decimal itemAmt { get; set; }
    
    }
}
