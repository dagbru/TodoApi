using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoApi.Repositories;

namespace TodoApi.Tests.Infrastructure;

public class CustomWebApplicationFactory<TProgram>(ITodoRepository todoRepository, IDbContextFactory<TodoDbContext> dbContextFactory) : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddTransient(_ => todoRepository);
            services.AddTransient(_ => dbContextFactory);
        });
        base.ConfigureWebHost(builder);
    }
}