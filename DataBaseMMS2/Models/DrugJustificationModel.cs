using System.Collections.Generic;

namespace MMS2
{
    public class DrugJustificationModel
    {
        public List<DetailsView> Details { get; set; }
        public List<TempListMdl> Tlist { get; set; }
        public int SelectedListID { get; set; }
    }
    public partial class DetailsView
    {
        public string OrderNo { get; set; }
        public int ServiceID { get; set; }
        public string PIN { get; set; }
        public string Station { get; set; }
        public string DrugName { get; set; }
        public string DoctorJustification { get; set; }
        public string Acknowledgement { get; set; }
        public string AcknowRemarks { get; set; }
        public string OrderdDoctor { get; set; }
        public string NurseName { get; set; }
        
        public int OrderID { get; set; }
        public int Accepted { get; set; }
        public int MainAccepted { get; set; }
        public string ErrMsg { get; set; }
                                                                                                                         }
}



