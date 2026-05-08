using Models;
using Interfaces;
using System;
using System.Collections.Generic;

namespace DataAccessLayer
{
    public class NotificationRepository
    {
        private static List<Notification> notifications = new List<Notification>();

        public void Save(Notification notification)
        {
            try
            {
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification));

                notifications.Add(notification);
            }
            catch (ArgumentNullException ex)
            {
                throw new DataAccessException("Cannot save null notification.", ex);
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
                return new List<Notification>(notifications);
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Error retrieving notifications: {ex.Message}", ex);
            }
        }
    }
}