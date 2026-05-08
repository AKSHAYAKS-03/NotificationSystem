using System;
using System.Collections.Generic;
using Models;
using BusinessLayer;
using NotificationSenders;
using Interfaces;
using DataAccessLayer;

namespace PresentationLayer
{
    class Program
    {
        static UserRepository userRepository = new UserRepository();
        static NotificationService notificationService = new NotificationService();
        
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=============================================================");
            Console.WriteLine("                 Simple Notification System                   ");
            Console.WriteLine("=============================================================");
            Console.WriteLine();
            Console.ResetColor();

            while (true)
            {
                Console.WriteLine("-------------------------------------------------------------");
                Console.WriteLine("\n1. Add User");
                Console.WriteLine("2. View Users");
                Console.WriteLine("3. Update User");
                Console.WriteLine("4. Delete User");
                Console.WriteLine("5. Send Notification");
                Console.WriteLine("6. Display All Notifications");
                Console.WriteLine("7. Exit");
                Console.WriteLine("-------------------------------------------------------------");

                Console.Write("\nEnter your choice: ");

                string choice = Console.ReadLine() ?? "0";

                switch (choice)
                {
                    case "1":
                        AddUser();
                        break;

                    case "2":
                        ViewUsers();
                        break;

                    case "3":
                        UpdateUser();
                        break;

                    case "4":
                        DeleteUser();
                        break;

                    case "5":
                        SendNotification();
                        break;

                    case "6":
                        try
                        {
                            notificationService.DisplayAllNotifications();
                        }
                        catch (DataAccessException ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Error loading notifications: {ex.Message}");
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Unexpected error: {ex.Message}");
                            Console.ResetColor();
                        }
                        break;

                    case "7":
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nThank you!");
                        Console.ResetColor();
                        return;

                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid choice! Please enter 1-7.");
                        Console.ResetColor();
                        break;
                }
            }
        }

        static void AddUser()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=========================== Add New User ===========================");
            Console.ResetColor();
            
            User user = new User();

            Console.Write("Enter Name: ");
            user.Name = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(user.Name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Name cannot be empty!");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter Email: ");
            user.Email = Console.ReadLine() ?? "";

            if (!NotificationService.IsValidEmail(user.Email))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid email format!");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter Phone: ");
            user.Phone = Console.ReadLine() ?? "";

            if (!NotificationService.IsValidPhone(user.Phone))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid phone format!");
                Console.ResetColor();
                return;
            }

            userRepository.Create(user);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nUser added successfully!");
            Console.ResetColor();
        }

        static void ViewUsers()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=========================== View All Users ===========================");
            

            var users = userRepository.GetAll();

            if (users == null || users.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No users found!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine();
            foreach (var user in users)
            {
                Console.WriteLine(user+"\n");
            }
            Console.WriteLine("\n=====================================================================");
            Console.ResetColor();

        }

        static void UpdateUser()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n========== Update User ==========");
            Console.ResetColor();
            
            Console.Write("Enter User ID to update: ");
            string userId = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User ID cannot be empty!");
                Console.ResetColor();
                return;
            }

            User? existingUser = userRepository.Get(userId);
            if (existingUser == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User not found!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\nCurrent Details:");
            Console.WriteLine($"  Name: {existingUser.Name}");
            Console.WriteLine($"  Email: {existingUser.Email}");
            Console.WriteLine($"  Phone: {existingUser.Phone}");

            Console.WriteLine("\nEnter New Details:");

            Console.Write("Enter Name: ");
            string newName = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(newName))
                existingUser.Name = newName;

            Console.Write("Enter Email: ");
            string newEmail = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(newEmail))
            {
                if (!NotificationService.IsValidEmail(newEmail))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid email format!");
                    Console.ResetColor();
                    return;
                }
                existingUser.Email = newEmail;
            }

            Console.Write("Enter Phone: ");
            string newPhone = Console.ReadLine() ?? "";
            if (!string.IsNullOrWhiteSpace(newPhone))
            {
                if (!NotificationService.IsValidPhone(newPhone))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid phone format!");
                    Console.ResetColor();
                    return;
                }
                existingUser.Phone = newPhone;
            }

            userRepository.Update(userId, existingUser);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n User updated successfully!");
            Console.ResetColor();       
        }

        static void DeleteUser()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=========================== Delete User ===========================");
            Console.ResetColor();

            Console.Write("Enter User ID to delete: ");
            string userId = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User ID cannot be empty!");
                Console.ResetColor();
                return;
            }

            User? user = userRepository.Get(userId);
            if (user == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User not found!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine($"\nUser to delete: {user.Name}");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Are you sure? (yes/no): ");
            Console.ResetColor();
            string confirm = Console.ReadLine() ?? "";

            if (confirm.ToLower() == "yes")
            {
                userRepository.Delete(userId);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nUser deleted successfully!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Delete cancelled.");
                Console.ResetColor();
            }
        }

        static void SendNotification()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n=========================== Send Notification ===========================");
                Console.ResetColor();
                
                Console.Write("Enter User ID to send notification: ");
                string userId = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(userId))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("User ID cannot be empty!");
                    Console.ResetColor();
                    return;
                }

                User? user = userRepository.Get(userId);
                if (user == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("User not found!");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine($"\nSending notification to: {user.Name}");

                Console.Write("Enter Message: ");
                string message = Console.ReadLine() ?? "";

                if (string.IsNullOrWhiteSpace(message))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Message cannot be empty!");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine("\nChoose Notification Type:");
                Console.WriteLine("1. Email");
                Console.WriteLine("2. SMS");
                Console.Write("Enter your choice (1 or 2): ");
                string notifChoice = Console.ReadLine() ?? "0";

                INotificationSender? notificationSender = null;

                if (notifChoice == "1")
                {
                    notificationSender = new EmailNotification();
                }
                else if (notifChoice == "2")
                {
                    notificationSender = new SmsNotification();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid notification type!");
                    Console.ResetColor();
                    return;
                }

                if (notificationSender != null)
                {
                    notificationService.Send(notificationSender, message, user);
                }
            }
            catch (ValidationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Validation Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (NotificationSendException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Notification Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (DataAccessException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Data Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (NotificationException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" System Error: {ex.Message}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($" Unexpected Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}
