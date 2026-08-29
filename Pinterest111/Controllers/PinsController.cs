using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class PinsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
        public IActionResult Details(int id) => View();
    }
}