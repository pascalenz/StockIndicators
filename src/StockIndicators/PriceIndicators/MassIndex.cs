using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="MassIndex"/> indicator.
/// </summary>
public sealed class MassIndexSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the moving average.
    /// </summary>
    [DisplayName("MA Periods")]
    [Description("The number of look-back periods for the moving average.")]
    [Range(1, 100)]
    [DefaultValue(9)]
    public int MovingAveragePeriods { get; init; } = 9;

    /// <summary>
    /// Gets or sets the number of values to calculte the sum from.
    /// </summary>
    [DisplayName("Sum Periods")]
    [Description("The number of values to calculte the sum from.")]
    [Range(1, 100)]
    [DefaultValue(25)]
    public int SumPeriods { get; init; } = 25;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Exponential)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Exponential;
}

/// <summary>
/// Developed by Donald Dorsey, the Mass Index uses the high-low range to identify trend reversals based on range expansions.
/// In this sense, the Mass Index is a volatility indicator that does not have a directional bias. Instead,
/// the Mass Index identifies range bulges that can foreshadow a reversal of the current trend. (StockCharts.com)
/// </summary>
[DisplayName("Mass Index")]
[Category(IndicatorCategory.Trend)]
public sealed class MassIndex : IPriceIndicator, IChartProvider
{
    private readonly int maPeriods;
    private readonly int sumPeriods;
    private readonly IAverageIndicator singleMA;
    private readonly IAverageIndicator doubleMA;
    private readonly AnalysisWindow ratios;

    /// <summary>
    /// Initializes a new instance of the <see cref="MassIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public MassIndex(IndicatorCapacity capacity)
        : this(capacity, new MassIndexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MassIndex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public MassIndex(IndicatorCapacity capacity, MassIndexSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        maPeriods = settings.MovingAveragePeriods;
        sumPeriods = settings.SumPeriods;
        singleMA = MovingAverageFactory.Create(settings.MovingAverageType, maPeriods);
        doubleMA = MovingAverageFactory.Create(settings.MovingAverageType, maPeriods);
        ratios = new AnalysisWindow(sumPeriods, true, false);

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
        singleMA.Add(price.High - price.Low);
        if (singleMA.IsReady)
        {
            doubleMA.Add(singleMA.Last!.Value);
            if (doubleMA.IsReady)
            {
                ratios.Add(singleMA.Last.Value / doubleMA.Last!.Value);
                if (ratios.IsFilled)
                    Values.Add(ratios.Sum);
            }
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "Mass Index",
            ValueFormat = "N2",
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(27)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}