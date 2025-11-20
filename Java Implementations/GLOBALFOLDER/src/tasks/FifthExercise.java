package tasks;

import java.util.Arrays;
import java.util.Comparator;
import java.util.function.IntFunction;

public class FifthExercise {
    public static void main(String... args) {
        MinHeap<Integer> heap = new MinHeap<>(Integer[]::new, 5, 3, 8, 1, 4);
        System.out.println(heap); // [1, 3, 8, 5, 4]
        System.out.println("peek: " + heap.peek());
        System.out.println("poll: " + heap.poll());
        System.out.println(heap);

        heap.decreaseKey(2, 0);
        System.out.println("after decreaseKey: " + heap);

        MinHeap<Integer> heap2 = new MinHeap<>(Integer[]::new, 7, 2);
        MinHeap<Integer> merged = heap.merge(heap2, Integer[]::new);
        System.out.println("merged: " + merged);
    }
}

/**
 * Обобщённый класс, который строит минимальную кучу.
 * @param <T> тип, который реализовывает {@code Comparable<T>}.
 *           Это нужно, чтобы понимать по какому принципу строить кучу
 */
class MinHeap<T extends Comparable<T>> {
    private T[] heap;

    /**
     * «Указатель» на текущий свободный индекс
     */
    private int size;

    private static int DEFAULT_CAPACITY = 20;

    /**
     * Конструктор, получающий на вход массив объектов,
     * реализующих {@code Comparable}
     * @param constr
     * @param args
     */
    @SafeVarargs
    public MinHeap(IntFunction<T[]> constr,
                   T... args) {
        heap = constr.apply(Math.max(
                DEFAULT_CAPACITY,
                args.length
        ));
        for (var arg : args) {
            add(arg);
        }
    }

    public void add(T argument) {
        ensureCapacity();

        heap[size++] = argument;
        heapifyUp();
    }

    public T peek() {
        return heap[0];
    }

    public T poll() {
        if (size == 0) { throw new IllegalStateException("Heap is empty!"); }

        T minimum = heap[0];
        heap[0] = heap[size - 1];
        size--;

        heapifyDown();
        return minimum;
    }

    private void heapifyUp() {
        int index = size - 1;

        while (hasParent(index)) {
            int parentIndex = getParentIndex(index);

            if (heap[parentIndex].compareTo(heap[index]) <= 0) { break; }

            localSwap(parentIndex, index);
            index = parentIndex;
        }
    }

    private void heapifyDown() {
        int index = 0;

        while (hasLeftChild(index)) {
            int lowerChildIndex = getLeftChildIndex(index);

            if (hasRightChild(index)
                    && heap[getRightChildindex(index)].compareTo(heap[getLeftChildIndex(index)]) < 0) {
                lowerChildIndex = getRightChildindex(index);
            }

            if (heap[index].compareTo(heap[lowerChildIndex]) <= 0) {
                break;
            } else {
                localSwap(index, lowerChildIndex);
            }
        }
    }

    private void ensureCapacity() {
        if (size == heap.length) {
            heap = Arrays.copyOf(heap, heap.length * 2);
        }
    }

    private void localSwap(int i1, int i2) {
        var temp = heap[i1];
        heap[i1] = heap[i2];
        heap[i2] = temp;
    }

    private int getLeftChildIndex(int parent) { return 2 * parent + 1; }
    private int getRightChildindex(int parent) { return 2 * parent + 2; }
    private int getParentIndex(int child) { return (child - 1) / 2; }

    private boolean hasLeftChild(int index) { return getLeftChildIndex(index) < size; }
    private boolean hasRightChild(int index) { return getRightChildindex(index) < size; }
    private boolean hasParent(int index) { return index > 0; }

    public void decreaseKey(int index, T newValue) {
        if (index < 0 || index >= size) throw new IndexOutOfBoundsException();

        if (heap[index].compareTo(newValue) < 0)
            throw new IllegalArgumentException("New key is greater than current key!");

        heap[index] = newValue;
        heapifyUpFrom(index);
    }

    private void heapifyUpFrom(int index) {
        while (hasParent(index)) {
            int parentIndex = getParentIndex(index);
            if (heap[parentIndex].compareTo(heap[index]) <= 0) break;
            localSwap(parentIndex, index);
            index = parentIndex;
        }
    }

    public MinHeap<T> merge(MinHeap<T> other, IntFunction<T[]> constr) {
        T[] merged = constr.apply(this.size + other.size);
        System.arraycopy(this.heap, 0, merged, 0, this.size);
        System.arraycopy(other.heap, 0, merged, this.size, other.size);

        return new MinHeap<>(constr, merged);
    }

    @Override
    public String toString() {
        return Arrays.toString(Arrays.copyOf(heap, size));
    }
}