using System;
using System.Collections.Generic;


namespace MMS2
{
    public partial class OPLOAOrder
    {
        public string TxtRegNumber { get; set; }
        public List<TempListMdl> AuthorityLIST { get; set; }
        public String CategoryName { get; set; }
        public String CompanyName { get; set; }
        public string GradeName { get; set; }
        public string DoctorName { get; set; }
        public decimal PrvLoa { get; set; }
        public decimal PrvPhAmount { get; set; }
        public decimal ConsumedPhAmount { get; set; }
        public decimal ConsumedLoaAmount { get; set; }

        public int PrvDays { get; set; }
        public String StrLoaDateTime { get; set; }
        public String ApprovalNo { get; set; }
        public String Notes { get; set; }
        public String ErrMsg { get; set; }

        public Boolean BtnUpdateAllowed { get; set; }
    }
    
}
