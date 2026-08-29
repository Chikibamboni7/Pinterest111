using System.ComponentModel.DataAnnotations;

namespace Pinterest111.Models
{
    public class PinCreateViewModel
    {
        [Required(ErrorMessage = "Укажите название.")]
        [Display(Name = "Название")]
        public string Title { get; set; } = "";

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Выберите изображение.")]
        [Display(Name = "Изображение")]
        public IFormFile? Image { get; set; }
    }
}
