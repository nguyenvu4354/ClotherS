using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Cấu hình Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Accounts/Login";
        options.AccessDeniedPath = "/Accounts/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("1"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("2"));
    options.AddPolicy("CustomerOnly", policy => policy.RequireRole("3"));
});

// Cấu hình DbContext
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:ConnectedDB"]);
});

// Xây dựng ứng dụng
var app = builder.Build();

// Lấy dịch vụ DbContext để seeding dữ liệu
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    context.Database.Migrate(); // Áp dụng migration (nếu có)
    SeedData(context); // Gọi phương thức seeding
}

// Kiểm tra môi trường để xử lý lỗi
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Quản lý lỗi cho môi trường sản xuất
    app.UseHsts();  // Kích hoạt HSTS trong môi trường sản xuất
}
else
{
    app.UseDeveloperExceptionPage(); // Hiển thị lỗi trong môi trường phát triển
}

// Cấu hình HTTPS và các file tĩnh
app.UseHttpsRedirection();
app.UseStaticFiles();

// Cấu hình Authentication và Session Middleware
app.UseAuthentication();
app.UseSession();

// Cấu hình routing và Authorization
app.UseRouting();
app.UseAuthorization();

// Thiết lập routing mặc định cho các Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// Chạy ứng dụng
app.Run();

// Phương thức SeedData để chèn dữ liệu mẫu
void SeedData(DataContext context)
{
    if (!context.Roles.Any())
    {
        var roles = new List<Role>
        {
            new Role { RoleName = "Admin", Disable = false },
            new Role { RoleName = "Customer", Disable = false },
            new Role { RoleName = "Staff", Disable = false },
            new Role { RoleName = "Shipper", Disable = false }
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }

    // Seed dữ liệu cho bảng Category
    if (!context.Categories.Any())
    {
        var categories = new List<Category>
        {
            new Category { CategoryName = "Áo Thun" },
            new Category { CategoryName = "Quần Jean" },
            new Category { CategoryName = "Áo Khoác" },
            new Category { CategoryName = "Giày Dép" },
            new Category { CategoryName = "Túi Xách" },
            new Category { CategoryName = "Phụ Kiện" },
            new Category { CategoryName = "Đồng Hồ" },
            new Category { CategoryName = "Đồ Lót" },
            new Category { CategoryName = "Váy Đầm" },
            new Category { CategoryName = "Áo Sơ Mi" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();
    }

    // Seed dữ liệu cho bảng Brand
    if (!context.Brands.Any())
    {
        var brands = new List<Brand>
        {
            new Brand { BrandName = "Nike" },
            new Brand { BrandName = "Adidas" },
            new Brand { BrandName = "Puma" },
            new Brand { BrandName = "Gucci" },
            new Brand { BrandName = "Louis Vuitton" },
            new Brand { BrandName = "Hermès" },
            new Brand { BrandName = "Chanel" },
            new Brand { BrandName = "Rolex" },
            new Brand { BrandName = "Zara" },
            new Brand { BrandName = "H&M" }
        };

        context.Brands.AddRange(brands);
        context.SaveChanges();
    }

    // Seed dữ liệu cho bảng Product (nếu chưa có)
    if (!context.Products.Any())
    {
        var fakeProducts = new List<Product>
        {
            new Product
            {
                ProductName = "Áo Thun Nam",
                Quantity = 50,
                Price = 250000,
                BrandId = 1,
                Image = "product1.jpg",
                Material = "Cotton",
                CategoryId = 1,
                Size = "M",
                Discount = 10,
                Description = "Áo thun nam thoáng mát, phù hợp cho mùa hè.",
                Disable = false,
                Status = "Available"
            },
            new Product
            {
                ProductName = "Quần Jean Nữ",
                Quantity = 30,
                Price = 450000,
                BrandId = 2,
                Image = "product2.jpg",
                Material = "Jean",
                CategoryId = 2,
                Size = "S",
                Discount = 15,
                Description = "Quần jean nữ phong cách Hàn Quốc.",
                Disable = false,
                Status = "Available"
            },
            new Product
            {
                ProductName = "Áo Khoác Hoodie",
                Quantity = 20,
                Price = 550000,
                BrandId = 3,
                Image = "product3.jpg",
                Material = "Nỉ",
                CategoryId = 3,
                Size = "L",
                Discount = 20,
                Description = "Áo khoác hoodie form rộng, thích hợp cho mùa đông.",
                Disable = false,
                Status = "Available"
            }
        };

        context.Products.AddRange(fakeProducts);
        context.SaveChanges();
    }

    // Seed dữ liệu cho bảng Account (nếu chưa có)
    if (!context.Accounts.Any())
    {
        var fakeAccounts = new List<Account>
        {
            new Account
            {
                Email = "user1@example.com",
                FirstName = "Nguyễn",
                LastName = "Văn A",
                Phone = "0123456789",
                Password = "@1",
                AccountImage = "avatar1.jpg",
                Address = "Hà Nội",
                Gender = "Male",
                Active = true,
                Description = "Khách hàng thường xuyên",
                RoleId = 1,
                DateOfBirth = "1995-05-10",
                Disable = false
            },
            new Account
            {
                Email = "user2@example.com",
                FirstName = "Trần",
                LastName = "Thị B",
                Phone = "0987654321",
                Password = "@1",
                AccountImage = "avatar2.jpg",
                Address = "TP.HCM",
                Gender = "Female",
                Active = true,
                Description = "Khách hàng VIP",
                RoleId = 3,
                DateOfBirth = "1992-08-15",
                Disable = false
            }
        };

        context.Accounts.AddRange(fakeAccounts);
        context.SaveChanges();
    }
}

