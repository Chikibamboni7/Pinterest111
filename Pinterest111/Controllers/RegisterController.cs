using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pinterest111.Models;
using Pinterest111.Services;
using System.Security.Claims;

namespace Pinterest111.Controllers
{
    public class RegisterController : Controller
    {
        private readonly FileUserStore _users;
        private readonly IWebHostEnvironment _env;

        public RegisterController(FileUserStore users, IWebHostEnvironment env)
        {
            _users = users;
            _env = env;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Register", new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (_users.UsernameOrEmailExists(model.Username, model.Email))
            {
                ModelState.AddModelError(string.Empty, "Пользователь с таким именем или email уже существует.");
            }

            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            var avatarUrl = "/img/default-avatar.png";
            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                avatarUrl = await SaveUploadedFileAsync(model.Avatar, "avatars");
            }

            var (hash, salt) = PasswordHasher.Hash(model.Password);

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Username = model.Username.Trim(),
                Email = model.Email.Trim(),
                PasswordHash = hash,
                Salt = salt,
                AvatarUrl = avatarUrl,
                CreatedAt = DateTime.UtcNow
            };

            _users.Add(user);

            await SignInAsync(user);

            return RedirectToAction("Index", "Home");
        }

        private async Task SignInAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FullName", user.FullName),
                new Claim("AvatarUrl", user.AvatarUrl)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
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
