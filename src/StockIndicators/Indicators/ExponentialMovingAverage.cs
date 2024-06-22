using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="ExponentialMovingAverage"/> indicator.
/// </summary>
public sealed class ExponentialMovingAverageSettings
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
[DisplayName("EMA")]
[Description("Exponential Moving Average")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class ExponentialMovingAverage : IAverageIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly double factor;
    private int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public ExponentialMovingAverage(IndicatorCapacity capacity)
        : this(capacity, new ExponentialMovingAverageSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public ExponentialMovingAverage(IndicatorCapacity capacity, ExponentialMovingAverageSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        factor = 2.0 / (1.0 + settings.Periods);
        count = 0;

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
        var previous = Last ?? value;
        Last = (value - previous) * factor + previous;

        if (count < periods)
        {
            count++;
            return;
        }

        Values.Add(Last.Value);
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries() =>
    [
        new ChartValueSeries($"EMA ({periods})", Values, ChartValueSeriesStyle.Line)
    ];
}
