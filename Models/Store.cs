using System.ComponentModel.DataAnnotations;

namespace BalloonShop.Models;

public class Store
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название магазина")]
    [Display(Name = "Название")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Введите адрес")]
    [Display(Name = "Адрес")]
    public string Address { get; set; } = "";

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
