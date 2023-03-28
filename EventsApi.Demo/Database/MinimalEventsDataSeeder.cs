using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EventsApi.Demo.Database;

public class MinimalEventsDataSeeder
{
    private readonly ModelBuilder modelBuilder;

    public MinimalEventsDataSeeder(ModelBuilder modelBuilder)
    {
        this.modelBuilder = modelBuilder;

    }

    public void SeedDatabaseWithData()
    {
        modelBuilder.Entity<MinimalEvent>().HasData(SeedMinimalEvents());
    }

    private static List<MinimalEvent> SeedMinimalEvents()
    {
        var jsonString = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\Database\events.json");

        var events = JsonSerializer.Deserialize<List<MinimalEvent>>(jsonString, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        return events;
    }
}