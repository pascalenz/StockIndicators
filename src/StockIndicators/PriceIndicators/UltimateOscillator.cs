using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="UltimateOscillator"/> indicator.
/// </summary>
public sealed class UltimateOscillatorSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the short period.
    /// </summary>
    [DisplayName("Short Periods")]
    [Description("The number of look-back periods for the short period.")]
    [Range(1, 100)]
    [DefaultValue(7)]
    public int ShortPeriods { get; init; } = 7;

    /// <summary>
    /// Gets or sets the number of look-back periods for the medium period.
    /// </summary>
    [DisplayName("Medium Periods")]
    [Description("The number of look-back periods for the medium period.")]
    [Range(1, 100)]
    [DefaultValue(14)]
    public int MediumPeriods { get; init; } = 14;

    /// <summary>
    /// Gets or sets the number of look-back periods for the long period.
    /// </summary>
    [DisplayName("Long Periods")]
    [Description("The number of look-back periods for the long period.")]
    [Range(1, 100)]
    [DefaultValue(28)]
    public int LongPeriods { get; init; } = 28;
}

/// <summary>
/// The Ultimate Oscillator is a momentum oscillator designed to capture momentum across three different time frames.
/// The multiple time frame objective seeks to avoid the pitfalls of other oscillators. Many momentum oscillators
/// surge at the beginning of a strong advance and then form bearish divergence as the advance continues.
/// This is because they are stuck with one time frame. The Ultimate Oscillator attempts to correct this fault by
/// incorporating longer time frames into the basic formula. Williams identified a buy signal a based on a bullish
/// divergence and a sell signal based on a bearish divergence. (StockCharts.com)
/// </summary>
[DisplayName("Ultimate Oscillator")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class UltimateOscillator : IPriceIndicator, IChartProvider
{
    private readonly int shortPeriods, mediumPeriods, longPeriods;
    private readonly AnalysisWindow shortBP, mediumBP, longBP;
    private readonly AnalysisWindow shortTR, mediumTR, longTR;
    private IPrice? previous;

    /// <summary>
    /// Initializes a new instance of the <see cref="UltimateOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public UltimateOscillator(IndicatorCapacity capacity)
        : this(capacity, new UltimateOscillatorSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UltimateOscillator"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public UltimateOscillator(IndicatorCapacity capacity, UltimateOscillatorSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        if (settings.ShortPeriods >= settings.MediumPeriods)
            throw new ArgumentException("Argument 'shortPeriods' must be less than 'mediumPeriods'.");

        if (settings.ShortPeriods >= settings.MediumPeriods)
            throw new ArgumentException("Argument 'mediumPeriods' must be less than 'longPeriods'.");

        shortPeriods = settings.ShortPeriods;
        mediumPeriods = settings.MediumPeriods;
        longPeriods = settings.LongPeriods;

        shortBP = new AnalysisWindow(shortPeriods, true, false);
        mediumBP = new AnalysisWindow(mediumPeriods, true, false);
        longBP = new AnalysisWindow(longPeriods, true, false);
        shortTR = new AnalysisWindow(shortPeriods, true, false);
        mediumTR = new AnalysisWindow(mediumPeriods, true, false);
        longTR = new AnalysisWindow(longPeriods, true, false);

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
        if (previous == null)
        {
            previous = price;
            return;
        }

        var bp = price.Close - Math.Min(price.Low, previous.Close);
        shortBP.Add(bp);
        mediumBP.Add(bp);
        longBP.Add(bp);

        var tr = Math.Max(price.High, previous.Close) - Math.Min(price.Low, previous.Close);
        shortTR.Add(tr);
        mediumTR.Add(tr);
        longTR.Add(tr);

        if (longBP.IsFilled && longTR.IsFilled)
        {
            var shortAverage = shortBP.Sum / shortTR.Sum;
            var mediumAverage = mediumBP.Sum / mediumTR.Sum;
            var longAverage = longBP.Sum / longTR.Sum;
            var result = 100 * ((4 * shortAverage) + (2 * mediumAverage) + longAverage) / 7;
            Values.Add(result);
        }

        previous = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"Ultimate Oscillator ({shortPeriods}, {mediumPeriods}, {longPeriods})",
            ValueFormat = "N0",
            MinValue = 0,
            MaxValue = 100,
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(30),
                new ChartGridLine(50),
                new ChartGridLine(70)
            ],
            Highlights =
            [
                new ChartHighlight(0, 30),
                new ChartHighlight(70, 100)
            ],
            ValueSeries =
            [
                new ChartValueSeries(null, Values, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}