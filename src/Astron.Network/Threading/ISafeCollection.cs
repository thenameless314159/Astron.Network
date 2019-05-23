using System;
using System.Collections.Generic;
using System.Text;

namespace Astron.Network.Threading
{
    public interface ISafeCollection<T>
    {
        T this[int index] { get; set; }
        int Capacity { get; }
        int Count { get; }


        void Add(T item);
        bool Remove(T item);
        void Clear();

        IReadOnlyCollection<T> GetSnapshot();
    }
}
