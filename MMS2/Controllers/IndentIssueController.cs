using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class IndentIssueController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 75;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            IndentOrderModel dd = IndentIssueFun.ClearIndent();
            return View("IndentIssue", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<IndentIssueView> dd = IndentIssueFun.GetIndents(Parm);
            return Json(dd);
        }
        public JsonResult LoadList(String Str, String Order = "", String GroupBy = "", bool WithStationID = false)
        {

            User UserData = (User)Session["User"];
            if (WithStationID == true)
            {
                Str = Str + UserData.selectedStationID + GroupBy + Order;
            }
            else
            {
                Str = Str + GroupBy + Order;
            }
            List<TempListMdl> it = MainFunction.LoadComboList(Str, 0);

            return Json(it);
        }
        public JsonResult ViewDetails(int OrderID,int status)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel it = IndentIssueFun.ViewDetails(OrderID, UserData.selectedStationID, status);

            return Json(it);
        }
        public JsonResult InsertItem(int ItemID, List<IndentIssueInsertedItemList> SelectedItem,int IndentID,int Generic)
        {

            User UserData = (User)Session["User"];
            List<IndentIssueInsertedItemList> it = IndentIssueFun.InsertItem(ItemID, UserData.selectedStationID, SelectedItem, IndentID, Generic);
            return Json(it);
        }
        public JsonResult ChangQtyUOM(IndentIssueInsertedItemList ItemsList)
        {
            User UserData = (User)Session["User"];
            IndentIssueInsertedItemList it = IndentIssueFun.ChangeQty(ItemsList);
            return Json(it);

        }

        public JsonResult Save(IndentOrderModel Order)
        {
            User UserData = (User)Session["User"];
            
                int featureid = 75;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Save!";
                    return Json(Order);
                };
            

            IndentOrderModel it = IndentIssueFun.Save(Order, UserData);
            return Json(it);
        }

        public JsonResult GetQty(int unitid,int itemid)
        {
            User UserData = (User)Session["User"];
            int it = MainFunction.GetQuantity(unitid,itemid);
            return Json(it);

        }

        public ActionResult Print(int OrderID)
        {
            User UserData = (User)Session["User"];
            IndentIssueView Order = IndentIssueFun.PrintOut(OrderID, UserData.selectedStationID);
            if (Request.IsAjaxRequest())
            {
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
