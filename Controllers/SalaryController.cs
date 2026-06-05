using BalloonShop.Data;
using BalloonShop.Models;
using BalloonShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BalloonShop.Controllers;

[Authorize]
public class SalaryController : Controller
{
    private readonly AppDbContext _context;
    private readonly SalaryService _salaryService;
    private readonly UserManager<AppUser> _userManager;

    public SalaryController(AppDbContext context, SalaryService salaryService, UserManager<AppUser> userManager)
    {
        _context = context;
        _salaryService = salaryService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var model = new SalaryRequestModel
        {
            DateFrom = DateTime.Today.AddMonths(-1),
            DateTo = DateTime.Today
        };

        if (User.IsInRole("Admin"))
        {
            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName");
        }
        else if (user?.EmployeeId != null)
        {
            // Seller can only view their own salary
            model.EmployeeId = user.EmployeeId.Value;
            var emp = await _context.Employees.FindAsync(user.EmployeeId);
            ViewBag.EmployeeName = emp?.FullName;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(SalaryRequestModel request)
    {
        var user = await _userManager.GetUserAsync(User);

        // Seller can only calculate for themselves
        if (!User.IsInRole("Admin"))
        {
            if (user?.EmployeeId == null)
                return Forbid();
            request.EmployeeId = user.EmployeeId.Value;
        }

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
