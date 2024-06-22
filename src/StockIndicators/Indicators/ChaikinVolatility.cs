using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="ChaikinVolatility"/> indicator.
/// </summary>
public sealed class ChaikinVolatilitySettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods to compare.
    /// </summary>
    [DisplayName("Compare Periods")]
    [Description("The number of look-back periods to compare.")]
    [Range(1, 50)]
    [DefaultValue(10)]
    public int ComparePeriods { get; init; } = 10;

    /// <summary>
    /// Gets or sets the number of look-back periods for smooting.
    /// </summary>
    [DisplayName("Smoothing Periods")]
    [Description("The number of look-back periods for smooting.")]
    [Range(1, 50)]
    [DefaultValue(10)]
    public int SmoothingPeriods { get; init; } = 10;
}

/// <summary>
/// Chaikin's Volatility indicator compares the spread between a security's high and low prices.
/// It quantifies volatility as a widening of the range between the high and the low price.
/// (metastock.com)
/// </summary>
[DisplayName("Chaikin Volatility")]
[Category(IndicatorCategory.Volatility)]
public sealed class ChaikinVolatility : IPriceIndicator, IChartProvider
{
    private readonly int comparePeriods;
    private readonly int smoothingPeriods;
    private readonly ExponentialMovingAverage ema;
    private readonly AnalysisWindow emaValues;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinVolatility"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public ChaikinVolatility(IndicatorCapacity capacity)
        : this(capacity, new ChaikinVolatilitySettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ChaikinVolatility"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public ChaikinVolatility(IndicatorCapacity capacity, ChaikinVolatilitySettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        comparePeriods = settings.ComparePeriods;
        smoothingPeriods = settings.SmoothingPeriods;
        ema = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = smoothingPeriods });
        emaValues = new AnalysisWindow(comparePeriods + 1, false, false);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => ema.IsReady && emaValues.IsFilled;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        ema.Add(price.High - price.Low);

        if (ema.IsReady)
        {
            emaValues.Add(ema.Last!.Value);

            if (emaValues.IsFilled)
                Values.Add((emaValues.Last - emaValues.First) / emaValues.First * 100d);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Chaikin Volatility ({comparePeriods}, {smoothingPeriods})",
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
