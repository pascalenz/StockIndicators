using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="Stochastic"/> indicator.
/// </summary>
public sealed class StochasticSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the %K.
    /// </summary>
    [DisplayName("%K Periods")]
    [Description("The nnumber of look-back periods for the %K.")]
    [Range(1, 100)]
    [DefaultValue(14)]
    public int KPeriods { get; init; } = 14;

    /// <summary>
    /// Gets or sets the number of periods to smooth the %K..
    /// </summary>
    [DisplayName("%K Smoothing Periods")]
    [Description("The number of periods to smooth the %K..")]
    [Range(1, 100)]
    [DefaultValue(3)]
    public int KSmoothPeriods { get; init; } = 3;

    /// <summary>
    /// Gets or sets the number of periods to smooth the %D.
    /// </summary>
    [DisplayName("%D Smoothing Periods")]
    [Description("The number of periods to smooth the %D.")]
    [Range(1, 100)]
    [DefaultValue(3)]
    public int DSmoothPeriods { get; init; } = 3;

    /// <summary>
    /// Gets or sets the type of moving average to use.
    /// </summary>
    [DisplayName("Moving Average Type")]
    [Description("The type of moving average to use.")]
    [DefaultValue(MovingAverageType.Simple)]
    public MovingAverageType MovingAverageType { get; init; } = MovingAverageType.Simple;
}

/// <summary>
/// The Stochastic Oscillator is a momentum indicator that shows the location of the close relative to the high-low
/// range over a set number of periods. As a rule, the momentum changes direction before price. As such, bullish and
/// bearish divergences in the Stochastic Oscillator can be used to foreshadow reversals.
/// Because the Stochastic Oscillator is range bound, is also useful for identifying overbought and oversold levels.
/// (StockCharts.com)
/// </summary>
[DisplayName("Stochastic Oscillator")]
[Category(IndicatorCategory.Momentum)]
public sealed class Stochastic : IPriceIndicator, IChartProvider
{
    private readonly int kPeriods;
    private readonly int kSmoothPeriods;
    private readonly int dSmoothPeriods;
    private readonly AnalysisWindow lows;
    private readonly AnalysisWindow highs;
    private readonly IAverageIndicator fastMA;
    private readonly IAverageIndicator fullMA;

    /// <summary>
    /// Initializes a new instance of the <see cref="Stochastic"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public Stochastic(IndicatorCapacity capacity)
        : this(capacity, new StochasticSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Stochastic"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public Stochastic(IndicatorCapacity capacity, StochasticSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        kPeriods = settings.KPeriods;
        kSmoothPeriods = settings.KSmoothPeriods;
        dSmoothPeriods = settings.DSmoothPeriods;

        lows = new AnalysisWindow(kPeriods, false, true);
        highs = new AnalysisWindow(kPeriods, false, true);
        fastMA = MovingAverageFactory.Create(settings.MovingAverageType, kSmoothPeriods);
        fullMA = MovingAverageFactory.Create(settings.MovingAverageType, dSmoothPeriods);

        KLine = capacity.CreateList<double>();
        DLine = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the K-Line.
    /// </summary>
    public IReadOnlyList<double> KLine { get; }

    /// <summary>
    /// Gets the values for the D-Line.
    /// </summary>
    public IReadOnlyList<double> DLine { get; }

    /// <inheritdoc/>
    public bool IsReady => fullMA.IsReady;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        lows.Add(price.Low);
        highs.Add(price.High);

        if (lows.IsFilled && highs.IsFilled)
        {
            var k = (price.Close - lows.Min) / (highs.Max - lows.Min) * 100;
            fastMA.Add(k);

            if (fastMA.IsReady)
            {
                var fullK = fastMA.Last!.Value;
                fullMA.Add(fullK);

                if (fullMA.IsReady)
                {
                    var fullD = fullMA.Last!.Value;
                    KLine.Add(fullK);
                    DLine.Add(fullD);
                }
            }
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "Stochastic",
            ValueFormat = "N0",
            MinValue = 0,
            MaxValue = 100,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(20),
                new ChartGridLine(50),
                new ChartGridLine(80)
            ],
            Highlights =
            [
                new ChartHighlight(0, 20),
                new ChartHighlight(80, 100)
            ],
            ValueSeries =
            [
                new ChartValueSeries($"%K ({kPeriods}, {kSmoothPeriods})", KLine, ChartValueSeriesStyle.Line, ChartColor.Red),
                new ChartValueSeries($"%D ({dSmoothPeriods})", DLine, ChartValueSeriesStyle.Line, ChartColor.StrongGray)
            ]
        };
    }
}