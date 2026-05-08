using System;
using System.Text.RegularExpressions;
using Interfaces;
using Models;

namespace NotificationSenders
{
    public class EmailNotification : INotificationSender
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$", 
            RegexOptions.Compiled //pattern is precompiled once and tested for all 
        );
        

        public void Validate(User user, string message)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name))
            {
                throw new NotificationSendException("User details are required for email notification.");
            }

            if (message == null || message.Trim() == "")
            {
                throw new NotificationSendException("Message cannot be empty.");
            }

            if (message.Length < 5)
            {
                throw new NotificationSendException("Message should be at least 5 characters long.");
            }

            if (string.IsNullOrWhiteSpace(user.Email) || !IsValidEmail(user.Email))
            {
                throw new NotificationSendException("Invalid email address. Please provide a valid email to send notification.");
            }
        }

        public void Send(User user, Notification notification)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=========================== EMAIL NOTIFICATION ===========================");
               
                Console.WriteLine($"To: {user.Email}");
                Console.WriteLine($"Message: {notification.Message}");
                Console.WriteLine($"Sent at: {notification.SentDate}");
                Console.WriteLine("=========================================================================");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                throw new NotificationSendException($"Failed to send email notification: {ex.Message}",ex);
            }
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) 
                return false;
            return EmailRegex.IsMatch(email);
        }
    }
}