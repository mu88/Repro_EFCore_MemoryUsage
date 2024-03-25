using Microsoft.EntityFrameworkCore;

namespace Core;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<InteractionPoint> InteractionPoints { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<InteractionPoint>(builder => builder.HasKey(point => point.Key));
    }
}