using Microsoft.AspNetCore.Identity;
using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.DAL;

public static class DBInit
{
    // Used to seed the database with some initial data for testing
    // This is called from Program.cs and takes in the app and seeds the database
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        PointDbContext context = serviceScope.ServiceProvider.GetRequiredService<PointDbContext>();
        UserManager<Account> userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<Account>>();

        //Creates database if it doesn't exists
        context.Database.EnsureDeleted();
        bool wasCreated = context.Database.EnsureCreated();
        if(wasCreated == false)
        {
            return;
        }

        Account admin = new Account { UserName = "admin@admin.com", Email = "admin@admin.com" };
        Account user = new Account { UserName = "user@user.com", Email = "user@user.com" };
        if (!context.Accounts.Any())
        {
            var result = userManager.CreateAsync(admin, "Admin2000!");
            var result1 = userManager.CreateAsync(user, "User2000!");
        }

        if (!context.Points.Any())
        {
            var points = new List<Point>
                {
                    new Point
                    {
                        Name = "OsloMet",
                        Latitude = 59.9139,
                        Longitude = 10.7522,
                        Description = "OsloMet er best",
                        AccountId = admin.Id,
                    },
                    new Point
                    {
                        Name = "Stavern",
                        Latitude = 59,
                        Longitude = 10,
                        Description = "Stavern",
                        AccountId = admin.Id,
                    },
                    new Point
                    {
                        Name = "Bergen",
                        Latitude = 60.3913,
                        Longitude = 5.3221,
                        Description = "Masse regn",
                        AccountId = admin.Id,
                    },
                    new Point
                    {
                        Name = "Stockholm",
                        Latitude = 59.19,
                        Longitude = 18.3221,
                        Description = "Fin by",
                        AccountId = user.Id,
                    },
                    new Point
                    {
                        Name = "Copenhagen",
                        Latitude = 55.40,
                        Longitude = 12.34,
                        Description = "I Danmark",
                        AccountId = user.Id,
                    },
                    new Point
                    {
                        Name = "Oslo",
                        Latitude = 60,
                        Longitude = 11,
                        Description = "Oslo",
                        AccountId = user.Id,
                    }
                };
            context.AddRange(points);
            context.SaveChanges();
        }
    }
}