using System.ComponentModel.DataAnnotations;

namespace Pinterest111.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Укажите email или имя пользователя.")]
        [Display(Name = "Email или имя пользователя")]
        public string Identifier { get; set; } = "";

        [Required(ErrorMessage = "Укажите пароль.")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = "";

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
