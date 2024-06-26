﻿using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="NegativeVolumeIndex"/> indicator.
/// </summary>
public sealed class NegativeVolumeIndexSettings
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
[DisplayName("NVI")]
[Description("Negative Volume Index")]
[Category(IndicatorCategory.Volume)]
public sealed class NegativeVolumeIndex : IPriceIndicator, IChartProvider
{
    private readonly int signalPeriods;
    private readonly IAverageIndicator signalMA;
    private IPrice? previous;
    private double last = 1000;

    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeVolumeIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public NegativeVolumeIndex(IndicatorCapacity capacity)
        : this(capacity, new NegativeVolumeIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NegativeVolumeIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public NegativeVolumeIndex(IndicatorCapacity capacity, NegativeVolumeIndexSettings settings)
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

        if (price.Volume < previous.Volume)
        {
            last += (price.Close - previous.Close) / previous.Close * last;
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
            Title = "NVI",
            ValueFormat = "N0",
            ValueSeries =
            [
                new ChartValueSeries("NVI", Values, ChartValueSeriesStyle.Line, ChartColor.Black),
                new ChartValueSeries($"Signal ({signalPeriods})", Signal, ChartValueSeriesStyle.Line, ChartColor.Red),
            ]
        };
    }
}