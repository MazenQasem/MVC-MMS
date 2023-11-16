using System.Collections.Generic;

namespace MMS2
{
    public class WardReturnHeader
    {
        public string ErrMsg { get; set; }
        public List<WardReturnViewList> ListView { get; set; }
        public List<WardReturnDetail> Details { get; set; }

        
        public int OrderNO { get; set; }
        public string Pin { get; set; }
        public string Patient { get; set; }
        public string Bed { get; set; }
        public string Station { get; set; }

        public string DateTime { get; set; }
        public string ReturnBy { get; set; }
        public byte Status { get; set; }
        public int SlNO { get; set; }
        public bool VIP { get; set; }


        public string Sex { get; set; }
        public string Age { get; set; }
        public decimal Total { get; set; }


    }
    public class WardReturnDetail
    {
                          public int SNO { get; set; }
        public string Item { get; set; }
        public string BatchNo { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public decimal Total { get; set; }
        public decimal RetQty { get; set; }
        public string Remarks { get; set; }

        public string UOM { get; set; }

        public decimal CostPrice { get; set; }         public int ItemID { get; set; }
        public int BatchID { get; set; }         public string ErrMsg { get; set; }
    }
    public class WardReturnViewList
    {
                          public int OrderNO { get; set; }
        public string Pin { get; set; }
        public string Patient { get; set; }
        public string Bed { get; set; }
        public string Station { get; set; }

        public string DateTime { get; set; }
        public string ReturnBy { get; set; }
        public byte Status { get; set; }
        public int SlNO { get; set; }
        public bool VIP { get; set; }

        public string Sex { get; set; }
        public string Age { get; set; }
        public string ErrMsg { get; set; }

    }

    public class RptWardReturnHeader
    {
        public int IPID { get; set; }
        public string Pin { get; set; }
        public string Name { get; set; }
        public string Station { get; set; }
        public string Bed { get; set; }
        public string Ward { get; set; }
        public string Slno { get; set; }
        public string DateTime { get; set; }
        public string Doctor { get; set; }
        public int OrderNo { get; set; }
        public List<RptWardReturnDetail> dtl { get; set; }
    }
    public class RptWardReturnDetail
    {
        public int SNO { get; set; }
        public string ItemName { get; set; }
        public int Qty { get; set; }
        public string BatchNo { get; set; }
        public decimal price { get; set; }
        public int BatchID { get; set; }
        public decimal TotalPrice { get; set; }
        public string ExpiryDate { get; set; }



    }
}