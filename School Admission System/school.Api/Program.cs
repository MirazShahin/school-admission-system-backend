using School.Api.Middleware;
using School.Application.Classes;
using School.Application.Holds;
using School.Infrastructure;
using School.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<HoldService>();
builder.Services.Configure<HoldOptions>(o =>
    o.DurationSeconds = int.TryParse(builder.Configuration["HOLD_DURATION_SECONDS"], out var s) ? s : 600);

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:8080")
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.WebHost.UseUrls("http://+:8080");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }