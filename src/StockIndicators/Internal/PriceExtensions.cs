namespace StockIndicators.Internal;

/// <summary>
/// Provides commonly used utility methods for share prices.
/// </summary>
internal static class PriceExtensions
{
    /// <summary>
    /// Calculates the typical price.
    /// </summary>
    /// <param name="price">The share price.</param>
    /// <returns>The typical price value.</returns>
    public static double Typical(this IPrice price)
    {
        return (price.High + price.Low + price.Close) / 3;
    }

    /// <summary>
    /// Calculates the true range for the share price.
    /// </summary>
    /// <param name="price">The current share price.</param>
    /// <param name="previous">The previous share price.</param>
    /// <returns>The true range value.</returns>
    public static double TrueRange(this IPrice price, IPrice previous)
    {
        double highMinusPrevClose = Math.Abs(price.High - previous.Close);
        double lowMinusPrevClose = Math.Abs(price.Low - previous.Close);

        double trueRangeValue = price.High - price.Low;

        if (highMinusPrevClose > trueRangeValue)
            trueRangeValue = highMinusPrevClose;

        if (lowMinusPrevClose > trueRangeValue)
            trueRangeValue = lowMinusPrevClose;

        return trueRangeValue;
    }

    /// <summary>
    /// Calculates the money flow multiplier for the share price.
    /// </summary>
    /// <param name="price">The share price.</param>
    /// <returns>The money flow multiplier value.</returns>
    public static double MoneyFlowMultiplier(this IPrice price)
    {
        return (((price.Close - price.Low) - (price.High - price.Close)) / (price.High - price.Low));
    }

    /// <summary>
    /// Calculates the money flow volume for the share price.
    /// </summary>
    /// <param name="price">The share price.</param>
    /// <returns>The money flow volume value.</returns>
    public static double MoneyFlowVolume(this IPrice price)
    {
        return price.MoneyFlowMultiplier() * price.Volume;
    }
}
