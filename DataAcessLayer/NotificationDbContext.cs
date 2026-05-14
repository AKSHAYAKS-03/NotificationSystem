using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccessLayer
{
    public class NotificationDbContext : DbContext
    {

        // Define entities, database schema mapping, relationships ie when application is at runtime to query/save data
        private const string ConnectionString =
            "Host=localhost;Port=5432;Database=notification;Username=postgres;Password=12345";

        public NotificationDbContext()
        {
        }

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; } = null!;

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user.ToTable("Users");

                user.HasKey(u => u.Id)
                    .HasName("users_pkey");

                user.Property(u => u.Name)
                    .HasMaxLength(100);

                user.Property(u => u.Email)
                    .HasMaxLength(150);

                user.Property(u => u.Phone)
                    .HasMaxLength(20);

                user.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("idx_users_email");
            });

            modelBuilder.Entity<Notification>(notification =>
            {
                notification.ToTable("Notifications");

                notification.HasKey(n => n.NotificationId)
                    .HasName("notifications_pkey");

                notification.Property(n => n.Type)
                    .HasMaxLength(50);

                notification.Property(n => n.SentDate)
                    .HasColumnType("timestamp without time zone");

                notification.HasIndex(n => n.UserId)
                    .HasDatabaseName("idx_notifications_userid");

                notification.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .HasConstraintName("fk_user")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
