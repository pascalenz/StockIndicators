using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="CommodityChannelIndex"/> indicator.
/// </summary>
public sealed class CommodityChannelIndexSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 100)]
    [DefaultValue(20)]
    public int Periods { get; init; } = 20;
}

/// <summary>
/// The Commodity Channel Index (CCI) is a versatile indicator that can be used to identify a new trend or warn of
/// extreme conditions. In general, CCI measures the current price level relative to an average price level over a
/// given period of time. CCI is relatively high when prices are far above their average. CCI is relatively low when
/// prices are far below their average. In this manner, CCI can be used to identify overbought and oversold levels.
/// (StockCharts.com)
/// </summary>
[DisplayName("CCI")]
[Description("Commodity Channel Index")]
[Category(IndicatorCategory.Momentum)]
public sealed class CommodityChannelIndex : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow sma;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommodityChannelIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public CommodityChannelIndex(IndicatorCapacity capacity)
        : this(capacity, new CommodityChannelIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommodityChannelIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public CommodityChannelIndex(IndicatorCapacity capacity, CommodityChannelIndexSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        sma = new AnalysisWindow(periods, true, false);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => sma.IsFilled;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        var typical = price.Typical();
        sma.Add(typical);

        if (IsReady)
        {
            var meanDeviation = sma.Select(v => Math.Abs(sma.Average - v)).Sum() / sma.Count;
            var cci = (typical - sma.Average) / (0.015d * meanDeviation);
            Values.Add(cci);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"CCI ({periods})",
            ValueFormat = "N0",
            GridLines =
            [
                new ChartGridLine(-100),
                new ChartGridLine(0),
                new ChartGridLine(100)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}
