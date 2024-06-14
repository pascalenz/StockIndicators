namespace StockIndicators;

/// <summary>
/// Implementations of this interface provide avarage analysis indicators.
/// </summary>
public interface IAverageIndicator
{
    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    IReadOnlyList<double> Values { get; }

    /// <summary>
    /// Gets the last indicator value.
    /// </summary>
    double? Last { get; }

    /// <summary>
    /// Gets a value indicating whether the indicator is ready to be used.
    /// </summary>
    bool IsReady { get; }

    /// <summary>
    /// Adds a new value to the indicator.
    /// </summary>
    /// <param name="value">The new value to add to the indicator.</param>
    void Add(double value);
}
