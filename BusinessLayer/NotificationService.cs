using System;
using NotificationSenders;
using System.Text.RegularExpressions;
using Interfaces;
using DataAccessLayer;
using Models;

namespace BusinessLayer
{
    public class NotificationService
    {
        private NotificationRepository repository;

        public NotificationService()
        {
            repository = new NotificationRepository();
        }

        public void Send(INotificationSender notificationSender, string message, User user)
        {
            try
            {
                if (notificationSender == null)
                    throw new ArgumentNullException(nameof(notificationSender));
                    //nameof = makes it string 
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                if (string.IsNullOrWhiteSpace(message))
                    throw new ValidationException("Message cannot be empty.");

                notificationSender.Validate(user, message);

                Notification notification = new Notification(message, notificationSender.GetType().Name)
                {
                    UserId = user.Id,
                    UserName = user.Name
                };

                notificationSender.Send(user, notification);
                repository.Save(notification);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nNotification sent and saved successfully!");
                Console.ResetColor();
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (NotificationSendException)
            {
                throw;
            }
            catch (DataAccessException)
            {
                throw;
            }
        }

        public void DisplayAllNotifications()
        {
            try
            {
                var notifications = repository.GetNotifications();

                if (notifications.Count == 0)
                {
                    Console.WriteLine("\nNo notifications sent yet!");
                    return;
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=========================== ALL SENT NOTIFICATION ===========================");

                Console.WriteLine();

                int count = 1;
                foreach (var notification in notifications)
                {
                    Console.WriteLine($"{count}:");
                    Console.WriteLine($"  User Name: {( !string.IsNullOrWhiteSpace(notification.UserName)? notification.UserName : "Unknown User")}");
                    Console.WriteLine($"  Type: {notification.Type}");
                    Console.WriteLine($"  Message: {notification.Message}");
                    Console.WriteLine($"  Sent Date: {notification.SentDate:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine();
                    count++;
                }
                Console.WriteLine("\n=========================================================================");
                Console.ResetColor();
            }
            catch (DataAccessException)
            {
                throw;
            }
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
            return regex.IsMatch(email);
        }

        public static string? GetNameValidationError(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Invalid name format! Name cannot be empty.";

            string trimmedName = name.Trim();
            var regex = new Regex(@"^[A-Za-z][A-Za-z\s]{1,49}$", RegexOptions.Compiled);
            if (!regex.IsMatch(trimmedName))
                return "Invalid name format! Use letters and spaces only.";

            return null;
        }

        public static string? GetEmailValidationError(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return "Invalid email format! Email cannot be empty.";

            if (!IsValidEmail(email))
                return "Invalid email format! Example: name@example.com";

            return null;
        }

        public static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            var regex = new Regex(@"^(\+91|0)?[6-9][0-9]{9}$", RegexOptions.Compiled);
            return regex.IsMatch(phone);
        }

        public static string? GetPhoneValidationError(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "Invalid phone format! Phone cannot be empty.";

            if (!IsValidPhone(phone))
                return "Invalid phone format! Use a valid 10-digit number like 9876543210.";

            return null;
        }

    }
}
