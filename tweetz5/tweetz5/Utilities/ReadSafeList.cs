using System.Collections;
using System.Collections.Generic;

namespace tweetz5.Utilities
{
    public class ReadSafeList<T> : IList<T>
    {
        private readonly List<T> _list = new List<T>();
        private readonly object _lock = new object();

        public IEnumerator<T> GetEnumerator()
        {
            return Clone().GetEnumerator();
        }

        private List<T> Clone()
        {
            lock (_lock)
            {
                var newList = new List<T>();
                newList.AddRange(_list);
                return newList;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            lock (_lock)
            {
                _list.Add(item);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_lock)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_lock)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_lock)
            {
                return _list.Remove(item);
            }
        }

        public int Count { get { return _list.Count; } }
        public bool IsReadOnly { get { return false; } }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (_lock)
            {
                _list.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lock)
            {
                _list.RemoveAt(index);
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_lock)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (_lock)
                {
                    _list[index] = value;
                }
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            lock (_lock)
            {
                _list.AddRange(collection);
            }
        }
    }
}