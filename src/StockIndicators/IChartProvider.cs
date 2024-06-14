namespace StockIndicators;

/// <summary>
/// Implementations of this interface provide standalone indicator charts.
/// </summary>
public interface IChartProvider
{
    /// <summary>
    /// Creates the indicator chart.
    /// </summary>
    /// <returns>An instance of <see cref="Chart"/>.</returns>
    Chart CreateChart();
}
