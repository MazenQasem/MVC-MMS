using System.Collections.Generic;

namespace MMS2
{
    public partial class AdjIssueModel
    {
        public List<TempListMdl> StationList { get; set; }
        public List<AdjView> ViewList { get; set; }

        public int StationID { get; set; }
        public int lblStation { get; set; }
        public int ReceiptID { get; set; }
        public string lbldate { get; set; }
        public string lblOperator { get; set; }
        public string txtRefNo { get; set; }
        public string lblNo { get; set; }
        public List<AdjItemsInserted> SelectedItem { get; set; }
        public string ErrMsg { get; set; }
        public int IssueTo { get; set; }
    }

     
                                             

                                                                                                                        

}



