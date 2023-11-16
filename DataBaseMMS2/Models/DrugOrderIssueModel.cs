using System.Collections.Generic;

namespace MMS2
{
    public partial class DrugOrderIssueModel
    {
        public int OrderID { get; set; }
        public int IPID { get; set; }

        public string lblPinNo { get; set; }
        public string lblBed { get; set; }
        public string lblWard { get; set; }
        public string lblOrderNo { get; set; }
        public string lblName { get; set; }
        public string lblDateTime { get; set; }
        public string lblAge { get; set; }
        public string lblSex { get; set; }
        public string lblOperator { get; set; }
        public string lblDoctor { get; set; }
        public int CompanyID { get; set; }
        public bool isPartial { get; set; }
        public List<TempListMdl> DrugAllergiesList { get; set; }
        public List<TempListMdl> FoodAllergiesList { get; set; }
        public int CmbAssPHID { get; set; }
        public int CmbPHID { get; set; }

        public List<TempListMdl> PharmacistList { get; set; }
        public List<TempListMdl> AsstPharmacistList { get; set; }

        public List<OrdersItems> lvwItem { get; set; }
        public List<DrugInsertedItems> ItemList { get; set; }
        public List<arrItem> ArrItems { get; set; }

        public bool mDispatched { get; set; }
        public bool VIP { get; set; }
        public bool mAddNew { get; set; }
        public bool OrderType { get; set; }
        public decimal Amount { get; set; }


        public string ErrMsg { get; set; }
        public bool DrugInterActionFlag { get; set; }
        public string DrugInterActionStr { get; set; }
        public bool MinLvlFlag { get; set; }
        public string MinLvlStr { get; set; }


    }


         public class DrugOrderIssueView
    {
        public string OrderNo { get; set; }

        public int PINNO { get; set; }
        public string registrationno { get; set; }
        public string PatientName { get; set; }

        public string BedNo { get; set; }
        public string DateTime { get; set; }
        public string Operator { get; set; }

        public string Station { get; set; }
        public int Status { get; set; }
        public int isPartial { get; set; }

        public bool PrintStatus { get; set; }
        public int StationSlno { get; set; }
        public int type { get; set; }

        public int TakeHome { get; set; }
        public bool isCocktail { get; set; }
        public int OrderID { get; set; }


    }

         public class OrdersItems
    {
        public int Seq { get; set; }
        public int ItemID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UOM { get; set; }
        public string Strength { get; set; }

        public int Qty { get; set; }
        public int QOH { get; set; }
        public bool LvwPrescription { get; set; }
    }

    public class DrugInsertedItems
    {
        public int SNO { get; set; }
        public string DrugName { get; set; }
        public string Unit { get; set; }
        public int ReqQty { get; set; }
        public string BatchNo { get; set; }
        public string Expirydate { get; set; }
        public decimal QOH { get; set; }
        public int DispatchedQty { get; set; }
        public string Substitute { get; set; }
        public decimal TQOH { get; set; }
        public decimal lquantity { get; set; }
        public string Remarks { get; set; }
        public decimal Total { get; set; }
        public decimal Price { get; set; }
        public int UnitID { get; set; }
        public int ConversionQty { get; set; }

        public int BatchID { get; set; }
        public int ItemID { get; set; }
        public int SubItemID { get; set; }
        public int CSSD { get; set; }

        public int MinLevel { get; set; }


        public bool DrugInteraction { get; set; }
        public string DrugInteractionMSG { get; set; }

        public bool FoodInteraction { get; set; }
        public string FoodInteractionMSG { get; set; }

                                                                                                                                                                                    

    }

    public class DrugSubList
    {
        public int ItemID { get; set; }
        public long QOH { get; set; }
        public int ConvQty { get; set; }
        public int UnitID { get; set; }
        public string UnitName { get; set; }
        public int CSSD { get; set; }
        public decimal SellingPrice { get; set; }
        public List<TempListMdl> PackingList { get; set; }

    }

    public class DrugOrderPrintView
    {
        public string OrderNo { get; set; }
        public string Name { get; set; }
        public string Dcode { get; set; }
        public string Orderdatetime { get; set; }
        public string OrderType { get; set; }
        public string Doctor { get; set; }
        public string PinNo { get; set; }
        public string Bed { get; set; }
        public string Station { get; set; }
        public string EmployeeID { get; set; }
        public string DispatchDateTime { get; set; }
        public string PatientName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Company { get; set; }
        public string Pharmacist { get; set; }
        public string AsstPharmacist { get; set; }
        public List<DrugOrderPrintDetail> Items { get; set; }
        public List<DrugLableDetail> DrugLabels { get; set; }
        
        public string StationID { get; set; }
        public string mOrderID { get; set; }
    }
    public class DrugOrderPrintDetail
    {
        public int slno { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ExpiryDate { get; set; }
        public string Qty { get; set; }
        public string Unit { get; set; }
        public string price { get; set; }
        public string TotPrice { get; set; }

    }
    public class DrugLableDetail
    {
        public string PatientName { get; set; }
        public string Bed { get; set; }
        public string IssueDate { get; set; }
        public string Medicine { get; set; }
        public string Expdate { get; set; }
        public string Qty { get; set; }
        public string PIN { get; set; }


    }


}



