using System.ComponentModel.DataAnnotations;

namespace Pinterest111.Models
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Укажите имя.")]
        [Display(Name = "Имя и фамилия")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Укажите имя пользователя.")]
        [RegularExpression(@"^[a-zA-Z0-9_]{3,20}$", ErrorMessage = "Только латиница, цифры и _, от 3 до 20 символов.")]
        [Display(Name = "Имя пользователя")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Укажите email.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        public string Email { get; set; } = "";

        [MaxLength(300, ErrorMessage = "Описание не длиннее 300 символов.")]
        [Display(Name = "О себе")]
        public string? Bio { get; set; }

        [Display(Name = "Новое фото профиля")]
        public IFormFile? Avatar { get; set; }

        public string CurrentAvatarUrl { get; set; } = "/img/default-avatar.png";
    }
}