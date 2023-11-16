using System.Web.Mvc;

namespace MMS2.Controllers
{
    public class PatientController : Controller
    {
        [MMCFilter]                    
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult PatientFind()
        {
            return PartialView();
        }

    }
}
