﻿using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="ChandelierShortExit"/> indicator.
/// </summary>
public sealed class ChandelierShortExitSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 200)]
    [DefaultValue(22)]
    public int Periods { get; init; } = 22;

    /// <summary>
    /// Gets or sets the ATR multiplication factor.
    /// </summary>
    [DisplayName("Factor")]
    [Description("The ATR multiplication factor.")]
    [Range(0.5, 10.0)]
    [DefaultValue(3.0)]
    public double Factor { get; init; } = 3.0;
}

/// <summary>
/// The Chandelier Exit sets a trailing stop-loss based on the Average True Range (ATR). The indicator is
/// designed to keep traders in a trend and prevent an early exit as long as the trend extends.
/// Typically, the Chandelier Exit will be above prices during a downtrend and below prices during an uptrend. 
/// </summary>
[DisplayName("Chandelier Short Exit")]
[Category(IndicatorCategory.Signal)]
public sealed class ChandelierShortExit : IPriceIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly double factor;
    private readonly AnalysisWindow prices;
    private readonly AverageTrueRange atr;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChandelierShortExit"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public ChandelierShortExit(IndicatorCapacity capacity)
        : this(capacity, new ChandelierShortExitSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChandelierShortExit"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public ChandelierShortExit(IndicatorCapacity capacity, ChandelierShortExitSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        factor = settings.Factor;

        prices = new AnalysisWindow(periods, false, true);
        atr = new AverageTrueRange(IndicatorCapacity.Minimum, new AverageTrueRangeSettings { Periods = periods });

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
        prices.Add(price.Low);
        atr.Add(price);

        if (atr.IsReady)
            Values.Add(prices.Min + atr.Values[atr.Values.Count - 1] * factor);
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        var title = $"Chandelier Short Exit ({periods}, {factor:N1})";

        return
        [
            new ChartValueSeries(title, Values, ChartValueSeriesStyle.Dot, ChartColor.NegativeValue)
        ];
    }
}
