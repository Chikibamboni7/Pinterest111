using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index(string username) => View();
    }
}