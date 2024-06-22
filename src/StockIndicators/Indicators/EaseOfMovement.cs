using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="EaseOfMovement"/> indicator.
/// </summary>
public sealed class EaseOfMovementSettings
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
/// Ease of Movement (EMV) is a volume-based oscillator that fluctuates above and below the zero line. As its name implies,
/// it is designed to measure the “ease” of price movement. Arms created Equivolume charts to visually display price ranges and volume.
/// Ease of Movement takes Equivolume to the next level by quantifying the price/volume relationship and showing the results as an oscillator.
/// In general, prices are advancing with relative ease when the oscillator is in positive territory. Conversely, prices are declining with
/// relative ease when the oscillator is in negative territory. (StockCharts.com)
/// </summary>
[DisplayName("EMV")]
[Description("Ease of Movement")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class EaseOfMovement : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow emvs;
    private IPrice? previous;

    /// <summary>
    /// Initializes a new instance of the <see cref="EaseOfMovement"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public EaseOfMovement(IndicatorCapacity capacity)
        : this(capacity, new EaseOfMovementSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EaseOfMovement"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public EaseOfMovement(IndicatorCapacity capacity, EaseOfMovementSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        emvs = new AnalysisWindow(periods, true, false);

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
        if (previous == null)
        {
            previous = price;
            return;
        }

        var emv = ((price.High + price.Low) / 2 - (previous.High + previous.Low) / 2) / (price.Volume / 100000000d / (price.High - price.Low));
        emvs.Add(emv);

        if (emvs.IsFilled)
        {
            Values.Add(emvs.Average);
        }

        previous = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "EMV",
            ValueFormat = "N1",
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