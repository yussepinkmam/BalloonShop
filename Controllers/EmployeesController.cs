using BalloonShop.Data;
using BalloonShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

[Authorize(Roles = "Admin")]
public class EmployeesController : Controller
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees.Include(e => e.Store).ToListAsync();
        return View(employees);
    }

    public async Task<IActionResult> Details(int id)
    {
        var employee = await _context.Employees.Include(e => e.Store).FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Employee employee)
    {
        if (ModelState.IsValid)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Сотрудник добавлен.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", employee.StoreId);
        return View(employee);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return NotFound();
        ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", employee.StoreId);
        return View(employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Employee employee)
    {
        if (id != employee.Id) return NotFound();
        if (ModelState.IsValid)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Данные сотрудника обновлены.";
            return RedirectToAction(nameof(Index));
        }
        ViewBag.Stores = new SelectList(await _context.Stores.ToListAsync(), "Id", "Name", employee.StoreId);
        return View(employee);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees.Include(e => e.Store).FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return NotFound();
        return View(employee);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Сотрудник удалён.";
        }
        return RedirectToAction(nameof(Index));
    }
}
