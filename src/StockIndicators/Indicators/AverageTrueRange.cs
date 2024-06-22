using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="AverageTrueRange"/> indicator.
/// </summary>
public sealed class AverageTrueRangeSettings
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
/// The Average True Range (ATR) indicator measures volatility. It is important to remember that
/// ATR does not provide an indication of price direction, just volatility. (StockCharts.com)
/// </summary>
[DisplayName("ATR")]
[Description("Average True Range")]
[Category(IndicatorCategory.Volatility)]
public sealed class AverageTrueRange : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private IPrice? previousPrice;
    private double previousATR;
    private int count;

    /// <summary>
    /// Initializes a new instance of the <see cref="AverageTrueRange"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public AverageTrueRange(IndicatorCapacity capacity)
        : this(capacity, new AverageTrueRangeSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AverageTrueRange"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public AverageTrueRange(IndicatorCapacity capacity, AverageTrueRangeSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        count = 0;

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => count >= periods;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        if (count == 0 || previousPrice == null)
        {
            previousATR = price.High - price.Low;
            count++;
        }
        else if (count < periods - 1)
        {
            previousATR += price.TrueRange(previousPrice);
            count++;
        }
        else if (count < periods)
        {
            previousATR = (previousATR + price.TrueRange(previousPrice)) / 14;
            Values.Add(previousATR);
            count++;
        }
        else
        {
            previousATR = (previousATR * (periods - 1) + price.TrueRange(previousPrice)) / periods;
            Values.Add(previousATR);
        }

        previousPrice = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart()
        {
            Title = $"ATR ({periods})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}
