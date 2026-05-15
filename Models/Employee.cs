using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BalloonShop.Models;

public enum PaymentType
{
    [Display(Name = "Оклад")]
    Salary,
    [Display(Name = "Процент от продаж")]
    Percentage
}

public class Employee
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите ФИО")]
    [Display(Name = "ФИО")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Введите должность")]
    [Display(Name = "Должность")]
    public string Position { get; set; } = "";

    [Display(Name = "Тип оплаты")]
    public PaymentType PaymentType { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Ставка должна быть больше 0")]
    [Display(Name = "Ставка (руб. или %)")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Rate { get; set; }

    [Display(Name = "Магазин")]
    public int? StoreId { get; set; }
    public Store? Store { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
