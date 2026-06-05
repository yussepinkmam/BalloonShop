using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalloonShop.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название товара")]
    [Display(Name = "Название")]
    public string Name { get; set; } = "";

    [Display(Name = "Категория")]
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена закупки должна быть больше 0")]
    [Display(Name = "Цена закупки")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена продажи должна быть больше 0")]
    [Display(Name = "Цена продажи")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SalePrice { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    [Display(Name = "Остаток на складе")]
    public int StockQuantity { get; set; }

    [Display(Name = "Магазин")]
    public int? StoreId { get; set; }
    public Store? Store { get; set; }
}
