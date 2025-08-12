using Microsoft.AspNetCore.Mvc;

namespace TodoMVC.Controllers;

public class HomeController:Controller
{
    public IActionResult Index()
    {
        return View();
    }
}