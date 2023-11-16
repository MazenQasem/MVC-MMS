using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class IndentOrderController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 71;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            IndentOrderModel dd = IndentOrderFun.ClearIndent();
            return View("IndentOrder", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;

            List<IndentView> dd = IndentOrderFun.GetIndents(Parm);
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
        public JsonResult InsertItem(int ItemID, int ToStationID, List<IndentInsertedItemList> SelectedItem)
        {

            User UserData = (User)Session["User"];
            List<IndentInsertedItemList> it = IndentOrderFun.InsertItem(ItemID, UserData.selectedStationID, ToStationID, SelectedItem);
            return Json(it);
        }
        public JsonResult Save(IndentOrderModel Order)
        {
            User UserData = (User)Session["User"];
            if (Order.OrderID == 0)
            {
                int featureid = 71;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Save!";
                    return Json(Order);
                };
            }
            else
            {
                int CheckStatus = MainFunction.NullToInteger(MainFunction.GetOneVal("Select isnull(uploaded,0) uploaded from Indent where Id=" + Order.OrderID, "uploaded").ToString());
                if (CheckStatus > 0)
                {
                    Order.ErrMsg = "Indent has already been sent to ERP!";
                    return Json(Order);
                }

                int featureid = 71;
                int funactionid = 3;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Edit!";
                    return Json(Order);
                };

            }

            IndentOrderModel it = IndentOrderFun.Save(Order, UserData);
            return Json(it);
        }
        public JsonResult ViewDetails(int OrderID)
        {
            User UserData = (User)Session["User"];
            List<IndentInsertedItemList> it = IndentOrderFun.ViewDetails(OrderID);
            return Json(it);
        }

        public ActionResult Print(int OrderID)
        {
            User UserData = (User)Session["User"];
            IndentOrderModel Order = IndentOrderFun.PrintOut(OrderID, UserData.selectedStationID);
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
