namespace StockIndicators;

/// <summary>
/// Represents a single OHLC price.
/// </summary>
public interface IPrice
{
    /// <summary>
    /// Gets the date and time of the close.
    /// </summary>
    DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Gets the opening price value.
    /// </summary>
    double Open { get; }

    /// <summary>
    /// Gets the highest price value.
    /// </summary>
    double High { get; }

    /// <summary>
    /// Gets the lowest price value.
    /// </summary>
    double Low { get; }

    /// <summary>
    /// Gets the closing price value.
    /// </summary>
    double Close { get; }

    /// <summary>
    /// Gets the volume.
    /// </summary>
    long Volume { get; }
}
