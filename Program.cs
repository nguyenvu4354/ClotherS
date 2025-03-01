﻿using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// **1. Cấu hình MVC**
builder.Services.AddControllersWithViews();

// **2. Cấu hình Session**
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// **3. Cấu hình DbContext (EF Core với SQL Server)**
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectedDB")));

// **4. Cấu hình Identity**
builder.Services.AddIdentity<Account, Role>()
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// **5. Đăng ký DataSeeder**
builder.Services.AddScoped<DataSeeder>();

// **6. Cấu hình Authentication & Cookie**
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Accounts/Login";  // Trang đăng nhập
    options.AccessDeniedPath = "/Accounts/AccessDenied"; // Trang từ chối truy cập
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// **7. Cấu hình Authorization (Phân quyền dựa trên Role)**
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
});

// **8. Xây dựng ứng dụng**
var app = builder.Build();

// **9. Gọi DataSeeder để tạo Role & Admin mặc định**
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await dataSeeder.SeedRolesAndAdminAsync();
    await dataSeeder.SeedBrandsCategoriesProductsAsync();
}

// **10. Cấu hình xử lý lỗi**
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Trang lỗi production
    app.UseHsts(); // Tăng cường bảo mật HSTS
}
else
{
    app.UseDeveloperExceptionPage(); // Hiển thị lỗi khi phát triển
}

// **11. Cấu hình HTTPS & file tĩnh**
app.UseHttpsRedirection();
app.UseStaticFiles();

// **12. Middleware: Authentication, Session, Routing, Authorization**
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// **13. Cấu hình route cho Areas (Admin)**
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// **14. Cấu hình route mặc định**
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// **15. Chạy ứng dụng**
app.Run();
