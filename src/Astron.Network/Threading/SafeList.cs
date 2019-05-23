using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Astron.Network.Threading
{
    public class SafeList<T> : ISafeCollection<T>
    {
        private readonly List<T> _list;
        private readonly object  _locker = new object();

        public T this[int index]
        {
            get { lock (_locker) return _list[index]; }
            set { lock (_locker) _list[index] = value; }
        }

        public int Capacity { get; }

        public int Count { get { lock (_locker) return _list.Count; } }

        public SafeList(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            Capacity = capacity;
            _list    = new List<T>(capacity);
        }

        public void Add(T item)
        {
            lock (_locker) _list.Add(item);
        }

        public bool Remove(T item)
        {
            lock (_locker) return _list.Remove(item);
        }

        public void Clear()
        {
            lock (_locker) _list.Clear();
        }

        public IReadOnlyCollection<T> GetSnapshot()
        {
            lock (_locker)
                return ImmutableList<T>.Empty.AddRange(_list);
        }
    }
}
