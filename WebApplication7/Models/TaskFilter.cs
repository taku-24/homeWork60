namespace WebApplication7.Models;

public class TaskFilter
{
    public string? Title { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? Keywords { get; set; }
    public Priority? Priority { get; set; }
    public TaskStatus? Status { get; set; }
    public string? SortOrder { get; set; }
    public bool Descending { get; set; }
}