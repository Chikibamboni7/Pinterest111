using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View("Register");
        }
    }
}