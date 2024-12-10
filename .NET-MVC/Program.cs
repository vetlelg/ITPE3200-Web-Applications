using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Serilog.Events;
using ITPE3200ExamProject.DAL;
using ITPE3200ExamProject.Models;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Get database connection string from appsettings.json
var ConnectionString = builder.Configuration.GetConnectionString("DbConnection") ?? throw new
InvalidOperationException("Could not find connection string: DbConnection");

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

// Add services for handling controllers and views in the MVC pattern
builder.Services.AddControllersWithViews();

// Add services for working with sqlite database
builder.Services.AddDbContext<PointDbContext>(options => 
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:DbConnection"]);
});

// Add services for using the repository pattern
builder.Services.AddScoped<IPointRepository, PointRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();


// Add services for using Identity/Authentication/Authorization
// AddIdentity configures password hashing and salting automatically by default
builder.Services.AddIdentity<Account, IdentityRole>(options =>
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

// Redirect to login page if user is not authenticated
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
});

// Create the application
var app = builder.Build();

// Run this code only in development mode
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Seed the database with test data if database doesn't exist
DBInit.Seed(app);

// Add middleware. The order of these lines are important
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultControllerRoute();
app.UseRequestLocalization(localizationOptions);

app.Run();