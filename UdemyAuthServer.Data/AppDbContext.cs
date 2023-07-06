using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UdemyAuthServer.Core.Entities;
using UdemyAuthServer.Data.Configurations;

namespace UdemyAuthServer.Data;

public class AppDbContext : IdentityDbContext<UserApp, IdentityRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();   
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(builder);
    }
}
