using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap_5
{
    public class MinHeap<T> where T : IComparable<T>
    {
        private T[] heap;
        private int size;
        private const int DefaultCapacity = 20;

        public MinHeap(params T[] elements)
        {
            int capacity = Math.Max(DefaultCapacity, elements.Length);
            heap = new T[capacity];
            foreach (var el in elements)
                Add(el);
        }

        public void Add(T value)
        {
            EnsureCapacity();
            heap[size++] = value;
            HeapifyUp();
        }

        public T Peek()
        {
            if (size == 0)
                throw new InvalidOperationException("Heap is empty!");
            return heap[0];
        }

        public T Poll()
        {
            if (size == 0)
                throw new InvalidOperationException("Heap is empty!");

            T min = heap[0];
            heap[0] = heap[size - 1];
            size--;
            HeapifyDown();
            return min;
        }

        private void HeapifyUp()
        {
            int index = size - 1;
            while (HasParent(index))
            {
                int parent = GetParentIndex(index);
                if (heap[parent].CompareTo(heap[index]) <= 0)
                    break;
                Swap(parent, index);
                index = parent;
            }
        }

        private void HeapifyDown()
        {
            int index = 0;
            while (HasLeftChild(index))
            {
                int smallerChild = GetLeftChildIndex(index);
                if (HasRightChild(index) &&
                    heap[GetRightChildIndex(index)].CompareTo(heap[GetLeftChildIndex(index)]) < 0)
                {
                    smallerChild = GetRightChildIndex(index);
                }

                if (heap[index].CompareTo(heap[smallerChild]) <= 0)
                    break;

                Swap(index, smallerChild);
                index = smallerChild;
            }
        }

        public void DecreaseKey(int index, T newValue)
        {
            if (index < 0 || index >= size)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (heap[index].CompareTo(newValue) < 0)
                throw new ArgumentException("New key is greater than current key!");

            heap[index] = newValue;
            HeapifyUpFrom(index);
        }

        private void HeapifyUpFrom(int index)
        {
            while (HasParent(index))
            {
                int parent = GetParentIndex(index);
                if (heap[parent].CompareTo(heap[index]) <= 0)
                    break;
                Swap(parent, index);
                index = parent;
            }
        }

        public MinHeap<T> Merge(MinHeap<T> other)
        {
            T[] merged = new T[this.size + other.size];
            Array.Copy(this.heap, 0, merged, 0, this.size);
            Array.Copy(other.heap, 0, merged, this.size, other.size);

            return new MinHeap<T>(merged);
        }

        private void EnsureCapacity()
        {
            if (size == heap.Length)
                Array.Resize(ref heap, heap.Length * 2);
        }

        private void Swap(int i, int j)
        {
            (heap[i], heap[j]) = (heap[j], heap[i]);
        }

        private int GetLeftChildIndex(int parent) => 2 * parent + 1;
        private int GetRightChildIndex(int parent) => 2 * parent + 2;
        private int GetParentIndex(int child) => (child - 1) / 2;

        private bool HasLeftChild(int i) => GetLeftChildIndex(i) < size;
        private bool HasRightChild(int i) => GetRightChildIndex(i) < size;
        private bool HasParent(int i) => i > 0;

        public override string ToString()
        {
            T[] actual = new T[size];
            Array.Copy(heap, actual, size);
            return "[" + string.Join(", ", actual) + "]";
        }
    }

    public static class Program
    {
        public static void Main()
        {
            var heap = new MinHeap<int>(5, 3, 8, 1, 4);
            Console.WriteLine(heap); // [1, 3, 8, 5, 4]
            Console.WriteLine("peek: " + heap.Peek());
            Console.WriteLine("poll: " + heap.Poll());
            Console.WriteLine(heap);

            heap.DecreaseKey(2, 0);
            Console.WriteLine("after decreaseKey: " + heap);

            var heap2 = new MinHeap<int>(7, 2);
            var merged = heap.Merge(heap2);
            Console.WriteLine("merged: " + merged);
        }
    }
}
