using BalloonShop.Data;
using BalloonShop.Models;
using BalloonShop.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

public class SalaryController : Controller
{
    private readonly AppDbContext _context;
    private readonly SalaryService _salaryService;

    public SalaryController(AppDbContext context, SalaryService salaryService)
    {
        _context = context;
        _salaryService = salaryService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");
        var model = new SalaryRequestModel
        {
            DateFrom = DateTime.Today.AddMonths(-1),
            DateTo = DateTime.Today
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(SalaryRequestModel request)
    {
        ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", request.EmployeeId);

        if (request.DateFrom > request.DateTo)
        {
            ModelState.AddModelError("DateFrom", "Дата начала не может быть позже даты окончания.");
            return View("Index", request);
        }

        var result = await _salaryService.CalculateAsync(request.EmployeeId, request.DateFrom, request.DateTo);
        ViewBag.Request = request;
        return View("Result", result);
    }
}
