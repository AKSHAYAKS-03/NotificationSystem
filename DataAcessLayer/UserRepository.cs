using System;
using System.Collections.Generic;
using Interfaces;
using Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer
{
    public class UserRepository : BaseRepository<int, User>, IUserRepository<int, User>
    {
        // Database context object
        private readonly NotificationDbContext con = new NotificationDbContext();

        public bool EmailExists(string email)
        {
            // Convert email to lowercase and remove spaces
            string normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;

            return con.Users.Any(u => u.Email == normalizedEmail);
        }

        public bool EmailExists(string email, int excludeUserId)
        {
            string normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;

            // Check duplicate email except current user
            return con.Users.Any(u => u.Email == normalizedEmail && u.Id != excludeUserId);
        }

        public User Create(User user)
        {
            try
            {
                // Prevent duplicate email entries
                if (EmailExists(user.Email))
                {
                    throw new Exception("Email already exists. Please use a different email.");
                }

                con.Users.Add(user);

                // Save changes into database
                con.SaveChanges();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating user in the database: {ex.Message}");
                throw;
            }
        }

        public User? Get(int id)
        {
            try
            {
                // Fetch single user by id
                return con.Users.FirstOrDefault(u => u.Id == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting user from the database: {ex.Message}");
                throw;
            }
        }

        public List<User> GetAll()
        {
            try
            {
                // Return all users from database
                return con.Users.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting users from the database: {ex.Message}");
                throw;
            }
        }

        public User? Update(int id, User user)
        {
            try
            {
                // Get existing user details
                var updateuser = Get(id);

                if (updateuser == null)
                {
                    throw new Exception($"User with id {id} not found.");
                }

                if (EmailExists(user.Email, id))
                {
                    throw new Exception("Email already exists. Please use a different email.");
                }

                updateuser.Name = user.Name;
                updateuser.Email = user.Email;
                updateuser.Phone = user.Phone;

                con.SaveChanges();

                return updateuser;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user in the database: {ex.Message}");
                throw;
            }
        }

        public User? Delete(int id)
        {
            var user = Get(id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            try
            {
                con.Users.Remove(user);

                con.SaveChanges();

                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user from the database: {ex.Message}");
                throw;
            }
        }
    }
}