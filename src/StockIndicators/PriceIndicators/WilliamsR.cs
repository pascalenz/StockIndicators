using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="WilliamsR"/> indicator.
/// </summary>
public sealed class WilliamsRSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 100)]
    [DefaultValue(14)]
    public int Periods { get; init; } = 14;
}

/// <summary>
/// Williams %R is a momentum indicator that is the inverse of the Fast Stochastic Oscillator.
/// Also referred to as %R, Williams %R reflects the level of the close relative to the highest high for
/// the look-back period. In contrast, the Stochastic Oscillator reflects the level of the close relative
/// to the lowest low. %R corrects for the inversion by multiplying the raw value by -100. As a result,
/// the Fast Stochastic Oscillator and Williams %R produce the exact same lines, only the scaling is different.
/// (StockCharts.com)
/// </summary>
[DisplayName("Williams %R")]
[Category(IndicatorCategory.Momentum)]
public sealed class WilliamsR : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow highs;
    private readonly AnalysisWindow lows;

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsR"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public WilliamsR(IndicatorCapacity capacity)
        : this(capacity, new WilliamsRSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="WilliamsR"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public WilliamsR(IndicatorCapacity capacity, WilliamsRSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        highs = new AnalysisWindow(periods, false, true);
        lows = new AnalysisWindow(periods, false, true);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => highs.IsFilled && lows.IsFilled;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        highs.Add(price.High);
        lows.Add(price.Low);

        if (IsReady)
            Values.Add((highs.Max - price.Close) / (highs.Max - lows.Min) * -100);
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Williams %R ({periods})",
            ValueFormat = "N0",
            MinValue = -100,
            MaxValue = 0,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(-20),
                new ChartGridLine(-50),
                new ChartGridLine(-80)
            ],
            Highlights =
            [
                new ChartHighlight(0, -20),
                new ChartHighlight(-80, -100)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}