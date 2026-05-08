using System.Collections.Generic;

namespace DataAccessLayer
{
    public abstract class BaseRepository<TKey, T>
        where TKey : notnull
    {
        protected Dictionary<TKey, T> store = new Dictionary<TKey, T>();

        // Indexer to access items in the store like a dictionary
        public T this[TKey id]
        {
            get { return store[id]; }
            set { store[id] = value; }
        }
    }
}