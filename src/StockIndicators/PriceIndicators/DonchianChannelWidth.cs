using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Donchian Channel Width is an indicator that displays the width of Donchian Channels.
/// Donchian Channels plot the highest high and lowest low over the last period time intervals.
/// (barchart.com)
/// </summary>
[DisplayName("Donchian Channel Width")]
[Category(IndicatorCategory.Trend)]
public sealed class DonchianChannelWidth : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly DonchianChannel channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianChannelWidth"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public DonchianChannelWidth(IndicatorCapacity capacity)
        : this(capacity, new DonchianChannelSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianChannelWidth"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public DonchianChannelWidth(IndicatorCapacity capacity, DonchianChannelSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        channel = new DonchianChannel(IndicatorCapacity.Minimum, settings);

        Values = capacity.CreateList<double>();
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
        channel.Add(price);

        if (channel.IsReady)
        {
            var upper = channel.UpperLine[channel.UpperLine.Count - 1];
            var lower = channel.LowerLine[channel.LowerLine.Count - 1];
            Values.Add(upper - lower);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Donchian Width ({periods})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}