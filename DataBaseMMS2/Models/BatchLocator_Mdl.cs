
using System;
using System.Collections.Generic;

namespace MMS2
{
    public partial class BatchLocator
    {
        public List<TransactionType> TransTypeLIst { get; set; }
        public int HoldingTransTypeListID_Add { get; set; }

        public List<ListBox> TransNoList { get; set; }
        public int HoldingTransNo_Add { get; set; }

        public List<ListBox> Racklist { get; set; }
        public int HoldingRackListID_Add { get; set; }

        public List<ListBox> Shelflist { get; set; }
        public int HoldingShelfListID_Add { get; set; }

                 public int ID { get; set; }
        public int SNO { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Rack { get; set; }
        public string Shelf { get; set; }
        public string expdate { get; set; }
        public int Qty { get; set; }
        public Nullable<decimal> Cost { get; set; }
        public Nullable<decimal> SelPrice { get; set; }
        public List<BatchLocator> BatchLocatorList { get; set; }
        public int receiptid { get; set; }
        public string ErrMsg { get; set; }
    }

    public class ListBox
    {
        public int ID { get; set; }
        public String NAME { get; set; }

    }

}