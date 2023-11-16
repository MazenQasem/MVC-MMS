using System;

namespace MMS2
{

    public class MessageModel
    {
        public String Message = "";
        public Boolean isSuccess = false;
        public String date;
        public int id = 0;
    }

    public partial class arrItem
    {

        
        public int ID { get; set; }
        public string BatchNo { get; set; }
        public string Batchid { get; set; }
        public decimal Qty { get; set; }
        public int DedQty { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal EPR { get; set; }

        public int UnitId { get; set; }
        public string ExpDt { get; set; }
        public int SubId { get; set; }


        
        public byte Period { get; set; }
        public byte UpdatedPeriod { get; set; }
        public int PresID { get; set; }
        public int ItemID { get; set; }

    }

    public class ParamTable
    {
        public string IssueNo { get; set; }
        public int stationid { get; set; }
        public string Sdate { get; set; }
        public string Edate { get; set; }

        public int gStationid { get; set; }
        
        public int IPID { get; set; }
        
        public string RecType { get; set; }         public int OrderID { get; set; }
        public string RegNo { get; set; }
        public int DocID { get; set; }
        public string Reason { get; set; }
        public string OtherType { get; set; }     }

    

}
