using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class BoardsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Create() => View();
    }
}