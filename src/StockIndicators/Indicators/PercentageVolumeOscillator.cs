using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="PercentageVolumeOscillator"/> indicator.
/// </summary>
public sealed class PercentageVolumeOscillatorSettings
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
/// The Percentage Volume Oscillator (PVO) is a momentum oscillator for volume. PVO measures the difference
/// between two volume-based moving averages as a percentage of the larger moving average. (StockCharts.com)
/// </summary>
[DisplayName("PVO")]
[Description("Percentage Volume Oscillator")]
[Category(IndicatorCategory.Volume)]
public sealed class PercentageVolumeOscillator : IPriceIndicator, IChartProvider
{
    private readonly int fastPeriods;
    private readonly int slowPeriods;
    private readonly int signalPeriods;
    private readonly IAverageIndicator fastMA;
    private readonly IAverageIndicator slowMA;
    private readonly IAverageIndicator signalMA;

    /// <summary>
    /// Initializes a new instance of the <see cref="PercentageVolumeOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public PercentageVolumeOscillator(IndicatorCapacity capacity)
        : this(capacity, new PercentageVolumeOscillatorSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PercentageVolumeOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public PercentageVolumeOscillator(IndicatorCapacity capacity, PercentageVolumeOscillatorSettings settings)
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
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        fastMA.Add(price.Volume);
        slowMA.Add(price.Volume);

        if (fastMA.IsReady && slowMA.IsReady)
        {
            var pvo = (fastMA.Last!.Value - slowMA.Last!.Value) / slowMA.Last.Value * 100;
            signalMA.Add(pvo);

            if (signalMA.IsReady)
            {
                var signal = signalMA.Last!.Value;
                var histogram = pvo - signal;

                Values.Add(pvo);
                Signal.Add(signal);
                Histogram.Add(histogram);
            }
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "PVO",
            ValueFormat = "N2",
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(0)
            ],
            ValueSeries =
            [
                new ChartValueSeries($"PVO ({fastPeriods}, {slowPeriods})", Values, ChartValueSeriesStyle.Line, ChartColor.Black),
                new ChartValueSeries($"Signal ({signalPeriods})", Signal, ChartValueSeriesStyle.Line, ChartColor.Red),
                new ChartValueSeries("Histogram", Histogram, ChartValueSeriesStyle.Bar, ChartColor.StrongGray)
            ]
        };
    }
}