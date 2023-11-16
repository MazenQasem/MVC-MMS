using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class DrugOrderIssueController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 84;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            DrugOrderIssueModel dd = DrugOrderIssueFun.ClearIndent(UserData.selectedStationID, UserData.ModuleID,
               MainFunction.NullToInteger(UserData.gHospitalBranchCode));
            return View("DrugOrderIssue", dd);
        }

        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<DrugOrderIssueView> dd = DrugOrderIssueFun.GetOrders(Parm);
            return Json(dd);
        }

        public JsonResult ViewDetails(int OrderID)
        {
            User UserData = (User)Session["User"];
            DrugOrderIssueModel it = DrugOrderIssueFun.ViewDetails(OrderID, UserData.selectedStationID);
            return Json(it);
        }

        public JsonResult SubItemList(int ItemID)
        {
            User UserData = (User)Session["User"];
            DrugSubList it = DrugOrderIssueFun.GetSubListDetail(ItemID, UserData.selectedStationID);
            return Json(it);
        }

        public JsonResult Save(DrugOrderIssueModel Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 84;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };
            DrugOrderIssueModel it = DrugOrderIssueFun.Save(Order, UserData);
            return Json(it);
        }

        public ActionResult Print(int OrderID)
        {
            User UserData = (User)Session["User"];

            DrugOrderPrintView Order = DrugOrderIssueFun.PrintOut(OrderID, UserData.selectedStationID);
            if (Request.IsAjaxRequest())
            {
                                 return Json(new { htmlData = RenderRazorViewToString("ReportViewerHtml", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(Order);
             
        }
        public ActionResult Printlbl(int OrderID)
        {
            User UserData = (User)Session["User"];

                         DrugOrderPrintView Order = new DrugOrderPrintView();
            Order.mOrderID = OrderID.ToString();
            Order.StationID = UserData.selectedStationID.ToString();

            if (Request.IsAjaxRequest())
            {
                                 return Json(new { htmlData = RenderRazorViewToString("ReportLabelHtml", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(Order);
             
        }
        public static string RenderRazorViewToString(string viewName, object model, ControllerContext controllerContext, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            viewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }


    }
}
