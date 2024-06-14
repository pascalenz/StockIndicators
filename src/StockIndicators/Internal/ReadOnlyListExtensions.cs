namespace StockIndicators.Internal;

/// <summary>
/// Provides commonly used utility methods for <see cref="IReadOnlyList{T}"/>.
/// </summary>
internal static class ReadOnlyListExtensions
{
    /// <summary>
    /// Adds the supplied value to the list.
    /// </summary>
    /// <typeparam name="T">The type of the list items.</typeparam>
    /// <param name="source">The list instance.</param>
    /// <param name="value">The value to add.</param>
    internal static void Add<T>(this IReadOnlyList<T> source, T value)
    {
        if (source is FixedSizeReadOnlyList<T> fixedSizeList)
        {
            fixedSizeList.Add(value);
            return;
        }

        if (source is List<T> list)
        {
            list.Add(value);
            return;
        }

        throw new InvalidOperationException("Cannot add item to this type of list.");
    }
}
