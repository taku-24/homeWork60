using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication7.Models;

public class ToDoTask
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public Priority Priority { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.Новая;

    public string? Description { get; set; } = string.Empty;

    public int? CreatorId { get; set; }

    [BindNever]
    public User? Creator { get; set; }

    public int? ExecutorId { get; set; }

    [BindNever]
    public User? Executor { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? OpenedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}