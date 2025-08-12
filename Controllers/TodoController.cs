using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TodoMVC.Data;
using TodoMVC.Models;
using Todo = TodoMVC.Models.Todo;

namespace TodoMVC.Controllers;
[Authorize]
public class TodoController(AppDbContext context,SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager):Controller
{
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
            return Unauthorized();

        var todos = context.Todos.Where(t => t.UserId == user.Id).ToList();

        return View(todos);
    }
    public IActionResult Add()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddTodo(TodoAdd todos)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();
        
        var todo = new Todo()
        {
            Title = todos.Title,
            Description = todos.Description,
            UserId = user.Id,
            Created = DateTime.Now
        };

        context.Todos.Add(todo);
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo is null || todo.UserId != user.Id) return NotFound();
        
        todo.IsDone = !todo.IsDone;
        todo.Updated = DateTime.Now;
        await context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo is not null && todo.UserId == user.Id)
        {
            context.Todos.Remove(todo);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        return NotFound();
    }
    
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo is null || todo.UserId != user.Id)
        {
            return NotFound();
        }

        return View(todo);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, Todo updatedTodo)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null) return Unauthorized();

        var todo = await context.Todos.FindAsync(id);
        if (todo is null || todo.UserId != user.Id)
        {
            return NotFound();
        }
        
        todo.Title = updatedTodo.Title;
        todo.Description = updatedTodo.Description;
        todo.Updated = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return RedirectToAction("Index");
    }




}