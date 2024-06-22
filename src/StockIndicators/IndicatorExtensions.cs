namespace StockIndicators;

/// <summary>
/// Provides useful extension methods for the indicator interfaces.
/// </summary>
public static class IndicatorExtensions
{
    /// <summary>
    /// Add multiple values to the indicator.
    /// </summary>
    /// <param name="indicator">The indicator to add the values to.</param>
    /// <param name="values">The values to add to the indicator.</param>
    public static void Add(this IAverageIndicator indicator, IEnumerable<double> values)
    {
        foreach (var value in values)
        {
            indicator.Add(value);
        }
    }

    /// <summary>
    /// Add multiple prices to the indicator.
    /// </summary>
    /// <param name="indicator">The indicator to add the prices to.</param>
    /// <param name="prices">The prices to add to the indicator.</param>
    public static void Add(this IPriceIndicator indicator, IEnumerable<IPrice> prices)
    {
        foreach (var price in prices)
        {
            indicator.Add(price);
        }
    }

    /// <summary>
    /// Add multiple prices to the indicator.
    /// </summary>
    /// <param name="indicator">The indicator to add the prices to.</param>
    /// <param name="prices">The prices to add to the indicator.</param>
    public static void Add(this IPriceComparisonIndicator indicator, IEnumerable<(IPrice, IPrice)> prices)
    {
        foreach (var price in prices)
        {
            indicator.Add(price.Item1, price.Item2);
        }
    }
}
