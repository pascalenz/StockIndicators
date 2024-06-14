using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="MACD"/> indicator.
/// </summary>
public sealed class MACDSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the fast moving average.
    /// </summary>
    [DisplayName("Fast Periods")]
    [Description("The number of look-back periods for the fast moving average.")]
    [Range(1, 100)]
    [DefaultValue(12)]
    public int FastPeriods { get; init; } = 12;

    /// <summary>
    /// Gets or sets the number of look-back periods for the slow moving average.
    /// </summary>
    [DisplayName("Slow Periods")]
    [Description("The number of look-back periods for the slow moving average.")]
    [Range(1, 100)]
    [DefaultValue(26)]
    public int SlowPeriods { get; init; } = 26;

    /// <summary>
    /// Gets or sets the number of look-back periods for the signal period.
    /// </summary>
    [DisplayName("Signal Periods")]
    [Description("The number of look-back periods for the signal period.")]
    [Range(1, 100)]
    [DefaultValue(9)]
    public int SignalPeriods { get; init; } = 9;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Exponential)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Exponential;
}

/// <summary>
/// The Moving Average Convergence/Divergence oscillator (MACD) is one of the simplest and most effective momentum indicators available.
/// The MACD turns two trend-following indicators, moving averages, into a momentum oscillator by subtracting the longer moving average
/// from the shorter moving average. As a result, the MACD offers the best of both worlds: trend following and momentum.
/// The MACD fluctuates above and below the zero line as the moving averages converge, cross and diverge. Traders can look for signal
/// line crossovers, centerline crossovers and divergences to generate signals. Because the MACD is unbounded, it is not particularly
/// useful for identifying overbought and oversold levels.
/// The MACD-Histogram measures the distance between MACD and its signal line. Like MACD, the MACD-Histogram is also an oscillator that
/// fluctuates above and below the zero line. Aspray developed the MACD-Histogram to anticipate signal line crossovers in MACD.
/// Because MACD uses moving averages and moving averages lag price, signal line crossovers can come late and affect the reward-to-risk
/// ratio of a trade. Bullish or bearish divergences in the MACD-Histogram can alert chartists to an imminent signal line crossover in MACD.
/// (StockCharts.com)
/// </summary>
[DisplayName("MACD")]
[Description("Moving Average Convergence/Divergence")]
[Category(IndicatorCategory.Momentum)]
public sealed class MACD : IPriceIndicator, IChartProvider
{
    private readonly int fastPeriods;
    private readonly int slowPeriods;
    private readonly int signalPeriods;
    private readonly IAverageIndicator fastMA;
    private readonly IAverageIndicator slowMA;
    private readonly IAverageIndicator signalMA;

    /// <summary>
    /// Initializes a new instance of the <see cref="MACD"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public MACD(IndicatorCapacity capacity)
        : this(capacity, new MACDSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MACD"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public MACD(IndicatorCapacity capacity, MACDSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        fastPeriods = settings.FastPeriods;
        slowPeriods = settings.SlowPeriods;
        signalPeriods = settings.SignalPeriods;

        fastMA = MovingAverageFactory.Create(settings.MovingAverageType, fastPeriods);
        slowMA = MovingAverageFactory.Create(settings.MovingAverageType, slowPeriods);
        signalMA = MovingAverageFactory.Create(settings.MovingAverageType, signalPeriods);

        Values = capacity.CreateList<double>();
        Signal = capacity.CreateList<double>();
        Histogram = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <summary>
    /// Gets the values for the signal line.
    /// </summary>
    public IReadOnlyList<double> Signal { get; }

    /// <summary>
    /// Gets the values for the historgram.
    /// </summary>
    public IReadOnlyList<double> Histogram { get; }

    /// <inheritdoc/>
    public bool IsReady => fastMA.IsReady && slowMA.IsReady && signalMA.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        fastMA.Add(price.Close);
        slowMA.Add(price.Close);

        if (fastMA.Last.HasValue && slowMA.Last.HasValue)
        {
            var macd = fastMA.Last.Value - slowMA.Last.Value;

            signalMA.Add(macd);
            if (signalMA.Last.HasValue)
            {
                var signal = signalMA.Last.Value;

                if (IsReady)
                {
                    Values.Add(macd);
                    Histogram.Add(macd - signal);
                    Signal.Add(signal);
                }
            }
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "MACD",
            ValueFormat = "N2",
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(0)
            ],
            ValueSeries =
            [
                new ChartValueSeries($"MACD ({fastPeriods}, {slowPeriods})", Values, ChartValueSeriesStyle.Line, ChartColor.Black),
                new ChartValueSeries($"Signal ({signalPeriods})", Signal, ChartValueSeriesStyle.Line, ChartColor.Red),
                new ChartValueSeries("Histogram", Histogram, ChartValueSeriesStyle.Bar, ChartColor.StrongGray)
            ]
        };
    }
}