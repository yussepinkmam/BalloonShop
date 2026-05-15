namespace BalloonShop.Models;

public class SalaryCalculation
{
    public Employee? Employee { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public List<Sale> Sales { get; set; } = new();
    public decimal TotalSalesAmount { get; set; }
    public decimal CalculatedSalary { get; set; }
    public int TotalItemsSold { get; set; }
    public string PaymentDescription { get; set; } = "";
}

public class SalaryRequestModel
{
    public int EmployeeId { get; set; }
    public DateTime DateFrom { get; set; } = DateTime.Today.AddMonths(-1);
    public DateTime DateTo { get; set; } = DateTime.Today;
}
