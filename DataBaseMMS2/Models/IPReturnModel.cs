using System.Collections.Generic;

namespace MMS2
{
    public class IPReturnHeader
    {
        public string ErrMsg { get; set; }
        public List<IPReturnViewList> ListView { get; set; }
        public List<IPReturnDetail> Details { get; set; }

        
        public string ReturnNo { get; set; }
        public string DateTime { get; set; }
        public string Bed { get; set; }
        public string Operator { get; set; }
        public bool VIP { get; set; }
        
        public int ReturnID { get; set; }
        public string gIACode { get; set; }
        public int Pin { get; set; }
        public int IPID { get; set; }
        public string PatientName { get; set; }
        public int DoctorID { get; set; }
        public int BedID { get; set; }
        public int CategoryID { get; set; }
        public decimal Total { get; set; }
        public string RegNo { get; set; }
    }
    public class IPReturnDetail
    {
        public int SNO { get; set; }
        public string Item { get; set; }
        public decimal QtyIss { get; set; }
        public string UOM { get; set; }
        public int TotRet { get; set; }
        public int Rtns { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string BatchNo { get; set; }
        public int UnitID { get; set; }
        public int BatchID { get; set; }
        public int OrderID { get; set; }
        public int ItemID { get; set; }

    }
    public class IPReturnViewList
    {
        public string ReturnNo { get; set; }
        public string DateTime { get; set; }
        public string Bed { get; set; }
        public string Operator { get; set; }
        public bool VIP { get; set; }
        
        public int ReturnID { get; set; }
        public string gIACode { get; set; }
        public int Pin { get; set; }
        public int IPID { get; set; }
        public string PatientName { get; set; }
        public int DoctorID { get; set; }
        public int BedID { get; set; }

        public string ErrMsg { get; set; }

    }


                                                                                                                        


     }
