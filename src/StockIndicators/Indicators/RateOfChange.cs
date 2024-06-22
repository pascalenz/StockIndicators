using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="RateOfChange"/> indicator.
/// </summary>
public sealed class RateOfChangeSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 200)]
    [DefaultValue(12)]
    public int Periods { get; init; } = 12;
}

/// <summary>
/// The Rate-of-Change (ROC) indicator, which is also referred to as simply Momentum, is a pure momentum
/// oscillator that measures the percent change in price from one period to the next. The ROC calculation
/// compares the current price with the price "n" periods ago. The plot forms an oscillator that fluctuates
/// above and below the zero line as the Rate-of-Change moves from positive to negative. As a momentum
/// oscillator, ROC signals include centerline crossovers, divergences and overbought-oversold readings.
/// (StockCharts.com)
/// </summary>
[DisplayName("ROC")]
[Description("Rate-of-Change")]
[Category(IndicatorCategory.Momentum)]
public sealed class RateOfChange : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow prices;

    /// <summary>
    /// Initializes a new instance of the <see cref="RateOfChange"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public RateOfChange(IndicatorCapacity capacity)
        : this(capacity, new RateOfChangeSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="RateOfChange"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public RateOfChange(IndicatorCapacity capacity, RateOfChangeSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        prices = new AnalysisWindow(periods, false, false);

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
        if (prices.Count == periods)
            Values.Add((price.Close - prices[0]) / prices[0] * 100d);

        prices.Add(price.Close);
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"ROC ({periods})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}
