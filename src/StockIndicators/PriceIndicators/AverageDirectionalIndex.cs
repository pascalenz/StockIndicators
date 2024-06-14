using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="AverageDirectionalIndex"/> indicator.
/// </summary>
public sealed class AverageDirectionalIndexSettings
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
/// The Average Directional Index (ADX), Minus Directional Indicator (-DI) and Plus Directional Indicator (+DI)
/// represent a group of directional movement indicators that form a trading system.
/// The Average Directional Index (ADX) measures trend strength without regard to trend direction. The other two indicators,
/// Plus Directional Indicator (+DI) and Minus Directional Indicator (-DI), complement ADX by defining trend direction.
/// Used together, chartists can determine both the direction and strength of the trend. (StockCharts.com)
/// </summary>
[DisplayName("ADX")]
[Description("Average Directional Index")]
[Category(IndicatorCategory.Trend)]
public sealed class AverageDirectionalIndex : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow tr, plusDM, minusDM, dx;
    private double? previousTR, previousPlusDM, previousMinusDM, previousADX;
    private IPrice? previous;

    /// <summary>
    /// Initializes a new instance of the <see cref="AverageDirectionalIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public AverageDirectionalIndex(IndicatorCapacity capacity)
        : this(capacity, new AverageDirectionalIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="AverageDirectionalIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public AverageDirectionalIndex(IndicatorCapacity capacity, AverageDirectionalIndexSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        tr = new AnalysisWindow(periods, true, false);
        plusDM = new AnalysisWindow(periods, true, false);
        minusDM = new AnalysisWindow(periods, true, false);
        dx = new AnalysisWindow(periods, true, false);

        Up = capacity.CreateList<double>();
        Down = capacity.CreateList<double>();
        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the up-trend indicator.
    /// </summary>
    public IReadOnlyList<double> Up { get; }

    /// <summary>
    /// Gets the values for the down-trend indicator.
    /// </summary>
    public IReadOnlyList<double> Down { get; }

    /// <summary>
    /// Gets the values for the average indicator.
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

        // Calculate TR1
        var tr1 = price.TrueRange(previous);
        tr.Add(tr1);

        // Calculate +DM1
        var plusDM1 = (price.High - previous.High > previous.Low - price.Low) ? Math.Max(price.High - previous.High, 0) : 0;
        plusDM.Add(plusDM1);

        // Calculate -DM1
        var minusDM1 = (previous.Low - price.Low > price.High - previous.High) ? Math.Max(previous.Low - price.Low, 0) : 0;
        minusDM.Add(minusDM1);

        if (tr.IsFilled && plusDM.IsFilled && minusDM.IsFilled)
        {
            // Caculate TR14, +DM14, -DM14
            previousTR = previousTR.HasValue ? previousTR.Value - (previousTR.Value / periods) + tr1 : tr.Sum;
            previousPlusDM = previousPlusDM.HasValue ? previousPlusDM.Value - (previousPlusDM.Value / periods) + plusDM1 : plusDM.Sum;
            previousMinusDM = previousMinusDM.HasValue ? previousMinusDM.Value - (previousMinusDM.Value / periods) + minusDM1 : minusDM.Sum;

            // Calculate +DI14, -DI14
            var plusDI14 = 100 * (previousPlusDM.Value / previousTR.Value);
            var minusDI14 = 100 * (previousMinusDM.Value / previousTR.Value);

            // Calculate DX
            dx.Add(100 * (Math.Abs(plusDI14 - minusDI14) / (plusDI14 + minusDI14)));

            if (dx.IsFilled)
            {
                previousADX = previousADX.HasValue ? ((previousADX.Value * (periods - 1)) + dx.Last) / periods : dx.Average;
                Up.Add(plusDI14);
                Down.Add(minusDI14);
                Values.Add(previousADX.Value);
            }
        }

        previous = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart()
        {
            Title = $"ADX ({periods})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries("Up", Up, ChartValueSeriesStyle.Line, ChartColor.PositiveValue),
                new ChartValueSeries("Down", Down, ChartValueSeriesStyle.Line, ChartColor.NegativeValue),
                new ChartValueSeries("Average", Values, ChartValueSeriesStyle.Line, ChartColor.Black)
            ]
        };
    }
}