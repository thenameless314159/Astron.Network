using System.Collections.Generic;

namespace Astron.Core.Threading
{
    public interface ISafeCollection<T>
    {
        int Capacity { get; }
        int Count { get; }


        void Add(T item);
        bool Remove(T item);
        void Clear();

        IReadOnlyCollection<T> GetSnapshot();
    }
}
