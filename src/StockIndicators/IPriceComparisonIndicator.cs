namespace StockIndicators;

/// <summary>
/// Implementations of this interface provide price comparison analysis indicators.
/// </summary>
public interface IPriceComparisonIndicator
{
    /// <summary>
    /// Gets a value indicating whether the indicator is ready to be used.
    /// </summary>
    bool IsReady { get; }

    /// <summary>
    /// Adds a new price to the indicator.
    /// </summary>
    /// <param name="price1">The first new price to add to the indicator.</param>
    /// <param name="price2">The second new price to add to the indicator.</param>
    void Add(IPrice price1, IPrice price2);
}
