using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.Indicators;

/// <summary>
/// On Balance Volume (OBV) measures buying and selling pressure as a cumulative indicator that adds volume
/// on up days and subtracts volume on down days. Chartists can look for divergences between OBV and price
/// to predict price movements or use OBV to confirm price trends. (StockCharts.com)
/// </summary>
[DisplayName("OBV")]
[Description("On Balance Volume")]
[Category(IndicatorCategory.Momentum)]
public sealed class OnBalanceVolume : IPriceIndicator, IChartProvider
{
    private IPrice? last;

    /// <summary>
    /// Initializes a new instance of the <see cref="OnBalanceVolume"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public OnBalanceVolume(IndicatorCapacity capacity)
    {
        IndicatorValidator.Verify(capacity);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => true;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        if (last == null)
        {
            last = price;
            return;
        }

        var value = Values.Count > 0 ? Values[Values.Count - 1] : 0;

        if (price.Close > last.Close)
            value += price.Volume;

        if (price.Close < last.Close)
            value -= price.Volume;

        Values.Add(value);
        last = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "OBV",
            ValueFormat = "N0",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}