using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="AroonUpDown"/> indicator.
/// </summary>
public sealed class AroonUpDownSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 100)]
    [DefaultValue(25)]
    public int Periods { get; init; } = 25;
}

/// <summary>
/// The Aroon Oscillator is the difference between Aroon-Up and Aroon-Down. These two indicators are usually plotted
/// together for easy comparison, but chartists can also view the difference of these two indicators with the Aroon Oscillator.
/// This indicator fluctuates between -100 and +100 with zero as the middle line. A upward trend bias is present when the
/// oscillator is positive, while a downward trend bias exists when the oscillator is negative. Chartists can also expand the
/// bull-bear threshold to identify stronger signals. See our ChartSchool article for more details on Aroon-Up and Aroon-Down.
/// (StockCharts.com)
/// </summary>
[DisplayName("Aroon Up/Down")]
[Category(IndicatorCategory.Trend)]
public sealed class AroonUpDown : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow prices;

    /// <summary>
    /// Initializes a new instance of the <see cref="AroonUpDown"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public AroonUpDown(IndicatorCapacity capacity)
        : this(capacity, new AroonUpDownSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AroonUpDown"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public AroonUpDown(IndicatorCapacity capacity, AroonUpDownSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        prices = new AnalysisWindow(periods, false, false);

        Up = capacity.CreateList<double>();
        Down = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the up-trend indicator.
    /// </summary>
    public IReadOnlyList<double> Up { get; }

    /// <summary>
    /// Gets the values for the down-trend indicator.
    /// </summary>
    public IReadOnlyList<double> Down { get; }

    /// <inheritdoc/>
    public bool IsReady => prices.IsFilled;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        prices.Add(price.Close);

        if (prices.IsFilled)
        {
            double highestValue = double.MinValue;
            double lowestValue = double.MaxValue;
            int daysSinceHighest = 0;
            int daysSinceLowest = 0;

            foreach (var previousPrice in prices)
            {
                if (previousPrice > highestValue)
                {
                    highestValue = previousPrice;
                    daysSinceHighest = 0;
                }
                else
                {
                    daysSinceHighest++;
                }

                if (previousPrice < lowestValue)
                {
                    lowestValue = previousPrice;
                    daysSinceLowest = 0;
                }
                else
                {
                    daysSinceLowest++;
                }
            }

            Up.Add((periods - daysSinceHighest) * 100d / periods);
            Down.Add((periods - daysSinceLowest) * 100d / periods);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart()
        {
            Title = $"Aroon Up/Down ({periods})",
            ValueFormat = "N0",
            MinValue = 0,
            MaxValue = 100,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(30),
                new ChartGridLine(50),
                new ChartGridLine(70)
            ],
            Highlights =
            [
                new ChartHighlight(0, 30),
                new ChartHighlight(70, 100)
            ],
            ValueSeries =
            [
                new ChartValueSeries("Up", Up, ChartValueSeriesStyle.Line, ChartColor.PositiveValue),
                new ChartValueSeries("Down", Down, ChartValueSeriesStyle.Line, ChartColor.NegativeValue)
            ]
        };
    }
}
