using BalloonShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var totalSales = await _context.Sales.SumAsync(s => s.TotalAmount);
        var totalItems = await _context.Sales.SumAsync(s => s.Quantity);

        var topEmployees = await _context.Sales
            .Include(s => s.Employee)
            .GroupBy(s => new { s.EmployeeId, s.Employee!.FullName, s.Employee.Position })
            .Select(g => new
            {
                g.Key.FullName,
                g.Key.Position,
                TotalAmount = g.Sum(s => s.TotalAmount),
                TotalItems = g.Sum(s => s.Quantity)
            })
            .OrderByDescending(x => x.TotalAmount)
            .Take(3)
            .ToListAsync();

        var storeStats = await _context.Stores
            .Include(s => s.Sales)
            .Select(s => new
            {
                s.Name,
                TotalAmount = s.Sales.Sum(sale => sale.TotalAmount),
                TotalItems = s.Sales.Sum(sale => sale.Quantity)
            })
            .ToListAsync();

        ViewBag.TotalSales = totalSales;
        ViewBag.TotalItems = totalItems;
        ViewBag.TopEmployees = topEmployees;
        ViewBag.StoreStats = storeStats;

        return View();
    }
}
