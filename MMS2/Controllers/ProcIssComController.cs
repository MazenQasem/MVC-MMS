using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace MMS2.Controllers
{
    [MMCFilter]      public class ProcIssComController : Controller
    {
                  
        public ActionResult Index()
        {
            User UserData = (User)Session["User"];
            int featureid = 1093;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            ProcIssueMain dd = ProcIssComFun.ClearRecord();
            return View("ProcIssComView", dd);
        }
        public JsonResult LoadBillList(int DeptID, DateTime dT)
        {
            List<TempListMdl> dd = ProcIssComFun.LoadBillNoList(DeptID, dT);
            return Json(dd);
        }


        public JsonResult GetProcList(int BillNo, string BillTxt)
        {
            User UserData = (User)Session["User"];
            ProcIssueMain dd = ProcIssComFun.GetProcList(BillNo, BillTxt);
            dd.lblOperator = UserData.Name;

            return Json(dd);
        }
        public JsonResult GetProcListItems(int ProcID, int ProcQty)
        {
            User UserData = (User)Session["User"];
            List<ProcIssueItemList> ll = new List<ProcIssueItemList>();
            ll = ProcIssComFun.GetProcedureItems(ProcID, ProcQty, UserData.selectedStationID);
            if (ll.Count > 0 && ll[0].ErrMsg == "")
            {
                return Json(ll);

            }
            else
            { return Json(ll[0].ErrMsg); }


        }
        public JsonResult ItemSelect(List<ProcIssueItemList> ItemList, int id,int ReplacedItem)
        {
            User UserData = (User)Session["User"];
            List<ProcIssueItemList> ll = new List<ProcIssueItemList>();
            ll = ProcIssComFun.InsertItem(ItemList, id, ReplacedItem, UserData.selectedStationID);
            if (ll.Count > 0 && ll[0].ErrMsg == "")
            {
                return Json(ll);

            }
            else
            { return Json(ll[0].ErrMsg); }
        
        }

        public JsonResult Save(ProcIssueMain Order)
        {
            User UserData = (User)Session["User"];
            string MSG = "";
            int featureid = 1093;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                MSG = "You are not allowed to Save!";
                return Json(MSG);
            };
            MessageModel MSGResult = ProcIssComFun.Save(Order, UserData);
            return Json(MSGResult);
        }

    }
}
