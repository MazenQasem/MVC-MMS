using System;
using System.Collections.Generic;
using System.Web.Mvc;
  
namespace MMS2.Controllers
{
    [MMCFilter]      public class RolRecalcuationController : Controller
    {
            public ActionResult Index(String ErrMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 99;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
                                 ViewBag.ErrorMsg = ErrMsg;
                ErrMsg = null;
                ItemAvgSale AT = new ItemAvgSale();
                AT = RolReCalculationFun.LoadItem();
                return View(AT);
            }

            public JsonResult GetOtherInfo(int im)
            {
                User UserData = (User)Session["User"];
                ItemStore Imx = new ItemStore();
                Imx = RolReCalculationFun.GetQOH(im,UserData.selectedStationID );
                return Json(Imx);
            
            }

            public JsonResult GetCategoryItems(int im,string srch)
            {
                List<MMS_ItemMaster> itm = new List<MMS_ItemMaster>();
                if (MainFunction.NullToInteger(im.ToString()) > 0 )
                {
                    User UserData = (User)Session["User"];
                    itm = RolReCalculationFun.GetCategoryItem(im,srch.ToUpper(),UserData.selectedStationID );
                }
                return Json(itm);

            }

        
        
        public JsonResult Save(ItemAvgSale itemAvg)
            {
                User UserData = (User)Session["User"];
                int featureid = 99;
                int functionid = 1;
                if (MainFunction.UserAllowedFunction(UserData,featureid, functionid) == true)
                {
                                         itemAvg.Stationid =short.Parse(UserData.selectedStationID.ToString());
                    bool bn = RolReCalculationFun.Save(itemAvg, UserData.EmpID);
                    if (bn == false)
                    {
                        return Json(new MessageModel { Message = "Error during saving,Check Your Entry!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
                    }
                    else
                    { return Json(new MessageModel { Message = "Max Level,Min Level & Reorder Level updated for all items.Check Report for details.!", isSuccess = true, date = DateTime.Now.ToShortDateString() }); }
                }
                else
                {
                    return Json(new MessageModel { Message = "you not allowed to Save!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
                }

            }

    }
}