using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="Envelopes"/> indicator.
/// </summary>
public sealed class EnvelopesSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 200)]
    [DefaultValue(20)]
    public int Periods { get; init; } = 20;

    /// <summary>
    /// Gets or sets the envelope width.
    /// </summary>
    [DisplayName("Envelope")]
    [Description("The envelope width.")]
    [Range(0.005, 0.500)]
    [DefaultValue(0.025)]
    public double Envelope { get; init; } = 0.025;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Exponential)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Exponential;
}

/// <summary>
/// Moving Average Envelopes are percentage-based envelopes set above and below a moving average. The moving average,
/// which forms the base for this indicator, can be a simple or exponential moving average. Each envelope is then set
/// the same percentage above or below the moving average. This creates parallel bands that follow price action.
/// With a moving average as the base, Moving Average Envelopes can be used as a trend following indicator.
/// However, this indicator is not limited to just trend following. The envelopes can also be used to identify
/// overbought and oversold levels when the trend is relatively flat. (StockCharts.com)
/// </summary>
[DisplayName("Envelopes")]
[Category(IndicatorCategory.Momentum)]
public sealed class Envelopes : IPriceIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly double envelope;
    private readonly IAverageIndicator average;

    /// <summary>
    /// Initializes a new instance of the <see cref="Envelopes"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public Envelopes(IndicatorCapacity capacity)
        : this(capacity, new EnvelopesSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Envelopes"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public Envelopes(IndicatorCapacity capacity, EnvelopesSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        envelope = settings.Envelope;
        average = MovingAverageFactory.Create(settings.MovingAverageType, periods);

        UpperEnvelope = capacity.CreateList<double>();
        Average = capacity.CreateList<double>();
        LowerEnvelope = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the upper envelope.
    /// </summary>
    public IReadOnlyList<double> UpperEnvelope { get; }

    /// <summary>
    /// Gets the values for the average.
    /// </summary>
    public IReadOnlyList<double> Average { get; }

    /// <summary>
    /// Gets the values for the lower envelope.
    /// </summary>
    public IReadOnlyList<double> LowerEnvelope { get; }

    /// <inheritdoc/>
    public bool IsReady => average.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        average.Add(price.Close);

        if (average.IsReady)
        {
            var average = this.average.Last!.Value;
            Average.Add(average);
            UpperEnvelope.Add(average + average * envelope);
            LowerEnvelope.Add(average - average * envelope);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        var title = $"Envelopes ({periods}, {envelope * 100:N1}%)";

        return
        [
            new ChartValueSeries(null, UpperEnvelope, ChartValueSeriesStyle.Line),
            new ChartValueSeries(title, Average, ChartValueSeriesStyle.Line),
            new ChartValueSeries(null, LowerEnvelope, ChartValueSeriesStyle.Line)
        ];
    }
}