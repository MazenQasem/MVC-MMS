using System.Collections.Generic;
using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class LabPreprationController : Controller
    {
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 105;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            LabModel dd = LabPreprationFun.Clear(UserData.selectedStationID);
            return View("LabPrepration", dd);
        }

        public JsonResult ViewDetails(int ProID)
        {
            
            List<ProfileItems> it = LabPreprationFun.InsertItem(ProID);
            return Json(it);
        }

        public JsonResult InsertItem(int ItemID, List<ProfileItems> ItemList)
        {

            User UserData = (User)Session["User"];
            List<ProfileItems> it = LabPreprationFun.InsertItem(ItemID, ItemList);
            return Json(it);
        }



        public JsonResult Save(LabModel Order)
        {
            User UserData = (User)Session["User"];
          
                int featureid = 105;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Save!";
                    return Json(Order);
                };
                funactionid = 3;                  if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Edit!";
                    return Json(Order);
                };
          
            LabModel it = LabPreprationFun.Save(Order, UserData);
            return Json(it);
        }



    }
}
