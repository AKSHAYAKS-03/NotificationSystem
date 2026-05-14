using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessLayer
{
    public sealed class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        // Help EF Core CLI tools (migrations) work during development
        // Factory ensures migrations work independently of your app's startup logic
        public NotificationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=notification;Username=postgres;Password=12345");

            return new NotificationDbContext(optionsBuilder.Options);
        }
    }
}
