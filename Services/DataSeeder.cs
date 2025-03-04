using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DataSeeder
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<Account> _userManager;
    private readonly DataContext _context;

    public DataSeeder(RoleManager<Role> roleManager, UserManager<Account> userManager, DataContext context)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
    }

    public async Task SeedRolesAndAdminAsync()
    {
        string[] roles = { "Admin", "Staff", "Customer", "Shipper" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new Role { Name = role });
            }
        }

        await CreateUserIfNotExists("admin@example.com", "Admin@123", "Admin");
        await CreateUserIfNotExists("shipper@example.com", "@1", "Shipper");
    }

    private async Task CreateUserIfNotExists(string email, string password, string role)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new Account { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }
    }

    public async Task SeedBrandsCategoriesProductsAsync()
    {
        if (!_context.Brands.Any())
        {
            var brands = new List<Brand>
        {
            new Brand { BrandName = "Nike", Description = "Thương hiệu giày và quần áo thể thao hàng đầu.", Disable = false },
            new Brand { BrandName = "Adidas", Description = "Thương hiệu thể thao nổi tiếng với ba sọc đặc trưng.", Disable = false },
            new Brand { BrandName = "Puma", Description = "Thương hiệu thời trang thể thao và giày thể thao.", Disable = false },
            new Brand { BrandName = "Reebok", Description = "Chuyên sản xuất giày thể thao và quần áo tập luyện.", Disable = false },
            new Brand { BrandName = "Under Armour", Description = "Thương hiệu chuyên về đồ thể thao và phụ kiện.", Disable = false },
            new Brand { BrandName = "New Balance", Description = "Hãng giày thể thao với thiết kế êm ái và thời trang.", Disable = false },
            new Brand { BrandName = "Fila", Description = "Thương hiệu thời trang thể thao với phong cách retro.", Disable = false },
            new Brand { BrandName = "ASICS", Description = "Hãng giày thể thao nổi tiếng với công nghệ Gel Cushioning.", Disable = false },
            new Brand { BrandName = "Champion", Description = "Thương hiệu thời trang streetwear và thể thao.", Disable = false },
            new Brand { BrandName = "Converse", Description = "Hãng giày nổi tiếng với dòng Chuck Taylor.", Disable = false }
        };

            _context.Brands.AddRange(brands);
            await _context.SaveChangesAsync();
        }

        if (!_context.Categories.Any())
        {
            var categories = new List<Category>
        {
            new Category { CategoryName = "Shoes", Description = "Các loại giày thể thao, sneaker, và giày casual.", Disable = false },
            new Category { CategoryName = "Shirts", Description = "Áo sơ mi, áo thun và áo thể thao.", Disable = false },
            new Category { CategoryName = "Pants", Description = "Quần dài, quần jeans và quần thể thao.", Disable = false },
            new Category { CategoryName = "Jackets", Description = "Áo khoác, hoodie và áo gió.", Disable = false },
            new Category { CategoryName = "Shorts", Description = "Quần short thể thao và thời trang.", Disable = false },
            new Category { CategoryName = "Socks", Description = "Các loại tất và phụ kiện liên quan.", Disable = false },
            new Category { CategoryName = "Accessories", Description = "Phụ kiện thời trang như kính, găng tay, túi xách.", Disable = false },
            new Category { CategoryName = "Hats", Description = "Mũ lưỡi trai, mũ len và mũ bucket.", Disable = false },
            new Category { CategoryName = "Sweatshirts", Description = "Áo nỉ dài tay và áo sweater.", Disable = false },
            new Category { CategoryName = "Gloves", Description = "Găng tay thời trang và găng tay thể thao.", Disable = false }
        };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        if (!_context.Products.Any())
        {
            var brands = await _context.Brands.ToListAsync();
            var categories = await _context.Categories.ToListAsync();

            var products = new List<Product>
            {
                new Product { ProductName = "Nike Air Max", Quantity = 50, Price = 120, BrandId = brands[0].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Leather", Image = "P1.jpeg", Discount = 10, Description = "Comfortable running shoes", Status = "Available" },
                new Product { ProductName = "Adidas Ultraboost", Quantity = 40, Price = 140, BrandId = brands[1].BrandId, CategoryId = categories[0].CategoryId, Size = "L", Material = "Mesh", Image = "P10.png", Discount = 5, Description = "Best for sports", Status = "Available" },
                new Product { ProductName = "Puma RS-X", Quantity = 30, Price = 110, BrandId = brands[2].BrandId, CategoryId = categories[0].CategoryId, Size = "S", Material = "Synthetic", Image = "P2.jpeg", Discount = 15, Description = "Stylish and modern", Status = "Available" },
                new Product { ProductName = "Reebok Classic", Quantity = 25, Price = 90, BrandId = brands[3].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Leather", Image = "P3.jpeg", Discount = 0, Description = "Classic design", Status = "Available" },
                new Product { ProductName = "Under Armour HOVR", Quantity = 35, Price = 130, BrandId = brands[4].BrandId, CategoryId = categories[0].CategoryId, Size = "L", Material = "Foam", Image = "P4.jpeg", Discount = 20, Description = "Great for training", Status = "Available" },
                new Product { ProductName = "New Balance 574", Quantity = 20, Price = 100, BrandId = brands[5].BrandId, CategoryId = categories[0].CategoryId, Size = "S", Material = "Suede", Image = "P5.jpeg", Discount = 5, Description = "Casual wear", Status = "Available" },
                new Product { ProductName = "Fila Disruptor", Quantity = 15, Price = 95, BrandId = brands[6].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Rubber", Image = "P6.jpeg", Discount = 10, Description = "Chunky sneakers", Status = "Available" },
                new Product { ProductName = "ASICS Gel-Kayano", Quantity = 28, Price = 150, BrandId = brands[7].BrandId, CategoryId = categories[0].CategoryId, Size = "L", Material = "Gel", Image = "P7.jpeg", Discount = 8, Description = "Top-tier running shoes", Status = "Available" },
                new Product { ProductName = "Champion Rally Pro", Quantity = 12, Price = 85, BrandId = brands[8].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Canvas", Image = "P8.jpeg", Discount = 12, Description = "High-top comfort", Status = "Available" },
                new Product { ProductName = "Converse Chuck Taylor", Quantity = 50, Price = 70, BrandId = brands[9].BrandId, CategoryId = categories[0].CategoryId, Size = "S", Material = "Textile", Image = "P9.jpeg", Discount = 0, Description = "Classic sneaker", Status = "Available" }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }
    }
}
