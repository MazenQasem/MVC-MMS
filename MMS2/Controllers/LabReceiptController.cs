using System.Collections.Generic;
using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class LabReceiptController : Controller
    {
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 107;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            LabModel dd = LabReceiptFun.Clear();
            return View("LabReceipt", dd);
        }

        public JsonResult XView()
        {
            User UserData = (User)Session["User"];
            List<LabReceiptView> dd = LabReceiptFun.GetIssues();
            return Json(dd);
        }
        public JsonResult IssueList(string Str)
        {
            User UserData = (User)Session["User"];
            List<InitGrid> dd = LabReceiptFun.GetIssuesList(Str);
            return Json(dd);
        }


        public JsonResult GetItems(string Str)
        {


            List<ReceiptItems> it = LabReceiptFun.GetItems(Str);
            if (it.Count == 0)
            {
                ReceiptItems nx = new ReceiptItems();
                nx.ErrMsg = "No Data Found";
                it.Add(nx);

            }
            return Json(it);
        }
        
        public JsonResult Save(InitGrid Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 107;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };


            InitGrid it = LabReceiptFun.Save(Order, UserData);
            return Json(it);
        }

        


    }
}
