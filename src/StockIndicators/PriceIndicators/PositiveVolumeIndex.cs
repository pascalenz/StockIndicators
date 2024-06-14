using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="PositiveVolumeIndex"/> indicator.
/// </summary>
public sealed class PositiveVolumeIndexSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the signal period.
    /// </summary>
    [DisplayName("Signal Periods")]
    [Description("The number of look-back periods for the signal period.")]
    [Range(1, 1000)]
    [DefaultValue(255)]
    public int SignalPeriods { get; init; } = 255;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Exponential)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Exponential;
}

/// <summary>
/// The Negative Volume Index (NVI) is a cumulative indicator that uses the change in volume to decide when the smart money is active.
/// The Index works under the assumption that the smart money is active on days when volume decreases and the not-so-smart money is
/// active on days when volume increases. The traditional use of the NVI is quite simple. The odds favor a bull market when NVI is above 
/// its 255-day EMA and the odds favor a bear market when NVI is below. However, these odds are not symmetrical. There is a 96% chance
/// of a bull market when NVI is above its 255-day EMA, but only a 53% chance of a bear market when NVI is below its 255-day EMA.
/// (StockCharts.com)
/// </summary>
[DisplayName("PVI")]
[Description("Positive Volume Index")]
[Category(IndicatorCategory.Volume)]
public sealed class PositiveVolumeIndex : IPriceIndicator, IChartProvider
{
    private readonly int signalPeriods;
    private readonly IAverageIndicator signalMA;
    private IPrice? previous;
    private double last = 1000;

    /// <summary>
    /// Initializes a new instance of the <see cref="PositiveVolumeIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public PositiveVolumeIndex(IndicatorCapacity capacity)
        : this(capacity, new PositiveVolumeIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PositiveVolumeIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public PositiveVolumeIndex(IndicatorCapacity capacity, PositiveVolumeIndexSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        signalPeriods = settings.SignalPeriods;
        signalMA = MovingAverageFactory.Create(settings.MovingAverageType, signalPeriods);

        Values = capacity.CreateList<double>();
        Signal = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <summary>
    /// Gets the values for the signal line.
    /// </summary>
    public IReadOnlyList<double> Signal { get; }

    /// <inheritdoc/>
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        if (previous == null)
        {
            previous = price;
            return;
        }

        if (price.Volume > previous.Volume)
        {
            last += ((price.Close - previous.Close) / previous.Close) * last;
        }

        signalMA.Add(last);
        if (signalMA.IsReady)
        {
            Values.Add(last);
            Signal.Add(signalMA.Last!.Value);
        }

        previous = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "PVI",
            ValueFormat = "N0",
            ValueSeries =
            [
                new ChartValueSeries("PVI", Values, ChartValueSeriesStyle.Line, ChartColor.Black),
                new ChartValueSeries($"Signal ({signalPeriods})", Signal, ChartValueSeriesStyle.Line, ChartColor.Red),
            ]
        };
    }
}