using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pinterest111.Models;
using Pinterest111.Services;
using System.Security.Claims;

namespace Pinterest111.Controllers
{
    public class ProfileController : Controller
    {
        private readonly FileUserStore _users;
        private readonly FilePinStore _pins;
        private readonly IWebHostEnvironment _env;

        public ProfileController(FileUserStore users, FilePinStore pins, IWebHostEnvironment env)
        {
            _users = users;
            _pins = pins;
            _env = env;
        }

        public IActionResult Index(string username)
        {
            var user = _users.FindByUsernameOrEmail(username ?? "");
            if (user == null) return NotFound();

            var pins = _pins.GetByAuthor(user.Username);

            var vm = new ProfileViewModel
            {
                User = user,
                Pins = pins,
                IsOwnProfile = string.Equals(User.Identity?.Name, user.Username, StringComparison.OrdinalIgnoreCase)
            };

            return View(vm);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit()
        {
            var user = _users.FindByUsernameOrEmail(User.Identity!.Name!);
            if (user == null) return NotFound();

            var vm = new EditProfileViewModel
            {
                FullName = user.FullName,
                Username = user.Username,
                Email = user.Email,
                Bio = user.Bio,
                CurrentAvatarUrl = user.AvatarUrl
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileViewModel model)
        {
            var user = _users.FindByUsernameOrEmail(User.Identity!.Name!);
            if (user == null) return NotFound();

            if (_users.UsernameOrEmailTakenByAnotherUser(user.Id, model.Username, model.Email))
            {
                ModelState.AddModelError(string.Empty, "Это имя пользователя или email уже заняты.");
            }

            if (!ModelState.IsValid)
            {
                model.CurrentAvatarUrl = user.AvatarUrl;
                return View(model);
            }

            var oldUsername = user.Username;

            var avatarUrl = user.AvatarUrl;
            if (model.Avatar != null && model.Avatar.Length > 0)
            {
                avatarUrl = await SaveUploadedFileAsync(model.Avatar, "avatars");
            }

            user.FullName = model.FullName.Trim();
            user.Username = model.Username.Trim();
            user.Email = model.Email.Trim();
            user.Bio = model.Bio?.Trim() ?? "";
            user.AvatarUrl = avatarUrl;

            _users.Update(user);

            // Если username изменился — переносим авторство постов и обновляем сессию
            if (!string.Equals(oldUsername, user.Username, StringComparison.OrdinalIgnoreCase))
            {
                _pins.UpdateAuthorUsername(oldUsername, user.Username);
            }

            await RefreshSignInAsync(user);

            return RedirectToAction("Index", new { username = user.Username });
        }

        private async Task RefreshSignInAsync(User user)
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