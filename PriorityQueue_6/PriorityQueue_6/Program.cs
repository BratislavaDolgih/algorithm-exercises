using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriorityQueue_6
{
    public class MyPriorityQueue<T>
    {
        private T[] _queue;
        private int _size;
        private IComparer<T> _comparator;

        private const int DEFAULT_CAPACITY = 11;

        public MyPriorityQueue() : this(DEFAULT_CAPACITY, Comparer<T>.Default)
        {
            if (_comparator == Comparer<T>.Default && !typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
            {
                throw new InvalidOperationException("Тип T должен реализовать IComparable<T> для использования конструктора по умолчанию.");
            }
        }

        public MyPriorityQueue(int initialCapacity) : this(initialCapacity, Comparer<T>.Default) { }

        public MyPriorityQueue(int initialCapacity, IComparer<T> comparator)
        {
            if (initialCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Начальная ёмкость должна быть положительной.");
            }

            _queue = new T[initialCapacity];
            _size = 0;
            _comparator = comparator;
        }

        public MyPriorityQueue(T[] a) : this(a, Comparer<T>.Default) { }

        private MyPriorityQueue(T[] a, IComparer<T> comparator)
        {
            _comparator = comparator;
            if (a == null)
            {
                _queue = new T[DEFAULT_CAPACITY];
                _size = 0;
            }
            else
            {
                int capacity = Math.Max(DEFAULT_CAPACITY, a.Length);
                _queue = new T[capacity];
                Array.Copy(a, 0, _queue, 0, a.Length);
                _size = a.Length;
                HeapifyAll();
            }
        }

        public MyPriorityQueue(MyPriorityQueue<T> c)
        {
            if (c == null) throw new ArgumentNullException(nameof(c));
            _queue = new T[c._queue.Length];
            Array.Copy(c._queue, _queue, c._size);
            _size = c._size;
            _comparator = c._comparator;
        }

        public int Size() => _size;

        public bool IsEmpty() => _size == 0;

        public bool Add(T e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e), "Элемент не может быть null.");

            EnsureCapacity();
            _queue[_size++] = e;
            HeapifyUp(_size - 1);
            return true;
        }

        public bool Offer(T obj) => Add(obj);

        public void AddAll(T[] a)
        {
            if (a == null) return;
            foreach (var el in a)
            {
                Add(el);
            }
        }

        public T Peek()
        {
            return IsEmpty() ? default(T) : _queue[0];
        }

        public T Element()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Очередь пуста.");
            }
            return _queue[0];
        }

        public T Poll()
        {
            if (IsEmpty())
            {
                return default(T);
            }

            T elementToReturn = _queue[0];

            if (_size == 1)
            {
                _queue[0] = default(T);
                _size = 0;
                return elementToReturn;
            }

            _queue[0] = _queue[_size - 1];
            _queue[_size - 1] = default(T);
            _size--;
            HeapifyDown(0);
            return elementToReturn;
        }

        public T Remove()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Очередь пуста.");
            }
            return Poll();
        }

        public bool Remove(object o)
        {
            if (o == null) return false;

            for (int i = 0; i < _size; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(_queue[i], (T)o))
                {
                    RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public void Clear()
        {
            Array.Clear(_queue, 0, _size);
            _size = 0;
        }

        public bool Contains(object o)
        {
            if (o == null) return false;

            for (int i = 0; i < _size; ++i)
            {
                if (EqualityComparer<T>.Default.Equals(_queue[i], (T)o))
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsAll(T[] a)
        {
            if (a == null) return true;
            foreach (var el in a)
            {
                if (!Contains(el)) return false;
            }
            return true;
        }

        public bool RemoveAll(T[] a)
        {
            if (a == null) return false;
            bool modified = false;
            var elementsToRemove = new HashSet<T>(a);

            for (int i = _size - 1; i >= 0; i--)
            {
                if (elementsToRemove.Contains(_queue[i]))
                {
                    RemoveAt(i);
                    modified = true;
                }
            }
            return modified;
        }

        public bool RetainAll(T[] a)
        {
            if (a == null) return false;
            bool modified = false;
            var elementsToRetain = new HashSet<T>(a);

            for (int i = _size - 1; i >= 0; i--)
            {
                if (!elementsToRetain.Contains(_queue[i]))
                {
                    RemoveAt(i);
                    modified = true;
                }
            }
            return modified;
        }

        public object[] ToArray()
        {
            object[] result = new object[_size];
            Array.Copy(_queue, 0, result, 0, _size);
            return result;
        }

        public T[] ToArray(T[] a)
        {
            if (a == null || a.Length < _size)
            {
                T[] newArray = new T[_size];
                Array.Copy(_queue, 0, newArray, 0, _size);
                return newArray;
            }

            Array.Copy(_queue, 0, a, 0, _size);
            if (a.Length > _size)
            {
                a[_size] = default(T);
            }
            return a;
        }

        private int Compare(T o1, T o2)
        {
            return _comparator.Compare(o1, o2);
        }

        private void RemoveAt(int index)
        {
            if (index < 0 || index >= _size) return;

            if (index == _size - 1)
            {
                _queue[index] = default(T);
                _size--;
                return;
            }

            _queue[index] = _queue[_size - 1];
            _queue[_size - 1] = default(T);
            _size--;

            int parentIndex = GetParentIndex(index);

            if (index > 0 && Compare(_queue[index], _queue[parentIndex]) < 0)
            {
                HeapifyUp(index);
            }
            else
            {
                HeapifyDown(index);
            }
        }

        private void HeapifyAll()
        {
            for (int i = _size / 2 - 1; i >= 0; --i)
            {
                HeapifyDown(i);
            }
        }

        private void HeapifyUp(int index)
        {
            int parentIndex = GetParentIndex(index);

            while (index > 0 && Compare(_queue[index], _queue[parentIndex]) < 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
                parentIndex = GetParentIndex(index);
            }
        }

        private void HeapifyDown(int index)
        {
            int smallest = index;
            int left = GetLeftChildIndex(index);
            int right = GetRightChildIndex(index);

            if (left < _size && Compare(_queue[left], _queue[smallest]) < 0)
            {
                smallest = left;
            }
            if (right < _size && Compare(_queue[right], _queue[smallest]) < 0)
            {
                smallest = right;
            }

            if (smallest != index)
            {
                Swap(index, smallest);
                HeapifyDown(smallest);
            }
        }

        private void EnsureCapacity()
        {
            if (_size >= _queue.Length)
            {
                int currentCapacity = _queue.Length;
                int newCapacity;

                if (currentCapacity < 64)
                {
                    newCapacity = currentCapacity + 2;
                }
                else
                {
                    newCapacity = currentCapacity + (currentCapacity / 2);
                }

                if (newCapacity <= currentCapacity)
                {
                    newCapacity = currentCapacity + 1;
                }

                Array.Resize(ref _queue, newCapacity);
            }
        }

        private void Swap(int f, int s)
        {
            T temporary = _queue[f];
            _queue[f] = _queue[s];
            _queue[s] = temporary;
        }

        private int GetLeftChildIndex(int parentIndex) => 2 * parentIndex + 1;
        private int GetRightChildIndex(int parentIndex) => 2 * parentIndex + 2;
        private int GetParentIndex(int childIndex) => (childIndex - 1) / 2;
    }
}
