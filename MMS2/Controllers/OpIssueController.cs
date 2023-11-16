using System;
using System.Collections.Generic;
using System.Web.Mvc;



namespace MMS2.Controllers
{
    public class OpIssueController : Controller
    {


        public ActionResult Index(String Msg = null)
        {
            User UserData = (User)Session["User"];
            Patient pt = OpIssueFun.OpenClearPatient(UserData.gIACode, UserData.Name);
            if (Msg != null)
            { pt.ErrMsg = Msg; }

            return View("OpIssue", pt);
        }
        public JsonResult GetPatient(String Str, String PIN, int DocID)
        {
            Patient xpt = new Patient();
            User UserData = (User)Session["User"];
            if (DocID == 0)
            {
                xpt = OpIssueFun.GetPatientDetails(PIN, UserData.Name, UserData, DocID);
            }
            else
            {
                Patient pt = new Patient();
                pt.OperatorName = UserData.Name;
                pt.DoctorId = DocID;
                pt.Registrationno = int.Parse(MainFunction.getRegNumber(PIN.ToString()));
                pt.IssueAuthorityCode = MainFunction.getAuthorityCode(PIN.ToString());
                if (UserData.gHospitalBranchCode == "110") { OpIssueFun.GetDetails(ref pt); }
                else if (UserData.gHospitalBranchCode == "120") { OpIssueFun.GetDetailsAseer(ref pt); }
                else if (UserData.gHospitalBranchCode == "170") { OpIssueFun.GetDetailsCairo(ref pt); }
                else if (UserData.gHospitalBranchCode == "180") { OpIssueFun.GetDetailsAseer(ref pt); }
                xpt = pt;
            }
            return Json(xpt);
        }
        public JsonResult InsertNewLOA(String PIN, int DocID)
        {

            User UserData = (User)Session["User"];
            string pt = OpIssueFun.LOAApproval(PIN, DocID, UserData.selectedStationID, UserData.EmpID);
            return Json(pt);
        }
        public JsonResult LoadPrescription(int mRegNo, int DocID)
        {
            User UserData = (User)Session["User"];
            List<Prescription> pc = OpIssueFun.LoadPrescription(mRegNo, DocID, UserData);

            return Json(pc);
        }
        public JsonResult DisplayPrescription(int OrderNo, string Ddate, int MPrescriptionF, int docid, int RegNumber)
        {
            User UserData = (User)Session["User"];
            List<PrescriptionDetail> pt = OpIssueFun.DisplayPrescription(OrderNo, Ddate, MPrescriptionF, UserData.selectedStationID, docid, RegNumber);
            return Json(pt);

        }
        public JsonResult DisplayDiagnosis(long pinno, long orderId)
        {
            User UserData = (User)Session["User"];
            List<Diagnosis> pt = OpIssueFun.GetDiagnosis(pinno, orderId, UserData.gIACode);
            return Json(pt);
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
        public JsonResult LoadList(String Str, String Order = "", String GroupBy = "")
        {

            User UserData = (User)Session["User"];
            Str = Str + UserData.selectedStationID + GroupBy + Order;
            List<TempListMdl> it = MainFunction.LoadComboList(Str, 0);

            return Json(it);
        }
        public JsonResult InsertItem(ItemSelectParam itm)
        {
            Item it = new Item();
            User UserData = (User)Session["User"];
            itm.StationID = UserData.selectedStationID;
            if (itm.GenericID > 0)
            {
                it = OpIssueFun.ItemSelectGeneric(itm, itm.GenericID);
            }
            else
            {
                it = OpIssueFun.ItemSelect(itm);
            }
            return Json(it);
        }
        public JsonResult ListInsertItem(List<ItemSelectParam> itm)
        {
            List<InsertedItemsList> it = new List<InsertedItemsList>();
            User UserData = (User)Session["User"];
            int StationID = UserData.selectedStationID;
            it = OpIssueFun.ListItemSelect(itm, StationID);
            return Json(it);
        }
        public JsonResult GetItemUOMList(int ItemID)
        {
            List<TempListMdl> UOMLIST = new List<TempListMdl>();
            UOMLIST = OpIssueFun.ItemUOMList(ItemID);
            return Json(UOMLIST);
        }
        public JsonResult GetItemGenericList(int ItemID)
        {
            User UserData = (User)Session["User"];
            List<TempListMdl> GList = new List<TempListMdl>();
            GList = OpIssueFun.ItemGenericList(ItemID, UserData.selectedStationID);
            return
                Json(GList);
        }
        public JsonResult ConvertQtyxxxx(
             int lsamllQty,
             int ItemID,
             string BatchID,
             int ConverQty,
             int Qoh,
             int Qoh2,
             decimal Price,
             int InsertedQty,
             decimal Amount,
             float Tax,
            int UnitID)
        {
            InsertedItemsList nn = new InsertedItemsList();
            nn.lsmallqty = lsamllQty;
            nn.ID = ItemID;
            nn.batchid = BatchID;
            nn.conversionqty = ConverQty;
            nn.qoh = Qoh;
            nn.qoh2 = Qoh2;
            nn.price = Price;
            nn.DispatchQty = InsertedQty;
            nn.amount = Amount;
            nn.tax = Tax;
            nn.NewUomID = UnitID;
            nn = (OpIssueFun.GetNewConverisionInfo(nn));
            return Json(nn);

        }
        public JsonResult CheckDeposit(string DepositChkNo, int RegNo, string gIACode)
        {
            Deposit dt = new Deposit();
            dt.DepositChkNo = DepositChkNo;
            dt.RegNo = RegNo;
            dt.gIACode = gIACode;
            dt = OpIssueFun.GetDepositInfo(dt);
            return Json(dt);
        }
        public JsonResult Bill(Patient PT)
        {
            User UserData = (User)Session["User"];
            PT.gStationID = UserData.selectedStationID;
            Patient ss = OpIssueFun.Bill(PT);
            return Json(ss);
        }
        public JsonResult SaveBill(Patient PT)
        {
            User UserData = (User)Session["User"];
            int featureid = 79;
            int funactionid = 2;
            if (MainFunction.UserAllowedFunction(UserData, featureid, funactionid) == false)
            {
                PT.ErrMsg = "You are not allowed to Save!";
                return Json(PT);
            };

            PT.gStationID = UserData.selectedStationID;

            if (PT.lbldonationAMT > 0)
            {
                PT.txtAmountToBeCollected = PT.txtAmountToBeCollected - PT.lbldonationAMT;
            }
            Patient ss = OpIssueFun.SaveBill(PT, UserData);




            if (ss.lbldonationAMT > 0)
            {
                ss.txtAmountToBeCollected = ss.txtAmountToBeCollected + ss.lbldonationAMT;
            }
            ss.sPrefix = " Bill No " + ss.sPrefix + " " + ss.COBILLNO.ToString().PadLeft(10, '0') + "  Saved Sucessfully ";



            if (PT.BillisCredit == true)
            {
                ss.PrintType = 1; if (PT.lbldedamt > 0)
                {
                    ss.PrintType = 2;
                }
            }
            else
            {
                ss.PrintType = 3;
            }






            return Json(ss);
        }
        public JsonResult SearchBills(int billno, int billtype)
        {
            User UserData = (User)Session["User"];
            List<SearchTable> ll = OpIssueFun.FoundBills(billno, billtype, UserData.selectedStationID);
            return Json(ll);
        }
        public JsonResult ViewDetailItems(Patient pt)
        {
            User UserData = (User)Session["User"];
            pt.gStationID = UserData.selectedStationID;
            Patient nn = OpIssueFun.GetBIllDetails(pt);
            return Json(nn);
        }
        public ActionResult ViewDetail(Patient pt)
        {
            User UserData = (User)Session["User"];
            int featureid = 79;
            if (MainFunction.UserAllowedMenu(UserData, featureid) == false)
            {
                return View("Error");
            }
            pt.gStationID = UserData.selectedStationID;
            Patient nn = OpIssueFun.GetBIllDetails(pt);
            return PartialView(nn);
        }

        public JsonResult ScannerOnOFF(bool reslt)
        {
            User UserData = (User)Session["User"];
            UserData.ScannerOn = reslt;
            return Json("");

        }
    }
}
