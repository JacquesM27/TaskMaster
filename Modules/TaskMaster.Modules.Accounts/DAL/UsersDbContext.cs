using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Accounts.Entities;

namespace TaskMaster.Modules.Accounts.DAL;

public sealed class UsersDbContext(DbContextOptions<UsersDbContext> options)
    : DbContext(options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("Users");
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}