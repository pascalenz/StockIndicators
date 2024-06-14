﻿using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.AverageIndicators;

/// <summary>
/// Settings for the <see cref="DoubleExponentialMovingAverage"/> indicator.
/// </summary>
public sealed class DoubleExponentialMovingAverageSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 1000)]
    [DefaultValue(50)]
    public int Periods { get; init; } = 50;
}

/// <summary>
/// Moving averages smooth the price data to form a trend following indicator. They do not predict price direction,
/// but rather define the current direction with a lag. Moving averages lag because they are based on past prices.
/// Despite this lag, moving averages help smooth price action and filter out the noise. (StockCharts.com)
/// </summary>
[DisplayName("DEMA")]
[Description("Double Exponential Moving Average")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class DoubleExponentialMovingAverage : IAverageIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly ExponentialMovingAverage singleEMA;
    private readonly ExponentialMovingAverage doubleEMA;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleExponentialMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public DoubleExponentialMovingAverage(IndicatorCapacity capacity)
        : this(capacity, new DoubleExponentialMovingAverageSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleExponentialMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public DoubleExponentialMovingAverage(IndicatorCapacity capacity, DoubleExponentialMovingAverageSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;

        singleEMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = periods });
        doubleEMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = periods });

        Values = capacity.CreateList<double>();
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public double? Last { get; private set; }

    /// <inheritdoc/>
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(double value)
    {
        singleEMA.Add(value);
        doubleEMA.Add(singleEMA.Last!.Value);
        Last = 2 * singleEMA.Last.Value - doubleEMA.Last!.Value;

        if (doubleEMA.IsReady)
            Values.Add(Last.Value);
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries() =>
    [
        new ChartValueSeries($"DEMA ({periods})", Values, ChartValueSeriesStyle.Line)
    ];
}