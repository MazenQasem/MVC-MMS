using System;
using System.Collections.Generic;
using System.Web.Mvc;

  

namespace MMS2.Controllers
{
    [MMCFilter]  
    public class SupplierController : Controller
    {
                 public JsonResult Save(SupplierS supplier)
        {
            User UserData = (User)Session["User"];
            if (supplier.ID > 0)               {
                int featureid = 89;
                int functionid = 2;
                                 if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
                {
                                         bool bn = SupplierFun.SaveSupplier(supplier, supplier.ID);
                    if (bn == true)
                    {
                        return Json(new MessageModel { Message = "Successfully Updated!", isSuccess = true, date = DateTime.Now.ToShortDateString() });
                    }
                    else
                    {
                        return Json(new MessageModel { Message = supplier.SupplierErr, isSuccess = false, date = DateTime.Now.ToShortDateString() });
                    }
                }
                else
                {
                    return Json(new MessageModel { Message = "you not allowed to Save!", isSuccess = false, date = DateTime.Now.ToShortDateString() });

                }
            }
            else               {
                int featureid = 89;
                int functionid = 3;
                                 if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
                {
                                         bool bn = SupplierFun.SaveSupplier(supplier, 0);
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
        public JsonResult Delete(SupplierS supplier)
        {
            User UserData = (User)Session["User"];
            int featureid = 89;
            int functionid = 4;

                         if (MainFunction.UserAllowedFunction(UserData,featureid,functionid)==true)
            {
                if (SupplierFun.DeleteSupplier(supplier.ID) == true)
                {
                    return Json(new MessageModel { Message = "Successfully Deleted!", isSuccess = true, date = DateTime.Now.ToShortDateString() });
                }
                else
                {
                    return Json(new MessageModel
                    {
                        Message = "Cannot delete this supplier as an order is pending against him. Please close or cancel the order to delete.",
                        isSuccess = false,
                        date = DateTime.Now.ToShortDateString()
                    });
                }
            }
            else
            {
                return Json(new MessageModel
                {
                    Message = "You Not Allowed To Delete.",
                    isSuccess = false,
                    date = DateTime.Now.ToShortDateString()
                });
            }

        }

        public ActionResult Index(String ErrorMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 89;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
                         ViewBag.ErrorMsg = ErrorMsg;
            ErrorMsg = null;
            List<SupplierS> sp = SupplierFun.GetSupplierList();
            return View(sp);
        }
        public ActionResult SupplierDetails(int SupplierID, String ErrorMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 89;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }


            try
            {
                ViewBag.ErrorMsg = ErrorMsg;
                ErrorMsg = null;
                                 SupplierS sp = SupplierFun.GetSupplierMaster(UserData.selectedStationID, SupplierID);
                if (sp.SupplierErr == null)
                {
                    return View("_Supplier", sp);
                }
                else
                {
                                         return RedirectToAction("Index", new { ErrMsg = sp.SupplierErr });


                }
            }
            catch (Exception e)
            {
                                 return RedirectToAction("Index", new { ErrMsg = e.Message });
            }

        }
        public ActionResult NewSupplier(String ErrMsg = "")
        {

            User UserData = (User)Session["User"];
            int featureid = 89;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }

            SupplierS sp = SupplierFun.GetSupplierMasterNew(UserData.selectedStationID);
            sp.Active = false;
            sp.StartdateTime = DateTime.Now;
            return View("_Supplier", sp);
        }
    }
}
