using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.AverageIndicators;

/// <summary>
/// Settings for the <see cref="WeightedMovingAverage"/> indicator.
/// </summary>
public sealed class WeightedMovingAverageSettings
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
[DisplayName("WMA")]
[Description("Weighted Moving Average")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class WeightedMovingAverage : IAverageIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly double total;
    private readonly AnalysisWindow prices;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public WeightedMovingAverage(IndicatorCapacity capacity)
        : this(capacity, new WeightedMovingAverageSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeightedMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public WeightedMovingAverage(IndicatorCapacity capacity, WeightedMovingAverageSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        total = Enumerable.Range(1, periods).Sum();
        prices = new AnalysisWindow(periods, false, false);

        Values = capacity.CreateList<double>();
    }

    /// <inheritdoc/>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public double? Last { get; private set; }

    /// <inheritdoc/>
    public bool IsReady => prices.IsFilled;

    /// <inheritdoc/>
    public void Add(double value)
    {
        prices.Add(value);

        if (IsReady)
        {
            Last = Enumerable.Range(1, periods).Sum(i => prices[i - 1] * i / total);
            Values.Add(Last.Value);
        }
        else
        {
            var total = Enumerable.Range(1, prices.Count).Sum();
            Last = Enumerable.Range(1, prices.Count).Sum(i => prices[i - 1] * i / total);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries() =>
    [
        new ChartValueSeries($"WMA ({periods})", Values, ChartValueSeriesStyle.Line)
    ];
}