using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class AdjIssueController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1094;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            AdjIssueModel dd = AdjIssueFun.Clear();
            return View("AdjIssue", dd);
        }

        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;
            List<AdjView> dd = AdjIssueFun.GetAdjIssueList(Parm);
            return Json(dd);
        }

        public JsonResult ViewDetails(int AdjID)
        {
            User UserData = (User)Session["User"];
            List<AdjItemsInserted> it = AdjIssueFun.ViewDetails(AdjID);
            return Json(it);
        }


        public JsonResult InsertItem(int ItemID, List<AdjItemsInserted> SelectedItem, bool NewBatch = false)
        {

            User UserData = (User)Session["User"];
            List<AdjItemsInserted> it = new List<AdjItemsInserted> { };
            if (NewBatch == true)
            {
                it = AdjIssueFun.InsertItem(ItemID, UserData.selectedStationID, SelectedItem, true);
            }
            else
            {
                it = AdjIssueFun.InsertItem(ItemID, UserData.selectedStationID, SelectedItem, false);
            }
            return Json(it);
        }


        public JsonResult Save(AdjIssueModel Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 1094;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Edit!";
                return Json(Order);
            };



            AdjIssueModel it = AdjIssueFun.Save(Order, UserData);
            return Json(it);
        }


                                                                                          
                  
         
                                                                                                            
    }
}
