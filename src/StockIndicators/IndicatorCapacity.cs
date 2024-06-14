using StockIndicators.Internal;

namespace StockIndicators;

/// <summary>
/// Defines the capacity of an indicator.
/// </summary>
public sealed class IndicatorCapacity : IEquatable<IndicatorCapacity>
{
    /// <summary>
    /// The minimum number of periods required,
    /// </summary>
    private const int MinimumPeriods = 1;

    /// <summary>
    /// Initializes static members of the <see cref="IndicatorCapacity"/> class.
    /// </summary>
    static IndicatorCapacity()
    {
        Infinite = new IndicatorCapacity(-1);
        Minimum = new IndicatorCapacity(MinimumPeriods);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndicatorCapacity"/> class.
    /// </summary>
    /// <param name="periods">The internal value.</param>
    private IndicatorCapacity(int periods)
    {
        Periods = periods;
    }

    /// <summary>
    /// Gets the indicator capacity value to keep infinite amounts of data.
    /// </summary>
    public static IndicatorCapacity Infinite { get; }

    /// <summary>
    /// Gets the indicator capacity value to keep only a single value.
    /// </summary>
    public static IndicatorCapacity Minimum { get; }

    /// <summary>
    /// The number of values to keep in the indicator.
    /// </summary>
    public int Periods { get; }

    /// <summary>
    /// Creates a new indicator capacity value from a number of periods.
    /// </summary>
    /// <param name="periods">The number of periods to keep.</param>
    /// <returns>The indicator capacity.</returns>
    public static IndicatorCapacity FromPeriods(int periods)
    {
        return periods < MinimumPeriods
            ? throw new ArgumentException("The periods for the indicator capacity cannot be less than 1.")
            : new IndicatorCapacity(periods);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => Periods.GetHashCode();

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as IndicatorCapacity);

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(IndicatorCapacity? other) => Periods.Equals(other?.Periods);

    /// <summary>
    /// Creates a new instance of a readonly list from the supplied capacity.
    /// </summary>
    /// <typeparam name="T">The type of the list to create.</typeparam>
    /// <returns>The readonly list instance with the correct capacity.</returns>
    internal IReadOnlyList<T> CreateList<T>() => this == Infinite ? new List<T>() : new FixedSizeReadOnlyList<T>(Periods);
}
