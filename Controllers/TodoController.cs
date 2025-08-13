using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using TodoMVC.Data;
using TodoMVC.Models;
using Todo = TodoMVC.Models.Todo;

namespace TodoMVC.Controllers;
[Authorize]
public class TodoController(AppDbContext context,SignInManager<IdentityUser> signInManager,IEmailSender emailSender,UserManager<IdentityUser> userManager):Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var todos = context.Todos.Where(t => t.UserId == user.Id).ToList();
        return View(todos);
    }

    [HttpGet]
    public IActionResult Add() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTodo(TodoAdd todos)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var todo = new Todo
        {
            Title = todos.Title,
            Description = todos.Description,
            UserId = user.Id,
            Created = DateTime.UtcNow
        };

        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        await SendTodoEmail(user.Email, user.UserName, todo.Title, "eklendi");

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo == null || todo.UserId != user.Id) return NotFound();

        todo.IsDone = !todo.IsDone;
        todo.Updated = DateTime.UtcNow;
        await context.SaveChangesAsync();

        string action = todo.IsDone ? "tamamlandı" : "aktif edildi";
        await SendTodoEmail(user.Email, user.UserName, todo.Title, action);

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo == null || todo.UserId != user.Id) return NotFound();

        context.Todos.Remove(todo);
        await context.SaveChangesAsync();

        await SendTodoEmail(user.Email, user.UserName, todo.Title, "silindi");

        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo == null || todo.UserId != user.Id) return NotFound();

        return View(todo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, Todo updatedTodo)
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo == null || todo.UserId != user.Id) return NotFound();

        todo.Title = updatedTodo.Title;
        todo.Description = updatedTodo.Description;
        todo.Updated = DateTime.UtcNow;

        await context.SaveChangesAsync();

        await SendTodoEmail(user.Email, user.UserName, todo.Title, "güncellendi");

        return RedirectToAction("Index");
    }
    
    private async Task SendTodoEmail(string email, string username, string title, string action)
    {
        string subject = $"Todo {action}";
        string body = $"Merhaba {username},<br/>" +
                      $"'{title}' başlıklı todo {action}.<br/>";

        await emailSender.SendEmailAsync(email, subject, body);
    }
}