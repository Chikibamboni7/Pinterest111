using Microsoft.AspNetCore.Mvc;
using Pinterest111.Models;
using Pinterest111.Services;

namespace Pinterest111.Controllers
{
    public class SearchController : Controller
    {
        private readonly FilePinStore _pins;
        private readonly FileUserStore _users;

        public SearchController(FilePinStore pins, FileUserStore users)
        {
            _pins = pins;
            _users = users;
        }

        public IActionResult Results(string q)
        {
            var query = (q ?? "").Trim();

            var vm = new SearchResultsViewModel { Query = query };

            if (!string.IsNullOrEmpty(query))
            {
                vm.Pins = _pins.GetAll()
                    .Where(p =>
                        p.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        p.Description.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

                vm.Users = _users.GetAll()
                    .Where(u =>
                        u.Username.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        u.FullName.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return View(vm);
        }
    }
}