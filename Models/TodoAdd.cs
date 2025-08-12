using System.ComponentModel.DataAnnotations;

namespace TodoMVC.Models;

public class TodoAdd
{
    [Required,MaxLength(20)]
    public string Title { get; set; }
    [Required,MaxLength(70)]
    public string Description { get; set; }
    [Required]
    public bool IsDone { get; set; }
}