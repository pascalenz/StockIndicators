using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.Indicators;

/// <summary>
/// The Accumulation Distribution Line (ADL) is a volume-based indicator designed to measure the cumulative flow of
/// money into and out of a security. Chartists can use this indicator to affirm a security's underlying trend or
/// anticipate reversals when the indicator diverges from the security price. (StockCharts.com)
/// </summary>
[DisplayName("ADL")]
[Description("Accumulation Distribution Line")]
[Category(IndicatorCategory.Momentum)]
public sealed class AccumulationDistributionLine : IPriceIndicator, IChartProvider
{
    private double last = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="AccumulationDistributionLine"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public AccumulationDistributionLine(IndicatorCapacity capacity)
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
        last += price.MoneyFlowVolume();
        Values.Add(last);
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart()
        {
            Title = "ADL",
            ValueFormat = "N0",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}
