using System;

namespace DataAccessLayer
{
    public static class DatabaseInitializer
    {
        // Verifies the database connection on application startup (called from Program.cs).
        // Tables already exist in PostgreSQL — no migration needed.
        public static void Initialize()
        {
            try
            {
                using var dbContext = new NotificationDbContext();

                // Just check that we can reach the database
                bool canConnect = dbContext.Database.CanConnect();

                if (canConnect)
                {
                    Console.WriteLine("Database connection established successfully.");
                }
                else
                {
                    throw new Exception("Cannot connect to the database. Please check the connection string.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                throw;
            }
        }
    }
}

