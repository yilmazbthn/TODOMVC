using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TodoMVC.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "E-posta alanı boş olamaz.")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Şifre alanı boş olamaz.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
