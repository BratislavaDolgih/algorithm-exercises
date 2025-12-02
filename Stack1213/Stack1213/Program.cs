using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MyVector<T>
{
    protected T[] elementData;
    protected int elementCount;
    protected int capacityIncrement;

    private const int DEFAULT_CAPACITY = 10;

    public MyVector()
    {
        elementData = new T[DEFAULT_CAPACITY];
        elementCount = 0;
        capacityIncrement = 0;
    }

    public MyVector(int initialCapacity)
    {
        if (initialCapacity < 0)
            throw new ArgumentException("Illegal Capacity: " + initialCapacity);

        elementData = new T[initialCapacity];
        elementCount = 0;
        capacityIncrement = 0;
    }

    public MyVector(int initialCapacity, int capacityIncrement)
    {
        if (initialCapacity < 0)
            throw new ArgumentException("Illegal Capacity: " + initialCapacity);

        elementData = new T[initialCapacity];
        elementCount = 0;
        this.capacityIncrement = capacityIncrement;
    }

    public void Add(T item)
    {
        EnsureCapacity(elementCount + 1);
        elementData[elementCount++] = item;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= elementCount)
            throw new ArgumentOutOfRangeException(nameof(index),
                $"Index: {index}, Size: {elementCount}");

        return elementData[index];
    }

    public void Set(int index, T item)
    {
        if (index < 0 || index >= elementCount)
            throw new ArgumentOutOfRangeException(nameof(index),
                $"Index: {index}, Size: {elementCount}");

        elementData[index] = item;
    }

    public T RemoveAt(int index)
    {
        if (index < 0 || index >= elementCount)
            throw new ArgumentOutOfRangeException(nameof(index),
                $"Index: {index}, Size: {elementCount}");

        T oldValue = elementData[index];

        int numMoved = elementCount - index - 1;
        if (numMoved > 0)
        {
            Array.Copy(elementData, index + 1, elementData, index, numMoved);
        }

        elementData[--elementCount] = default(T);
        return oldValue;
    }

    public int Size()
    {
        return elementCount;
    }

    public bool IsEmpty()
    {
        return elementCount == 0;
    }

    public int IndexOf(T item)
    {
        for (int i = 0; i < elementCount; i++)
        {
            if (Equals(elementData[i], item))
                return i;
        }
        return -1;
    }

    protected void EnsureCapacity(int minCapacity)
    {
        if (minCapacity > elementData.Length)
        {
            int newCapacity;
            if (capacityIncrement > 0)
            {
                newCapacity = elementData.Length + capacityIncrement;
            }
            else
            {
                newCapacity = elementData.Length * 2;
            }

            if (newCapacity < minCapacity)
                newCapacity = minCapacity;

            T[] newArray = new T[newCapacity];
            Array.Copy(elementData, newArray, elementCount);
            elementData = newArray;
        }
    }
}

public class MyStack<T> : MyVector<T>
{
    public MyStack() : base()
    {
    }

    public MyStack(int initialCapacity) : base(initialCapacity)
    {
    }

    public void Push(T item)
    {
        Add(item);
    }

    public T Pop()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Stack is empty");

        return RemoveAt(elementCount - 1);
    }

    public T Peek()
    {
        if (IsEmpty())
            throw new InvalidOperationException("Stack is empty");

        return Get(elementCount - 1);
    }

    public bool Empty()
    {
        return IsEmpty();
    }

    public int Search(T item)
    {
        for (int i = elementCount - 1; i >= 0; i--)
        {
            if (Equals(elementData[i], item))
            {
                return elementCount - i;
            }
        }
        return -1;
    }
}

/*public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Тест MyStack<int> ===\n");

        MyStack<int> stack = new MyStack<int>();

        Console.WriteLine("Push: 10, 20, 30, 40, 50");
        stack.Push(10);
        stack.Push(20);
        stack.Push(30);
        stack.Push(40);
        stack.Push(50);

        Console.WriteLine($"Peek: {stack.Peek()}");
        Console.WriteLine($"Empty: {stack.Empty()}");
        Console.WriteLine($"Size: {stack.Size()}");

        Console.WriteLine("\n--- Поиск элементов ---");
        Console.WriteLine($"Search(50): {stack.Search(50)}");
        Console.WriteLine($"Search(30): {stack.Search(30)}");
        Console.WriteLine($"Search(10): {stack.Search(10)}");
        Console.WriteLine($"Search(999): {stack.Search(999)}");

        Console.WriteLine("\n--- Pop элементы ---");
        while (!stack.Empty())
        {
            int value = stack.Pop();
            Console.WriteLine($"Pop: {value}, Size: {stack.Size()}");
        }

        Console.WriteLine($"\nStack is empty: {stack.Empty()}");

        Console.WriteLine("\n=== Тест с string ===\n");
        MyStack<string> strStack = new MyStack<string>();

        strStack.Push("First");
        strStack.Push("Second");
        strStack.Push("Third");

        Console.WriteLine($"Peek: {strStack.Peek()}");
        Console.WriteLine($"Search('Second'): {strStack.Search("Second")}");
        Console.WriteLine($"Search('First'): {strStack.Search("First")}");

        Console.WriteLine("\n=== Тест исключений ===\n");
        MyStack<int> emptyStack = new MyStack<int>();

        try
        {
            emptyStack.Pop();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Pop на пустом стеке: {ex.Message}");
        }

        try
        {
            emptyStack.Peek();
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Peek на пустом стеке: {ex.Message}");
        }

        Console.WriteLine("\n=== Тест с начальной ёмкостью ===\n");
        MyStack<double> doubleStack = new MyStack<double>(5);

        for (int i = 1; i <= 10; i++)
        {
            doubleStack.Push(i * 1.5);
        }

        Console.WriteLine($"Size после 10 push: {doubleStack.Size()}");
        Console.WriteLine($"Peek: {doubleStack.Peek()}");
        Console.WriteLine($"Search(7.5): {doubleStack.Search(7.5)}");
    }
}
*/