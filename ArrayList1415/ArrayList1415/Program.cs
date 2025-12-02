using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class MyArrayDeque<T>
{
    private T[] elements;
    private int head;
    private int tail;

    private const int DEFAULT_CAPACITY = 16;

    public MyArrayDeque()
    {
        elements = new T[DEFAULT_CAPACITY];
        head = 0;
        tail = 0;
    }

    public MyArrayDeque(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        int capacity = Math.Max(a.Length, DEFAULT_CAPACITY);
        elements = new T[capacity];
        head = 0;
        tail = 0;

        foreach (var item in a)
        {
            AddLast(item);
        }
    }

    public MyArrayDeque(int numElements)
    {
        if (numElements < 1)
            throw new ArgumentException("Capacity must be positive", nameof(numElements));

        elements = new T[numElements];
        head = 0;
        tail = 0;
    }

    public void Add(T e)
    {
        AddLast(e);
    }

    public void AddAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        foreach (var item in a)
        {
            AddLast(item);
        }
    }

    public void Clear()
    {
        int size = Size();
        for (int i = 0; i < size; i++)
        {
            elements[(head + i) & (elements.Length - 1)] = default(T);
        }
        head = 0;
        tail = 0;
    }

    public bool Contains(object o)
    {
        int size = Size();
        for (int i = 0; i < size; i++)
        {
            int index = (head + i) & (elements.Length - 1);
            if (Equals(elements[index], o))
                return true;
        }
        return false;
    }

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

    public bool IsEmpty()
    {
        return head == tail;
    }

    public bool Remove(object o)
    {
        return RemoveFirstOccurrence(o);
    }

    public bool RemoveAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        bool modified = false;
        foreach (var item in a)
        {
            while (Remove(item))
            {
                modified = true;
            }
        }
        return modified;
    }

    public bool RetainAll(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        bool modified = false;
        int size = Size();

        for (int i = size - 1; i >= 0; i--)
        {
            int index = (head + i) & (elements.Length - 1);
            T element = elements[index];

            if (!a.Contains(element))
            {
                RemoveAt(i);
                modified = true;
            }
        }

        return modified;
    }

    public int Size()
    {
        return (tail - head) & (elements.Length - 1);
    }

    public object[] ToArray()
    {
        int size = Size();
        object[] result = new object[size];

        for (int i = 0; i < size; i++)
        {
            result[i] = elements[(head + i) & (elements.Length - 1)];
        }

        return result;
    }

    public T[] ToArray(T[] a)
    {
        int size = Size();

        if (a == null || a.Length < size)
        {
            a = new T[size];
        }

        for (int i = 0; i < size; i++)
        {
            a[i] = elements[(head + i) & (elements.Length - 1)];
        }

        if (a.Length > size)
        {
            a[size] = default(T);
        }

        return a;
    }

    public T Element()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Deque is empty");

        return elements[head];
    }

    public bool Offer(T obj)
    {
        AddLast(obj);
        return true;
    }

    public T Peek()
    {
        if (IsEmpty())
            return default(T);

        return elements[head];
    }

    public T Poll()
    {
        if (IsEmpty())
            return default(T);

        return RemoveFirst();
    }

    public void AddFirst(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        head = (head - 1) & (elements.Length - 1);
        elements[head] = obj;

        if (head == tail)
        {
            DoubleCapacity();
        }
    }

    public void AddLast(T obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        elements[tail] = obj;
        tail = (tail + 1) & (elements.Length - 1);

        if (tail == head)
        {
            DoubleCapacity();
        }
    }

    public T GetFirst()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Deque is empty");

        return elements[head];
    }

    public T GetLast()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Deque is empty");

        return elements[(tail - 1) & (elements.Length - 1)];
    }

    public bool OfferFirst(T obj)
    {
        AddFirst(obj);
        return true;
    }

    public bool OfferLast(T obj)
    {
        AddLast(obj);
        return true;
    }

    public T Pop()
    {
        return RemoveFirst();
    }

    public void Push(T obj)
    {
        AddFirst(obj);
    }

    public T PeekFirst()
    {
        if (IsEmpty())
            return default(T);

        return elements[head];
    }

    public T PeekLast()
    {
        if (IsEmpty())
            return default(T);

        return elements[(tail - 1) & (elements.Length - 1)];
    }

    public T PollFirst()
    {
        if (IsEmpty())
            return default(T);

        return RemoveFirst();
    }

    public T PollLast()
    {
        if (IsEmpty())
            return default(T);

        return RemoveLast();
    }

    public T RemoveLast()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Deque is empty");

        tail = (tail - 1) & (elements.Length - 1);
        T result = elements[tail];
        elements[tail] = default(T);
        return result;
    }

    public T RemoveFirst()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Deque is empty");

        T result = elements[head];
        elements[head] = default(T);
        head = (head + 1) & (elements.Length - 1);
        return result;
    }

    public bool RemoveLastOccurrence(object obj)
    {
        if (obj == null)
            return false;

        int size = Size();
        for (int i = size - 1; i >= 0; i--)
        {
            int index = (head + i) & (elements.Length - 1);
            if (Equals(elements[index], obj))
            {
                RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    public bool RemoveFirstOccurrence(object obj)
    {
        if (obj == null)
            return false;

        int size = Size();
        for (int i = 0; i < size; i++)
        {
            int index = (head + i) & (elements.Length - 1);
            if (Equals(elements[index], obj))
            {
                RemoveAt(i);
                return true;
            }
        }
        return false;
    }

    private void RemoveAt(int position)
    {
        int size = Size();
        if (position < 0 || position >= size)
            throw new ArgumentOutOfRangeException(nameof(position));

        int index = (head + position) & (elements.Length - 1);

        if (position < size / 2)
        {
            for (int i = position; i > 0; i--)
            {
                int curr = (head + i) & (elements.Length - 1);
                int prev = (head + i - 1) & (elements.Length - 1);
                elements[curr] = elements[prev];
            }
            elements[head] = default(T);
            head = (head + 1) & (elements.Length - 1);
        }
        else
        {
            for (int i = position; i < size - 1; i++)
            {
                int curr = (head + i) & (elements.Length - 1);
                int next = (head + i + 1) & (elements.Length - 1);
                elements[curr] = elements[next];
            }
            tail = (tail - 1) & (elements.Length - 1);
            elements[tail] = default(T);
        }
    }

    private void DoubleCapacity()
    {
        int oldCapacity = elements.Length;
        int newCapacity = oldCapacity * 2;

        if (newCapacity < 0)
            throw new InvalidOperationException("Deque is too large");

        T[] newElements = new T[newCapacity];
        int size = Size();

        for (int i = 0; i < size; i++)
        {
            newElements[i] = elements[(head + i) & (elements.Length - 1)];
        }

        elements = newElements;
        head = 0;
        tail = size;
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Тест MyArrayDeque<int> ===\n");

        MyArrayDeque<int> deque = new MyArrayDeque<int>();

        Console.WriteLine("AddLast: 1, 2, 3");
        deque.AddLast(1);
        deque.AddLast(2);
        deque.AddLast(3);
        PrintDeque(deque);

        Console.WriteLine("\nAddFirst: 0, -1");
        deque.AddFirst(0);
        deque.AddFirst(-1);
        PrintDeque(deque);

        Console.WriteLine($"\nGetFirst: {deque.GetFirst()}");
        Console.WriteLine($"GetLast: {deque.GetLast()}");
        Console.WriteLine($"PeekFirst: {deque.PeekFirst()}");
        Console.WriteLine($"PeekLast: {deque.PeekLast()}");

        Console.WriteLine($"\nSize: {deque.Size()}");
        Console.WriteLine($"IsEmpty: {deque.IsEmpty()}");
        Console.WriteLine($"Contains(2): {deque.Contains(2)}");
        Console.WriteLine($"Contains(999): {deque.Contains(999)}");

        Console.WriteLine("\n--- Poll операции ---");
        Console.WriteLine($"PollFirst: {deque.PollFirst()}");
        Console.WriteLine($"PollLast: {deque.PollLast()}");
        PrintDeque(deque);

        Console.WriteLine("\n--- Push/Pop (как стек) ---");
        deque.Push(100);
        deque.Push(200);
        PrintDeque(deque);
        Console.WriteLine($"Pop: {deque.Pop()}");
        Console.WriteLine($"Pop: {deque.Pop()}");
        PrintDeque(deque);

        Console.WriteLine("\n--- Remove операции ---");
        deque.AddLast(5);
        deque.AddLast(2);
        deque.AddLast(5);
        PrintDeque(deque);

        Console.WriteLine($"RemoveFirstOccurrence(5): {deque.RemoveFirstOccurrence(5)}");
        PrintDeque(deque);

        Console.WriteLine($"RemoveLastOccurrence(5): {deque.RemoveLastOccurrence(5)}");
        PrintDeque(deque);

        Console.WriteLine("\n--- AddAll и ToArray ---");
        int[] arr = { 10, 20, 30 };
        deque.AddAll(arr);
        PrintDeque(deque);

        object[] array = deque.ToArray();
        Console.WriteLine($"ToArray: [{string.Join(", ", array)}]");

        Console.WriteLine("\n--- Clear ---");
        deque.Clear();
        Console.WriteLine($"After Clear - Size: {deque.Size()}, IsEmpty: {deque.IsEmpty()}");

        Console.WriteLine("\n=== Тест с конструктором из массива ===");
        int[] initialData = { 1, 2, 3, 4, 5 };
        MyArrayDeque<int> deque2 = new MyArrayDeque<int>(initialData);
        PrintDeque(deque2);

        Console.WriteLine("\n=== Тест автоувеличения capacity ===");
        MyArrayDeque<int> smallDeque = new MyArrayDeque<int>(4);
        for (int i = 1; i <= 10; i++)
        {
            smallDeque.AddLast(i);
        }
        Console.WriteLine($"После добавления 10 элементов в deque capacity=4:");
        PrintDeque(smallDeque);

        Console.WriteLine("\n=== Тест RemoveAll/RetainAll ===");
        MyArrayDeque<int> deque3 = new MyArrayDeque<int>();
        for (int i = 1; i <= 5; i++)
            deque3.AddLast(i);

        PrintDeque(deque3);

        int[] toRemove = { 2, 4 };
        deque3.RemoveAll(toRemove);
        Console.WriteLine("После RemoveAll([2, 4]):");
        PrintDeque(deque3);

        int[] toRetain = { 1, 3 };
        deque3.RetainAll(toRetain);
        Console.WriteLine("После RetainAll([1, 3]):");
        PrintDeque(deque3);

        Console.WriteLine("\n=== Тест исключений ===");
        MyArrayDeque<int> emptyDeque = new MyArrayDeque<int>();

        try
        {
            emptyDeque.GetFirst();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"GetFirst на пустой deque: {ex.Message}");
        }

        try
        {
            emptyDeque.RemoveLast();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"RemoveLast на пустой deque: {ex.Message}");
        }

        Console.WriteLine($"\nPoll на пустой deque: {emptyDeque.Poll()} (должен вернуть default)");
        Console.WriteLine($"Peek на пустой deque: {emptyDeque.Peek()} (должен вернуть default)");
    }

    private static void PrintDeque<T>(MyArrayDeque<T> deque)
    {
        object[] arr = deque.ToArray();
        Console.WriteLine($"Deque (size={deque.Size()}): [{string.Join(", ", arr)}]");
    }
}
