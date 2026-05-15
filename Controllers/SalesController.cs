using BalloonShop.Data;
using BalloonShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

public class SalesController : Controller
{
    private readonly AppDbContext _context;

    public SalesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sales = await _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Employee)
            .Include(s => s.Store)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
        return View(sales);
    }

    public async Task<IActionResult> Details(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Product).Include(s => s.Employee).Include(s => s.Store)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();
        return View(sale);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View(new Sale { SaleDate = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Sale sale)
    {
        var product = await _context.Products.FindAsync(sale.ProductId);
        if (product == null)
        {
            ModelState.AddModelError("ProductId", "Товар не найден.");
        }
        else if (product.StockQuantity < sale.Quantity)
        {
            ModelState.AddModelError("Quantity", $"Недостаточно товара на складе. Доступно: {product.StockQuantity} шт.");
        }

        if (ModelState.IsValid && product != null)
        {
            sale.TotalAmount = product.SalePrice * sale.Quantity;
            product.StockQuantity -= sale.Quantity;

            _context.Sales.Add(sale);
            _context.Update(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Продажа успешно зарегистрирована.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdowns(sale);
        return View(sale);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return NotFound();
        await PopulateDropdowns(sale);
        return View(sale);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Sale sale)
    {
        if (id != sale.Id) return NotFound();

        var originalSale = await _context.Sales.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        var product = await _context.Products.FindAsync(sale.ProductId);

        if (product == null)
        {
            ModelState.AddModelError("ProductId", "Товар не найден.");
        }
        else if (originalSale != null)
        {
            // Return old quantity to stock, then check new quantity
            int adjustedStock = product.StockQuantity + (originalSale.ProductId == sale.ProductId ? originalSale.Quantity : 0);
            if (adjustedStock < sale.Quantity)
            {
                ModelState.AddModelError("Quantity", $"Недостаточно товара на складе. Доступно: {adjustedStock} шт.");
            }
        }

        if (ModelState.IsValid && product != null && originalSale != null)
        {
            // Restore old product stock if product changed
            if (originalSale.ProductId != sale.ProductId)
            {
                var oldProduct = await _context.Products.FindAsync(originalSale.ProductId);
                if (oldProduct != null)
                {
                    oldProduct.StockQuantity += originalSale.Quantity;
                    _context.Update(oldProduct);
                }
                product.StockQuantity -= sale.Quantity;
            }
            else
            {
                product.StockQuantity += originalSale.Quantity - sale.Quantity;
            }

            sale.TotalAmount = product.SalePrice * sale.Quantity;
            _context.Update(sale);
            _context.Update(product);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Продажа обновлена.";
            return RedirectToAction(nameof(Index));
        }

        await PopulateDropdowns(sale);
        return View(sale);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var sale = await _context.Sales
            .Include(s => s.Product).Include(s => s.Employee).Include(s => s.Store)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();
        return View(sale);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sale = await _context.Sales.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == id);
        if (sale != null)
        {
            // Restore stock
            if (sale.Product != null)
            {
                sale.Product.StockQuantity += sale.Quantity;
                _context.Update(sale.Product);
            }
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Продажа удалена.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(Sale? sale = null)
    {
        ViewBag.Products = new SelectList(await _context.Products.ToListAsync(), "Id", "Name", sale?.ProductId);
        ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", sale?.EmployeeId);
        ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", sale?.StoreId);
    }
}
