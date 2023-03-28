namespace EventsApi.Demo.Database;
public class MinimalEventsDbContext : DbContext
{
    public MinimalEventsDbContext(DbContextOptions options) : base(options) { }
    public DbSet<MinimalEvent> MinimalEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        new MinimalEventsDataSeeder(modelBuilder).SeedDatabaseWithData();
    }
}