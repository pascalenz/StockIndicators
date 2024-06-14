namespace StockIndicators;

/// <summary>
/// Implementations of this interface provide price analysis indicators.
/// </summary>
public interface IPriceIndicator
{
    /// <summary>
    /// Gets a value indicating whether the indicator is ready to be used.
    /// </summary>
    bool IsReady { get; }

    /// <summary>
    /// Adds a new price to the indicator.
    /// </summary>
    /// <param name="price">The new price to add to the indicator.</param>
    void Add(IPrice price);
}
