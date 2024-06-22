using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="HullMovingAverage"/> indicator.
/// </summary>
public sealed class HullMovingAverageSettings
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
/// The Hull Moving Average (HMA), developed by Alan Hull, is an extremely fast and smooth moving average.
/// In fact, the HMA almost eliminates lag altogether and manages to improve smoothing at the same time.
/// (fidelity.com)
/// </summary>
[DisplayName("HMA")]
[Description("Hull Moving Average")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class HullMovingAverage : IAverageIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly WeightedMovingAverage longWMA;
    private readonly WeightedMovingAverage shortWMA;
    private readonly WeightedMovingAverage resultWMA;

    /// <summary>
    /// Initializes a new instance of the <see cref="HullMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public HullMovingAverage(IndicatorCapacity capacity)
        : this(capacity, new HullMovingAverageSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="HullMovingAverage"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public HullMovingAverage(IndicatorCapacity capacity, HullMovingAverageSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        var sqrtPeriods = Convert.ToInt32(Math.Round(Math.Sqrt(periods)));

        longWMA = new WeightedMovingAverage(IndicatorCapacity.Minimum, new WeightedMovingAverageSettings { Periods = periods });
        shortWMA = new WeightedMovingAverage(IndicatorCapacity.Minimum, new WeightedMovingAverageSettings { Periods = periods / 2 });
        resultWMA = new WeightedMovingAverage(IndicatorCapacity.Minimum, new WeightedMovingAverageSettings { Periods = sqrtPeriods });

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
        longWMA.Add(value);
        shortWMA.Add(value);

        if (longWMA.IsReady && shortWMA.IsReady)
        {
            resultWMA.Add(2 * shortWMA.Last!.Value - longWMA.Last!.Value);
            Last = resultWMA.Last!.Value;

            if (resultWMA.IsReady)
                Values.Add(Last.Value);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries() =>
    [
        new ChartValueSeries($"HMA ({periods})", Values, ChartValueSeriesStyle.Line)
    ];
}