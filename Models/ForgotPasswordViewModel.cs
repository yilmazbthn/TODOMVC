using System.ComponentModel.DataAnnotations;

namespace TodoMVC.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "E-posta adresi gerekli.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    public string Email { get; set; }
}
public class ResetPasswordViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z]).+$",
        ErrorMessage = "Şifre en az bir büyük ve bir küçük harf içermelidir.")]
    public string Password { get; set; }

    [DataType(DataType.Password), Compare("Password", ErrorMessage = "Şifreler uyuşmuyor")]
    public string ConfirmPassword { get; set; }

    public string Token { get; set; }
}