﻿using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class DrugJustificationController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 75;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            DrugJustificationModel dd = DrugJustificationFun.Clear();
            return View("DrugJustificationView", dd);
        }
        public JsonResult View(string ListText)
        {
            List<DetailsView> dd = DrugJustificationFun.LoadView(ListText);
            return Json(dd);
        }

        public JsonResult Save(List<DetailsView> DetailTbl)
        {
            User UserData = (User)Session["User"];

            int featureid = 75;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                DetailTbl[0].ErrMsg = "You are not allowed to Save!";
                return Json(DetailTbl);
            };


            List<DetailsView> it = DrugJustificationFun.Save(DetailTbl, UserData);
            return Json(it);
        }



                  
                                                                                          
                                                      
                                    
                                                                                 
         


                                             
         
                                                                                          
                  
                                                                                                                     

    }
}
