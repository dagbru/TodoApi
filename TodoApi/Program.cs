using Microsoft.EntityFrameworkCore;
using TodoApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddDbContextFactory<TodoDbContext>((_, options) =>
{
    options.UseSqlServer(
        "Data Source=localhost;Database=Todo;Integrated Security=false;User ID=sa;Password=test;TrustServerCertificate=True");
});
builder.Services.AddTransient<ITodoRepository, TodoRepository>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();
app.Run();