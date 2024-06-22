using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="TrueStrengthIndex"/> indicator.
/// </summary>
public sealed class TrueStrengthIndexSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the first moving average.
    /// </summary>
    [DisplayName("1st Smoothing Periods")]
    [Description("The number of look-back periods for the first moving average.")]
    [Range(1, 100)]
    [DefaultValue(25)]
    public int FirstSmoothingPeriods { get; init; } = 25;

    /// <summary>
    /// Gets or sets the number of look-back periods for the second moving average.
    /// </summary>
    [DisplayName("2nd Smoothing Periods")]
    [Description("The number of look-back periods for the second moving average.")]
    [Range(1, 100)]
    [DefaultValue(13)]
    public int SecondSmoothingPeriods { get; init; } = 13;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Exponential)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Exponential;
}

/// <summary>
/// The True Strength Index (TSI) is a momentum oscillator based on a double smoothing of price changes.
/// Even though several steps are needed for calculation, the indicator is actually pretty straightforward.
/// By smoothing price changes, TSI captures the ebbs and flows of price action with a steadier line that
/// filters out the noise. As with most momentum oscillators, chartists can derive signals from overbought/oversold
/// readings, centerline crossovers, bullish/bearish divergences and signal line crossovers. (StockCharts.com)
/// </summary>
[DisplayName("TSI")]
[Description("True Strength Index")]
[Category(IndicatorCategory.Momentum)]
public sealed class TrueStrengthIndex : IPriceIndicator, IChartProvider
{
    private readonly int firstSmoothingPeriods;
    private readonly int secondSmoothingPeriods;
    private readonly IAverageIndicator firstMA;
    private readonly IAverageIndicator secondMA;
    private readonly IAverageIndicator firstAbsoluteMA;
    private readonly IAverageIndicator secondAbsoluteMA;
    private double? last;
    private double? previous;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrueStrengthIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public TrueStrengthIndex(IndicatorCapacity capacity)
        : this(capacity, new TrueStrengthIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrueStrengthIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public TrueStrengthIndex(IndicatorCapacity capacity, TrueStrengthIndexSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        firstSmoothingPeriods = settings.FirstSmoothingPeriods;
        secondSmoothingPeriods = settings.SecondSmoothingPeriods;

        firstMA = MovingAverageFactory.Create(settings.MovingAverageType, firstSmoothingPeriods);
        secondMA = MovingAverageFactory.Create(settings.MovingAverageType, secondSmoothingPeriods);
        firstAbsoluteMA = MovingAverageFactory.Create(settings.MovingAverageType, firstSmoothingPeriods);
        secondAbsoluteMA = MovingAverageFactory.Create(settings.MovingAverageType, secondSmoothingPeriods);

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
        previous = last;
        last = price.Close;

        if (previous.HasValue)
        {
            var pc = last.Value - previous.Value;
            firstMA.Add(pc);
            firstAbsoluteMA.Add(Math.Abs(pc));

            if (firstMA.IsReady && firstAbsoluteMA.IsReady)
            {
                secondMA.Add(firstMA.Last!.Value);
                secondAbsoluteMA.Add(firstAbsoluteMA.Last!.Value);

                if (secondMA.IsReady && secondAbsoluteMA.IsReady)
                {
                    var tsi = 100 * secondMA.Last!.Value / secondAbsoluteMA.Last!.Value;
                    Values.Add(tsi);
                }
            }
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "TSI",
            ValueFormat = "N1",
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(0)
            ],
            ValueSeries =
            [
                new ChartValueSeries($"TSI ({firstSmoothingPeriods}, {secondSmoothingPeriods})", Values, ChartValueSeriesStyle.Line, ChartColor.Black)
            ]
        };
    }
}