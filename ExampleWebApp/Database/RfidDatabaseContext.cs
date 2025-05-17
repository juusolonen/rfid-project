using Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class RfidDatabaseContext(DbContextOptions<RfidDatabaseContext> options) : DbContext(options)
{
    public DbSet<EventBaseDbEntity> Events { get; set; }
    public DbSet<ProcessedEventDbEntity> ProcessedEvents { get; set; }
    public DbSet<UserDbEntity> Users { get; set; }
    public DbSet<ToolDbEntity> Tools { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventBaseDbEntity>().Ignore(x => x.Deleted);
        modelBuilder.Entity<EventBaseDbEntity>().Ignore(x => x.DeletedAt);
    }
}