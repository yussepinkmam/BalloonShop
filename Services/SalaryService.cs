using BalloonShop.Data;
using BalloonShop.Models;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Services;

public class SalaryService
{
    private readonly AppDbContext _context;

    public SalaryService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SalaryCalculation> CalculateAsync(int employeeId, DateTime dateFrom, DateTime dateTo)
    {
        var employee = await _context.Employees
            .Include(e => e.Store)
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
            return new SalaryCalculation();

        var endOfDay = dateTo.Date.AddDays(1).AddTicks(-1);

        var sales = await _context.Sales
            .Include(s => s.Product)
            .Where(s => s.EmployeeId == employeeId
                     && s.SaleDate >= dateFrom.Date
                     && s.SaleDate <= endOfDay)
            .OrderBy(s => s.SaleDate)
            .ToListAsync();

        var totalSales = sales.Sum(s => s.TotalAmount);
        var totalItems = sales.Sum(s => s.Quantity);

        decimal salary;
        string description;

        if (employee.PaymentType == PaymentType.Salary)
        {
            salary = employee.Rate;
            description = $"Фиксированный оклад: {employee.Rate:N2} руб.";
        }
        else
        {
            salary = totalSales * (employee.Rate / 100m);
            description = $"{employee.Rate}% от суммы продаж ({totalSales:N2} руб.) = {salary:N2} руб.";
        }

        return new SalaryCalculation
        {
            Employee = employee,
            DateFrom = dateFrom,
            DateTo = dateTo,
            Sales = sales,
            TotalSalesAmount = totalSales,
            CalculatedSalary = salary,
            TotalItemsSold = totalItems,
            PaymentDescription = description
        };
    }
}
