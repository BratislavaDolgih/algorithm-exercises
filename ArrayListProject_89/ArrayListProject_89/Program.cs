using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayListProject_89
{
    public class MyArrayList<T>
    {
        private T[] elementData;
        private int size;

        private const int DEFAULT_CAPACITY = 10;

        // Конструктор 1: Пустой список
        public MyArrayList()
        {
            elementData = new T[DEFAULT_CAPACITY];
            size = 0;
        }

        // Конструктор 2: Из массива
        public MyArrayList(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            elementData = new T[a.Length];
            Array.Copy(a, elementData, a.Length);
            size = a.Length;
        }

        // Конструктор 3: С заданной ёмкостью
        public MyArrayList(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentException("Capacity cannot be negative", nameof(capacity));

            elementData = new T[capacity];
            size = 0;
        }

        // Добавление элемента в конец
        public void Add(T e)
        {
            EnsureCapacity(size + 1);
            elementData[size++] = e;
        }

        // Добавление массива элементов
        public void AddAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            EnsureCapacity(size + a.Length);
            Array.Copy(a, 0, elementData, size, a.Length);
            size += a.Length;
        }

        // Очистка списка
        public void Clear()
        {
            Array.Clear(elementData, 0, size);
            size = 0;
        }

        // Проверка наличия элемента
        public bool Contains(object o)
        {
            return IndexOf(o) >= 0;
        }

        // Проверка наличия всех элементов
        public bool ContainsAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            foreach (var item in a)
            {
                if (!Contains(item))
                    return false;
            }
            return true;
        }

        // Проверка на пустоту
        public bool IsEmpty()
        {
            return size == 0;
        }

        // Удаление элемента (первого вхождения)
        public bool Remove(object o)
        {
            int index = IndexOf(o);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        // Удаление всех указанных элементов
        public bool RemoveAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            bool modified = false;
            for (int i = size - 1; i >= 0; i--)
            {
                if (a.Contains(elementData[i]))
                {
                    RemoveAt(i);
                    modified = true;
                }
            }
            return modified;
        }

        // Оставить только указанные элементы
        public bool RetainAll(T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            bool modified = false;
            for (int i = size - 1; i >= 0; i--)
            {
                if (!a.Contains(elementData[i]))
                {
                    RemoveAt(i);
                    modified = true;
                }
            }
            return modified;
        }

        // Размер списка
        public int Size()
        {
            return size;
        }

        // Преобразование в массив
        public object[] ToArray()
        {
            object[] result = new object[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = elementData[i];
            }
            return result;
        }

        // Преобразование в типизированный массив
        public T[] ToArray(T[] a)
        {
            if (a == null || a.Length < size)
            {
                a = new T[size];
            }
            Array.Copy(elementData, 0, a, 0, size);

            if (a.Length > size)
            {
                a[size] = default(T);
            }

            return a;
        }

        // Добавление элемента по индексу
        public void Add(int index, T e)
        {
            CheckIndexForAdd(index);
            EnsureCapacity(size + 1);

            if (index < size)
            {
                Array.Copy(elementData, index, elementData, index + 1, size - index);
            }

            elementData[index] = e;
            size++;
        }

        // Добавление массива по индексу
        public void AddAll(int index, T[] a)
        {
            if (a == null)
                throw new ArgumentNullException(nameof(a));

            CheckIndexForAdd(index);
            EnsureCapacity(size + a.Length);

            if (index < size)
            {
                Array.Copy(elementData, index, elementData, index + a.Length, size - index);
            }

            Array.Copy(a, 0, elementData, index, a.Length);
            size += a.Length;
        }

        // Получение элемента по индексу
        public T Get(int index)
        {
            CheckIndex(index);
            return elementData[index];
        }

        // Поиск индекса элемента
        public int IndexOf(object o)
        {
            for (int i = 0; i < size; i++)
            {
                if (Equals(elementData[i], o))
                    return i;
            }
            return -1;
        }

        // Поиск последнего индекса элемента
        public int LastIndexOf(object o)
        {
            for (int i = size - 1; i >= 0; i--)
            {
                if (Equals(elementData[i], o))
                    return i;
            }
            return -1;
        }

        // Удаление элемента по индексу
        public T RemoveAt(int index)
        {
            CheckIndex(index);
            T oldValue = elementData[index];

            int numMoved = size - index - 1;
            if (numMoved > 0)
            {
                Array.Copy(elementData, index + 1, elementData, index, numMoved);
            }

            elementData[--size] = default(T);
            return oldValue;
        }

        // Замена элемента по индексу
        public T Set(int index, T e)
        {
            CheckIndex(index);
            T oldValue = elementData[index];
            elementData[index] = e;
            return oldValue;
        }

        // Получение подсписка
        public MyArrayList<T> SubList(int fromIndex, int toIndex)
        {
            if (fromIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(fromIndex), "Index cannot be negative");
            if (toIndex > size)
                throw new ArgumentOutOfRangeException(nameof(toIndex), "toIndex > size");
            if (fromIndex > toIndex)
                throw new ArgumentException("fromIndex > toIndex");

            int newSize = toIndex - fromIndex;
            T[] subArray = new T[newSize];
            Array.Copy(elementData, fromIndex, subArray, 0, newSize);

            return new MyArrayList<T>(subArray);
        }

        // Вспомогательные методы

        private void EnsureCapacity(int minCapacity)
        {
            if (minCapacity > elementData.Length)
            {
                int newCapacity = (int)(elementData.Length * 1.5) + 1;
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                T[] newArray = new T[newCapacity];
                Array.Copy(elementData, newArray, size);
                elementData = newArray;
            }
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= size)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Index: {index}, Size: {size}");
        }

        private void CheckIndexForAdd(int index)
        {
            if (index < 0 || index > size)
                throw new ArgumentOutOfRangeException(nameof(index),
                    $"Index: {index}, Size: {size}");
        }
    }

    // Пример использования
    //public class Program
    //{
    //    public static void Main()
    //    {
    //        // Тест основных операций
    //        MyArrayList<int> list = new MyArrayList<int>();

    //        // Добавление элементов
    //        list.Add(10);
    //        list.Add(20);
    //        list.Add(30);
    //        Console.WriteLine($"Size: {list.Size()}"); // 3

    //        // Добавление по индексу
    //        list.Add(1, 15);
    //        Console.WriteLine($"Get(1): {list.Get(1)}"); // 15

    //        // Поиск
    //        Console.WriteLine($"IndexOf(20): {list.IndexOf(20)}"); // 2

    //        // Contains
    //        Console.WriteLine($"Contains(30): {list.Contains(30)}"); // True

    //        // Удаление
    //        list.Remove(15);
    //        Console.WriteLine($"After remove(15), size: {list.Size()}"); // 3

    //        // SubList
    //        var subList = list.SubList(0, 2);
    //        Console.WriteLine($"SubList size: {subList.Size()}"); // 2

    //        // ToArray
    //        int[] arr = list.ToArray(null);
    //        Console.WriteLine($"Array: [{string.Join(", ", arr)}]");
    //    }
    // }
}
