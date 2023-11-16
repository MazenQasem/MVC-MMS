using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class MSFIssueController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1922;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            MSFIssueMdl dd = new MSFIssueMdl();
            return View("MSFIssue", dd);
        }
        public JsonResult View(ParamTable Parm)
        {
            User UserData = (User)Session["User"];
            Parm.gStationid = UserData.selectedStationID;
            List<ViewList> dd = MSFIssueFun.GetList(Parm);
            return Json(dd);
        }
        public JsonResult ViewDetails(ParamTable param)
        {
            User UserData = (User)Session["User"];
            param.gStationid = UserData.selectedStationID;
            MSFIssueMdl it = new MSFIssueMdl();
            if (param.OrderID == 0)             {
                it = MSFIssueFun.GetVisitDate(param);
            }
            else
            {
                it = MSFIssueFun.ViewDetails(param);
            }
            return Json(it);
        }
        public JsonResult NewRecord(ParamTable param)
        {
            User UserData = (User)Session["User"];
            param.gStationid = UserData.selectedStationID;
            MSFIssueMdl it = MSFIssueFun.NewOrder(param);
            return Json(it);
        }
        public JsonResult ItemInsert(List<InsertItems> ItemList, int ItemID)
        {
            User UserData = (User)Session["User"];
            List<InsertItems> it = MSFIssueFun.InsertItem(ItemList, ItemID, UserData.selectedStationID);
            return Json(it);
        }
        public JsonResult GetQty(InsertItems ItemRow)
        {
            InsertItems IRow = MSFIssueFun.GetQtyItem(ItemRow);

            return Json(IRow);
        }
        public JsonResult Save(MSFIssueMdl MainTbl)
        {
            User UserData = (User)Session["User"];

            int featureid = 1922;
            int funactionid = 2;
            MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Save!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };


            MessageModel it = MSFIssueFun.Save(MainTbl, UserData);
            return Json(it);
        }
        public JsonResult DeleteOrder(ParamTable param)
        {
            User UserData = (User)Session["User"];

            int featureid = 1922;
            int funactionid = 2;
            MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Delete!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };
            MessageModel it = MSFIssueFun.DeleteOrderD(param, UserData);
            return Json(it);
        }

        public ActionResult Print(List<ParamTable> OrderID)
        {
            User UserData = (User)Session["User"];
            List<MSFreportHeader> Order = MSFIssueFun.PrintOut(OrderID, UserData);
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ReportViewerHtml", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(Order);

        }
        public ActionResult PrintAll(ParamTable Param)
        {
            User UserData = (User)Session["User"];
            List<MSFPrintList> PList = new List<MSFPrintList>();

            PList = MSFIssueFun.GetPrintList(Param, UserData);
            return Json(PList);
        
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
