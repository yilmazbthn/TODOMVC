using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TodoMVC.Data;
using TodoMVC.Models;

namespace TodoMVC.Controllers;

public class AccountController(AppDbContext context,IRecaptchaValidator recaptchaValidator,IEmailSender emailSender,SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager):Controller
{
    
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null, string RecaptchaToken = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (string.IsNullOrEmpty(RecaptchaToken) || !await recaptchaValidator.IsValidAsync(RecaptchaToken))
        {
            ModelState.AddModelError("", "reCAPTCHA doğrulaması başarısız oldu.");
            return View(model);
        }

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user != null && !await userManager.IsEmailConfirmedAsync(user))
        {
            ModelState.AddModelError("", "Mail adresiniz doğrulanmamış. Lütfen mailinizi kontrol edin.");
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Todo");
        }

        ModelState.AddModelError("", "Geçersiz kullanıcı adı veya parola.");
        return View(model);
    }
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var existingUser = await userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("", "Bu e-posta adresi zaten kayıtlı.");
            return View(model);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Account",
                new { userId = user.Id, token }, Request.Scheme);

            await emailSender.SendEmailAsync(user.Email, "Email Doğrulama",
                $"Lütfen mailinizi doğrulamak için <a href='{confirmationLink}'>tıklayın</a>");
            
            return RedirectToAction("RegisterConfirmation");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult RegisterConfirmation() => View();

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return RedirectToAction("Index", "Home");

        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        var result = await userManager.ConfirmEmailAsync(user, token);
        return View(result.Succeeded ? "ConfirmEmail" : "Error");
    }
    
    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null || !await userManager.IsEmailConfirmedAsync(user))
        {
            return RedirectToAction("ForgotPasswordConfirmation");
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = Url.Action("ResetPassword", "Account",
            new { token, email = user.Email }, Request.Scheme);

        await emailSender.SendEmailAsync(user.Email, "Şifre Sıfırlama",
            $"Şifrenizi sıfırlamak için <a href='{resetLink}'>tıklayın</a>");

        return RedirectToAction("ForgotPasswordConfirmation");
    }

    [HttpGet]
    public IActionResult ForgotPasswordConfirmation() => View();

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            return NotFound(); 

        var model = new ResetPasswordViewModel { Token = token, Email = email };
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await userManager.FindByEmailAsync(model.Email);
        if (user == null)
            return RedirectToAction("ResetPasswordConfirmation");

        var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
        if (result.Succeeded)
        {
            string subject = "Şifreniz Değiştirildi";
            string body = $"Merhaba {user.UserName},<br/>" +
                          $"Hesabınızın şifresi başarıyla değiştirildi. Eğer bu işlemi siz yapmadıysanız lütfen hemen bizimle iletişime geçin.";

            await emailSender.SendEmailAsync(user.Email, subject, body);

            return RedirectToAction("ResetPasswordConfirmation");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("", error.Description);

        return View(model);
    }

    [HttpGet]
    public IActionResult ResetPasswordConfirmation() => View();

    [Authorize]
    public async Task<IActionResult> GenerateResetPassword()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        return RedirectToAction("ResetPassword", new { email = user.Email, token });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}