using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class IndentReturnController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 73;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            IndentOrderModel dd = IndentReturnFun.ClearIndent(UserData.selectedStationID);
            dd.CurrentStation = UserData.selectedStationID;
            return View("IndentReturn", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<IndentView> dd = IndentReturnFun.GetIndents(Parm);
            return Json(dd);
        }
        public JsonResult ViewDetails(int OrderID)
        {
            List<IndentReturnedItemList> it = IndentReturnFun.ViewDetails(OrderID);
            return Json(it);
        }
        public JsonResult GetIssueInfo(int issSlno)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel ll = IndentReturnFun.FindIssue(issSlno, UserData.selectedStationID);
            return Json(ll);
        }
        public JsonResult InsertItem(int ItemID, int ToStationID, int IssueID, List<IndentReturnedItemList> SelectedItem)
        {

            User UserData = (User)Session["User"];
            List<IndentReturnedItemList> it = IndentReturnFun.InsertItem(ItemID, UserData.selectedStationID, ToStationID, IssueID, SelectedItem);
            return Json(it);
        }
        public JsonResult Save(IndentOrderModel Order)
        {
            User UserData = (User)Session["User"];
            if (Order.OrderID == 0)
            {
                int featureid = 73;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Save!";
                    return Json(Order);
                };
            }
            else
            {
                int featureid = 73;
                int funactionid = 4;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Edit!";
                    return Json(Order);
                };

            }

            IndentOrderModel it = IndentReturnFun.Save(Order, UserData);
            return Json(it);
        }
        public ActionResult Print(int OrderID)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel Order = IndentReturnFun.PrintOut(OrderID, UserData.selectedStationID);
            if (Request.IsAjaxRequest())
            {
                Order.CurrentStationName = MainFunction.GetName("select top 1 Name from station where id=" + UserData.selectedStationID + "", "Name");
                return Json(new { htmlData = RenderRazorViewToString("ReportViewerHtml", Order, ControllerContext, ViewData, TempData) },
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
