using System;
using System.Collections.Generic;
using System.Web.Mvc;
  

namespace MMS2.Controllers
{
    [MMCFilter]  
    public class AlterMRPController : Controller
    {
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 90;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }

                                                   AlterMRP AT = new AlterMRP();
                         
            AT = AlterMRPFun.GetItemsList(UserData.selectedStationID);
            AT.OperatorName = UserData.Name;
            
            return View("_AlterMRP",AT);
        }

        public JsonResult GetItemRow(int ItemID)
        {
            User UserData = (User)Session["User"];
            List<Item> Im = AlterMRPFun.GetItemBatch(ItemID, UserData.selectedStationID);

            return Json(Im);

        }     
        
        public JsonResult Save(AlterMRP AlterMRPs)
        {
            User UserData = (User)Session["User"];
                int featureid = 90;
                int functionid = 2;
                if (MainFunction.UserAllowedFunction(UserData,featureid,functionid) == true)
                {
                                         bool bn = AlterMRPFun.Save(AlterMRPs,UserData.EmpID);
                    if (bn == false)
                    {
                        return Json(new MessageModel { Message = "Error during saving,Check Your Entry!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
                    }
                    else
                    { return Json(new MessageModel { Message = "Successfully Saved!", isSuccess = true, date = DateTime.Now.ToShortDateString() }); }
                }
                else
                {
                    return Json(new MessageModel { Message = "you not allowed to Save!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
                }

            }


    }
}
