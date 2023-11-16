using System;
using System.Collections.Generic;
using System.Web.Mvc;
  
namespace MMS2.Controllers
{
    [MMCFilter]      public class MazenMainController : Controller
    {
                  

        public JsonResult GetNameOrVal(string Str, string fieldname)
        {
            User UserData = (User)Session["User"];
            string Im = MainFunction.GetName(Str, fieldname);
            if (!(Im.Length > 0))
            {
                Im = "0";
            }
            return Json(Im);

        }

        public JsonResult TwoColumnTable(string Str)
        {
            List<TempListMdl> Im = MainFunction.LoadComboList(Str, 0);
            return Json(Im);

        }

        public JsonResult LoadListByItem(string query, string page = "")
        {
            try
            {
                List<Selec2List> Im = MainFunction.LoadSelec2List(query);
                return Json(Im);
            }
            catch (Exception e)
            {
                return Json("Error found in search parameters!");

            }
        }
        public JsonResult GetLQuantity(int UnitID, int ItemID)
        {
            try
            {
                int Im = MainFunction.GetQuantity(UnitID, ItemID);
                return Json(Im);
            }
            catch (Exception e)
            {
                return Json("Error Count the qty!");

            }
        }


    }
}
