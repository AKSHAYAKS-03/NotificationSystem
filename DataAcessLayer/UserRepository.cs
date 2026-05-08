using System;
using System.Collections.Generic;
using Interfaces;
using Models;

namespace DataAccessLayer
{
    // UserRepository inherits from BaseRepository and implements IUserRepository
    public class UserRepository : BaseRepository<string, User>, IUserRepository<string, User>
    {
        static int idCounter = 1;

        public User Create(User user)
        {
            user.Id = idCounter.ToString();
            idCounter++;
            this[user.Id] = user;  // User indexer
            return user;
        }

        public User? Get(string id)
        {
            if (store.ContainsKey(id))
            {
                return this[id];
            }
            return null;
        }

        public List<User> GetAll()
        {
            return new List<User>(store.Values);
        }

        public User? Update(string id, User user)
        {
            if (store.ContainsKey(id))
            {
                user.Id = id;
                this[id] = user;
                return user;
            }
            return null;
        }

        public User Delete(string id)
        {
            if (store.ContainsKey(id))
            {
                var user = this[id];
                store.Remove(id);
                return user;
            }
            throw new ArgumentException("User not found");
        }
    }
}