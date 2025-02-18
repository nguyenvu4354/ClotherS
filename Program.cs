using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options => 
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
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

// Cấu hình Session Middleware
app.UseSession();

// Cấu hình routing và Authorization
app.UseRouting();
app.UseAuthorization();

// Thiết lập routing mặc định cho các Controller
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "Areas",
    pattern: "{area:exists}/{controller=Product}/{action=Index}/{id?}");

// Chạy ứng dụng
app.Run();

// Phương thức SeedData để chèn dữ liệu mẫu
void SeedData(DataContext context)
{
    if (!context.Roles.Any())
    {
        var roles = new List<Role>
        {
            new Role { RoleName = "Admin" },
            new Role { RoleName = "Customer" },
            new Role { RoleName = "Staff" }
        };

        context.Roles.AddRange(roles);
        context.SaveChanges();
    }


    // Seed dữ liệu cho bảng Product
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
    }

    // Seed dữ liệu cho bảng Account
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
                RoleId = 2,
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
            },
            new Account
            {
                Email = "user3@example.com",
                FirstName = "Lê",
                LastName = "Văn C",
                Phone = "0777888999",
                Password = "@1",
                AccountImage = "avatar3.jpg",
                Address = "Đà Nẵng",
                Gender = "Male",
                Active = true,
                Description = "Người dùng mới",
                RoleId = 1,
                DateOfBirth = "2000-02-20",
                Disable = false
            },
            new Account
            {
                Email = "user4@example.com",
                FirstName = "Phạm",
                LastName = "Thị D",
                Phone = "0345678912",
                Password = "@1",
                AccountImage = "avatar4.jpg",
                Address = "Hải Phòng",
                Gender = "Female",
                Active = false,
                Description = "Chưa kích hoạt",
                RoleId = 2,
                DateOfBirth = "1988-11-30",
                Disable = false
            },
            new Account
            {
                Email = "user5@example.com",
                FirstName = "Hoàng",
                LastName = "Văn E",
                Phone = "0912345678",
                Password = "@1",
                AccountImage = "avatar5.jpg",
                Address = "Cần Thơ",
                Gender = "Male",
                Active = true,
                Description = "Khách hàng thân thiết",
                RoleId = 3,
                DateOfBirth = "1997-06-25",
                Disable = false
            }
        };

        context.Accounts.AddRange(fakeAccounts);
    }

    context.SaveChanges();
}

