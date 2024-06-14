using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="ChaikinMoneyFlow"/> indicator.
/// </summary>
public sealed class ChaikinMoneyFlowSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 100)]
    [DefaultValue(20)]
    public int Periods { get; init; } = 20;
}

/// <summary>
/// Chaikin Money Flow (CMF) measures the amount of Money Flow Volume over a specific period.
/// Money Flow Volume forms the basis for the Accumulation Distribution Line. Instead of a cumulative total
/// of Money Flow Volume, Chaikin Money Flow simply sums Money Flow Volume for a specific look-back period.
/// The resulting indicator fluctuates above/below the zero line just like an oscillator.
/// Chartists weigh the balance of buying or selling pressure with the absolute level of Chaikin Money Flow.
/// Chartists can also look for crosses above or below the zero line to identify changes on money flow.
/// (StockCharts.com)
/// </summary>
[DisplayName("CMF")]
[Description("Chaikin Money Flow")]
[Category(IndicatorCategory.Momentum)]
public sealed class ChaikinMoneyFlow : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow prices;
    private readonly AnalysisWindow volumes;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinMoneyFlow"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public ChaikinMoneyFlow(IndicatorCapacity capacity)
        : this(capacity, new ChaikinMoneyFlowSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinMoneyFlow"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public ChaikinMoneyFlow(IndicatorCapacity capacity, ChaikinMoneyFlowSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        prices = new AnalysisWindow(periods, true, false);
        volumes = new AnalysisWindow(periods, true, false);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => prices.IsFilled && volumes.IsFilled;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        prices.Add(price.MoneyFlowVolume());
        volumes.Add(price.Volume);

        if (IsReady)
            Values.Add(prices.Sum / volumes.Sum);
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"CMF ({periods})",
            ValueFormat = "N2",
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
