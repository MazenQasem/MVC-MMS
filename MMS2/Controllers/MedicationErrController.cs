using System.Web.Mvc;

namespace MMS2.Controllers
{
    public class MedicationErrController : Controller
    {
                          [MMCFilter]          public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1098;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            MedErrHeader n = new MedErrHeader();
            return View(n);
        }
        public JsonResult ViewList(string Query)
        {
            MedErrHeader n = MedicationErrorFun.LoadList(Query);
            return Json(n);
        }
        public JsonResult GetPatientInformation(string Query)
        {
            MedErrHeader n = MedicationErrorFun.PatientData(Query);
            return Json(n);
        }
        public JsonResult GetPatientInformationDetail(string Query)
        {
            User Users = (User)Session["User"];
            MedErrHeader n = MedicationErrorFun.PatientDataDetail(Query);
            
            n.OperatorID = Users.EmpID;
            return Json(n);
        }
        public JsonResult Save(MedErrHeader Order,int OID)
        {
            User UserData = (User)Session["User"];

            int featureid = 1098;              int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order.ErrMsg);
            };
            string Dd = "";
            if (OID == 0)              {
                 Dd = MedicationErrorFun.Saved(Order, UserData);
            }
            else
            {
                Dd = MedicationErrorFun.Update(Order, UserData,OID);

            }
            return Json(Dd);


        }
    }
}
