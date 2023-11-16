using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class UnitDoseBulkController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 982;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            UnitDoseBulkModel dd = UnitDoseBulkFun.Clear(UserData.selectedStationID, UserData.ModuleID,
                MainFunction.NullToInteger(UserData.gHospitalBranchCode));
            return View("UnitDoseBulk", dd);
        }
        public JsonResult GetDateList(int StationID, string dtpDate)
        {
            List<TempListMdl> ll = new List<TempListMdl>();
            try
            {
                ll = MainFunction.LoadComboList("Select distinct bulkprocessdate as Name,0 as ID from drugorder where bulkprocessdate >= '"
                + MainFunction.DateFormat(dtpDate, "dd", "MMM", "yyyy") + "' and stationid = " + StationID
                + " order by bulkprocessdate");
                return Json(ll);
            }
            catch (Exception) { return Json("Not Found"); }


        }
        public JsonResult ProcessBulk(string dtpDate, int StationID, int PHID, int AssPHID, string bulkDate)
        {
            try
            {

                User UserData = (User)Session["User"];

                int featureid = 982;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    return Json("You are not allowed to Process Bulk!");
                };

                string Processed = UnitDoseBulkFun.ProcessingBulk(dtpDate, StationID, PHID, AssPHID, bulkDate, UserData);
                return Json(Processed);
            }
            catch (Exception) { return Json("Error when try to Process the bulk !"); }
        }

    }
}
