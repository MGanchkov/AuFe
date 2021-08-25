using AuFe.Models;
using AuFe.Models.Interface;
using AuFe.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuFe.Controllers;

public class AccountController : Controller
{
    readonly IUsers db;
    public AccountController(IUsers context)
    {
        db = context;
    }
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (ModelState.IsValid)
        {
            User user = db.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            if (user != null)
            {
                await Authenticate(user.Login); // аутентификация

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Некорректные E-mail и(или) пароль");
        }
        return View(model);
    }
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            User user = db.Users.FirstOrDefault(u => u.Login == model.Login || u.Email == model.Email);
            if (user == null)
            {
                // добавляем пользователя в бд
                db.Add(new User { Login = model.Login, Email = model.Email, Password = model.Password });
                db.SaveChanges();

                await Authenticate(model.Login); // аутентификация

                return RedirectToAction("Index", "Home");
            }
            else
                ModelState.AddModelError("", "Некорректные: и(или) логин, и(или) E-mail, и(или) пароль.");
        }
        return View(model);
    }

    private async Task Authenticate(string userName)
    {
        // создаем один claim
        var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
        // создаем объект ClaimsIdentity
        ClaimsIdentity id = new(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        // установка аутентификационных куки
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Account");
    }
}


