using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class PharmacyMSGController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1086;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            DrugAllergyMdl dd = new DrugAllergyMdl();
            return View("PharmacyMSG", dd);
        }
        public JsonResult View(int PIN)
        {
            User UserData = (User)Session["User"];
            DrugAllergyMdl dd = PharmacyMSGFun.ViewDetail(PIN, UserData);
            return Json(dd);
        }

        public JsonResult Save(DrugAllergyMdl DrugAllergyT)
        {
            User UserData = (User)Session["User"];
            int featureid = 1086;
            int funactionid = 2;
            MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Save!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };


            MessageModel it = PharmacyMSGFun.Save(DrugAllergyT, UserData);
            return Json(it);
        }





    }
}
