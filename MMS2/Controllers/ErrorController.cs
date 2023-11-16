using System.Web.Mvc;

namespace MMS2.Controllers
{
    public class ErrorController : Controller
    {
                  
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotFound404()
        {
            return View("Index");
        }

    }
}
