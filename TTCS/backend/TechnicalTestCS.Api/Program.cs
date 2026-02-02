using Microsoft.EntityFrameworkCore;
using Serilog;
using TechnicalTestCS.Api.Interfaces;
using TechnicalTestCS.Api.Middlewares;
using TechnicalTestCS.Api.Services;
using TechnicalTestCS.Infrastructure.External;
using TechnicalTestCS.Infrastructure.External.Interfaces;
using TechnicalTestCS.Infrastructure.Persistence;
using Serilog.Sinks.Grafana.Loki;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, services, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration)
       .ReadFrom.Services(services);

    var lokiUri = ctx.Configuration["Loki:Uri"];
    if (!string.IsNullOrWhiteSpace(lokiUri))
    {
        cfg.WriteTo.GrafanaLoki(
            lokiUri,
            labels:
            [
                new LokiLabel { Key = "service", Value = "technicaltestcs-api" }
            ]);
    }
});



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cs = builder.Configuration.GetConnectionString("Db")
         ?? throw new InvalidOperationException("ConnectionStrings:Db is missing.");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

builder.Services.AddHttpClient<IExternalArticlesClient, ExternalArticlesClient>(client =>
{
    var baseUrl = builder.Configuration["ExternalApi:BaseUrl"] ?? "https://cms-beta.happytal.com";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Services
builder.Services.AddScoped<IArticleStatusService, ArticleStatusService>();
builder.Services.AddScoped<IArticlesService, ArticlesService>();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseSerilogRequestLogging();

app.MapControllers();
app.Run();
