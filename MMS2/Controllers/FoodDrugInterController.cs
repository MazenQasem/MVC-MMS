using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class FoodDrugInterController : Controller
    {
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1091;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            DrugInterMdl dd = new DrugInterMdl();
            return View("FoodDrugInter", dd);
        }
        public JsonResult GetGenDtl(int GID, string Gname)
        {
            List<GenericDrugDetail> dd = FoodDrugInterFun.ViewGenDtl(GID, Gname);
            return Json(dd);
        }
        public JsonResult Save(List<GenericDrugDetail> DrugTable)
        {
            User UserData = (User)Session["User"];
            int featureid = 1038;
            int funactionid = 2;
            if (DrugTable[0].GenericID > 0)
            {
                funactionid = 3;             }
            MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Save!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };
            MessageModel it = FoodDrugInterFun.Save(DrugTable, UserData);
            return Json(it);
        }
                 public JsonResult GetGenDtlF(int GID)
        {
            DrugForumlary dd = FoodDrugInterFun.ViewGenDtlF(GID);
            return Json(dd);
        }

        public JsonResult DeleteF(DrugForumlary DrugTable)
        {
            User UserData = (User)Session["User"];
            int featureid = 1091;
            int funactionid = 4;             MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Delete!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };
            MessageModel it = FoodDrugInterFun.DeleteF(DrugTable, UserData);
            return Json(it);
        }

        public JsonResult SaveF(DrugForumlary DrugTable)
        {
            User UserData = (User)Session["User"];
            int featureid = 1091;
            int funactionid = 2;
            if (DrugTable.GenericID == 0)              {
                funactionid = 3;             }

            MessageModel ErrMsg = new MessageModel();
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                ErrMsg.Message = "You are not allowed to Update!";
                ErrMsg.isSuccess = false;
                return Json(ErrMsg);
            };
            MessageModel it = FoodDrugInterFun.SaveF(DrugTable, UserData);
            return Json(it);
        }


    }
}
