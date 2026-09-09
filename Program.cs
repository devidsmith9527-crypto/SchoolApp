using SchoolApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// ១. ចុះឈ្មោះសេវាកម្ម (REGISTER SERVICES)
// ==========================================

// ២. ចាប់យកខ្សែស្រឡាយតភ្ជាប់ (Connection String) ពីឯកសារ appsettings.json
// Note: Merged to use "LimConnection" with a fallback to "DefaultConnection"
var connectionString = builder.Configuration.GetConnectionString("LimConnection") 
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

// ៣. ចុះឈ្មោះ AppDbContext ទៅក្នុង DI Container
builder.Services.AddDbContext<SchoolDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Identity Services
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SchoolDbContext>()
    .AddDefaultTokenProviders();

// Configure Dual Authentication (Cookie for MVC + JWT Bearer for REST Web API)
builder.Services.ConfigureApplicationCookie(opt => opt.LoginPath = "/Account/Login");
builder.Services.AddAuthentication()
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"], ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();

// ៤. ចុះឈ្មោះសេវាកម្ម CORS ឱ្យស្គាល់គ្រប់ប្រភព (ការពារបញ្ហា Block ពី HTML Frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // អនុញ្ញាតគ្រប់ Domain
              .AllowAnyHeader()  // អនុញ្ញាតរាល់ Headers ទាំងអស់
              .AllowAnyMethod(); // អនុញ្ញាតរាល់ Methods
    });
});

// ចុះឈ្មោះសេវាកម្មសម្រាប់ Controllers (ប្រើបានទាំង API និង MVC Views)
builder.Services.AddControllersWithViews(); 

// បន្ថែម Swagger សម្រាប់ងាយស្រួលក្នុងការតេស្ត API (Test API Document)
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

// ==========================================
// ២. បង្កើត APPLICATION (BUILD) - ហៅតែម្តងគត់!
// ==========================================
var app = builder.Build();

// ==========================================
// ៣. កំណត់ HTTP REQUEST PIPELINE (MIDDLEWARE)
// ==========================================

if (app.Environment.IsDevelopment()) 
{ 
    app.UseSwagger(); 
    app.UseSwaggerUI(); 
}

// ⚠️ ចំណាំសំខាន់៖ បិទ HttpsRedirection ដើម្បីកុំឱ្យវា Redirect ទៅ HTTPS ដែលបង្កឱ្យមានបញ្ហា ERR_SSL_PROTOCOL_ERROR
// app.UseHttpsRedirection(); 

// ៦. អនុញ្ញាតឱ្យអានឯកសារ Static (ដូចជា CSS, JS, រូបភាព ពី Folder wwwroot)
app.UseStaticFiles();

app.UseRouting();

// ៧. ប្រើប្រាស់ CORS (ត្រូវតែដាក់នៅចន្លោះ UseRouting និង UseAuthentication ជានិច្ច!)
app.UseCors("AllowAll");

// បន្ថែម Authentication និង Authorization
app.UseAuthentication(); 
app.UseAuthorization();


// ==========================================
// ៤. Data Seeding (Automatic Admin Account Initialization)
// ==========================================
using (var scope = app.Services.CreateScope()) 
{
    var um = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await rm.RoleExistsAsync("Admin")) await rm.CreateAsync(new IdentityRole("Admin"));
    if (await um.FindByEmailAsync("admin@example.com") == null) {
        var admin = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true };
        if ((await um.CreateAsync(admin, "Admin123!")).Succeeded) await um.AddToRoleAsync(admin, "Admin");
    }
}

// ==========================================
// ៥. កំណត់ផ្លូវ (ROUTING & ENDPOINTS)
// ==========================================
// បន្ថែមកូដមួយបន្ទាត់នេះ ដើម្បិឱ្យ [Route("api/...")] ដំណើរការ
app.MapControllers();
// ៨. Map Controller សម្រាប់កំណត់ផ្លូវ MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ៩. Minimal API (សម្រាប់តេស្តសាកល្បង)
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

// រត់កម្មវិធី
app.Run();

// Record សម្រាប់ Minimal API ខាងលើ
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}