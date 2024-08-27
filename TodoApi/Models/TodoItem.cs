namespace TodoApi.Models;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool Completed { get; set; }
    public DateTime? CompletedAt { get; set; }
}