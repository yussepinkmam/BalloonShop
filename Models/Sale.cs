using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalloonShop.Models;

public class Sale
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Дата продажи")]
    [DataType(DataType.Date)]
    public DateTime SaleDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "Товар")]
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Количество должно быть не менее 1")]
    [Display(Name = "Количество")]
    public int Quantity { get; set; }

    [Required]
    [Display(Name = "Сотрудник")]
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Display(Name = "Магазин")]
    public int? StoreId { get; set; }
    public Store? Store { get; set; }

    [Display(Name = "Итоговая сумма")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }
}
