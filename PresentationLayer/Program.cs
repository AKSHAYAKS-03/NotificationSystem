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
        // Repository object for user database operations
        static UserRepository userRepository = new UserRepository();

        // Service object for notification handling
        static NotificationService notificationService = new NotificationService();
        
        static void Main(string[] args)
        {
            try
            {
                // Initialize database and apply migrations
                DatabaseInitializer.Initialize();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Database initialization failed: {ex.Message}");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=============================================================");
            Console.WriteLine("                 Simple Notification System                   ");
            Console.WriteLine("=============================================================");
            Console.WriteLine();
            Console.ResetColor();

            while (true)
            {
                // Main menu
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

            // Validate user name
            string? nameError = NotificationService.GetNameValidationError(user.Name);
            if (nameError != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(nameError);
                Console.ResetColor();
                return;
            }

            Console.Write("Enter Email: ");
            user.Email = Console.ReadLine() ?? "";

            // Validate email format
            string? emailError = NotificationService.GetEmailValidationError(user.Email);
            if (emailError != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(emailError);
                Console.ResetColor();
                return;
            }

            // Check if email already exists
            if (userRepository.EmailExists(user.Email))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Email already exists! Please use a different email.");
                Console.ResetColor();
                return;
            }

            Console.Write("Enter Phone: ");
            user.Phone = Console.ReadLine() ?? "";

            // Validate phone number
            string? phoneError = NotificationService.GetPhoneValidationError(user.Phone);
            if (phoneError != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(phoneError);
                Console.ResetColor();
                return;
            }

            try
            {
                userRepository.Create(user);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nUser added successfully!");
                Console.ResetColor();
            }
            catch (DataAccessException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        static void ViewUsers()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=========================== View All Users ===========================");
            
            // Fetch all users from database
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
                Console.WriteLine(user + "\n");
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
            string input = Console.ReadLine() ?? "0";

            // Validate user id input
            if (!int.TryParse(input, out int userId) || userId == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid User ID! Please enter a valid number.");
                Console.ResetColor();
                return;
            }

            // Get existing user details
            User? existingUser = userRepository.Get(userId);

            if (existingUser == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("User not found!");
                Console.ResetColor();
                return;
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;

                Console.WriteLine(
                    "\n========== Current Details ==========");

                Console.ResetColor();

                Console.WriteLine(
                    $"Name  : {existingUser.Name}");

                Console.WriteLine(
                    $"Email : {existingUser.Email}");

                Console.WriteLine(
                    $"Phone : {existingUser.Phone}");

                Console.WriteLine(
                    "\nWhat do you want to update?");

                Console.WriteLine("1. Name");
                Console.WriteLine("2. Email");
                Console.WriteLine("3. Phone");
                Console.WriteLine("4. All");
                Console.WriteLine("5. Back");

                Console.Write("\nEnter Choice: ");

                string choice =
                    Console.ReadLine() ?? "";

                switch (choice)
                {
                    case "1":

                        Console.Write(
                            "Enter New Name: ");

                        string name =
                            Console.ReadLine() ?? "";

                        string? updateNameError = NotificationService.GetNameValidationError(name);

                        if (updateNameError != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(updateNameError);
                            Console.ResetColor();
                            break;
                        }

                        existingUser.Name = name;

                        userRepository.Update(
                            userId,
                            existingUser);

                        Console.ForegroundColor =
                            ConsoleColor.Green;

                        Console.WriteLine(
                            "Name updated successfully!");

                        Console.ResetColor();

                        break;

                    case "2":

                        Console.Write(
                            "Enter New Email: ");

                        string email =
                            Console.ReadLine() ?? "";

                        string? updateEmailError = NotificationService.GetEmailValidationError(email);

                        if (updateEmailError != null)
                        {
                            Console.ForegroundColor =
                                ConsoleColor.Red;

                            Console.WriteLine(
                                updateEmailError);

                            Console.ResetColor();

                            break;
                        }

                        // Prevent duplicate emails
                        if (userRepository.EmailExists(email, userId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Email already exists! Please use a different email.");
                            Console.ResetColor();
                            break;
                        }

                        existingUser.Email = email;

                        userRepository.Update(
                            userId,
                            existingUser);

                        Console.ForegroundColor =
                            ConsoleColor.Green;

                        Console.WriteLine(
                            "Email updated successfully!");

                        Console.ResetColor();

                        break;

                    case "3":

                        Console.Write(
                            "Enter New Phone: ");

                        string phone =
                            Console.ReadLine() ?? "";

                        string? updatePhoneError = NotificationService.GetPhoneValidationError(phone);

                        if (updatePhoneError != null)
                        {
                            Console.ForegroundColor =
                                ConsoleColor.Red;

                            Console.WriteLine(
                                updatePhoneError);

                            Console.ResetColor();

                            break;
                        }

                        existingUser.Phone = phone;

                        userRepository.Update(
                            userId,
                            existingUser);

                        Console.ForegroundColor =
                            ConsoleColor.Green;

                        Console.WriteLine(
                            "Phone updated successfully!");

                        Console.ResetColor();

                        break;

                    case "4":

                        Console.Write(
                            "Enter New Name: ");

                        existingUser.Name =
                            Console.ReadLine() ?? "";

                        string? allNameError = NotificationService.GetNameValidationError(existingUser.Name);

                        if (allNameError != null)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(allNameError);
                            Console.ResetColor();
                            break;
                        }

                        Console.Write(
                            "Enter New Email: ");

                        string newEmail =
                            Console.ReadLine() ?? "";

                        string? allEmailError = NotificationService.GetEmailValidationError(newEmail);

                        if (allEmailError != null)
                        {
                            Console.ForegroundColor =
                                ConsoleColor.Red;

                            Console.WriteLine(
                                allEmailError);

                            Console.ResetColor();

                            break;
                        }

                        if (userRepository.EmailExists(newEmail, userId))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Email already exists! Please use a different email.");
                            Console.ResetColor();
                            break;
                        }

                        existingUser.Email = newEmail;

                        Console.Write(
                            "Enter New Phone: ");

                        string newPhone =
                            Console.ReadLine() ?? "";

                        string? allPhoneError = NotificationService.GetPhoneValidationError(newPhone);

                        if (allPhoneError != null)
                        {
                            Console.ForegroundColor =
                                ConsoleColor.Red;

                            Console.WriteLine(
                                allPhoneError);

                            Console.ResetColor();

                            break;
                        }

                        existingUser.Phone = newPhone;

                        // Update all details together
                        userRepository.Update(
                            userId,
                            existingUser);

                        Console.ForegroundColor =
                            ConsoleColor.Green;

                        Console.WriteLine(
                            "User updated successfully!");

                        Console.ResetColor();

                        break;

                    case "5":

                        return;

                    default:

                        Console.ForegroundColor =
                            ConsoleColor.Red;

                        Console.WriteLine(
                            "Invalid Choice!");

                        Console.ResetColor();

                        break;
                }
            }
        }

        static void DeleteUser()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n=========================== Delete User ===========================");
            Console.ResetColor();

            Console.Write("Enter User ID to delete: ");
            string input = Console.ReadLine() ?? "0";

            // Check valid user id
            if (!int.TryParse(input, out int userId) || userId == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid User ID! Please enter a valid number.");
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

            // Confirm before deleting user
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
                string input = Console.ReadLine() ?? "0";

                if (!int.TryParse(input, out int userId) || userId == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid User ID! Please enter a valid number.");
                    Console.ResetColor();
                    return;
                }

                // Fetch user before sending notification
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

                // Message should not be empty
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

                // Select notification sender type
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