using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="MoneyFlowIndex"/> indicator.
/// </summary>
public sealed class MoneyFlowIndexSettings
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
/// The Money Flow Index (MFI) is an oscillator that uses both price and volume to measure buying and selling pressure.
/// MFI is also known as volume-weighted RSI. MFI starts with the typical price for each period. Money flow is positive
/// when the typical price rises (buying pressure) and negative when the typical price declines (selling pressure).
/// A ratio of positive and negative money flow is then plugged into an RSI formula to create an oscillator that moves
/// between zero and one hundred. As a momentum oscillator tied to volume, the Money Flow Index (MFI) is best suited to
/// identify reversals and price extremes with a variety of signals. (StockCharts.com)
/// </summary>
[DisplayName("MFI")]
[Description("Money Flow Index")]
[Category(IndicatorCategory.Momentum)]
public sealed class MoneyFlowIndex : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow prices;
    private double last = 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoneyFlowIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public MoneyFlowIndex(IndicatorCapacity capacity)
        : this(capacity, new MoneyFlowIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoneyFlowIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public MoneyFlowIndex(IndicatorCapacity capacity, MoneyFlowIndexSettings settings)
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
    public bool IsReady => prices.IsFilled;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        var typical = price.Typical();
        var raw = typical * price.Volume;

        if (last == 0)
        {
            last = typical;
        }

        int up = 0;
        if (typical > last) up = 1;
        if (typical < last) up = -1;

        prices.Add(raw * up);

        if (IsReady)
        {
            var pos = prices.Where(p => p > 0).Sum();
            var neg = prices.Where(p => p < 0).Sum();
            var ratio = Math.Abs(pos / neg);
            var idx = 100 - 100 / (1 + ratio);

            Values.Add(idx);
        }

        last = typical;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"MFI ({periods})",
            ValueFormat = "N0",
            MinValue = 0,
            MaxValue = 100,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(20),
                new ChartGridLine(50),
                new ChartGridLine(80)
            ],
            Highlights =
            [
                new ChartHighlight(0, 20),
                new ChartHighlight(80, 100)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}