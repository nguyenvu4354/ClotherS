using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClotherS.Models;
using System.Net.Mail;
using System.Net;
using ClotherS.Repositories;

namespace ClotherS.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DataContext _context;
        private readonly IConfiguration _config;

        public AuthenticationController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (account != null && BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, account.Email),
                    new Claim("FullName", account.FirstName + " " + account.LastName),
                    new Claim(ClaimTypes.Role, account.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account account)
        {
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == account.Email);
            if (existingAccount != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(account);
            }

            if (ModelState.IsValid)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(account);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);
            if (account == null)
            {
                ViewBag.Message = "Email không tồn tại.";
                return View();
            }

            account.ResetPasswordToken = Guid.NewGuid().ToString();
            account.ResetPasswordExpiry = DateTime.UtcNow.AddHours(1);
            await _context.SaveChangesAsync();

            var resetLink = Url.Action("ResetPassword", "Authentication", new { token = account.ResetPasswordToken }, Request.Scheme);

            var smtpServer = _config["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);
            var smtpUsername = _config["EmailSettings:SmtpUsername"];
            var smtpPassword = _config["EmailSettings:SmtpPassword"];
            var senderEmail = _config["EmailSettings:SenderEmail"];
            var senderName = _config["EmailSettings:SenderName"];

            try
            {
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(smtpUsername, smtpPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = "Password Reset",
                    Body = $"Click vào link để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại mật khẩu</a>",
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await smtpClient.SendMailAsync(mailMessage);

                ViewBag.Message = "Email đặt lại mật khẩu đã được gửi!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Không thể gửi email: " + ex.Message;
            }

            return View();
        }

        public async Task<IActionResult> ResetPassword(string token)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.ResetPasswordToken == token && a.ResetPasswordExpiry > DateTime.UtcNow);
            if (account == null)
            {
                return NotFound("Token không hợp lệ hoặc đã hết hạn.");
            }

            return View(new ResetPasswordViewModel { Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(model);
            }

            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.ResetPasswordToken == model.Token && a.ResetPasswordExpiry > DateTime.UtcNow);
            if (account == null)
            {
                return NotFound("Token không hợp lệ hoặc đã hết hạn.");
            }

            account.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            account.ResetPasswordToken = null;
            account.ResetPasswordExpiry = null;
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
    }
}
