using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.IO;
  

namespace MMS2.Controllers
{
    [MMCFilter]      public class MISReptController : Controller
    {
                 public ActionResult CashSummary()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Cash Summary";
            m.ReportNo = 1223;
            m.gStationID = UserData.selectedStationID;

            return View("MainScreen", m);
        }
        public ActionResult CashSummaryPrint(string Str)
        {
            User UserData = (User)Session["User"];
            CashSummary Order = MISReptFun.GetCashSummary(Str);

            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("CashSummary", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult CashIssueDetails()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Issued to OP(Cash/Credit)";
            m.ReportNo = 1164;
            m.gStationID = UserData.selectedStationID;
            m.gIACode = UserData.gIACode;

            return View("MainScreen", m);
        }
        public ActionResult CashIssueDetailsPrint(string Str)
        {
            User UserData = (User)Session["User"];
            List<CashIssueDetail> Order = MISReptFun.GetCashIssueDetails(Str);
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("CashIssueDetails", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);
                     }
                 public ActionResult ItemStockatSubStores()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Item Stock @ Sub Stores";
            m.ReportNo = 1097;
            m.gStationID = UserData.selectedStationID;
            m.gIACode = UserData.gIACode;
            m.AppPath = UserData.AppName;
            return View("ItemStockatSubStores", m);
        }
                 public ActionResult SummaryCashCreditReceipts()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Summary of Cash/Credit Receipts";
            m.ReportNo = 1168;
            m.gStationID = UserData.selectedStationID;

