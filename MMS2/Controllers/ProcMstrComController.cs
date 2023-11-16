using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class ProcMstrComController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1673;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            ProcMstrComMdl dd = ProcMstrComFun.ClearRecord();
            return View("ProcMstrComView", dd);
        }
        public JsonResult LoadListsVl(int DeptID = 0)
        {
            ProcMstrComMdl dd = ProcMstrComFun.LoadLists(DeptID);
            return Json(dd);
        }

        public JsonResult LoadProcDetailItems(int ProcID)
        {
            ProcMstrComMdl dd = ProcMstrComFun.GetProcDetailItems(ProcID);
            return Json(dd);

        }
        public JsonResult InsertItem(int ItemID, List<ItemInsertedList> itemlist)
        {
            List<ItemInsertedList> dd = ProcMstrComFun.InsertItem(ItemID, itemlist);
            return Json(dd);

        }

        public JsonResult Save(ProcMstrComMdl Order)
        {
            User UserData = (User)Session["User"];
            string MSG = "";
            int featureid = 1673;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                MSG = "You are not allowed to Save!";
                return Json(MSG);
            };
            MessageModel MSGResult = ProcMstrComFun.Save(Order, UserData);
            return Json(MSGResult);
        }

    }
}
