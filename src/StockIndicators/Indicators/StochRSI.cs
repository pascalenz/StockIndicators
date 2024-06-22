using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="StochRSI"/> indicator.
/// </summary>
public sealed class StochRSISettings
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
/// StochRSI is an oscillator that measures the level of RSI relative to its high-low range over a set time period.
/// StochRSI applies the Stochastics formula to RSI values, instead of price values. (StockCharts.com)
/// </summary>
[DisplayName("StochRSI")]
[Category(IndicatorCategory.Momentum)]
public sealed class StochRSI : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly RelativeStrengthIndex rsi;

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRSI"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public StochRSI(IndicatorCapacity capacity)
        : this(capacity, new StochRSISettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StochRSI"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public StochRSI(IndicatorCapacity capacity, StochRSISettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        rsi = new RelativeStrengthIndex(IndicatorCapacity.FromPeriods(periods), new RelativeStrengthIndexSettings { Periods = periods });

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => rsi.Values.Count >= periods;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        rsi.Add(price);

        if (IsReady)
        {
            var last = rsi.Values.Last();
            var highest = rsi.Values.Max();
            var lowest = rsi.Values.Min();
            Values.Add((last - lowest) / (highest - lowest));
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"StochRSI ({periods})",
            ValueFormat = "N2",
            MinValue = 0,
            MaxValue = 1,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(0.3),
                new ChartGridLine(0.5),
                new ChartGridLine(0.7)
            ],
            Highlights =
            [
                new ChartHighlight(0.0, 0.3),
                new ChartHighlight(0.7, 1.0)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}