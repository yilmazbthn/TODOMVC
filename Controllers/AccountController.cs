using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoMVC.Data;
using TodoMVC.Models;

namespace TodoMVC.Controllers;

public class AccountController(AppDbContext context,SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager):Controller
{
    // Login GET
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        Console.WriteLine("Login Result: " + result.Succeeded);

        Console.WriteLine(model.Email);
        Console.WriteLine(model.Password);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            

            return RedirectToAction("Index", "Todo");
        }

        ModelState.AddModelError("", "Geçersiz kullanıcı adı veya parola.");
        return View(model);
    }


    // Register GET
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // Register POST
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await signInManager.SignInAsync(user, false);
            return RedirectToAction("Login", "Account");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

}