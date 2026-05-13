using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using tsp.Contexts;
using tsp.Models;
using tsp.Services;

namespace tsp.Controllers;

public class AccountController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly FileDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public AccountController(IConfiguration configuration, FileDbContext context, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _context = context;
        _environment = environment;
    }

    [AllowAnonymous]
    public IActionResult Register() => View(new AuthViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(AuthViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        if (await _context.Accounts.AnyAsync(a => a.Email == normalizedEmail))
        {
            ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");
            return View(model);
        }

        var account = new Account
        {
            Email = normalizedEmail,
            PasswordHash = PasswordHashService.Hash(model.Password)
        };

        await _context.Accounts.AddAsync(account);
        await _context.SaveChangesAsync();

        SignIn(account);
        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult Login() => View(new AuthViewModel());

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AuthViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == normalizedEmail);
        if (account == null || !PasswordHashService.Verify(model.Password, account.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(model);
        }

        SignIn(account);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("tsp_auth");
        return RedirectToAction(nameof(Login));
    }

    private void SignIn(Account account)
    {
        var jwtKey = _configuration["JwtKey"] ?? "development-only-jwt-key-change-before-production-32";
        var jwtIssuer = _configuration["JwtIssuer"] ?? "tsp";
        var jwtAudience = _configuration["JwtAudience"] ?? "tsp";
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims:
            [
                new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                new Claim(ClaimTypes.Email, account.Email)
            ],
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        Response.Cookies.Append("tsp_auth", new JwtSecurityTokenHandler().WriteToken(token), new CookieOptions
        {
            HttpOnly = true,
            IsEssential = true,
            SameSite = SameSiteMode.Strict,
            Secure = Request.IsHttps || _environment.IsDevelopment(),
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });
    }
}
