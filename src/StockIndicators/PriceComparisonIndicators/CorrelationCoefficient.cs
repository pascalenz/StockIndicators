using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceComparisonIndicators;

/// <summary>
/// Settings for the <see cref="CorrelationCoefficient"/> indicator.
/// </summary>
public sealed class CorrelationCoefficientSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 200)]
    [DefaultValue(50)]
    public int Periods { get; init; } = 50;
}

/// <summary>
/// The Correlation Coefficient is a statistical measure that reflects the correlation between two securities.
/// In other words, this statistic tells us how closely one security is related to the other.
/// The Correlation Coefficient is positive when both securities move in the same direction, up or down.
/// The Correlation Coefficient is negative when the two securities move in opposite directions.
/// Determining the relationship between two securities is useful for analyzing intermarket relationships,
/// sector/stock relationships and sector/market relationships. This indicator can also help investors
/// diversify by identifying securities with a low or negative correlation to the stock market.
/// (StockCharts.com)
/// </summary>
[DisplayName("Correlation Coefficient")]
[Category(IndicatorCategory.Comparison)]
public sealed class CorrelationCoefficient : IPriceComparisonIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow prices1;
    private readonly AnalysisWindow prices2;
    private readonly AnalysisWindow squared1;
    private readonly AnalysisWindow squared2;
    private readonly AnalysisWindow prices1x2;

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationCoefficient"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public CorrelationCoefficient(IndicatorCapacity capacity)
        : this(capacity, new CorrelationCoefficientSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorrelationCoefficient"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public CorrelationCoefficient(IndicatorCapacity capacity, CorrelationCoefficientSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        prices1 = new AnalysisWindow(periods, true, false);
        prices2 = new AnalysisWindow(periods, true, false);
        squared1 = new AnalysisWindow(periods, true, false);
        squared2 = new AnalysisWindow(periods, true, false);
        prices1x2 = new AnalysisWindow(periods, true, false);

        Values = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <inheritdoc/>
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price1, IPrice price2)
    {
        prices1.Add(price1.Close);
        prices2.Add(price2.Close);
        squared1.Add(price1.Close * price1.Close);
        squared2.Add(price2.Close * price2.Close);
        prices1x2.Add(price1.Close * price2.Close);

        if (prices1x2.IsFilled)
        {
            var variance1 = squared1.Average - (prices1.Average * prices1.Average);
            var variance2 = squared2.Average - (prices2.Average * prices2.Average);
            var covariance = prices1x2.Average - (prices1.Average * prices2.Average);
            var coefficient = covariance / Math.Sqrt(variance1 * variance2);
            Values.Add(coefficient);
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "Correlation",
            ValueFormat = "N2",
            MinValue = -1,
            MaxValue = 1,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(0.75),
                new ChartGridLine(0.50),
                new ChartGridLine(0.25),
                new ChartGridLine(0.00),
                new ChartGridLine(-0.25),
                new ChartGridLine(-0.50),
                new ChartGridLine(-0.75),
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line)
            ]
        };
    }
}
