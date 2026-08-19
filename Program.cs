using SchoolApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Register services for MVC (Controllers WITH Views)
builder.Services.AddControllersWithViews(); 

// ១. ចាប់យកខ្សែស្រឡាយតភ្ជាប់ពីឯកសារ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("LimConnection");

// ២. ចុះឈ្មោះ AppDbContext ទៅក្នុង DI Container ដោយប្រាប់វាឱ្យប្រើ SQL Server
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Configure CORS (Essential for your HTML/JS frontend to connect)
// ១. ចុះឈ្មោះសេវាកម្ម CORS ឱ្យស្គាល់គ្រប់ប្រភព (សម្រាប់ Development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // អនុញ្ញាតគ្រប់ Domain / Port
              .AllowAnyHeader()  // អនុញ្ញាតរាល់ Headers
              .AllowAnyMethod(); // អនុញ្ញាត GET, POST, PUT, DELETE
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

// UseStaticFiles is highly recommended for MVC so you can serve CSS, JS, and images from wwwroot
app.UseStaticFiles();

// 3. Enable CORS in the pipeline (MUST be before routing/endpoints)
app.UseCors("AllowAll");

app.UseRouting();

// 4. Map the Controller endpoints for MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 5. Minimal API example
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

// Record definitions typically sit at the bottom of the file
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}