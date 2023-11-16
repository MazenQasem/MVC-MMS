using System.Web.Mvc;
  
namespace MMS2.Controllers
{
    [MMCFilter]      public class OrderController : Controller
    {
                          public ActionResult Item()
        {
              return View();
        }

        public ActionResult Show()
        {
            return View();
        }
        
    }
}
