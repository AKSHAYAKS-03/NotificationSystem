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
                // Map to the main PostgreSQL table name with exact casing
                user.ToTable("Users");

                user.HasKey(u => u.Id)
                    .HasName("users_pkey");

                // Use exact database column names for the quoted table
                user.Property(u => u.Id)
                    .HasColumnName("Id");

                user.Property(u => u.Name)
                    .HasColumnName("Name")
                    .HasMaxLength(100);

                user.Property(u => u.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(150);

                user.Property(u => u.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(20);

                user.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("idx_users_email");
            });

            modelBuilder.Entity<Notification>(notification =>
            {
                // Map to the main PostgreSQL table name with exact casing
                notification.ToTable("Notifications");

                notification.HasKey(n => n.NotificationId)
                    .HasName("notifications_pkey");

                // Use exact database column names for the quoted table
                notification.Property(n => n.NotificationId)
                    .HasColumnName("NotificationId");

                notification.Property(n => n.UserId)
                    .HasColumnName("UserId");

                notification.Property(n => n.Message)
                    .HasColumnName("Message");

                notification.Property(n => n.Type)
                    .HasMaxLength(50)
                    .HasColumnName("Type");

                notification.Property(n => n.SentDate)
                    .HasColumnName("SentDate")
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
