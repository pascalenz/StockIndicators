using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="ChaikinOscillator"/> indicator.
/// </summary>
public sealed class ChaikinOscillatorSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the fast moving average.
    /// </summary>
    [DisplayName("Fast Periods")]
    [Description("The number of look-back periods for the fast moving average.")]
    [Range(1, 100)]
    [DefaultValue(3)]
    public int FastPeriods { get; init; } = 3;

    /// <summary>
    /// Gets or sets the number of look-back periods for the slow moving average.
    /// </summary>
    [DisplayName("Slow Periods")]
    [Description("The number of look-back periods for the slow moving average.")]
    [Range(1, 100)]
    [DefaultValue(10)]
    public int SlowPeriods { get; init; } = 10;
}

/// <summary>
/// The Chaikin Oscillator measures the momentum of the Accumulation Distribution Line using the MACD formula.
/// The Chaikin Oscillator is the difference between the 3-day EMA of the Accumulation Distribution Line and
/// the 10-day EMA of the Accumulation Distribution Line. Like other momentum indicators, this indicator is
/// designed to anticipate directional changes in the Accumulation Distribution Line by measuring the momentum
/// behind the movements. A momentum change is the first step to a trend change.
/// Anticipating trend changes in the Accumulation Distribution Line can help chartists anticipate trend changes
/// in the underlying security. The Chaikin Oscillator generates signals with crosses above/below the zero line
/// or with bullish/bearish divergences. (StockCharts.com)
/// </summary>
[DisplayName("Chaikin Oscillator")]
[Category(IndicatorCategory.Momentum)]
public sealed class ChaikinOscillator : IPriceIndicator, IChartProvider
{
    private readonly int fastPeriods;
    private readonly int slowPeriods;
    private readonly ExponentialMovingAverage fastEMA;
    private readonly ExponentialMovingAverage slowEMA;
    private double last = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public ChaikinOscillator(IndicatorCapacity capacity)
        : this(capacity, new ChaikinOscillatorSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public ChaikinOscillator(IndicatorCapacity capacity, ChaikinOscillatorSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        fastPeriods = settings.FastPeriods;
        slowPeriods = settings.SlowPeriods;
        fastEMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = fastPeriods });
        slowEMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = slowPeriods });

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => fastEMA.IsReady && slowEMA.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        last += price.MoneyFlowVolume();

        fastEMA.Add(last);
        slowEMA.Add(last);

        if (IsReady)
            Values.Add(fastEMA.Last!.Value - slowEMA.Last!.Value);
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Chaikin Oscillator ({fastPeriods}, {slowPeriods})",
            ValueFormat = "N0",
            GridLines =
            [
                new ChartGridLine(0)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}