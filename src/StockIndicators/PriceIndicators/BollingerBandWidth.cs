using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Bandwidth tells how wide the Bollinger Bands are on a normalized basis.
/// </summary>
[DisplayName("Bollinger Bands - Bandwidth")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class BollingerBandWidth : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly double factor;
    private readonly BollingerBand bollingerBand;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBand"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public BollingerBandWidth(IndicatorCapacity capacity)
        : this(capacity, new BollingerBandSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandWidth"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public BollingerBandWidth(IndicatorCapacity capacity, BollingerBandSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        factor = settings.Factor;

        bollingerBand = new BollingerBand(IndicatorCapacity.Minimum, settings);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => bollingerBand.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        bollingerBand.Add(price);

        if (bollingerBand.IsReady)
        {
            var lower = bollingerBand.LowerBand[bollingerBand.LowerBand.Count - 1];
            var upper = bollingerBand.UpperBand[bollingerBand.UpperBand.Count - 1];
            var middle = bollingerBand.MiddleBand[bollingerBand.MiddleBand.Count - 1];
            Values.Add((upper - lower) / middle);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Bollinger Bands - Bandwidth ({periods}, {factor:N1})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}
