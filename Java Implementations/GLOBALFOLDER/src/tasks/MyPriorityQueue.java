package tasks;

import java.util.*;

/**
 * Класс, который реализует <b>очередь с приоритетом</b> на основе кучи.
 */
public class MyPriorityQueue<T>
        implements Queue<T>, Iterable<T> {
    private T[] heap;                         // Массив чисел (сама куча как таковая)
    private int size;                         // Текущая размерность, "курсор" заполнения.
    private Comparator<? super T> comparator; // Компаратор для сравнения

    private static final int DEFAULT_CAPACITY = 12; // Размерность по умолчанию

    /**
     * Fail-fast iterator-variable.
     * <br>Счётчик модификаций, который показывает,
     * сколько раз была модифицирована структура коллекции.</br>
     */
    private int modificationCount = 0;

    /**
     * Конструктор без параметров, заполняющий:
     * <b>массив и размерность</b>.
     */
    public MyPriorityQueue() {
        this(DEFAULT_CAPACITY, null);
    }

    /**
     * Конструктор с параметром, заполняющий:
     * <b>массив и размерность</b>.
     * @param initCapacity входная ёмкость
     */
    public MyPriorityQueue(int initCapacity) {
        this(initCapacity, null);
    }

    /**
     * Конструктор с входными параметрами.
     * @param initCapacity входная ёмкость
     * @param comparator компаратор для сравнения элементов
     */
    public MyPriorityQueue(int initCapacity,
                           Comparator<? super T> comparator) {
        if (initCapacity < 1) throw new IllegalArgumentException("Negative dimension of heap.");

        this.heap = (T[]) new Object[initCapacity];
        this.size = 0;
        this.comparator = comparator;
    }

    /**
     * Конструктор, не принимающий ёмкость.
     * @param another массив каких-то значений, которые мы хотим затолкать в кучу
     * @param comp компаратор сравнения
     */
    public MyPriorityQueue(T[] another, Comparator<? super T> comp) {
        this.comparator = comp;
        if (another == null) {
            this.heap = (T[]) new Object[DEFAULT_CAPACITY];
            this.size = 0;
        } else {
            this.heap = Arrays.copyOf(
                    another,
                    Math.max(DEFAULT_CAPACITY, another.length)
            );
            this.size = another.length;
            heapifyAll();
        }
    }

    /**
     * Конструктор, не принимающий ёмкость.
     * @param another массив каких-то значений, которые мы хотим затолкать в кучу
     */
    public MyPriorityQueue(T[] another) {
        this(another, null);
    }

    /**
     * Конструктор с параметром другой кучи для перезаписывания в новую кучу.
     * @param other другая минимальная куча
     */
    public MyPriorityQueue(MyPriorityQueue<T> other) {
        this.heap = Arrays.copyOf(other.heap, other.heap.length);
        this.size = other.size;
        this.comparator = other.comparator;
        this.modificationCount = other.modificationCount;
    }

    /**
     * Просмотр верхушки без удаления.
     * @return целочисленный элемент и {@code null}, если куча пуста.
     */
    @Override public T peek() {
        return isEmpty() ? null : heap[0];
    }

    /**
     * Аналог {@code peek()}, но с исключением.
     * @throws NoSuchElementException если куча пуста на данный момент
     * @return целочисленный элемент
     */
    @Override public T element() {
        if (isEmpty()) {
            throw new NoSuchElementException("Heap is empty");
        }
        return heap[0];
    }

    /**
     * Переопределённый из интерфейса {@link Queue} метод через кучу
     * @param willBeHeapElement элемент типа {@code Integer}
     * @return {@code true}, если элемент был добавлен, {@code false} в ином случае
     * @see MyPriorityQueue#extraEnsureCapacity()
     * @see MyPriorityQueue#heapifyUp()
     */
    @Override public boolean add(T willBeHeapElement) {
        extraEnsureCapacity(); // Обязательная проверка на увеличение вместимости
        heap[size++] = willBeHeapElement;
        heapifyUp();           // Поднимаем элемент наверх для соблюдения свойства кучи.
        modificationCount++;
        return true;
    }

    /**
     * Метод добавления элементов в непосредственно кучу.
     * @param another массив элементов
     */
    public void addAll(T[] another) {
        for (T el : another) {
            add(el);
        }
    }

    /**
     * Метод аналогичный {@code add()} в случае бесконечных структур.
     * Но будет иметь разницу в ограниченной очереди! В данной реализации — аналог.
     * @param willBeHeapElement элемент целочисленный
     * @return логическое значение успешности вставки
     */
    @Override public boolean offer(T willBeHeapElement) { return add(willBeHeapElement); }

    /**
     * Пустой приватный метод, который поднимает наверх вставленный элемент для соблюдения свойства максимальной кучи.
     * @see MyPriorityQueue#swap(int, int)
     * @see MyPriorityQueue#hasParent(int)
     */
    private void heapifyUp() {
        int index = size - 1; // Забираем индекс последнего вставленного элемента.

        // Проверка проходит, пока есть родитель у индекса.
        // Тормознётся на моменте, либо когда найдёт элемент, большие его самого, либо когда станет корнем
        while (hasParent(index)) {
            int parentIndex = (index - 1) / 2; // Формула родителя: (i-1) / 2.

            // По свойству максимальной кучи:
            // «если родитель больше элемента, то останавливаемся — он на своём месте»
            if (this.compare(heap[parentIndex], heap[index]) <= 0) { break; }

            // Меняем элементы по индексам родителя и текущего элемента, двигаясь наверх.
            swap(parentIndex, index);
            // Если поднялись на уровень родителя, то поимеем его индекс.
            index = parentIndex;
        }
    }

    /**
     * Частный случай метода {@link MyPriorityQueue#heapifyUp()}
     */
    private void heapifyAll() {
        for (int i = size / 2 - 1; i >= 0; --i) {
            heapifyDownFrom(i);
        }
    }

    /**
     * Переопределённый метод получения максимального элемента (<i>верхнего корня</i>).
     * @return элемент с верхушки кучи.
     * @see MyPriorityQueue#heapifyDown()
     */
    @Override public T poll() {
        if (isEmpty()) {
            return null;
        } else if (size == 1) {
            var maximum = heap[0]; // Забираем верхушку
            size--;                // Понижаем размерность (теперь 0)
            heap[0] = null;        // Заполняем пустоту ничем
            modificationCount++;
            return maximum;
        }

        T maximum = heap[0]; // Максимум ровно на верхушке.
        heap[0] = heap[size - 1];  // Поставим последний поставленный.
        size--;                    // Так как удалили верхушку — понижаем размер!
        heapifyDown();             // Выставленный корень следует проверить на свойства кучи.
        modificationCount++;
        return maximum;
    }

    /**
     * Аналогичный метод {@code poll()}, только с исключением в случае пустоты кучи.
     * @return элемент с верхушки кучи.
     * @throws NoSuchElementException если куча пуста.
     */
    @Override public T remove() {
        if (isEmpty()) {
            throw new NoSuchElementException("Heap is empty");
        }
        return this.poll();
    }

    /**
     * Удаляет конкретный элемент и перестраивает кучу
     * @param o element to be removed from this collection, if present
     */
    @Override public boolean remove(Object o) {
        for (int i = 0; i < size; ++i) {
            if (Objects.equals(heap[i], o)) {
                heap[i] = heap[size - 1];
                heap[size - 1] = null;
                size--;
                heapifyDownFrom(i);
                modificationCount++;
                return true;
            }
        }
        return false;
    }

    @Override
    public boolean containsAll(Collection<?> c) {
        for (Object o : c) {
            if (!contains(o)) return false;
        }
        return true;
    }

    @Override
    public boolean addAll(Collection<? extends T> c) {
        boolean modified = false;
        for (T el : c) {
            if (add(el)) modified = true;
        }
        return modified;
    }

    @Override
    public boolean removeAll(Collection<?> c) {
        boolean modified = false;
        for (Object el : c) {
            while (remove(el)) { // remove возвращает true, если элемент удалён
                modified = true;
            }
        }
        return modified;
    }

    @Override
    public boolean retainAll(Collection<?> c) {
        boolean modified = false;
        Set<?> set = new HashSet<>(c); // ускоряем поиск
        int i = 0;
        while (i < size) {
            if (!set.contains(heap[i])) {
                remove(heap[i]); // удаление перестраивает кучу
                modified = true;
            } else {
                i++;
            }
        }
        return modified;
    }

    public boolean removeAll(T[] a) { return removeAll(Arrays.asList(a)); }
    public boolean retainAll(T[] a) { return retainAll(Arrays.asList(a)); }
    public boolean containsAll(T[] a) { return containsAll(Arrays.asList(a)); }

    /**
     * Метод, спускающий корень вниз для установки свойства максимальной кучи.
     * @see MyPriorityQueue#hasLeftChild(int)
     * @see MyPriorityQueue#hasRightChild(int)
     * @see MyPriorityQueue#getLeftChildIndex(int)
     * @see MyPriorityQueue#getRightChildIndex(int)
     * @see MyPriorityQueue#rightChild(int)
     * @see MyPriorityQueue#leftChild(int)
     */
    private void heapifyDown() {
       int index = 0; // начиная с корня

        // Проверяем, существует ли левый ребёнок (если его нет, то и правого нет).
       while (hasLeftChild(index)) {
           int lowerChildIndex = getLeftChildIndex(index); // Забираем левого ребёнка

           /*
           Проверяем:
           (1) Существует ли правый ребёнок?
           (2) Какой больше: правый или левый?
            */
           if (hasRightChild(index)
                   && compare(rightChild(index), leftChild(index)) < 0) {
               // Если есть правый, и он больше левого, то ставим его!
               lowerChildIndex = getRightChildIndex(index);
           }

           // Именно такой проверкой мы гарантируем, что правило кучи не будет нарушено ни в коем случае.
           // largerChildIndex — реально индекс большего ребёнка.

           /*
           Условие максимальной кучи:
           «Если текущий корень больше или равен своему ребёнку, то стоп»
            */
           if (this.compare(heap[index], heap[lowerChildIndex]) <= 0) { break; }

           // Спускаемся по куче вниз, пока не break'немся.
           swap(index, lowerChildIndex);
           index = lowerChildIndex;
       }
    }

    /**
     * Частный случай метода {@link MyPriorityQueue#heapifyDown()}.
     * @param index индекс рассматриваемого узла
     */
    private void heapifyDownFrom(int index) {
        int smallest = index;
        int left = 2 * index + 1;
        int right = 2 * index + 2;

        if (left < this.size
                && this.compare(heap[left], heap[smallest]) < 0) {
            smallest = left;
        }
        if (right < this.size
                && this.compare(heap[right], heap[smallest]) < 0) {
            smallest = right;
        }

        if (smallest != index) {
            swap(index, smallest);
            heapifyDownFrom(smallest);
        }
    }

    /*
        Помощники для получения индексов детей или родителя
    */
    private int getLeftChildIndex(int parentIndex) { return 2 * parentIndex + 1; }
    private int getRightChildIndex(int parentIndex) { return 2 * parentIndex + 2; }
    private int getParentIndex(int childIndex) { return (childIndex - 1) / 2; }


    /*
        Логические помощники, проверяющие наличия родителя/детей
    */
    private boolean hasLeftChild(int index) { return getLeftChildIndex(index) < size; }
    private boolean hasRightChild(int index) { return getRightChildIndex(index) < size; }
    private boolean hasParent(int index) { return getParentIndex(index) >= 0; }


    /*
        Получение непосредственных значений из кучи по индексу
    */
    private T leftChild(int index) { return heap[getLeftChildIndex(index)]; }
    private T rightChild(int index) { return heap[getRightChildIndex(index)]; }

    /**
     * Увеличивающий вдвое ёмкость кучи метод, который вызывается при добавлении нового элемента.
     * @see MyPriorityQueue#add(T)
     */
    private void extraEnsureCapacity() {
        if (size >= heap.length) {
            int newCapacity = (heap.length < 64) ? (heap.length + 2) : (heap.length + (heap.length >> 1));
            heap = Arrays.copyOf(heap, newCapacity);
        }
    }

    /**
     * Сваппер элементов, который будет полезен на этапах <i>heapify</i>.
     * @param f индекс меняющего
     * @param s индекс меняемого
     */
    private void swap(int f, int s) {
        T temporary = heap[f];
        heap[f] = heap[s];
        heap[s] = temporary;
    }

    /**
     * Переопределённый метод очистки всех элементов в минимальной куче.
     */
    @Override public void clear() {
        Arrays.fill(this.heap, 0, this.size, null);
        this.size = 0;
        modificationCount++;
    }

    /**
     * Метод, проверяющий, содержится объект в коллекции, или нет.
     * @param o элемент, который сначала проверится на принадлежность, а потом на соответствие с конкретным элементов
     * @return {@code true} если содержится, и {@code false} в противном случае
     */
    @Override public boolean contains(Object o) {
        for (int i = 0; i < size; ++i) {
            if (Objects.equals(heap[i], o)) { return true; }
        }
        return false;
    }

    public void merge(MyPriorityQueue<T> other) {
        int newSize = this.size + other.size;
        T[] newHeap = Arrays.copyOf(this.heap, Math.max(newSize, heap.length));
        System.arraycopy(other.heap, 0, newHeap, this.size, other.size);
        this.heap = newHeap;
        this.size = newSize;
        heapifyAll();
        modificationCount++;
    }


    // =====================================================================================

    /**
     * Класс, реализуюющий итератор, который будет использовать очередь с приоритетом.
     */
    private class PriorityIterator implements Iterator<T> {
        private int cursor = 0;
        private final int expectedModCount = modificationCount;

        @Override
        public boolean hasNext() {
            checkForComodification();
            return !isEmpty() && (cursor < size);
        }

        @Override
        public T next() {
            checkForComodification();
            if (!hasNext()) { throw new NoSuchElementException("Heap is empty!"); }
            return heap[cursor++];
        }

        private void checkForComodification() {
            if (modificationCount != expectedModCount) {
                throw new ConcurrentModificationException();
            }
        }
    }

    @Override
    public Iterator<T> iterator() { return new PriorityIterator(); }


    /**
     * @return текущая размерность кучи
     */
    @Override public int size() { return this.size; }

    /**
     * Проверка на пустоту
     * @return {@code true} если пусто, {@code false} в противном случае.
     */
    @Override public boolean isEmpty() { return this.size == 0; }

    /**
     * Копирование в массив.
     * @return массив типа кучи.
     */
    @Override public Object[] toArray() { return Arrays.copyOf(heap, size); }

    @Override
    @SuppressWarnings("unchecked")
    public <E> E[] toArray(E[] a) {
        if (a == null || a.length < size) {
            return (E[]) Arrays.copyOf(heap, size, a == null ? Object[].class : a.getClass());
        }
        System.arraycopy(heap, 0, a, 0, size);
        if (a.length > size) a[size] = null;
        return a;
    }

    @SuppressWarnings("unchecked")
    public int compare(T o1, T o2) {
        if (comparator != null) return comparator.compare(o1, o2);
        return ((Comparable<? super T>) o1).compareTo(o2);
    }
}