using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinterest111.Models;
using Pinterest111.Services;
using System.Net.NetworkInformation;

namespace Pinterest111.Controllers
{
    public class PinsController : Controller
    {
        private readonly FilePinStore _pins;
        private readonly IWebHostEnvironment _env;

        public PinsController(FilePinStore pins, IWebHostEnvironment env)
        {
            _pins = pins;
            _env = env;
        }

        public IActionResult Index()
        {
            var pins = _pins.GetAllNewestFirst();
            return View(pins);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new PinCreateViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PinCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var imageUrl = await SaveUploadedFileAsync(model.Image!, "pins");

            var pin = new Pin
            {
                Title = model.Title.Trim(),
                Description = model.Description?.Trim() ?? "",
                ImageUrl = imageUrl,
                Author = User.Identity!.Name!,
                CreatedAt = DateTime.UtcNow
            };

            _pins.Add(pin);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Details(int id)
        {
            var pin = _pins.GetById(id);
            if (pin == null) return NotFound();
            return View(pin);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var pin = _pins.GetById(id);
            if (pin == null) return NotFound();

            // Удалять пин может только его автор
            if (!string.Equals(pin.Author, User.Identity!.Name, StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            _pins.Delete(id);

            // Пытаемся удалить файл изображения, если он лежит в uploads
            if (!string.IsNullOrWhiteSpace(pin.ImageUrl) && pin.ImageUrl.StartsWith("/uploads/"))
            {
                try
                {
                    var imagePath = Path.Combine(_env.WebRootPath, pin.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                catch
                {
                    // Не критично, если файл не получилось удалить
                }
            }

            TempData["Message"] = "Пин удалён";

            // Возвращаемся туда, откуда пришли (профиль, главная и т.д.), если это безопасный локальный адрес
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && Url.IsLocalUrl(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task<string> SaveUploadedFileAsync(IFormFile file, string subfolder)
        {
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadsDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subfolder}/{fileName}";
        }
    }
}