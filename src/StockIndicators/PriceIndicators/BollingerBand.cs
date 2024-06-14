using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="BollingerBand"/> indicator.
/// </summary>
public sealed class BollingerBandSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 100)]
    [DefaultValue(20)]
    public int Periods { get; init; } = 20;

    /// <summary>
    /// Gets or sets the standard deviation muliplication factor.
    /// </summary>
    [DisplayName("Factor")]
    [Description("The standard deviation muliplication factor.")]
    [Range(0.5, 5.0)]
    [DefaultValue(2.0)]
    public double Factor { get; init; } = 2.0;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Simple)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Simple;
}

/// <summary>
/// Bollinger Bands® are volatility bands placed above and below a moving average. Volatility is based on the standard deviation,
/// which changes as volatility increases and decreases. The bands automatically widen when volatility increases and narrow when
/// volatility decreases. (StockCharts.com)
/// </summary>
[DisplayName("Bollinger Bands")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class BollingerBand : IPriceIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly double factor;
    private readonly IAverageIndicator averageMA;
    private readonly IAverageIndicator deviationMA;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBand"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public BollingerBand(IndicatorCapacity capacity)
        : this(capacity, new BollingerBandSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBand"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public BollingerBand(IndicatorCapacity capacity, BollingerBandSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        factor = settings.Factor;

        averageMA = MovingAverageFactory.Create(settings.MovingAverageType, settings.Periods);
        deviationMA = MovingAverageFactory.Create(settings.MovingAverageType, settings.Periods);

        UpperBand = capacity.CreateList<double>();
        MiddleBand = capacity.CreateList<double>();
        LowerBand = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the upper indicator band.
    /// </summary>
    public IReadOnlyList<double> UpperBand { get; }

    /// <summary>
    /// Gets the values for the middle indicator band.
    /// </summary>
    public IReadOnlyList<double> MiddleBand { get; }

    /// <summary>
    /// Gets the values for the lower indicator band.
    /// </summary>
    public IReadOnlyList<double> LowerBand { get; }

    /// <inheritdoc/>
    public bool IsReady => deviationMA.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        averageMA.Add(price.Close);

        if (averageMA.IsReady)
        {
            var average = averageMA.Last!.Value;
            deviationMA.Add(Math.Pow(price.Close - average, 2));

            if (deviationMA.IsReady)
            {
                var deviation = Math.Sqrt(deviationMA.Last!.Value);
                MiddleBand.Add(average);
                UpperBand.Add(average + deviation * factor);
                LowerBand.Add(average - deviation * factor);
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        var title = $"Bollinger Bands ({periods}, {factor:N1})";

        return
        [
            new ChartValueSeries(null, UpperBand, ChartValueSeriesStyle.Line),
            new ChartValueSeries(title, MiddleBand, ChartValueSeriesStyle.Line),
            new ChartValueSeries(null, LowerBand, ChartValueSeriesStyle.Line)
        ];
    }
}