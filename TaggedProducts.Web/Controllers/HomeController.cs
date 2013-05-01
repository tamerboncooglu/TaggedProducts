using System.Web.Mvc;
using TaggedProducts.Web.Models;

namespace TaggedProducts.Web.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet, AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet, AllowAnonymous]
        public ViewResult About()
        {
            return this.View();
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Contact()
        {
            return this.View(new ContactModel());
        }
    }
}
