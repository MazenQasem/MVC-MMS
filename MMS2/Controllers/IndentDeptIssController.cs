using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class IndentDeptIssController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 76;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            IndentOrderModel dd = IndentDeptIssFun.ClearIndent(UserData.selectedStationID);
            dd.CurrentStation = UserData.selectedStationID;
            return View("IndentDeptIss", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<IndentDeptView> dd = IndentDeptIssFun.GetIndents(Parm);
            return Json(dd);
        }
        public JsonResult ViewDetails(int OrderID, int status)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel it = IndentDeptIssFun.ViewDetails(OrderID, status, UserData.selectedStationID);
            return Json(it);
        }

        public JsonResult InsertItem(int ItemID, int ToStationID, int IssueID, List<IndentIssueInsertedItemList> IssueList)
        {

            User UserData = (User)Session["User"];
            List<IndentIssueInsertedItemList> it = IndentDeptIssFun.InsertItem(ItemID, UserData.selectedStationID, ToStationID, IssueID, IssueList);
            return Json(it);
        }
        public JsonResult ChangQtyUOM(IndentIssueInsertedItemList ItemsList)
        {
            User UserData = (User)Session["User"];
            IndentIssueInsertedItemList it = IndentDeptIssFun.ChangeQty(ItemsList);
            return Json(it);

        }
        public JsonResult Save(IndentOrderModel Order)
        {
            User UserData = (User)Session["User"];
            if (Order.OrderID == 0)
            {
                int featureid = 76;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Save!";
                    return Json(Order);
                };
            }
            else
            {
                int featureid = 76;
                int funactionid = 4;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Edit!";
                    return Json(Order);
                };

            }

            IndentOrderModel it = IndentDeptIssFun.Save(Order, UserData);
            return Json(it);
        }


        public ActionResult Print(int OrderID)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel Order = IndentDeptIssFun.PrintOut(OrderID, UserData.selectedStationID);
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
