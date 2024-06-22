using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="AroonOscillator"/> indicator.
/// </summary>
public sealed class AroonOscillatorSettings
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
[DisplayName("Aroon Oscillator")]
[Category(IndicatorCategory.Trend)]
public sealed class AroonOscillator : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AroonUpDown aroon;

    /// <summary>
    /// Initializes a new instance of the <see cref="AroonOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public AroonOscillator(IndicatorCapacity capacity)
        : this(capacity, new AroonOscillatorSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AroonOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public AroonOscillator(IndicatorCapacity capacity, AroonOscillatorSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        aroon = new AroonUpDown(IndicatorCapacity.Minimum, new AroonUpDownSettings { Periods = periods });

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => aroon.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        aroon.Add(price);

        if (aroon.IsReady)
        {
            var up = aroon.Up[aroon.Up.Count - 1];
            var down = aroon.Down[aroon.Down.Count - 1];
            Values.Add(up - down);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart()
        {
            Title = $"Aroon Oscillator ({periods})",
            ValueFormat = "N0",
            MinValue = -100,
            MaxValue = 100,
            AutoGenerateGridLines = false,
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
