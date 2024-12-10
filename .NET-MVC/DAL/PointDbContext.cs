using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ITPE3200ExamProject.Models;

namespace ITPE3200ExamProject.DAL;

public class PointDbContext : IdentityDbContext
{
	public PointDbContext(DbContextOptions<PointDbContext> options) : base(options) {}
	public DbSet<Point> Points { get; set; }
    public DbSet<Account> Accounts { get; set; }
	public DbSet<Image> Images { get; set; }
    public DbSet<Comment> Comments { get; set; }
}