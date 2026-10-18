using Alumni.API.Common;
using Alumni.Application.Interfaces;
using Alumni.Persistence;
using Alumni.Persistence.Data;
using Alumni.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddPersistenceServices(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/api/swagger");
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Root route: GET / returns HTML landing page
app.MapGet("/", () => Results.Content(StaticHtmlPages.GetHomePageHtml(), "text/html")).WithTags("General");

// Hello route: GET /hello returns "Hello,World!"
app.MapGet("/hello", () => "Hello,World!").WithTags("Test");

// Dynamic hello route: GET /hello/{name} returns "Hello,{name}!"
app.MapGet("/hello/{name}", (string name) => $"Hello,{name}!").WithTags("Test");

// Sum route: GET /sum/{number1:double}/{number2:double} returns sum
app.MapGet("/sum/{number1:double}/{number2:double}", (double number1, double number2) =>
{
    double sum = number1 + number2;
    return sum;
}).WithTags("Test");

// About route: GET /about returns HTML about page
app.MapGet("/about", () => Results.Content(StaticHtmlPages.GetAboutPageHtml(), "text/html")).WithTags("General");

// Health route: GET /api/health returns API health status
app.MapGet("/api/health", () => 
{
    return Results.Ok(new
    {
        status = "Healthy",
        timestamp = DateTime.UtcNow,
        service = "Alumni Tracking System API"
    });
}).WithTags("Health");



app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// -----------------------------------------------------------------------------
// Otomatik Veritabanı Migration ve Tohumlama (Seed Data)
// -----------------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AlumniDbContext>();

        logger.LogInformation("Veritabanı tabloları kontrol ediliyor ve migration uygulanıyor...");
        context.Database.Migrate();

        if (!context.Users.Any())
        {
            logger.LogInformation("Veritabanında kayıtlı kullanıcı bulunamadı. Başlangıç verileri (Ahmet, Ayşe vb.) tohumlanıyor...");
            await DatabaseSeeder.SeedAsync(context, logger);
        }
        else
        {
            logger.LogInformation("Veritabanında mevcut kullanıcılar bulundu. Tohumlama (seed) atlandı.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Veritabanı migration veya tohumlama (seed) işlemi sırasında bir hata meydana geldi.");
    }
}

app.Run();

