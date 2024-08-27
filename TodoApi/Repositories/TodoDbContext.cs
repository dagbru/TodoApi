using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

namespace TodoApi.Repositories;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions options) : base(options)
    {
        
    }
    public DbSet<TodoItem> TodoItems { get; set; }
}