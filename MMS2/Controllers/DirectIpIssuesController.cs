using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;



namespace MMS2.Controllers
{
    [MMCFilter]      public class DirectIpIssuesController : Controller
    {
                          ListAllFun listAll = new ListAllFun();
 
        public ActionResult Index()
        {
          

            var viewModel = new ParamDirectIpIssuesModel() {
                DoctorList = listAll.getAllDoctors(),
                CategoryList = listAll.getAllCategories(),
                PinList = listAll.getAllInpatientEmployee(),
                BedList = listAll.getAllBedName()
            };
            return View(viewModel);
        }

        public ActionResult LoadList(int IpID)
        {
            User UserData = (User)Session["User"];
            int StationId = UserData.selectedStationID;
            return Json(listAll.getAjax_ViewDatafromIPid(IpID, StationId), JsonRequestBehavior.AllowGet);
           
        }
        public ActionResult LoadPatientList( )
        {
            return Json(listAll.getAllPatientList(), JsonRequestBehavior.AllowGet);
           
        }


        public JsonResult LoadListItembyDirectIpIssue(int Cat, string ItemCode )
        {
           
            try
            {
                User UserData = (User)Session["User"];
                int StationId = UserData.selectedStationID;

                return Json(DirectIpIssueFun.LoadItemsDirectIPIssue(Cat, StationId, ItemCode), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json("Error found in search parameters!");

            }
        
             
        }

        public JsonResult InformationLoadList(int IpId)
        {
            User UserData = (User)Session["User"];
            int StationId = UserData.selectedStationID;
            InformationDatalist FakeToHoldErr = new InformationDatalist();


            List<InformationDatalist> list = listAll.getInpatientInfo(IpId);

            int CheckIfHasOperationToday = listAll.checkIsPatientIsinOP(IpId);
            if (CheckIfHasOperationToday > 0)
            {
                FakeToHoldErr.AlertMsg = "Item not mapped to this Station";
                
            }

            list.Add(FakeToHoldErr);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult checkDispatch3days(DirectIpSaveModel Order)
        {
            User UserData = (User)Session["User"];
            int StationId = UserData.selectedStationID;

            if (StationId == 95)             {
                DirectIpSaveModel dispatch3days = listAll.checkdispatch3days(Order);

                return Json(dispatch3days, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json('0', JsonRequestBehavior.AllowGet);
            }
            
        }



        public JsonResult Save(DirectIpSaveModel Order)
        {
        
            User UserData = (User)Session["User"];
            if (Order.OrderID == 0)
            {
                int featureid = 101;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Save!";
                    return Json(Order);
                };
            }
            else
            {
                int featureid = 101;
                int funactionid = 2;
                if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
                {
                    Order.ErrMsg = "You are not allowed to Edit!";
                    return Json(Order);
                };

            }
                     DirectIpSaveModel it = DirectIpIssueFun.Save(Order, UserData);
            return Json(it);
        }



        public JsonResult InsertItemDirectIPIssue(int ItemID, List<IndentIssueInsertedItemListDirectIp> IssueList)
        {
            User UserData = (User)Session["User"];
            int StationId = UserData.selectedStationID;
            List<IndentIssueInsertedItemListDirectIp> it = DirectIpIssueFun.InsertItemDirectIp(ItemID, StationId, IssueList);
            return Json(it);
        }


        public JsonResult ViewDetails(int OrderID)
        {
            User UserData = (User)Session["User"];
            DirectIpSaveModel it = DirectIpIssueFun.ViewDetails(OrderID, UserData.selectedStationID);
            return Json(it);
        }



        public ActionResult Print(int OrderID)
        {
            User UserData = (User)Session["User"];
                        DirectIpSaveModel Order = DirectIpIssueFun.PrintOut(OrderID, UserData.selectedStationID);
            if (Request.IsAjaxRequest())
            {
                                 return Json(new { htmlData = RenderRazorViewToString("ReportViewerHtml", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(Order);
             
        }


        public static string RenderRazorViewToString(string viewName, object model, ControllerContext controllerContext, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            viewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }



        public JsonResult GetItemUOMList(int ItemID)
        {
            List<TempListMdl> UOMLIST = new List<TempListMdl>();
            UOMLIST = DirectIpIssueFun.ItemUOMList(ItemID);
            return Json(UOMLIST);
        }
        
 



    }
}
