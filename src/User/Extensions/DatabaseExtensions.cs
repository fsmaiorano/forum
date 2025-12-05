using Microsoft.EntityFrameworkCore;

namespace User.Extensions;

public static class DatabaseExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        try
        {
            if (context.Database.IsRelational())
            {
                // MigrateAsync will create the database if it doesn't exist and apply all pending migrations
                await context.Database.MigrateAsync();
                Console.WriteLine("Database migration completed successfully.");
            }
            else
            {
                // For non-relational databases, ensure the database is created
                await context.Database.EnsureCreatedAsync();
                Console.WriteLine("Using non-relational database provider - database ensured created.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database initialization failed: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine("Application will continue without database initialization.");
        }
    }
}