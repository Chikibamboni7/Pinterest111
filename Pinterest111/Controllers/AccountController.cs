using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
    }
}