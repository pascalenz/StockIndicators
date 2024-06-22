using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="Momentum"/> indicator.
/// </summary>
public sealed class MomentumSettings
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
/// Momentum measures the rate of change in closing prices and is used to detect trend
/// weakness and likely reversal points. It is often underrated because of its simplicity.
/// (incrediblecharts.com)
/// </summary>
[DisplayName("Momentum")]
[Category(IndicatorCategory.Momentum)]
public sealed class Momentum : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow window;

    /// <summary>
    /// Initializes a new instance of the <see cref="Momentum"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public Momentum(IndicatorCapacity capacity)
        : this(capacity, new MomentumSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Momentum"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public Momentum(IndicatorCapacity capacity, MomentumSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        window = new AnalysisWindow(periods + 1, false, true);

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
        window.Add(price.Close);

        if (window.IsFilled)
            Values.Add(window.Last - window.First);
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Momentum ({periods})",
            ValueFormat = "N0",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}