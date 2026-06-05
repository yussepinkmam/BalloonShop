using BalloonShop.Data;
using BalloonShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

[Authorize]
public class SalesController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public SalesController(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var query = _context.Sales
            .Include(s => s.Product)
            .Include(s => s.Employee)
            .Include(s => s.Store)
            .AsQueryable();

        // Seller sees only their own sales
        if (!User.IsInRole("Admin") && user?.EmployeeId != null)
            query = query.Where(s => s.EmployeeId == user.EmployeeId);

        return View(await query.OrderByDescending(s => s.SaleDate).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        var sale = await _context.Sales
            .Include(s => s.Product).Include(s => s.Employee).Include(s => s.Store)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (sale == null) return NotFound();

        // Seller can only view their own sales
        if (!User.IsInRole("Admin") && user?.EmployeeId != sale.EmployeeId)
            return Forbid();

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
            // Auto-set employee for seller users
            var user = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("Admin") && user?.EmployeeId != null)
                sale.EmployeeId = user.EmployeeId.Value;

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

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int id)
    {
        var sale = await _context.Sales.FindAsync(id);
        if (sale == null) return NotFound();
        await PopulateDropdowns(sale);
        return View(sale);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
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
            int adjustedStock = product.StockQuantity + (originalSale.ProductId == sale.ProductId ? originalSale.Quantity : 0);
            if (adjustedStock < sale.Quantity)
            {
                ModelState.AddModelError("Quantity", $"Недостаточно товара на складе. Доступно: {adjustedStock} шт.");
            }
        }

        if (ModelState.IsValid && product != null && originalSale != null)
        {
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

    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var sale = await _context.Sales.Include(s => s.Product).FirstOrDefaultAsync(s => s.Id == id);
        if (sale != null)
        {
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
