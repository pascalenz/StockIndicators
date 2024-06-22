using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.Indicators;

/// <summary>
/// Bollinger Bands %b (pronounced "percent b") is derived from the formula for Stochastics and shows
/// where price is in relation to the bands. %b equals 1 at the upper band and 0 at the lower band.
/// </summary>
[DisplayName("Bollinger Bands - %B")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class BollingerBandPercentB : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly double factor;
    private readonly BollingerBand bollingerBand;

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandPercentB"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public BollingerBandPercentB(IndicatorCapacity capacity)
        : this(capacity, new BollingerBandSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="BollingerBandPercentB"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public BollingerBandPercentB(IndicatorCapacity capacity, BollingerBandSettings settings)
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
            Values.Add((price.Close - lower) / (upper - lower));
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Bollinger Bands - %B ({periods}, {factor:N1})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}
