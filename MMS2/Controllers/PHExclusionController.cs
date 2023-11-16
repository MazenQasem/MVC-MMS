using System.Collections.Generic;
using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class PHExclusionController : Controller
    {
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1098;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            PHExclusionModel dd = PHExclusionFun.Clear(UserData.selectedStationID);
            return View("PHExclusion", dd);
        }

                  
                           
        public JsonResult InsertItem(string Str)
        {


            List<TempListMdl> it = PHExclusionFun.InsertItem(Str);
            if (it.Count == 0)
            {
                TempListMdl nx = new TempListMdl();
                nx.ErrMsg = "No Data Found";
                it.Add(nx);
            
            }
            return Json(it);
        }



        public JsonResult Save(PHExclusionModel Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 1098;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };


            PHExclusionModel it = PHExclusionFun.Save(Order, UserData);
            return Json(it);
        }



    }
}
