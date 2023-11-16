using System;
using System.Collections.Generic;

namespace MMS2
{

    
    public partial class ParamDirectIpIssuesModel
    {
      
        public int PinNo { get; set; }
        public List<PinList> PinList { get; set; }

        public string Patient { get; set; }
        public string OrderNo { get; set; }
        public int Age { get; set; }
        public int Sex { get; set; }
        public string BedName { get; set; }
        public string Bed { get; set; }
        public List<BedList> BedList { get; set; }

        public DateTime IssueDateTime { get; set; }
        public string Allergies { get; set; }
        public string Operator { get; set; }

        public int DoctorId { get; set; }
        public int CategoryId { get; set; }

        public List<DoctorList> DoctorList { get; set; }
        public List<CategoryList> CategoryList { get; set; }

        public string ItemCode { get; set; }
        public bool IsCodeOrName { get; set; }


    }


    public partial class DoctorList
    { 
            public int Id { get; set; }
            public string DeptCode { get; set; }
            public string Name { get; set; }
            public string AccountCode { get; set; }
    }

    public class CategoryList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }


    public class PinList
    {
        public int Id { get; set; }
        public int IPId { get; set; }
        public string RegNo { get; set; }
        public string Name { get; set; }
 
    }

    public class BedList
    {
        public int Id { get; set; }
        public string Name { get; set; }

    }

    public class DirectIpIssueList
    {
        public int Id { get; set; }
        public int StationSLno { get; set; }
        public string PatientName { get; set; }
        public string IpNo { get; set; }
        public string Bed { get; set; }
        public string Doctor { get; set; }
        public DateTime DateTime { get; set; }
        public string Operator { get; set; }

    }


    public partial class InformationDatalist
    {
         public int IpId { get; set; }
         public string Name { get; set; }
         public int Sex { get; set; }
         public int Age { get; set; }
         public string SexOthers { get; set; }
         public int DoctorId { get; set; }
         public string BedName { get; set; }
         public int BedId { get; set; }
         public string Vip { get; set; }
         public string AlertMsg { get; set; }
    }

    public partial class DirectIpSaveModel
    {
                 public List<IndentIssueInsertedItemListDirectIp> IssueList { get; set; }
        public string IpId { get; set; }
        public int ToStationID { get; set; }
        public string OperatorId { get; set; }
        public int DocID { get; set; }
        public string DateTime { get; set; }
        public int BedId { get; set; }
        public int ProfileId { get; set; }
        public string DispatchedDateTime { get; set; }
        public string Dispatched { get; set; }
        public int StationSlNo { get; set; }
        public int ordertype { get; set; }
        public int printStatus { get; set; }
        public int CssDItem { get; set; }

        public int OrderID { get; set; }
        public string ErrMsg { get; set; }
        public List<arrItemDirectIp> arrItem { get; set; }
        public int CatId { get; set; }

        public string BedName { get; set; }
        public string OperatorName { get; set; }
        public int Sex { get; set; }
        public int Age { get; set; }
        public string DoctorName { get; set; }
        public string PinNo { get; set; }
        public string PatientName { get; set; }
        public string Gender { get; set; }




                        
                               
               
                                        
                        
                                        
                        
                                                                                                                                                                                                    }

    public partial class arrItemDirectIp
    {
        
                                                                                                   
        
        public int ID { get; set; }          public string BatchNo { get; set; }            public string Batchid { get; set; }            public decimal Qty { get; set; }            public int DedQty { get; set; }            public int UnitId { get; set; }            public int Tax { get; set; }            public decimal mrp { get; set; }           public decimal cprice { get; set; }           public decimal TotalAmount { get; set; }           public decimal Price { get; set; }         public string Remarks { get; set; }                   public decimal BillablePrice { get; set; }         public decimal BillableQty { get; set; }         public int BillableUnitId { get; set; }                                                  }


    public partial class IndentIssueInsertedItemListDirectIp
    {

        
        public int SNO { get; set; }
        public int ID { get; set; }
        public string Name { get; set; }
        public int BatchId { get; set; }
        public int DrugType { get; set; }          public int conversionqty { get; set; }
        public int tax { get; set; }
        public double mrp { get; set; }
        public double sellingprice { get; set; }
        public decimal quantity { get; set; }
        public int unitId { get; set; }
        public string Unit { get; set; }
        public string BatchNo { get; set; }
        public DateTime expirydate { get; set; }
        public string itemcode { get; set; }
        public Boolean CSSDItem { get; set; }
        public int BillQty { get; set; }
        public string BillUnit { get; set; }
        public string Remarks { get; set; }

        public int IpId { get; set; }

        
       
        public string Batch { get; set; }
        public string Expiry { get; set; }
        public double QOH { get; set; }
        public int IssueUnitID { get; set; }
        public string Item { get; set; }

        public double QtyIssue { get; set; }         public string IssueUnits { get; set; }         public int ItemID { get; set; }         public int substituteid { get; set; }
      

        public int lstsub { get; set; }         
       
        public int PrevQty { get; set; }
        public int OrderQty { get; set; }

        public int PrevUnitID { get; set; }         public int IndentUnitID { get; set; }         public int batchQty { get; set; }          public int totqty { get; set; }         public int MinLevel { get; set; } 

        
        public string Action { get; set; }         public string ErrMsg { get; set; }         public bool ForEdit { get; set; } 
        
        public int lLargeQty { get; set; }
        public int lSmallOrderQty { get; set; }
        public Single Price { get; set; }

        
        public Single Amount { get; set; }


        


       
    }



}
