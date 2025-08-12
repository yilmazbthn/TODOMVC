using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TodoMVC.Models;

public class Todo
{
    public int Id { get; set; }
    [Required,MaxLength(20)]
    public string Title { get; set; }
    [Required,MaxLength(70)]
    public string Description { get; set; }
    [Required]
    public bool IsDone { get; set; }
    public string UserId { get; set; }
    public IdentityUser User { get; set; }

    public DateTime Created { get; set; }=DateTime.Now;
    public DateTime Updated { get; set; }=DateTime.Now;
}