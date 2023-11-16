using System.Web.Mvc;
 



namespace MMS2.Controllers
{
    [MMCFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return View("Index", "Login");
            }
                         return View();
        }

                                             
                                             
                  
        
    }
}
