using Models;
using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class NotificationRepository
    {
        private readonly NotificationDbContext dbContext = new NotificationDbContext();

        public void Save(Notification notification)
        {
            try
            {
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification));

                dbContext.Notifications.Add(notification);

                dbContext.SaveChanges();
            }
            catch (ArgumentNullException ex)
            {
                throw new DataAccessException("Cannot save null notification.", ex);
            }
            catch (DbUpdateException ex)
            {
                throw new DataAccessException("Failed to save notification to the database.", ex);
            }
            catch (DataAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Error saving notification: {ex.Message}", ex);
            }
        }

        public List<Notification> GetNotifications()
        {
            try
            {
                // Fetch notifications with user details
                var notifications = dbContext.Notifications
                    .Include(n => n.User)
                    .OrderByDescending(n => n.SentDate)
                    .ToList();

                foreach (var notification in notifications)
                {
                    // Set username for display purpose
                    notification.UserName = notification.User?.Name ?? "Unknown User";
                }

                return notifications;
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Error retrieving notifications: {ex.Message}", ex);
            }
        }
    }
}