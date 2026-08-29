using Microsoft.AspNetCore.Mvc;

namespace Pinterest111.Controllers
{
    public class SearchController : Controller
    {
        public IActionResult Results(string q) => View();
    }
}