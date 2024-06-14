namespace StockIndicators;

/// <summary>
/// Implementations of this interface provide inline series for price charts.
/// </summary>
public interface IChartValueSeriesProvider
{
    /// <summary>
    /// Creates the value series for the price chart.
    /// </summary>
    /// <returns>A collection of <see cref="ChartValueSeries"/>.</returns>
    IEnumerable<ChartValueSeries> CreateChartValueSeries();
}
