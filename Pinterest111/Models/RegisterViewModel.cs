using System.ComponentModel.DataAnnotations;

namespace Pinterest111.Models
{
    public class RegisterViewModel
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

        [Required(ErrorMessage = "Укажите пароль.")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не короче 6 символов.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "Подтвердите пароль.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
        [Display(Name = "Подтверждение пароля")]
        public string ConfirmPassword { get; set; } = "";

        public IFormFile? Avatar { get; set; }
    }
}
