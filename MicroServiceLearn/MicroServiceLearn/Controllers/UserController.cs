using Microsoft.AspNetCore.Mvc;

namespace MicroServiceLearn.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
