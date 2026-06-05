using System.ComponentModel.DataAnnotations;

namespace BalloonShop.Models;

public class Category
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название категории")]
    [Display(Name = "Название")]
    public string Name { get; set; } = "";

    [Display(Name = "Порядок отображения")]
    public int DisplayOrder { get; set; }

    public ICollection<Product> Products { get; set; } = new List<Product>();
}
