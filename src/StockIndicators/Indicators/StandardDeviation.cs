using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="StandardDeviation"/> indicator.
/// </summary>
public sealed class StandardDeviationSettings
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
/// Standard Deviation is a statistical term that measures the amount of variability or dispersion around an average.
/// Standard Deviation is also a measure of volatility. Generally speaking, dispersion is the difference between the
/// actual value and the average value. The larger this dispersion or variability is, the higher the standard deviation.
/// The smaller this dispersion or variability is, the lower the standard deviation. Chartists can use the standard
/// deviation to measure expected risk and determine the significance of certain price movements.
/// (StockCharts.com)
/// </summary>
[DisplayName("Standard Deviation")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class StandardDeviation : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow values1;
    private readonly AnalysisWindow values2;

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardDeviation"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public StandardDeviation(IndicatorCapacity capacity)
        : this(capacity, new StandardDeviationSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="StandardDeviation"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public StandardDeviation(IndicatorCapacity capacity, StandardDeviationSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;

        values1 = new AnalysisWindow(periods, true, false);
        values2 = new AnalysisWindow(periods, true, false);

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
        values1.Add(price.Close);
        values2.Add(Math.Pow(price.Close - values1.Average, 2));

        if (values1.IsFilled && values2.IsFilled)
            Values.Add(Math.Sqrt(values2.Average));
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"STDDEV ({periods})",
            ValueFormat = "N1",
            AutoGenerateGridLines = true,
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}