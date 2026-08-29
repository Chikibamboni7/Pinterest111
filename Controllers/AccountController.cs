using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }
    }
}