using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public class AppDbContext: DbContext{
    public DbSet<User> Users { get; set; }
    public DbSet<Child> Children { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options): base(options){
        Users = Set<User>();
        Children = Set<Child>();
        Payments = Set<Payment>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=mydatabase.db");
        }
    }
}