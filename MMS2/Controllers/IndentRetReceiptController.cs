using System.Collections.Generic;
using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class IndentRetReceiptController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 82;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            IndentOrderModel dd = IndentRetReceiptFun.ClearIndent(UserData.selectedStationID);
            dd.CurrentStation = UserData.selectedStationID;
            dd.lbloperator = UserData.Name;
            return View("IndentRetReceipt", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<ReturnReceiptIndentView> dd = IndentRetReceiptFun.GetIndents(Parm);
            return Json(dd);
        }

        public JsonResult ViewDetails(int OrderID, int Status)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel it = IndentRetReceiptFun.ViewDetails(OrderID, UserData.selectedStationID,UserData.Name, Status);
            return Json(it);
        }

        public JsonResult Save(IndentOrderModel Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 82;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };


            IndentOrderModel it = IndentRetReceiptFun.Save(Order, UserData);
            return Json(it);
        }

    }
}
