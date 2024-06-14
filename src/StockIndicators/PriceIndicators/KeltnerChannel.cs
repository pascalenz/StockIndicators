using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="KeltnerChannel"/> indicator.
/// </summary>
public sealed class KeltnerChannelSettings
{
    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Exponential)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Exponential;

    /// <summary>
    /// Gets or sets the number of look-back periods for the moving average.
    /// </summary>
    [DisplayName("MA Periods")]
    [Description("The number of look-back periods for the moving average.")]
    [Range(1, 200)]
    [DefaultValue(20)]
    public int MovingAveragePeriods { get; init; } = 20;

    /// <summary>
    /// Gets or sets the number of look-back periods for the ATR.
    /// </summary>
    [DisplayName("ATR Periods")]
    [Description("The number of look-back periods for the ATR.")]
    [Range(1, 100)]
    [DefaultValue(10)]
    public int AverageTrueRangePeriods { get; init; } = 10;

    /// <summary>
    /// Gets or sets the ATR multiplication factor.
    /// </summary>
    [DisplayName("Factor")]
    [Description("The ATR multiplication factor.")]
    [Range(1.0, 10.0)]
    [DefaultValue(3.0)]
    public double Factor { get; init; } = 3.0;
}

/// <summary>
/// Keltner Channels are volatility-based envelopes set above and below an exponential moving average.
/// This indicator is similar to Bollinger Bands, which use the standard deviation to set the bands, but instead
/// of using the standard deviation, Keltner Channels use the Average True Range (ATR) to set channel distance.
/// Keltner Channels are a trend following indicator used to identify reversals with channel breakouts and channel direction.
/// Channels can also be used to identify overbought and oversold levels when the trend is flat. (StockCharts.com)
/// </summary>
[DisplayName("Keltner Channels")]
[Category(IndicatorCategory.Momentum)]
public sealed class KeltnerChannel : IPriceIndicator, IChartValueSeriesProvider
{
    private readonly int movingAveragePeriods;
    private readonly int averageTrueRangePeriods;
    private readonly double factor;
    private readonly AverageTrueRange atr;
    private readonly IAverageIndicator ma;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeltnerChannel"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public KeltnerChannel(IndicatorCapacity capacity)
        : this(capacity, new KeltnerChannelSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeltnerChannel"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public KeltnerChannel(IndicatorCapacity capacity, KeltnerChannelSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        movingAveragePeriods = settings.MovingAveragePeriods;
        averageTrueRangePeriods = settings.AverageTrueRangePeriods;
        factor = settings.Factor;

        ma = MovingAverageFactory.Create(settings.MovingAverageType, settings.MovingAveragePeriods);
        atr = new AverageTrueRange(IndicatorCapacity.Minimum, new AverageTrueRangeSettings { Periods = settings.AverageTrueRangePeriods });

        UpperBand = capacity.CreateList<double>();
        MiddleBand = capacity.CreateList<double>();
        LowerBand = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the upper indicator band.
    /// </summary>
    public IReadOnlyList<double> UpperBand { get; }

    /// <summary>
    /// Gets the values for the middle indicator band.
    /// </summary>
    public IReadOnlyList<double> MiddleBand { get; }

    /// <summary>
    /// Gets the values for the lower indicator band.
    /// </summary>
    public IReadOnlyList<double> LowerBand { get; }

    /// <inheritdoc/>
    public bool IsReady => atr.IsReady && ma.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        atr.Add(price);
        ma.Add(price.Close);

        if (IsReady)
        {
            MiddleBand.Add(ma.Last!.Value);
            UpperBand.Add(ma.Last.Value + (factor * atr.Values[atr.Values.Count - 1]));
            LowerBand.Add(ma.Last.Value - (factor * atr.Values[atr.Values.Count - 1]));
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        var title = $"Keltner Chanels ({movingAveragePeriods}, {averageTrueRangePeriods}, {factor:N1})";

        return
        [
            new ChartValueSeries(null, UpperBand, ChartValueSeriesStyle.Line),
            new ChartValueSeries(title, MiddleBand, ChartValueSeriesStyle.Line),
            new ChartValueSeries(null, LowerBand, ChartValueSeriesStyle.Line)
        ];
    }
}