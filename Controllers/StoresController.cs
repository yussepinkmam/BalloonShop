using BalloonShop.Data;
using BalloonShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

[Authorize(Roles = "Admin")]
public class StoresController : Controller
{
    private readonly AppDbContext _context;

    public StoresController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var stores = await _context.Stores
            .Include(s => s.Sales)
            .Include(s => s.Employees)
            .Include(s => s.Products)
            .ToListAsync();
        return View(stores);
    }

    public async Task<IActionResult> Details(int id)
    {
        var store = await _context.Stores
            .Include(s => s.Sales).ThenInclude(sale => sale.Product)
            .Include(s => s.Employees)
            .Include(s => s.Products)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (store == null) return NotFound();
        return View(store);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Store store)
    {
        if (ModelState.IsValid)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Магазин добавлен.";
            return RedirectToAction(nameof(Index));
        }
        return View(store);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Store store)
    {
        if (id != store.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(store);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Магазин обновлён.";
            return RedirectToAction(nameof(Index));
        }
        return View(store);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store == null) return NotFound();
        return View(store);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var store = await _context.Stores.FindAsync(id);
        if (store != null)
        {
            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Магазин удалён.";
        }
        return RedirectToAction(nameof(Index));
    }
}
