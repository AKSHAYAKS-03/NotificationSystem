using Models;
using Interfaces;
using System;
using System.Collections.Generic;
using Npgsql;

namespace DataAccessLayer
{
    public class NotificationRepository
    {
        private DbConnection dbConnection = new DbConnection();

        public void Save(Notification notification)
        {
            try
            {
                if (notification == null)
                    throw new ArgumentNullException(nameof(notification));

                // Save to database with parameterized query to prevent SQL injection
                string query = "INSERT INTO Notifications(userid, message, type, sentdate) VALUES(@userid, @message, @type, @sentdate) RETURNING notificationid";
                
                NpgsqlConnection conn = dbConnection.GetConnection();
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
                
                // Add parameters to prevent SQL injection
                cmd.Parameters.AddWithValue("@userid", notification.UserId);
                cmd.Parameters.AddWithValue("@message", notification.Message ?? "");
                cmd.Parameters.AddWithValue("@type", notification.Type ?? "");
                cmd.Parameters.AddWithValue("@sentdate", notification.SentDate);

                try
                {
                    conn.Open();
                    object? result = cmd.ExecuteScalar();
                    if (result != null && int.TryParse(result.ToString(), out int notificationId))
                    {
                        notification.NotificationId = notificationId;
                        Console.WriteLine("Notification saved to database successfully.");
                    }
                    else
                    {
                        throw new DataAccessException("Failed to retrieve notification ID from database.");
                    }
                }
                finally
                {
                    conn.Close();
                }
            }
            catch (ArgumentNullException ex)
            {
                throw new DataAccessException("Cannot save null notification.", ex);
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
                // Load from database
                List<Notification> allNotifications = new List<Notification>();
                string query = "SELECT n.notificationid, n.userid, n.message, n.type, n.sentdate, u.name FROM Notifications n LEFT JOIN Users u ON n.userid = u.id ORDER BY n.sentdate DESC";
                
                NpgsqlConnection conn = dbConnection.GetConnection();
                NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

                try
                {
                    conn.Open();
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Notification notification = new Notification();
                        notification.NotificationId = (int)reader["notificationid"];
                        notification.UserId = (int)reader["userid"];
                        notification.Message = (reader["message"] as string) ?? "";
                        notification.Type = (reader["type"] as string) ?? "";
                        notification.SentDate = (DateTime)reader["sentdate"];
                        notification.UserName = (reader["name"] as string) ?? "Unknown User";
                        
                        allNotifications.Add(notification);
                    }
                }
                finally
                {
                    conn.Close();
                }

                return allNotifications;
            }
            catch (Exception ex)
            {
                throw new DataAccessException($"Error retrieving notifications: {ex.Message}", ex);
            }
        }
    }
}
