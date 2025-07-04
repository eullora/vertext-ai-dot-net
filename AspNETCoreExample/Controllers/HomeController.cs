using Microsoft.AspNetCore.Mvc;

namespace TechStrat.GPT.UI.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
            
        }
        public IActionResult Index()
        {             
            return View();
        }
               
    }
}
