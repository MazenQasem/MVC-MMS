namespace MMS2
{
    public class DrugInterMdl
    {
        public string ErrMsg { get; set; }
    }

    public class GenericDrugDetail
    {
        public int Seq { get; set; }
        public int GenericID { get; set; }
        public string Generic { get; set; }
        public int DrugID { get; set; }
        public string Drug { get; set; }
        public string Reaction { get; set; }
    }
    public class DrugForumlary {
        public int GenericID { get; set; }
        public string GenericName { get; set; }
        public string BrandName { get; set; }
        public string Category { get; set; }
        public string Use { get; set; }
        public string Indication { get; set; }
        public string Warning { get; set; }
        public string Reaction { get; set; }
        public string Mechanism { get; set; }
        public string Dosage { get; set; }
        public string DosageForms { get; set; }
        public string Remarks { get; set; }
    
    
    }

}



