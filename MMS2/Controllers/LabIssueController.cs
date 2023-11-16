using System.Collections.Generic;
using System.Web.Mvc;


namespace MMS2.Controllers
{
    [MMCFilter]      public class LabIssueController : Controller
    {
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 106;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            LabModel dd = LabIssueFun.Clear();
            return View("LabIssue", dd);
        }
        public JsonResult XView()
        {
            User UserData = (User)Session["User"];
            List<InitGrid> dd = LabIssueFun.GetIssues();
            return Json(dd);
        }
        public JsonResult GetItems(string Str)
        {


            List<TempListMdl> it = LabIssueFun.GetItems(Str);
            if (it.Count == 0)
            {
                TempListMdl nx = new TempListMdl();
                nx.ErrMsg = "No Data Found";
                it.Add(nx);

            }
            return Json(it);
        }
        public JsonResult GetItemsDtl(string Str)
        {


            List<ItemDtl> it = LabIssueFun.GetItemsDtl(Str);
            if (it.Count == 0)
            {
                ItemDtl nx = new ItemDtl();
                nx.ErrMsg = "No Data Found";
                it.Add(nx);

            }
            return Json(it);
        }
        public JsonResult InsertItem(string Str)
        {
            User UserData = (User)Session["User"];
            List<ProfileItems> t = LabIssueFun.InsertIssueItem(Str,UserData.selectedStationID);
            return Json(t);
      
        }
       
        public JsonResult ViewDetails(int OrderID)
        {
            User UserData = (User)Session["User"];
            InitGrid it = LabIssueFun.ViewDetails(OrderID, UserData.selectedStationID);

            return Json(it);
        }

        public JsonResult Save(InitGrid Order)
        {
            User UserData = (User)Session["User"];

            int featureid = 106;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                Order.ErrMsg = "You are not allowed to Save!";
                return Json(Order);
            };


            InitGrid it = LabIssueFun.Save(Order, UserData);
            return Json(it);
        }

        


    }
}
