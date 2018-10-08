using Microsoft.AspNetCore.Mvc;

namespace AkkaNetDemo.Controllers
{
   
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return new RedirectResult("~/api/akka/docs");
        }
    }
}
