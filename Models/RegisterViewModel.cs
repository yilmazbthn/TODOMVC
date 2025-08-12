using System.ComponentModel.DataAnnotations;

namespace TodoMVC.Models;

public class RegisterViewModel
{
    [Required,EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
    public string ConfirmPassword { get; set; }
}