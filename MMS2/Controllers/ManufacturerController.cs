using System;
using System.Collections.Generic;
using System.Web.Mvc;



namespace MMS2.Controllers
{
    [MMCFilter]  
    public class ManufacturerController : Controller
    {
        public ActionResult Index(String ErrMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 88;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
                         ViewBag.ErrorMsg = ErrMsg;
            ErrMsg = null;
            List<ManufacturerS> sp = ManufacturerFun.GetManufacturerList();
            return View(sp);
        }
        public ActionResult NewManufacturer(String ErrMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 88;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            ManufacturerS sp = ManufacturerFun.GetManufacturerMasterNew(UserData.selectedStationID);
            sp.Manufacturerx.Active = false;
            sp.Manufacturerx.StartdateTime = DateTime.Now;
            return View("_Manufacturer", sp);
        }
        public ActionResult ManufacturerDetails(int ManufacturereID, String ErrMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 88;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }

            try
            {
                ViewBag.ErrorMsg = ErrMsg;
                ErrMsg = null;

                ManufacturerS sp = ManufacturerFun.GetManufacturerMaster(UserData.selectedStationID, ManufacturereID);

                if (sp.MessageShow == null)
                {
                    return View("_Manufacturer", sp);
                }
                else
                {
                    return RedirectToAction("Index", new { ErrMsg = sp.MessageShow });


                }
            }
            catch (Exception e)
            {
                                 return RedirectToAction("Index", new { ErrMsg = e.Message });
            }

        }
        public JsonResult Save(ManufacturerS Manufacturer)
        {
            User UserData = (User)Session["User"];
            if (Manufacturer.Manufacturerx.id > 0)               {
                int featureid = 88;
                int functionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
                {
                                         bool bn = ManufacturerFun.SaveManufacturer(Manufacturer, Manufacturer.Manufacturerx.id);
                    if (bn == true)
                    {
                        return Json(new MessageModel { Message = "Successfully Updated!", isSuccess = true, date = DateTime.Now.ToShortDateString() });
                    }
                    else
                    {
                        return Json(new MessageModel { Message = Manufacturer.MessageShow, isSuccess = false, date = DateTime.Now.ToShortDateString() });
                    }
                }
                else
                {
                    return Json(new MessageModel { Message = "you not allowed to Save!", isSuccess = false, date = DateTime.Now.ToShortDateString() });

                }
            }
            else               {
                int featureid = 88;
                int functionid = 3;
                if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
                {
                                         bool bn = ManufacturerFun.SaveManufacturer(Manufacturer, 0);
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
        public JsonResult Delete(ManufacturerS Manufacturer)
        {
            User UserData = (User)Session["User"];
            int featureid = 88;
            int functionid = 4;

            if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
            {
                if (ManufacturerFun.DeleteManufacturer(Manufacturer.Manufacturerx.id, Manufacturer.SelectedSupplier) == true)
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
        public JsonResult GetItem(string src)
        {
            List<TempListMdl> itm = new List<TempListMdl>();
            User UserData = (User)Session["User"];
            itm = ManufacturerFun.GetCategoryItem(src.ToUpper(), UserData.selectedStationID);
            return Json(itm);

        }
    }
}
