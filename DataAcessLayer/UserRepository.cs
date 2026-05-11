using System;
using System.Collections.Generic;
using Interfaces;
using Models;
using Npgsql;




namespace DataAccessLayer
{
    // UserRepository inherits from BaseRepository and implements IUserRepository
    public class UserRepository : BaseRepository<int, User>, IUserRepository<int, User>
    {
        DbConnection con = new DbConnection();

        public User Create(User user)
        {       
            string query = "INSERT INTO Users(name, email, phone) VALUES(@name, @email, @phone) RETURNING id";
 
            NpgsqlConnection conn = con.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            
            // Add parameters to prevent SQL injection
            cmd.Parameters.AddWithValue("@name", user.Name ?? "");
            cmd.Parameters.AddWithValue("@email", user.Email ?? "");
            cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");

            try
            {
                conn.Open();
                object? result = cmd.ExecuteScalar();
                if(result != null && int.TryParse(result.ToString(), out int userId))
                {
                    user.Id = userId;
                    this[user.Id] = user;
                    Console.WriteLine("User created successfully in the database.");
                }
                else
                {
                    Console.WriteLine("Failed to create user in the database.");
                }
                
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                throw new DataAccessException("Email already exists. Please use a different email.", ex);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error creating user in the database: {ex.Message}");
                throw;
            }
            finally
            {
                conn.Close();
            }
            return user;
        }

        public bool EmailExists(string email, int? excludeUserId = null)
        {
            string query = excludeUserId.HasValue
                ? @"
                    SELECT 1
                    FROM Users
                    WHERE LOWER(email) = LOWER(@email)
                      AND id <> @excludeUserId
                    LIMIT 1"
                : @"
                    SELECT 1
                    FROM Users
                    WHERE LOWER(email) = LOWER(@email)
                    LIMIT 1";

            using NpgsqlConnection conn = con.GetConnection();
            using NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@email", email ?? "");

            if (excludeUserId.HasValue)
            {
                cmd.Parameters.AddWithValue("@excludeUserId", excludeUserId.Value);
            }

            try
            {
                conn.Open();
                object? result = cmd.ExecuteScalar();
                return result != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking duplicate email: {ex.Message}");
                throw;
            }
        }


        public User? Get(int id)
        {
            User? u = null;
            string query = "SELECT * FROM Users WHERE id = @id";
            NpgsqlConnection conn = con.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                conn.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    u = new User();
                    u.Id = (int)reader["id"];
                    u.Name = (reader["name"] as string) ?? "";
                    u.Email = (reader["email"] as string) ?? "";
                    u.Phone = (reader["phone"] as string) ?? "";   
                }
                
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error getting user from the database: {ex.Message}");
                throw;
            }
            finally
            {
                conn.Close();
            }

            // if (store.ContainsKey(id))
            // {
            //     return this[id];
            // }
            // return null;
            return u;
        }

        public List<User> GetAll()
        {
            List<User> users = new List<User>();
            string query = "SELECT * FROM Users";
            NpgsqlConnection conn = con.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);

            try
            {
                conn.Open();
                NpgsqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    User u = new User();
                    u.Id = (int)reader["id"];
                    u.Name = (reader["name"] as string) ?? "";
                    u.Email = (reader["email"] as string) ?? "";
                    u.Phone = (reader["phone"] as string) ?? "";
                    users.Add(u);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error getting user from the database: {ex.Message}");
                throw;
            }
            finally
            {
                conn.Close();
            }
                return users;
        }

        public User? Update(int id, User user)
        {
            if (EmailExists(user.Email, id))
            {
                throw new DataAccessException("Email already exists. Please use a different email.");
            }

            string query = "UPDATE Users SET name = @name, email = @email, phone = @phone WHERE id = @id";

            NpgsqlConnection conn = con.GetConnection();
            NpgsqlCommand cmd = new NpgsqlCommand(query, conn);
            
            // Add parameters to prevent SQL injection
            cmd.Parameters.AddWithValue("@name", user.Name ?? "");
            cmd.Parameters.AddWithValue("@email", user.Email ?? "");
            cmd.Parameters.AddWithValue("@phone", user.Phone ?? "");
            cmd.Parameters.AddWithValue("@id", id);

            try
            {
                conn.Open();
                int result = cmd.ExecuteNonQuery();
                if(result > 0)
                {
                    Console.WriteLine("User updated successfully in the database.");
                    user.Id = id;
                    this[id] = user; 
               
                }
                else
                {
                    Console.WriteLine("Failed to update user in the database.");
                }
                
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error updating user in the database: {ex.Message}");
                throw;
            }
            finally
            {
                conn.Close();
            }
                 return user;
        }

        public User? Delete(int id)
        {
            User? user = Get(id);
            
            NpgsqlConnection conn = con.GetConnection();

            try
            {
                conn.Open();

                // First, delete all notifications associated with this user (cascading delete)
                string deleteNotificationsQuery = "DELETE FROM Notifications WHERE userid = @id";
                NpgsqlCommand deleteNotificationsCmd = new NpgsqlCommand(deleteNotificationsQuery, conn);
                deleteNotificationsCmd.Parameters.AddWithValue("@id", id);
                
                int notificationResult = deleteNotificationsCmd.ExecuteNonQuery();
                if (notificationResult > 0)
                {
                    Console.WriteLine($"Deleted {notificationResult} associated notification(s).");
                }

                // Then, delete the user
                string deleteUserQuery = "DELETE FROM Users WHERE id = @id";
                NpgsqlCommand deleteUserCmd = new NpgsqlCommand(deleteUserQuery, conn);
                deleteUserCmd.Parameters.AddWithValue("@id", id);
                
                int userResult = deleteUserCmd.ExecuteNonQuery();
                if(userResult > 0)
                {
                    Console.WriteLine("User deleted successfully from the database.");            
                }
                else
                {
                    Console.WriteLine("Failed to delete user from the database.");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error deleting user in the database: {ex.Message}");
                throw;
            }
            finally
            {
                conn.Close();
            }
            return user;

        }
    }
}
