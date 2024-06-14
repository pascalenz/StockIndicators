using System.Collections;

namespace StockIndicators.Internal;

/// <summary>
/// A fixed-size read-only list used to maintain a fixed number of items.
/// </summary>
/// <typeparam name="T">The item type of the list.</typeparam>
public class FixedSizeReadOnlyList<T> : IReadOnlyList<T>
{
    /// <summary>
    /// The inner list.
    /// </summary>
    protected internal readonly T[] list;

    /// <summary>
    /// The current index in the list.
    /// </summary>
    protected internal int index;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedSizeReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="size">The maximum number of items maintained by the list.</param>
    public FixedSizeReadOnlyList(int size)
    {
        if (size <= 0)
            throw new ArgumentException("Argument 'size' cannot be zero or less.");

        Size = size;
        index = -1;
        list = new T[size];
    }

    /// <summary>
    /// Gets a value indicating whether the list contains the maximum number of items.
    /// </summary>
    public bool IsFilled { get; private set; }

    /// <summary>
    /// Gets the maximum number of items in the list.
    /// </summary>
    public int Size { get; }

    /// <summary>
    /// Gets the actual number of items in the list.
    /// </summary>
    public int Count => IsFilled ? Size : index + 1;

    /// <summary>
    /// Gets the item at the specified index.
    /// </summary>
    /// <param name="index">The position index.</param>
    /// <returns>The item at the specified index.</returns>
    public T this[int index]
    {
        get
        {
            if ((index < 0) || (index >= Count))
            {
                throw new IndexOutOfRangeException("Argument 'index' must be zero or more, but less than 'Count'.");
            }

            if (!IsFilled)
            {
                return list[index];
            }

            var off = Size - 1 - index;
            var relPos = this.index - off;
            var pos = (relPos + Size) % Size;
            return list[pos];
        }
    }

    /// <summary>
    /// Adds the supplied item to the list.
    /// </summary>
    /// <param name="value">The item to add.</param>
    public virtual void Add(T value)
    {
        index = (index + 1) % Size;
        list[index] = value;

        if (!IsFilled && (index == Size - 1))
        {
            IsFilled = true;
        }
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the list.</returns>
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => new FixedSizeReadOnlyListEnumerator(this);

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the list.</returns>
    IEnumerator IEnumerable.GetEnumerator() => new FixedSizeReadOnlyListEnumerator(this);

    private class FixedSizeReadOnlyListEnumerator(FixedSizeReadOnlyList<T> list) : IEnumerator, IEnumerator<T>
    {
        private readonly FixedSizeReadOnlyList<T> list = list;
        private int index = -1;

        public object Current => list[index]!;

        public bool MoveNext()
        {
            index++;
            return index < list.Count;
        }

        public void Reset() => index = -1;

        T IEnumerator<T>.Current => list[index];

        public void Dispose()
        { }
    }
}