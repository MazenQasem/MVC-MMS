using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class AdjReceiptController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 94;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            AdjReceiptModel dd = AdjReceiptFun.ClearIndent();
            return View("AdjReceipt", dd);
        }

        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;
            List<AdjView> dd = AdjReceiptFun.GetAdjRecList(Parm);
            return Json(dd);
        }

        public JsonResult ViewDetails(int AdjID)
        {
            User UserData = (User)Session["User"];
            List<AdjItemsInserted> it = AdjReceiptFun.ViewDetails(AdjID);
            return Json(it);
        }


        public JsonResult InsertItem(int ItemID, List<AdjItemsInserted> SelectedItem, bool NewBatch = false)
        {

            User UserData = (User)Session["User"];
            List<AdjItemsInserted> it = new List<AdjItemsInserted> { };
            if (NewBatch == true)
            {
                it = AdjReceiptFun.InsertItem(ItemID, UserData.selectedStationID, SelectedItem, true);
            }
            else
            {
                it = AdjReceiptFun.InsertItem(ItemID, UserData.selectedStationID, SelectedItem, false);
            }
            return Json(it);
        }
        public JsonResult Save(AdjReceiptModel Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 94;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Edit!";
                return Json(Order);
            };



            AdjReceiptModel it = AdjReceiptFun.Save(Order, UserData);
            return Json(it);
        }


                                                                                          
                  
         
                                                                                                            
    }
}
