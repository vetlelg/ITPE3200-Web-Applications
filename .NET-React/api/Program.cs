using Microsoft.EntityFrameworkCore;
using ITPE3200ExamProject.Models;
using Serilog;
using Serilog.Events;
using Microsoft.AspNetCore.Identity;
using ITPE3200ExamProject.DAL;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Get database connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DbConnection");
// Define type of database to use
builder.Services.AddDbContext<PointDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create logger configuration
var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information() // Levels: Trace < Information < Warning < Error < Fatal
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");

// Filter logs
loggerConfiguration.Filter.ByExcluding(
    e => e.Properties.TryGetValue("SourceContext", out var value) && // Filter logs that have the sourcecontext property
    e.Level == LogEventLevel.Information && // Information level is too verbose
    e.MessageTemplate.Text.Contains("Executed DbCommand") // Database logs have "Executed DbCommand" in the message
);

// Create logger and add it to the builder
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore
);

builder.Services.AddDistributedMemoryCache();
// Add services for using the repository pattern
builder.Services.AddScoped<IPointRepository, PointRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();


// Cross origin resource sharing (CORS) policy
// Needed to allow requests from the frontend
builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials());
    });

// Add services for using Identity/Authentication/Authorization
// AddIdentity configures password hashing and salting automatically by default
builder.Services.AddIdentityApiEndpoints<Account>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Sign-in settings
    options.SignIn.RequireConfirmedEmail = false; // Set to true to require email confirmation
})
.AddEntityFrameworkStores<PointDbContext>();

// Set up cookie/session settings
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.Name= ".ITPE3200ExamProject.Session";

    // Essential cookies are required for the app to work
    options.Cookie.IsEssential = true;
});
// Configure cookie authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// Fixes an issue where latitude and longitude expected ',' as decimal seperator, instead of '.'
var defaultCulture = new System.Globalization.CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = new List<CultureInfo> { defaultCulture };
    options.SupportedUICultures = new List<CultureInfo> { defaultCulture };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    DBInit.Seed(app);
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed the database with test data
DBInit.Seed(app);
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.MapControllers();

app.Run();


