using System;
using System.Collections.Generic;

namespace MMS2
{
    public partial class UnitDoseBulkModel
    {
        public int OrderID { get; set; }
        public List<TempListMdl> PharmacistList { get; set; }
        public List<TempListMdl> AsstPharmacistList { get; set; }
        public List<TempListMdl> StationList { get; set; }



        public string ErrMsg { get; set; }


    }
    public class HeaderInfo
    {
        public string dtpDate { get; set; }
        public int StationID { get; set; }
        public int PHID { get; set; }
        public int AssPHID { get; set; }
        public string bulkDate { get; set; }
        public int OperatorID { get; set; }
                 public List<arrItem> arrItemList { get; set; }
        public List<arrItem> arrItemList1 { get; set; }
        public string ErrMsg { get; set; }
        public int gStationID { get; set; }
    }

    public class UnitDoseItemList
    {
        public int PresID { get; set; }
        public int ItemID { get; set; }
        public byte FrequencyID { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime DiscontinuedDateTime { get; set; }
        public int PerDayQty { get; set; }
        public int PerDoseQty { get; set; }
        public byte Period { get; set; }
        public byte UpdatedPeriod { get; set; }
    }

    
    public class OrderPrintView
    {
        public string AdmitDatetime { get; set; }
        public string name { get; set; }
        public string DCode { get; set; }
        public string orderdatetime { get; set; }
        public string ORDERTYPE { get; set; }
        public string Doctor { get; set; }
        public string RegistrationNo { get; set; }
        public string Bed { get; set; }
        public string Station { get; set; }
        public string employeeid { get; set; }
        public string dispatcheddatetime { get; set; }
        public string patientname { get; set; }
        public string Age { get; set; }
        public string SEX { get; set; }
        public string Charge { get; set; }
        public string Pharmacist { get; set; }
        public string AsstPharmacist { get; set; }
        public List<OrderPrintDetail> Items { get; set; }
        public List<LableDetail> DrugLabels { get; set; }
        public string stationid { get; set; }
    }
    public class OrderPrintDetail
    {
        public int Slno { get; set; }
        public string RegistrationNo { get; set; }
        public string itemcode { get; set; }
        public string item { get; set; }
        public string strength { get; set; }
        public string Dos { get; set; }
        public string frequency { get; set; }
        public string Qty { get; set; }
        public string StartDateTime { get; set; }
        public string enddatetime { get; set; }

    }
    public class LableDetail
    {
        public string PatientName { get; set; }
        public string Bed { get; set; }
        public string Medicine { get; set; }
        public string Expdate { get; set; }
        public string Dose { get; set; }
        public string IssueDate { get; set; }
        public string Qty { get; set; }
        public string PIN { get; set; }



    }

}



