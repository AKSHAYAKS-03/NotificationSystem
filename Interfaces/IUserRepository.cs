using Models;
using System.Collections.Generic;

namespace Interfaces
{
    public interface IUserRepository<Key, T>
    {
        T Create(T item);
        T? Get(Key id);
        List<T>? GetAll();
        T? Update(Key id, T item);
        T Delete(Key id);
    }
}