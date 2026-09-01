using System.ComponentModel.DataAnnotations;

namespace Pinterest111.Models
{
    public class BoardCreateViewModel
    {
        [Required(ErrorMessage = "Укажите название доски.")]
        [Display(Name = "Название")]
        public string Title { get; set; } = "";

        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
