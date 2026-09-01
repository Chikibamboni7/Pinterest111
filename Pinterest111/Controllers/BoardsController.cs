using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinterest111.Models;
using Pinterest111.Services;

namespace Pinterest111.Controllers
{
    public class BoardsController : Controller
    {
        private readonly FileBoardStore _boards;
        private readonly FilePinStore _pins;

        public BoardsController(FileBoardStore boards, FilePinStore pins)
        {
            _boards = boards;
            _pins = pins;
        }

        public IActionResult Index()
        {
            var list = _boards.GetAllNewestFirst();
            return View(list);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new BoardCreateViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BoardCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var board = new Board
            {
                Title = model.Title.Trim(),
                Description = model.Description?.Trim() ?? "",
                Author = User.Identity!.Name!,
                CreatedAt = DateTime.UtcNow
            };

            _boards.Add(board);

            return RedirectToAction("Details", new { id = board.Id });
        }

        public IActionResult Details(int id)
        {
            var board = _boards.GetById(id);
            if (board == null) return NotFound();

            var pins = board.PinIds.Select(pid => _pins.GetById(pid)).Where(p => p != null).Cast<Pin>().ToList();

            var vm = new BoardDetailsViewModel
            {
                Board = board,
                Pins = pins,
                IsOwner = string.Equals(User.Identity?.Name, board.Author, StringComparison.OrdinalIgnoreCase)
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPin(int boardId, int pinId)
        {
            var board = _boards.GetById(boardId);
            if (board == null) return NotFound();

            // добавлять в доску может только её владелец
            if (!string.Equals(board.Author, User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var pin = _pins.GetById(pinId);
            if (pin == null) return NotFound();

            _boards.AddPinToBoard(boardId, pinId);

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
                return Redirect(referer);

            return RedirectToAction("Details", new { id = boardId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemovePin(int boardId, int pinId)
        {
            var board = _boards.GetById(boardId);
            if (board == null) return NotFound();

            if (!string.Equals(board.Author, User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            _boards.RemovePinFromBoard(boardId, pinId);

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
                return Redirect(referer);

            return RedirectToAction("Details", new { id = boardId });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var board = _boards.GetById(id);
            if (board == null) return NotFound();

            if (!string.Equals(board.Author, User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            _boards.Delete(id);

            return RedirectToAction("Index", "Profile", new { username = User.Identity!.Name });
        }
    }
}