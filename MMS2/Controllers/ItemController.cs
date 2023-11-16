using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
  

namespace MMS2.Controllers
{
    [MMCFilter]      public class ItemController : Controller
    {
                 public JsonResult LoadList(String Str)
        {
            User UserData = (User)Session["User"];
            List<TempListMdl> it = MainFunction.LoadComboList(Str, UserData.selectedStationID);
            return Json(it);
        }
        public JsonResult LoadListNoStation(String Str, bool AddNone = false)
        {
            List<TempListMdl> it = new List<TempListMdl>();
            if (AddNone == true)
            {
                it = MainFunction.LoadComboList(Str, 0, "<None>");
            }
            else
            {
                it = MainFunction.LoadComboList(Str);

            }
            return Json(it);
        }

                 public ActionResult LookUp(string ItemViewType)
        {
            User UserData = (User)Session["User"];
            int featureid = 92;
            
                                                                ViewBag.ItemViewType = ItemViewType;
            return View();
        }
        public ActionResult Show()
        {
            User UserData = (User)Session["User"];
            int featureid = 92;
            
                                                                return View();
        }
        public JsonResult LoadItemGroup()
        {
            User UserData = (User)Session["User"];
            List<TempListMdl> data = ItemFun.listMdl("Select Id,name from ItemGroup where Parent=0", UserData.selectedStationID);
            return Json(data.ToArray());
        }
        public JsonResult ItemLookupView(string str)
        {
            User UserData = (User)Session["User"];
            str = str + " and a.id in (select id from MMS_ItemMaster where stationid=" + UserData.selectedStationID + " and deleted=0) "
                    + " order by a.name ";
            List<ItemLookUp> it = ItemFun.ItemLookUpResult(str);
            var dt = it.Select(d => new string[] {d.ID.ToString(),d.ItemCode,d.ItemName,d.GroupName,
            d.SupName,d.ManuName,d.SellPrice.ToString()});
            return Json(dt);
        }
        public JsonResult ItemLookupBatch(int ItemID)
        {
            User UserData = (User)Session["User"];
            var it = ItemFun.ItemLookUpBatches(ItemID, UserData.selectedStationID);
             
                                      return Json(it);



        }

        
        public ActionResult ItemSubStoreView(int itmid)
        {
            User UserData = (User)Session["User"];
            int featureid = 87;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            MMS_ItemMaster it = ItemFun.ItemSubStoreResults(itmid, UserData.selectedStationID);
            return View("ItemSubStore", it);
        }
        public JsonResult LoadOptionList(String Str)
        {
            User UserData = (User)Session["User"];
            List<TempListMdl> it = MainFunction.LoadOptionList(Str, UserData.selectedStationID);
            return Json(it);

        }


        public JsonResult LoadShelfList(String Str)
        {
            User UserData = (User)Session["User"];
            List<TempListMdl> it = MainFunction.LoadOptionList(Str, 0);
            return Json(it);

        }


        public JsonResult LoadPackingList(int ItemID)
        {
            List<TempListMdl> it = ItemFun.LoadPacking(ItemID);
            return Json(it);
        }
        public JsonResult LoadBatchList(int ItemID)
        {
            User UserData = (User)Session["User"];
            var it = ItemFun.GetBatchInfo(ItemID, UserData.selectedStationID);
             
                                      return Json(it);
        }
        public JsonResult UpdateSubStore(ItemStore IT)
        {
            User UserData = (User)Session["User"];
            int featureid = 87;
            int functionid = 3;
            if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
            {
                                 bool bn = ItemFun.UpdateSubStore(IT, UserData.selectedStationID);
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

        
        public JsonResult InsertRack(string Name)
        {
            User UserData = (User)Session["User"];
            int Gstationid = UserData.selectedStationID;
            int GetExistRack = MainFunction.GetOneVal("select id from Rack where name='" + Name.Trim() + "' and stationid=" + Gstationid, "id");
            if (GetExistRack > 0)
            {
                return Json(new MessageModel { Message = "Rack Name Already Exists!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
            }
            else
            {
                bool InsertR = MainFunction.SSqlExcuite("insert into rack (name,deleted,stationid) values('" + Name.Trim() + "',0," + Gstationid + ")");
                int GetMaxID = MainFunction.GetOneVal("select max(id) as MaxId from Rack where stationid=" + Gstationid, "MaxId");
                return Json(new MessageModel { Message = "Successfully Saved!", isSuccess = true, date = DateTime.Now.ToShortDateString(), id = GetMaxID });
            }

        }
        public JsonResult InsertRackShelf(string Name, int RackID)
        {
            User UserData = (User)Session["User"];
            int Gstationid = UserData.selectedStationID;
            int GetExistRack = MainFunction.GetOneVal("select id from shelf where name='" + Name.Trim() + "' and stationid=" + Gstationid, "id");
            if (GetExistRack > 0)
            {
                return Json(new MessageModel { Message = "Shelf Name Already Exists!", isSuccess = false, date = DateTime.Now.ToShortDateString() });
            }
            else
            {
                bool InsertR = MainFunction.SSqlExcuite("insert into shelf (name,deleted,stationid) values('" + Name.Trim() + "',0," + Gstationid + ")");
                int GetMaxID = MainFunction.GetOneVal("select max(id) as MaxId from shelf where stationid=" + Gstationid, "MaxId");
                bool InsertRackShelf = MainFunction.SSqlExcuite("insert into rackshelf(rackid,shelfid,stationid) values(" + RackID + "," + GetMaxID
                    + "," + Gstationid + ")");

                return Json(new MessageModel { Message = "Successfully Saved!", isSuccess = true, date = DateTime.Now.ToShortDateString(), id = GetMaxID });
            }

        }



        
        public ActionResult Item(int itmid)
        {
            User UserData = (User)Session["User"];
            int featureid = 86;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            MMS_ItemMaster it = ItemFun.ItemMasterResults(itmid, UserData.selectedStationID);
            return View("ItemMaster", it);

        }
        public JsonResult SaveUOM(string UOMNAME, string[] TempList, int UOMID = 0)
        {
            String ss = ItemFun.SaveUOM(UOMNAME, TempList, UOMID);
            return Json(new MessageModel { Message = ss, isSuccess = false, date = DateTime.Now.ToShortDateString() });

        }
        public JsonResult SavePacking(MMS_ItemMaster tbl)
        {
            User UserData = (User)Session["User"];
            bool ss = ItemFun.SavePacking(tbl, UserData.EmpID, UserData.selectedStationID);
            return Json(new MessageModel { Message = tbl.ErrMsg, isSuccess = true, date = DateTime.Now.ToShortDateString() });

        }
        public JsonResult UpdateItemMaster(MMS_ItemMaster ItemMaster)
        {
            User UserData = (User)Session["User"];
            int featureid = 86;
            int functionid = 3;
            if (MainFunction.UserAllowedFunction(UserData, featureid, functionid) == true)
            {
                                                  bool bn = ItemFun.UPdateItemMaster(ItemMaster, UserData.EmpID, UserData.selectedStationID);
                if (bn == false)
                {
                    return Json(new MessageModel { Message = ItemMaster.ErrMsg, isSuccess = false, date = DateTime.Now.ToShortDateString() });
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
