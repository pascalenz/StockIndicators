using StockIndicators.AverageIndicators;
using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.PriceIndicators;

/// <summary>
/// Settings for the <see cref="TRIX"/> indicator.
/// </summary>
public sealed class TRIXSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods for the moving average.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods for the moving average.")]
    [Range(1, 100)]
    [DefaultValue(15)]
    public int Periods { get; init; } = 15;

    /// <summary>
    /// Gets or sets the number of look-back periods for the signal period.
    /// </summary>
    [DisplayName("Signal Periods")]
    [Description("The number of look-back periods for the signal period.")]
    [Range(1, 100)]
    [DefaultValue(9)]
    public int SignalPeriods { get; init; } = 9;
}

/// <summary>
/// TRIX is a momentum oscillator that displays the percent rate of change of a triple exponentially smoothed
/// moving average. With its triple smoothing, TRIX is designed to filter insignificant price movements.
/// Chartists can use TRIX to generate signals similar to MACD. A signal line can be applied to look for
/// signal line crossovers. A directional bias can be determined with the absolute level. Bullish and bearish
/// divergences can be used to anticipate reversals. (StockCharts.com)
/// </summary>
[DisplayName("TRIX")]
[Category(IndicatorCategory.MovingAverage)]
public sealed class TRIX : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly int signalPeriods;
    private readonly IAverageIndicator singleMA;
    private readonly IAverageIndicator doubleMA;
    private readonly IAverageIndicator tripleMA;
    private readonly IAverageIndicator signalMA;
    private double? last;
    private double? previous;

    /// <summary>
    /// Initializes a new instance of the <see cref="TRIX"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public TRIX(IndicatorCapacity capacity)
        : this(capacity, new TRIXSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="TRIX"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public TRIX(IndicatorCapacity capacity, TRIXSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;
        signalPeriods = settings.SignalPeriods;

        singleMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = periods });
        doubleMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = periods });
        tripleMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = periods });
        signalMA = new ExponentialMovingAverage(IndicatorCapacity.Minimum, new ExponentialMovingAverageSettings { Periods = signalPeriods });

        Values = capacity.CreateList<double>();
        Signal = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the indicator.
    /// </summary>
    public IReadOnlyList<double> Values { get; }

    /// <summary>
    /// Gets the values for the signal line.
    /// </summary>
    public IReadOnlyList<double> Signal { get; }

    /// <inheritdoc/>
    public bool IsReady => Values.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        singleMA.Add(price.Close);
        doubleMA.Add(singleMA.Last!.Value);
        tripleMA.Add(doubleMA.Last!.Value);

        previous = last;
        last = tripleMA.Last;

        if (previous.HasValue)
        {
            var trix = (last!.Value - previous.Value) / last.Value * 100d;
            signalMA.Add(trix);

            if (tripleMA.IsReady && signalMA.IsReady)
            {
                Values.Add(trix);
                Signal.Add(signalMA.Last!.Value);
            }
        }
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = "TRIX",
            ValueFormat = "N2",
            AutoGenerateGridLines = false,
            GridLines =
            [
                new ChartGridLine(0)
            ],
            ValueSeries =
            [
                new ChartValueSeries($"TRIX ({periods})", Values, ChartValueSeriesStyle.Line, ChartColor.Black),
                new ChartValueSeries($"Signal ({signalPeriods})", Signal, ChartValueSeriesStyle.Line, ChartColor.Red)
            ]
        };
    }
}