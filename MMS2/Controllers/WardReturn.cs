using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class WardReturnController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 85;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            WardReturnHeader dd = new WardReturnHeader();
            return View("WardReturnView", dd);
        }

        public JsonResult ViewList()
        {
            User UserData = (User)Session["User"];
            int gStationid = UserData.selectedStationID;
            WardReturnHeader dd = WardReturnFun.View(gStationid);
            return Json(dd.ListView);
        }

        public JsonResult ViewDetails(int OrderID, int Status)
        {
            User UserData = (User)Session["User"];
            int gStationid = UserData.selectedStationID;
            WardReturnHeader dd = WardReturnFun.GetDetails(OrderID, Status);
            return Json(dd);
        }

        public JsonResult Save(WardReturnHeader Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 85;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };

            WardReturnHeader it = WardReturnFun.Save(Order, UserData);
            return Json(it);


        }


                                             

                                                      
         
        
                                                                                                            
    }
}
