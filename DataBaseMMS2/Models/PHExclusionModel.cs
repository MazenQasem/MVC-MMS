using System;
using System.Collections.Generic;

namespace MMS2
{
    public class PHExclusionModel
    {
        public String OPIP { get; set; }
        public int CompanyID { get; set; }
        public int CategoryID { get; set; }
        public int GradeID { get; set; }
        public List<TempListMdl> ItemList { get; set; }
        public string ErrMsg { get; set; }
    }
}



 