            return View("MainScreen", m);
        }
        public ActionResult SummaryCashCreditReceiptsPrint(string Str)
        {
            User UserData = (User)Session["User"];
            CashSummary Order = MISReptFun.GetCashSummary(Str);
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("SummaryCashCreditReceipts", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult PhOperatorWiseDispenses()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Pharmacist/Operator wise dispenses";
            m.ReportNo = 1236;
            m.gStationID = UserData.selectedStationID;

            return View("MainScreen", m);
        }
        public ActionResult PhOperatorWiseDispensesPrint(string Str, int ReportType, string Title, string TitleDate, string TitleSheft, string TitleStation)
        {
                         User UserData = (User)Session["User"];
            PhOperatorWiseDispensesHeader Order = MISReptFun.PHWiseDispenses(Str, ReportType, UserData.gIACode);
            Order.ReprotType = ReportType;
            Order.Title = Title;
            Order.TitleDate = TitleDate;
            Order.TitleSheft = TitleSheft;
            Order.TitleStation = TitleStation;

            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("PhOperatorWiseDispenses", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ItemWiseIssuesDetails()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Item wise Issues Details";
            m.ReportNo = 1176;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ItemWiseIssuesDetailPrint(string Str, string Title, string TitleDate, string TitleStation)
        {
            User UserData = (User)Session["User"];
            ItemWiseIssuesDetailsHeader Order = MISReptFun.ItemWiseIssuesDetailsFun(Str);

            Order.Title = Title;
            Order.TitleDate = TitleDate;
            Order.TitleStation = TitleStation;

            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ItemWiseIssuesDetailsView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ItemLedgerSheet()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Item Ledger Sheet";
            m.ReportNo = 1103;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ItemLedgerSheetPrint(string Str, DateTime FromDate, DateTime ToDate, string ItemCode)
        {
            User UserData = (User)Session["User"];
            ItemLedgerHeader Order = MISReptFun.ItemLedger(FromDate, ToDate, ItemCode, UserData.StationName, UserData.selectedStationID);
            if (Order.ErrMsg != null) { return Json(Order); }
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ItemLedgerSheetView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ItemLedgerSheetPrice()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Item Ledger Sheet Price";
            m.ReportNo = 1104;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ItemLedgerSheetPricePrint(DateTime FromDate, DateTime ToDate, string ItemCode, int CatID, int RadioID, string RadioText)
        {
            User UserData = (User)Session["User"];
            ItemLedgerPriceHeader Order = MISReptFun.ItemLedgerPrice(FromDate, ToDate, ItemCode, UserData.StationName, UserData.selectedStationID, CatID, RadioID, RadioText);
            if (Order.ErrMsg != null) { return Json(Order); }



            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ItemLedgerSheetPriceView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ExpiryDateReport()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Expiry Date Report";
            m.ReportNo = 1114;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ExpiryDateReportPrint(DateTime FromDate, DateTime ToDate, int CatID, int RadioID, int chkAsDate, int chkPrice)
        {
            User UserData = (User)Session["User"];
            ExpiryDateReportHeader Order = MISReptFun.ExpiryDateReport(FromDate, ToDate, UserData.StationName, UserData.selectedStationID, CatID, RadioID, chkAsDate, chkPrice);
            if (Order.ErrMsg != null) { return Json(Order); }



            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ExpiryDateReportView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ISBS()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Item Stock By Supplier";
            m.ReportNo = 1189;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ISBSPrint(int SupID, int RadioID)
        {
                         try
            {
                                                                                                                                                        MISRept tt = new MISRept();
                tt.MSupplierID = SupID;
                tt.MRadioID = RadioID;
                ViewData["ISBSModel"] = tt;
                return View("ViewPage2", tt);             }
            catch (Exception e) { return Json(e.Message); }
        }
                 public ActionResult ItemMasterList()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Item Master List With Location";
            m.ReportNo = 1087;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ItemMasterListPrint(int CatID, string prefix, string suffix, int QtyList, int OrderList, string FromDate, string ToDate, int chkWithoutLoc, int chkWithExpiry, int chkCost, int ShelfID)
        {

            User UserData = (User)Session["User"];
            ItemMasterListHeader Order = MISReptFun.ItemList(UserData.selectedStationID, CatID, prefix, suffix, QtyList, OrderList, FromDate, ToDate, chkWithoutLoc, chkWithExpiry, chkCost, ShelfID);

            if (Order.ErrMsg != null) { return Json(Order); }

            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ItemMasterListView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }

            return Json(Order);

        }
                 public ActionResult IPWiseIssue()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "IP Wise Issue";
            m.ReportNo = 1234;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult IPWiseIssuePrint(int StationID, int IPID, string FromDate, string ToDate)
        {

            User UserData = (User)Session["User"];
            IPWiseIssueHeader Order = MISReptFun.IPWise(UserData.selectedStationID, StationID, IPID, FromDate, ToDate);
            if (Order.ErrMsg != null) { return Json(Order); }

            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("IPWiseIssueView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult OpIssuePrintOut(Patient Patient, int PrintType)
        {

            User UserData = (User)Session["User"];
            PrnIssueMaster Order = new PrnIssueMaster();

            if (Patient.BillisCredit == true)
            {
                Order = OpIssueFun.CreditPrnIssue(Patient, false, Patient.MaxBillNo);
                if (Patient.lbldedamt > 0)
                {
                    Order = OpIssueFun.CashPrnIssue(Patient, false, Patient.csMaxBillNo, 1, Patient.MaxBillNo);
                }

            }
            else
            {
                Order = OpIssueFun.CashPrnIssue(Patient, false, Patient.MaxBillNo);
            }


            if (Order.ErrMsg != null) { return Json(Order); }

            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ItemMasterListView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult PrintReturn(int OrderID, int Status)
        {
            User UserData = (User)Session["User"];
            RptWardReturnHeader Order = WardReturnFun.RetPrint(OrderID, UserData);


            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("WardReturnReportView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ErrMedSummary()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Medical Error Summary Report";
            m.ReportNo = 1241;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
                 public ActionResult ActiveProfile()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Active Profile";
            m.ReportNo = 0;
            m.ReportStrID = "ActiveProfile";
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
                 public ActionResult AdjIssueRPT()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Adjustment Issue Report";
            m.ReportNo = 1101;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult AdjIssueRPTSheetPrint(int CatID, bool chkCost, DateTime FromDate, DateTime ToDate)
        {
            User UserData = (User)Session["User"];
            AdjustIssueRptMdl Order = MISReptFun.AdjIssuereport(CatID, chkCost, FromDate, ToDate, UserData);
            if (Order.ErrMsg != null) { return Json(Order); }
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("AdjIssueReportView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult AdjRecRPT()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Adjustment Receipt Report";
            m.ReportNo = 1102;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult AdjRecRPTSheetPrint(int CatID, bool chkCost, DateTime FromDate, DateTime ToDate)
        {
            User UserData = (User)Session["User"];
            AdjustRecRptMdl Order = MISReptFun.AdjReceiptreport(CatID, chkCost, FromDate, ToDate, UserData);
            if (Order.ErrMsg != null) { return Json(Order); }
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("AdjRecReportView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult ADT()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Patient Admissions and Discharges";
            m.ReportNo = 1233;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult ADTPrint(int TypeID, int StationID, string STName, string PIN, DateTime FromDate, DateTime ToDate)
        {
            User UserData = (User)Session["User"];
            ADTRptMdl Order = MISReptFun.ADTReport(TypeID, StationID, STName, PIN, FromDate, ToDate, UserData);
            if (Order.ErrMsg != null) { return Json(Order); }
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("ADTreportView", Order, ControllerContext, ViewData, TempData) },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(Order);

        }
                 public ActionResult AntiUsed()
        {
            User UserData = (User)Session["User"];
            MISRept m = new MISRept();
            m.ReportName = "Antibiotic used in Surgery-Monthly";
            m.ReportNo = 1240;
            m.gStationID = UserData.selectedStationID;
            return View("MainScreen", m);
        }
        public ActionResult AntiUsedPrint(DateTime FromDate)
        {
            User UserData = (User)Session["User"];
            AntiUsedHDRMdl Order = new AntiUsedHDRMdl();
            Order.AsOfDate = FromDate.ToString("MMM") + " " + FromDate.Year.ToString();
            List<AntiUsedMdl> DetailRecord = MISReptFun.GetListAnti(FromDate, UserData);
            Order.ItemList = new List<AntiUsedMdl>();
            Order.ItemList = DetailRecord;

            if (DetailRecord[0].ErrMsg != null && DetailRecord.Count > 0)
            {
                Order.ErrMsg = DetailRecord[0].ErrMsg;
                return Json(Order);

            }
            if (Request.IsAjaxRequest())
            {
                return Json(new { htmlData = RenderRazorViewToString("AntiUsedreportView", Order, ControllerContext, ViewData, TempData) },
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



    }
}

