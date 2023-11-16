using System.Web.Mvc;
using System.IO;

namespace MMS2.Controllers
{
    [MMCFilter]      public class DrugAllergyController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1091;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            DrugAllergyMdl dd = new DrugAllergyMdl();
            return View("DrugAllergy", dd);
        }
        public JsonResult View(int PIN)
        {
            User UserData = (User)Session["User"];
            DrugAllergyMdl dd = DrugAllergyFun.ViewDetail(PIN, UserData);
            return Json(dd);
        }

        public JsonResult Save(DrugAllergyMdl DrugAllergyT)
        {
            User UserData = (User)Session["User"];
            int featureid = 1091;
            int funactionid = 2;
            MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Save!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };


            MessageModel it = DrugAllergyFun.Save(DrugAllergyT, UserData);
            return Json(it);
        }


        public ActionResult Print(string OrderID)
        {
            User UserData = (User)Session["User"];
            DrugAllergyMdl Order = DrugAllergyFun.PrintOut(OrderID, UserData);
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
