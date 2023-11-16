using System.Collections.Generic;

namespace MMS2
{
    public class DrugAllergyMdl
    {
        public string PIN { get; set; }
        public string PTName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string DateTime { get; set; }
        public string PinNo { get; set; }
        public string OperatorName { get; set; }
        public string othertxt { get; set; }
        public List<TempListMdl> DrugList { get; set; }
        public List<PHMsgTable> PHmsgList { get; set; }
        public string ErrMsg { get; set; }
    }

    public class PHMsgTable
    {
        public string msg { get; set; }
        public string Station { get; set; }
        public string ID { get; set; }
        public string Operator { get; set; }
        public string deleted { get; set; }
   
    }
}
