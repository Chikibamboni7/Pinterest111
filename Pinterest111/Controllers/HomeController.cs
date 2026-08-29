using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pinterest111.Models;
using Pinterest111.Services;

namespace Pinterest111.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FilePinStore _pins;

        public HomeController(ILogger<HomeController> logger, FilePinStore pins)
        {
            _logger = logger;
            _pins = pins;
        }

        public IActionResult Index()
        {
            var pins = _pins.GetAllNewestFirst();
            return View(pins);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
