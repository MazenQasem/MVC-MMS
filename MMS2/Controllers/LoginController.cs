using System;
using System.Web.Mvc;



namespace MMS2.Controllers
{

    public class LoginController : Controller
    {
        public ActionResult Index(string ErrMsg = "")
        {
            Session.Clear();
            User u = EmployeeEntry.GetUserStations();
            if (ErrMsg != "") { u.ErrMsg = ErrMsg; }
            return View(u);
        }


        [HttpPost]
        public ActionResult Enter(User u)
        {
            if (ModelState.IsValid)              {
                
                EmployeeEntry.GetEmpID(u);
                if (string.IsNullOrEmpty(u.ErrMsg) != true)
                {
                    ViewBag.Error = u.ErrMsg;
                    u = EmployeeEntry.GetUserStations();
                                         return View("Index", u);
                }
                User tt = EmployeeEntry.GetMenuList(u);
                                 tt.ScannerOn = false;
                Session["User"] = tt;
                ViewBag.Error = "";
                tt.LoginDateTime = String.Format("{0:dd/MMM/yyyy HH:mm:ss}", DateTime.Now);
                tt.StationName = MainFunction.GetName("select name from station where id=" + tt.selectedStationID, "name");
                tt.SwapStationList = EmployeeEntry.SwapStationList(tt);
                tt.AppName = u.AppName;
                tt.gIACode = u.gIACode;


                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index");
            }

        }


        [HttpPost]
        public ActionResult SwapEntery(User u, FormCollection col, int LoginEnteryEmployeeID, string LocalPathAppName)
        {

            u.EmployeeID = LoginEnteryEmployeeID;

            EmployeeEntry.SwapAccess(u);
            if (string.IsNullOrEmpty(u.ErrMsg) != true)
            {
                ViewBag.Error = u.ErrMsg;
                u = EmployeeEntry.GetUserStations();
                                 return View("Index", u);
            }
            User tt = EmployeeEntry.GetMenuList(u);
            tt = EmployeeEntry.GetReportList(tt);
            Session["User"] = tt;
            ViewBag.Error = "";
            tt.LoginDateTime = String.Format("{0:dd/MMM/yyyy HH:mm:ss}", DateTime.Now);
            tt.StationName = MainFunction.GetName("select name from station where id=" + tt.selectedStationID, "name");
            tt.AppName = LocalPathAppName;
            tt.SwapStationList = EmployeeEntry.SwapStationList(tt);
            



            return RedirectToAction("Index", "Home");


        }


    }
}