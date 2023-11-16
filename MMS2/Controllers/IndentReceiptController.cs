using System.Collections.Generic;
using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class IndentReceiptController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 81;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            IndentOrderModel dd = IndentReceiptFun.ClearIndent(UserData.selectedStationID);
            dd.CurrentStation = UserData.selectedStationID;
            dd.lbloperator = UserData.Name;
            return View("IndentReceipt", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<ReceiptIndentView> dd = IndentReceiptFun.GetIndents(Parm);
            return Json(dd);
        }
        public JsonResult ViewDetails(int OrderID, int IssueNo, int Status)
        {
            List<IndentReceiptDtl> it = IndentReceiptFun.ViewDetails(OrderID, IssueNo, Status);
            return Json(it);
        }
        public JsonResult Save(IndentOrderModel Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 81;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };


            IndentOrderModel it = IndentReceiptFun.Save(Order, UserData);
            return Json(it);
        }

    }
}
