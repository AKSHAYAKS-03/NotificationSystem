using System;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public static class DatabaseInitializer
    {

        // Apply migrations and initialize the database , on application startup (called from Program.cs)
        public static void Initialize()
        {
            try
            {
                using var dbContext = new NotificationDbContext();

                Console.WriteLine("Applying EF Core migrations...");
                dbContext.Database.Migrate();
                Console.WriteLine("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }
    }
}
