# TodoMVC - ASP.NET Core Identity Uygulaması
**Canlı Demo İçin

Bu proje, **ASP.NET Core MVC**, **Entity Framework Core** ve **Identity** kullanarak geliştirilmiş bir Todo uygulamasıdır. Kullanıcılar kayıt olabilir,
giriş yapabilir, todo ekleyebilir, güncelleyebilir ve silebilir. Her işlem sonrası kullanıcıya e-posta bildirimi gönderilmektedir. 
Ayrıca **Google reCAPTCHA v3** doğrulaması ile bot aktiviteleri önlenmiştir.

---

## Özellikler

- Kullanıcı kaydı ve girişi (Identity)
- E-posta doğrulama (Email Confirmation)
- Şifre sıfırlama (Forgot/Reset Password)
- Todo ekleme, güncelleme, silme
- Todo durum değiştirme (Aktif/Tamamlandı)
- İşlem sonrası e-posta bildirimleri
  - Todo eklendi
  - Todo güncellendi
  - Todo silindi
  - Şifre Değiştirildi
- Google reCAPTCHA v3 doğrulama
- Role tabanlı yetkilendirme hazır (IdentityRole)
- Responsive ve basit tasarım

---

## Teknolojiler

- ASP.NET Core 9
- Entity Framework Core 9
- SQL Server
- Identity (Kullanıcı yönetimi)
- FluentEmail (SMTP ile e-posta gönderimi)
- Razor View Engine
- Bootstrap 5 (Frontend)
- Google reCAPTCHA v3
---

## Kurulum

1. **Projeyi klonlayın**

```bash
git clone https://github.com/yilmazbthn/TodoMVC.git
cd TodoMVC
