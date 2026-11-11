using Alumni.API.Common;
using Alumni.API.Middlewares;
using Alumni.Application.Interfaces;
using Alumni.Persistence;
using Alumni.Persistence.Data;
using Alumni.Persistence.Seed;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddPersistenceServices(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Global Request Logger Middleware (Terminale formatlı log çıktısı)
app.UseRequestLogging();

// 5xx Guardian Middleware (500 ve 5xx sunucu hatalarını gerekçesiyle/reason ile döndürür)
app.Use5xxGuardian();

// 404 Guardian Middleware (404 hatalarını gerekçesiyle/reason ile döndürür)
app.UseNotFoundGuardian();

// The Bouncer Middleware (Yetkisiz veri değiştirme isteklerini kapıdan çevirir)
app.UseBouncer();

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

// 404 Route: GET /404 returns generic 404 HTML page (define generic 404 page)
app.MapGet("/404", (string? path, string? reason) => 
    Results.Content(StaticHtmlPages.GetNotFoundPageHtml(path, reason), "text/html", statusCode: StatusCodes.Status404NotFound))
    .WithTags("General");

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

// Test 500 route: GET /api/test-500 (5xx Guardian Middleware test rotası)
app.MapGet("/api/test-500", () =>
{
    throw new InvalidOperationException("5xx Guardian Testi: Sunucu tarafında beklenmeyen bir hata simüle edildi.");
}).WithTags("Test");



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

        logger.LogInformation("Veritabanı migration ve tohumlama (Seed) kontrolleri yapılıyor...");
        await DatabaseSeeder.SeedAsync(context, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Veritabanı migration veya tohumlama (seed) işlemi sırasında bir hata meydana geldi.");
    }
}

app.Run();

