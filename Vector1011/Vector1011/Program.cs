using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MyVector<T>
{
    private T[] elementData;
    private int elementCount;
    private int capacityIncrement;

    public MyVector(int initialCapacity, int capacityIncrement)
    {
        if (initialCapacity < 0)
            throw new ArgumentException("Capacity cannot be negative");

        this.elementData = new T[Math.Max(initialCapacity, 1)];
        this.elementCount = 0;
        this.capacityIncrement = capacityIncrement;
    }

    public MyVector(int initialCapacity) : this(initialCapacity, 0) { }

    public MyVector() : this(10, 0) { }

    public MyVector(T[] a)
    {
        if (a == null)
            throw new ArgumentNullException(nameof(a));

        this.elementData = new T[a.Length];
        Array.Copy(a, elementData, a.Length);
        this.elementCount = a.Length;
        this.capacityIncrement = 0;
    }

    private void EnsureCapacity(int minCapacity)
    {
        if (minCapacity > elementData.Length)
        {
            int newCapacity = capacityIncrement > 0
                ? elementData.Length + capacityIncrement
                : elementData.Length * 2;

            if (newCapacity < minCapacity)
                newCapacity = minCapacity;

            T[] newArray = new T[newCapacity];
            Array.Copy(elementData, newArray, elementCount);
            elementData = newArray;
        }
    }

    public void Add(T e)
    {
        EnsureCapacity(elementCount + 1);
        elementData[elementCount++] = e;
    }

    public void AddAll(T[] a)
    {
        if (a == null || a.Length == 0) return;

        EnsureCapacity(elementCount + a.Length);
        Array.Copy(a, 0, elementData, elementCount, a.Length);
        elementCount += a.Length;
    }

    public void Clear()
    {
        for (int i = 0; i < elementCount; i++)
            elementData[i] = default(T);
        elementCount = 0;
    }

    public bool Contains(object o)
    {
        return IndexOf(o) >= 0;
    }

    public bool ContainsAll(T[] a)
    {
        if (a == null) return false;
        foreach (var item in a)
        {
            if (!Contains(item))
                return false;
        }
        return true;
    }

    public bool IsEmpty()
    {
        return elementCount == 0;
    }

    public bool Remove(object o)
    {
        if (!(o is T)) return false;

        int index = IndexOf(o);
        if (index >= 0)
        {
            RemoveAt(index);
            return true;
        }
        return false;
    }

    public void RemoveAll(T[] a)
    {
        if (a == null) return;
        foreach (var item in a)
            Remove(item);
    }

    public void RetainAll(T[] a)
    {
        if (a == null)
        {
            Clear();
            return;
        }

        HashSet<T> toKeep = new HashSet<T>(a);
        int writeIndex = 0;

        for (int i = 0; i < elementCount; i++)
        {
            if (toKeep.Contains(elementData[i]))
            {
                elementData[writeIndex++] = elementData[i];
            }
        }

        for (int i = writeIndex; i < elementCount; i++)
            elementData[i] = default(T);

        elementCount = writeIndex;
    }

    public int Size()
    {
        return elementCount;
    }

    public T[] ToArray()
    {
        T[] result = new T[elementCount];
        Array.Copy(elementData, result, elementCount);
        return result;
    }

    public T[] ToArray(T[] a)
    {
        if (a == null || a.Length < elementCount)
            return ToArray();

        Array.Copy(elementData, a, elementCount);
        if (a.Length > elementCount)
            a[elementCount] = default(T);

        return a;
    }

    public void Add(int index, T e)
    {
        if (index < 0 || index > elementCount)
            throw new IndexOutOfRangeException();

        EnsureCapacity(elementCount + 1);
        Array.Copy(elementData, index, elementData, index + 1, elementCount - index);
        elementData[index] = e;
        elementCount++;
    }

    public void AddAll(int index, T[] a)
    {
        if (a == null || a.Length == 0) return;
        if (index < 0 || index > elementCount)
            throw new IndexOutOfRangeException();

        EnsureCapacity(elementCount + a.Length);
        Array.Copy(elementData, index, elementData, index + a.Length, elementCount - index);
        Array.Copy(a, 0, elementData, index, a.Length);
        elementCount += a.Length;
    }

    public T Get(int index)
    {
        if (index < 0 || index >= elementCount)
            throw new IndexOutOfRangeException();
        return elementData[index];
    }

    public int IndexOf(object o)
    {
        if (!(o is T)) return -1;

        T item = (T)o;
        for (int i = 0; i < elementCount; i++)
        {
            if (EqualityComparer<T>.Default.Equals(elementData[i], item))
                return i;
        }
        return -1;
    }

    public int LastIndexOf(object o)
    {
        if (!(o is T)) return -1;

        T item = (T)o;
        for (int i = elementCount - 1; i >= 0; i--)
        {
            if (EqualityComparer<T>.Default.Equals(elementData[i], item))
                return i;
        }
        return -1;
    }

    public T RemoveAt(int index)
    {
        if (index < 0 || index >= elementCount)
            throw new IndexOutOfRangeException();

        T removed = elementData[index];
        Array.Copy(elementData, index + 1, elementData, index, elementCount - index - 1);
        elementData[--elementCount] = default(T);
        return removed;
    }

    public void Set(int index, T e)
    {
        if (index < 0 || index >= elementCount)
            throw new IndexOutOfRangeException();
        elementData[index] = e;
    }

    public MyVector<T> SubList(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex > elementCount || fromIndex > toIndex)
            throw new ArgumentOutOfRangeException();

        int length = toIndex - fromIndex;
        T[] subArray = new T[length];
        Array.Copy(elementData, fromIndex, subArray, 0, length);
        return new MyVector<T>(subArray);
    }

    public T FirstElement()
    {
        if (elementCount == 0)
            throw new InvalidOperationException("Vector is empty");
        return elementData[0];
    }

    public T LastElement()
    {
        if (elementCount == 0)
            throw new InvalidOperationException("Vector is empty");
        return elementData[elementCount - 1];
    }

    public void RemoveElementAt(int pos)
    {
        RemoveAt(pos);
    }

    public void RemoveRange(int begin, int end)
    {
        if (begin < 0 || end > elementCount || begin > end)
            throw new ArgumentOutOfRangeException();

        int shiftCount = elementCount - end;
        Array.Copy(elementData, end, elementData, begin, shiftCount);

        int newCount = elementCount - (end - begin);
        for (int i = newCount; i < elementCount; i++)
            elementData[i] = default(T);

        elementCount = newCount;
    }
}
