using StockIndicators.Internal;
using System.ComponentModel;

namespace StockIndicators.Indicators;

/// <summary>
/// Pivots Points are significant levels chartists can use to determine directional movement, support and resistance.
/// Pivot Points use the prior period's high, low and close to formulate future support and resistance.
/// In this regard, Pivot Points are predictive or leading indicators. (StockCharts.com)
/// </summary>
[DisplayName("Fibonacci Pivot Points")]
[Category(IndicatorCategory.Pivot)]
public sealed class FibonacciPivotPoints : IPriceIndicator, IChartValueSeriesProvider
{
    private IPrice? lastPrice;

    /// <summary>
    /// Initializes a new instance of the <see cref="FibonacciPivotPoints"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public FibonacciPivotPoints(IndicatorCapacity capacity)
    {
        IndicatorValidator.Verify(capacity);

        Resistance3 = capacity.CreateList<double>();
        Resistance2 = capacity.CreateList<double>();
        Resistance1 = capacity.CreateList<double>();
        Values = capacity.CreateList<double>();
        Support1 = capacity.CreateList<double>();
        Support2 = capacity.CreateList<double>();
        Support3 = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the 3rd resistance line.
    /// </summary>
    public IReadOnlyList<double> Resistance3 { get; }

    /// <summary>
    /// Gets the values for the 2nd resistance line.
    /// </summary>
    public IReadOnlyList<double> Resistance2 { get; }

    /// <summary>
    /// Gets the values for the 1st resistance line.
    /// </summary>
    public IReadOnlyList<double> Resistance1 { get; }

    /// <summary>
    /// Gets the values for the pivot points.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <summary>
    /// Gets the values for the 1st support line.
    /// </summary>
    public IReadOnlyList<double> Support1 { get; }

    /// <summary>
    /// Gets the values for the 2nd support line.
    /// </summary>
    public IReadOnlyList<double> Support2 { get; }

    /// <summary>
    /// Gets the values for the 3rd support line.
    /// </summary>
    public IReadOnlyList<double> Support3 { get; }

    /// <inheritdoc/>
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        if (lastPrice == null)
        {
            lastPrice = price;
            return;
        }

        if (lastPrice.Timestamp.Date != price.Timestamp.Date)
        {
            var p = (lastPrice.High + lastPrice.Low + lastPrice.Close) / 3;

            Resistance3.Add(p + 1.000 * (lastPrice.High - lastPrice.Low));
            Resistance2.Add(p + 0.618 * (lastPrice.High - lastPrice.Low));
            Resistance1.Add(p + 0.382 * (lastPrice.High - lastPrice.Low));
            Values.Add(p);
            Support1.Add(p - 0.382 * (lastPrice.High - lastPrice.Low));
            Support2.Add(p - 0.618 * (lastPrice.High - lastPrice.Low));
            Support3.Add(p - 1.000 * (lastPrice.High - lastPrice.Low));

            lastPrice = price;
        }
        else if (Values.Count > 0)
        {
            Resistance3.Add(Resistance3.Last());
            Resistance2.Add(Resistance2.Last());
            Resistance1.Add(Resistance1.Last());
            Values.Add(Values.Last());
            Support1.Add(Support1.Last());
            Support2.Add(Support2.Last());
            Support3.Add(Support3.Last());
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        return
        [
            new ChartValueSeries("R3", Resistance3, ChartValueSeriesStyle.Line, ChartColor.NegativeValue),
            new ChartValueSeries("R2", Resistance2, ChartValueSeriesStyle.Line, ChartColor.NegativeValue),
            new ChartValueSeries("R1", Resistance1, ChartValueSeriesStyle.Line, ChartColor.NegativeValue),
            new ChartValueSeries("V", Values, ChartValueSeriesStyle.Line, ChartColor.Black),
            new ChartValueSeries("S1", Support1, ChartValueSeriesStyle.Line, ChartColor.PositiveValue),
            new ChartValueSeries("S2", Support2, ChartValueSeriesStyle.Line, ChartColor.PositiveValue),
            new ChartValueSeries("S3", Support3, ChartValueSeriesStyle.Line, ChartColor.PositiveValue)
        ];
    }
}