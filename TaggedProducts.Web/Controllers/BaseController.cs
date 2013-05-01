using System.Web.Mvc;

namespace TaggedProducts.Web.Controllers
{
    public class BaseController : Controller
    {
        public ActionResult RedirectToHome()
        {
            return RedirectToAction("Index", "Home");
        }

    }
}
