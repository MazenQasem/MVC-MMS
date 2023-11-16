using System.Collections.Generic;
 
namespace MMS2
{
    public class MedErrHeader
    {
        public int Registrationno { get; set; }
        public string txtPinNo { get; set; }
        public string txtRptNo { get; set; }
        public string txtPtName { get; set; }
        public string txtDateTime { get; set; }
        public string txtAge { get; set; }
        public string txtSex { get; set; }
        public string txtDrugAllergies { get; set; }
        public string txtAddress { get; set; }
        public string txtFoodAllergies { get; set; }
        public string RptDate { get; set; }
        public string txtDiagnosis { get; set; }
        public string DtpErrDate { get; set; }

        public bool UsedByPatient { get; set; }
        
        public int lstErrType { get; set; }
        public string txtErrType { get; set; }
        public string txtErrDescription { get; set; }
        public string txtIntervention { get; set; }
        public int lstDosage1 { get; set; }
        public string txtDosage1 { get; set; }
        public int lstDosage2 { get; set; }
        public string txtDosage2 { get; set; }
        public string txtErrPerputed { get; set; }
        public int lstErrDiscoverBy { get; set; }
        public string txtErrDiscoverBy { get; set; }
        public string txtHowDiscovered { get; set; }
        public int lstInitialErr { get; set; }
        public string txtInitialErr { get; set; }

        public int lstErrAction { get; set; }
        public string txtErrAction { get; set; }
        
        public bool isoptErrPerputed { get; set; }
        public string lstStage { get; set; }
        
        public int lstOutcome { get; set; }
        public string txtOutcome { get; set; }
        
        public string txtGenericName { get; set; }
        public string txtStrength { get; set; }
        public int lstRoute { get; set; }
        public string txtRoute { get; set; }

        public int lstErrMadeBy { get; set; }
        public string txtErrMadeBy { get; set; }
        public string DtpDateDiscover { get; set; }
        public string lstCauses { get; set; }
        public string txtRecommendation { get; set; }
        public string txtAction { get; set; }


        public List<MedErrShowList> showlist { get; set; }
        public int OperatorID { get; set; }
        public int ServiceID { get; set; }
        public string ErrMsg { get; set; }
    }
    public class MedErrShowList
    {
                          public int SNO { get; set; }
        public int OrderID { get; set; }
        public string PIN { get; set; }
        public string PTName { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public string Date { get; set; }
        public string Diagnosis { get; set; }
        public string EventDate { get; set; }
        public int IPID { get; set; }
        public int OperatorID { get; set; }
    }

}