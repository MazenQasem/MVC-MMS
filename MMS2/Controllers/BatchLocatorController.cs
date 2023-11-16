using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
  
namespace MMS2.Controllers
{
    [MMCFilter]      public class BatchLocatorController : Controller
    {
                  
        public ActionResult Index(String ErrMsg = "")
        {
            User UserData = (User)Session["User"];
            int featureid = 1089;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }

                         ViewBag.ErrorMsg = ErrMsg;
            ErrMsg = null;
            BatchLocator AT = new BatchLocator();
            AT = BatchLocatorFun.LoadTransTypeList();
            return View(AT);
        }
        public JsonResult GetTransNoList(int TabID, int TransTypeID)
        {
            User UserData = (User)Session["User"];
            List<ListBox> Im = BatchLocatorFun.GetTransNoList(TabID, UserData.selectedStationID, TransTypeID);
            return Json(Im);

        }
        public JsonResult LoadRack()
        {
            List<ListBox> ll = BatchLocatorFun.LoadCombo("select id,name from rack order by name ", true);
            return Json(ll);
        }
        public JsonResult LoadCellRack(int ItemID)
        {
            List<ListBox> ll = BatchLocatorFun.LoadCellCombo("Select Distinct A.ID,A.Name from Rack a,ItemLocation B where B.RackID=A.ID  and  B.ItemID=" + ItemID);
            return Json(ll);
        }
        public JsonResult LoadShelf(int rackid)
        {
            string st = "Select Id,Name from Shelf S,RackShelf RS where RS.RackId=" + rackid + " and RS.ShelfId=S.Id order by Name";
            List<ListBox> ll = BatchLocatorFun.LoadCombo(st, true);
            return Json(ll);
        }

        
        public JsonResult GetDetails0(jQueryDataTableParamModel param)
        {
            User UserData = (User)Session["User"];
                         int TabID =MainFunction.NullToInteger(Request.QueryString["TabID"].ToString());
            int TransTypeOrOption = MainFunction.NullToInteger(Request.QueryString["TransTypeOrOption"].ToString());
            int RackOrTransNo = MainFunction.NullToInteger(Request.QueryString["RackOrTransNo"].ToString());
            int ShelfID = MainFunction.NullToInteger(Request.QueryString["ShelfID"].ToString());
            
            if (Request.QueryString["Name"] != null)
                param.sSearch = Request.QueryString["Name"].ToString();

            param.iSortCol_0 = Convert.ToInt32(Request["iSortCol_0"]);               param.sSortDir_0 = Request["sSortDir_0"];              
            var data = BatchLocatorFun.GetTableResults(TabID, TransTypeOrOption, RackOrTransNo, ShelfID, UserData.selectedStationID, param);
            
            var aaData = data.Select(d => new string[] {d.ID.ToString(),d.SNO.ToString(),d.ItemCode,d.ItemName,d.BatchNo,d.Quantity.ToString(),d.Rack,d.Shelf,d.RackID.ToString(),d.ShelfID.ToString()
                ,d.expdate,d.Cost.ToString(),d.SelPrice.ToString(),d.ItemID.ToString(),d.StationID.ToString(),d.receiptid.ToString(),d.BatchID.ToString()}).ToArray();

            return Json(new
                {
                    sEcho = param.sEcho,
                    aaData = aaData,
                    iTotalRecords = param.iTotalRecords,
                    iTotalDisplayRecords = param.iTotalDisplayRecords
                }, JsonRequestBehavior.AllowGet);






        }
        public JsonResult GetDetails1(int TabID,int TransOrOpt,int RackOrTransNo,int ShelfID)
        {
            User UserData = (User)Session["User"];
            var data = BatchLocatorFun.GetTransNoQueryNew(TabID, TransOrOpt, RackOrTransNo, ShelfID, UserData.selectedStationID);

                                      return Json(data);
        }
        public JsonResult GetDetails2(int TabID, int TransOrOpt, int RackOrTransNo, int ShelfID)
        {
            User UserData = (User)Session["User"];
            var data = BatchLocatorFun.GetTransNoQueryNew(TabID, TransOrOpt, RackOrTransNo, ShelfID, UserData.selectedStationID);
            return Json(data);
        }
       
                                                      
         
                                                                        
                  
                  
         
                  
                                                               





                                                                                 
                  
                  
         
                           
                                                               





         

    }
}
