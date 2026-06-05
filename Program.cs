using BalloonShop.Data;
using BalloonShop.Models;
using BalloonShop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrEmpty(connStr))
        options.UseSqlServer(connStr);
    else
        options.UseInMemoryDatabase("BalloonShopDb");
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddScoped<SalaryService>();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    // Create roles
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    if (!await roleManager.RoleExistsAsync("Seller"))
        await roleManager.CreateAsync(new IdentityRole("Seller"));

    // Create admin user
    if (await userManager.FindByEmailAsync("admin@balloonshop.ru") == null)
    {
        var admin = new AppUser { UserName = "admin@balloonshop.ru", Email = "admin@balloonshop.ru" };
        var result = await userManager.CreateAsync(admin, "123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, "Admin");
    }

    // Create seller linked to Employee 1 (Иванова Мария)
    if (await userManager.FindByEmailAsync("maria@balloonshop.ru") == null)
    {
        var seller1 = new AppUser { UserName = "maria@balloonshop.ru", Email = "maria@balloonshop.ru", EmployeeId = 1 };
        var result = await userManager.CreateAsync(seller1, "123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(seller1, "Seller");
    }

    // Create seller linked to Employee 2 (Петров Алексей)
    if (await userManager.FindByEmailAsync("alex@balloonshop.ru") == null)
    {
        var seller2 = new AppUser { UserName = "alex@balloonshop.ru", Email = "alex@balloonshop.ru", EmployeeId = 2 };
        var result = await userManager.CreateAsync(seller2, "123");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(seller2, "Seller");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
