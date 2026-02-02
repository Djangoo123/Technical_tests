using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnicalTestCS.Api.Interfaces;
using TechnicalTestCS.Api.Services;
using TechnicalTestCS.Infrastructure.External.Interfaces;
using TechnicalTestCS.Infrastructure.Persistence;

namespace TechnicalTestCS.Tests;

public sealed class TestHost : IDisposable
{
    private readonly SqliteConnection _conn;
    public ServiceProvider Services { get; }

    public TestHost(IExternalArticlesClient externalClient)
    {
        var services = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalApi:BaseUrl"] = "https://cms-beta.happytal.com"
            })
            .Build();

        services.AddSingleton<IConfiguration>(config);

        // SQLite in-memory 
        _conn = new SqliteConnection("DataSource=:memory:");
        _conn.Open();

        services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(_conn));
        services.AddSingleton(externalClient);
        services.AddScoped<IArticleStatusService, ArticleStatusService>(); 
        services.AddScoped<IArticlesService, ArticlesService>();

        Services = services.BuildServiceProvider();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    public IServiceScope CreateScope() => Services.CreateScope();

    public void Dispose()
    {
        Services.Dispose();
        _conn.Dispose();
    }
}
