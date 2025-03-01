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
        string[] roles = { "Admin", "Staff", "Customer" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new Role { Name = role });
            }
        }

        string adminEmail = "admin@example.com";
        string adminPassword = "Admin@123";

        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new Account { UserName = adminEmail, Email = adminEmail };
            var result = await _userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public async Task SeedBrandsCategoriesProductsAsync()
    {
        if (!_context.Brands.Any())
        {
            var brands = new List<Brand>
            {
                new Brand { BrandName = "Nike" },
                new Brand { BrandName = "Adidas" },
                new Brand { BrandName = "Puma" },
                new Brand { BrandName = "Reebok" },
                new Brand { BrandName = "Under Armour" },
                new Brand { BrandName = "New Balance" },
                new Brand { BrandName = "Fila" },
                new Brand { BrandName = "ASICS" },
                new Brand { BrandName = "Champion" },
                new Brand { BrandName = "Converse" }
            };

            _context.Brands.AddRange(brands);
            await _context.SaveChangesAsync();
        }

        if (!_context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { CategoryName = "Shoes" },
                new Category { CategoryName = "Shirts" },
                new Category { CategoryName = "Pants" },
                new Category { CategoryName = "Jackets" },
                new Category { CategoryName = "Shorts" },
                new Category { CategoryName = "Socks" },
                new Category { CategoryName = "Accessories" },
                new Category { CategoryName = "Hats" },
                new Category { CategoryName = "Sweatshirts" },
                new Category { CategoryName = "Gloves" }
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
                new Product { ProductName = "Nike Air Max", Quantity = 50, Price = 120, BrandId = brands[0].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Leather", Image = "nike_air_max.jpg", Discount = 10, Description = "Comfortable running shoes", Status = "Available" },
                new Product { ProductName = "Adidas Ultraboost", Quantity = 40, Price = 140, BrandId = brands[1].BrandId, CategoryId = categories[0].CategoryId, Size = "L", Material = "Mesh", Image = "adidas_ultraboost.jpg", Discount = 5, Description = "Best for sports", Status = "Available" },
                new Product { ProductName = "Puma RS-X", Quantity = 30, Price = 110, BrandId = brands[2].BrandId, CategoryId = categories[0].CategoryId, Size = "S", Material = "Synthetic", Image = "puma_rsx.jpg", Discount = 15, Description = "Stylish and modern", Status = "Available" },
                new Product { ProductName = "Reebok Classic", Quantity = 25, Price = 90, BrandId = brands[3].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Leather", Image = "reebok_classic.jpg", Discount = 0, Description = "Classic design", Status = "Available" },
                new Product { ProductName = "Under Armour HOVR", Quantity = 35, Price = 130, BrandId = brands[4].BrandId, CategoryId = categories[0].CategoryId, Size = "L", Material = "Foam", Image = "ua_hovr.jpg", Discount = 20, Description = "Great for training", Status = "Available" },
                new Product { ProductName = "New Balance 574", Quantity = 20, Price = 100, BrandId = brands[5].BrandId, CategoryId = categories[0].CategoryId, Size = "S", Material = "Suede", Image = "nb_574.jpg", Discount = 5, Description = "Casual wear", Status = "Available" },
                new Product { ProductName = "Fila Disruptor", Quantity = 15, Price = 95, BrandId = brands[6].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Rubber", Image = "fila_disruptor.jpg", Discount = 10, Description = "Chunky sneakers", Status = "Available" },
                new Product { ProductName = "ASICS Gel-Kayano", Quantity = 28, Price = 150, BrandId = brands[7].BrandId, CategoryId = categories[0].CategoryId, Size = "L", Material = "Gel", Image = "asics_gel_kayano.jpg", Discount = 8, Description = "Top-tier running shoes", Status = "Available" },
                new Product { ProductName = "Champion Rally Pro", Quantity = 12, Price = 85, BrandId = brands[8].BrandId, CategoryId = categories[0].CategoryId, Size = "M", Material = "Canvas", Image = "champion_rally.jpg", Discount = 12, Description = "High-top comfort", Status = "Available" },
                new Product { ProductName = "Converse Chuck Taylor", Quantity = 50, Price = 70, BrandId = brands[9].BrandId, CategoryId = categories[0].CategoryId, Size = "S", Material = "Textile", Image = "converse_chuck.jpg", Discount = 0, Description = "Classic sneaker", Status = "Available" }
            };

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }
    }
}
