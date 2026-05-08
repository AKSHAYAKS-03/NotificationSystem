using System;
using System.Text.RegularExpressions;
using Interfaces;
using Models;

namespace NotificationSenders
{
    public class SmsNotification : INotificationSender
    {
        private static readonly Regex PhoneRegex = new Regex(
            @"^(\+91|0)?[6-9][0-9]{9}$",
            RegexOptions.Compiled //pattern is precompiled once and tested for all , phn no starts with 6 - 9 and has 10 numbers
        );

        public void Validate(User user, string message)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Name))
            {
                throw new NotificationSendException("User details are required for SMS notification.");
            }

            if (message == null || message.Trim() == "")
            {
                throw new NotificationSendException("Message cannot be empty.");
            }

            if (message.Length < 5)
            {
                throw new NotificationSendException("Message should be at least 5 characters long.");
            }

            if (message.Length > 160)
            {
                throw new NotificationSendException($"SMS message exceeds 160 chars.");
            }

            if (string.IsNullOrWhiteSpace(user.Phone) || !IsValidPhone(user.Phone))
            {
                throw new NotificationSendException(
                    "Invalid phone number. Please provide a valid Indian phone number (eg, 9876543210))."
                );
            }
        }

        public void Send(User user, Notification notification)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=========================== SMS NOTIFICATION ===========================");
                Console.WriteLine($"To: {user.Phone}");
                Console.WriteLine($"Message: {notification.Message}");
                Console.WriteLine($"Sent at: {notification.SentDate}");
                Console.WriteLine("=========================================================================");
                Console.ResetColor();

            }
            catch (Exception ex)
            {
                throw new NotificationSendException($"Failed to send SMS notification: {ex.Message}", ex);
            }
        }

        private static bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) 
                return false;
            return PhoneRegex.IsMatch(phone);
        }
    }
}