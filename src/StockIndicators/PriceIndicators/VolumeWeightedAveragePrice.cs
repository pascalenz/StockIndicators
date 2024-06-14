using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Volume-Weighted Average Price (VWAP) is exactly what it sounds like: the average price weighted by volume.
/// VWAP equals the dollar value of all trading periods divided by the total trading volume for the current day.
/// Calculation starts when trading opens and ends when trading closes.
/// Because it is good for the current trading day only, intraday periods and data are used in the calculation.
/// (StockCharts.com)
/// </summary>
[DisplayName("VWAP")]
[Description("Volume-Weighted Average Price")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class VolumeWeightedAveragePrice : IPriceIndicator, IChartValueSeriesProvider
{
    private DateTimeOffset lastDate;
    private double cumulativePrice = 0;
    private double cumulativeVolume = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="VolumeWeightedAveragePrice"/> class.
    /// </summary>
    public VolumeWeightedAveragePrice()
    {
        Values = new List<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        if (price.Timestamp.Date != lastDate)
        {
            ((List<double>)Values).Clear();
            lastDate = price.Timestamp.Date;
            cumulativePrice = 0;
            cumulativeVolume = 0;
        }

        cumulativeVolume += price.Volume;
        cumulativePrice += price.Typical() * price.Volume;
        Values.Add(cumulativePrice / cumulativeVolume);
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        return
        [
            new ChartValueSeries("VWAP", Values, ChartValueSeriesStyle.Line, ChartColor.Red)
        ];
    }
}