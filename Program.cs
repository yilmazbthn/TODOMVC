using System.Net;
using System.Net.Mail;
using FluentEmail.Core;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using TodoMVC;
using TodoMVC.Data;

var builder = WebApplication.CreateBuilder(args);

//SMTP ayarları
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

builder.Services
    .AddFluentEmail(smtpSettings.FromEmail, smtpSettings.FromName)
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient(smtpSettings.Host)
    {
        Port = smtpSettings.Port,
        Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
        EnableSsl = true,
    });

builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//reCAPTCHA
builder.Services.AddHttpClient<IRecaptchaValidator, RecaptchaValidator>();


builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 1;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireDigit = false;
    
    options.SignIn.RequireConfirmedEmail = true; 
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders(); 

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
