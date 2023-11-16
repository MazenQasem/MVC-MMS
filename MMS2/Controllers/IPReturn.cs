using System.Web.Mvc;



namespace MMS2.Controllers
{
    [MMCFilter]      public class IPReturnController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 100;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }



            IPReturnHeader dd = new IPReturnHeader();
            dd.gIACode = UserData.gIACode;
            return View("IPReturnView", dd);
        }
        public JsonResult ViewList(int PIN)
        {
            User userdata = (User)Session["User"];
            IPReturnHeader nn = IPReturnFun.FindPatient(PIN, userdata.StationName, userdata.gIACode);
            return Json(nn);
        }

        public JsonResult ViewDetails(int OrderID, int IPID)
        {
            IPReturnHeader ip = IPReturnFun.ViewDetailsFun(OrderID, IPID);
            return Json(ip);
        }

        public JsonResult NewIP(int PIN)
        {
            IPReturnHeader ip = IPReturnFun.GetNewPT(PIN);
            return Json(ip);
        }

        public JsonResult InsertItem(int ItemID, int IPID)
        {
            User UserData = (User)Session["User"];
            IPReturnHeader ip = IPReturnFun.InsertItem(ItemID, IPID, UserData.selectedStationID);
            return Json(ip);
        }


        public JsonResult Save(IPReturnHeader Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 100;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };

            IPReturnHeader it = IPReturnFun.Save(Order, UserData);
            return Json(it);


        }

    }
}
