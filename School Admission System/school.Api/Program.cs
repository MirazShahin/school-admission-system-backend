using Microsoft.EntityFrameworkCore;
using School.Api.Middleware;
using School.Application.Classes;
using School.Application.Holds;
using School.Infrastructure;
using School.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<School.Api.Background.ExpirySweeper>();
builder.Services.AddScoped<ClassService>();
builder.Services.AddScoped<HoldService>();
builder.Services.Configure<HoldOptions>(o =>
    o.DurationSeconds = int.TryParse(builder.Configuration["HOLD_DURATION_SECONDS"], out var s) ? s : 600);

builder.Services.AddCors(o => o.AddDefaultPolicy(p => p
    .WithOrigins(builder.Configuration["FRONTEND_ORIGIN"] ?? "http://localhost:3000")
    .AllowAnyHeader()
    .AllowAnyMethod()));

builder.WebHost.UseUrls("http://+:8080");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }