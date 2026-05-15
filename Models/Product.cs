using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalloonShop.Models;

public enum BalloonType
{
    [Display(Name = "Латексный")]
    Latex,
    [Display(Name = "Фольгированный")]
    Foil,
    [Display(Name = "С рисунком")]
    Printed
}

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название")]
    [Display(Name = "Название")]
    public string Name { get; set; } = "";

    [Display(Name = "Тип")]
    public BalloonType Type { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
    [Display(Name = "Цена закупки")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
    [Display(Name = "Цена продажи")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal SalePrice { get; set; }

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Остаток не может быть отрицательным")]
    [Display(Name = "Остаток на складе")]
    public int StockQuantity { get; set; }

    [Display(Name = "Магазин")]
    public int? StoreId { get; set; }
    public Store? Store { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
