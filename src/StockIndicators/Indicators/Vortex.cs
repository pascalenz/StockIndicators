using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="Vortex"/> indicator.
/// </summary>
public sealed class VortexSettings
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
/// The Vortex Indicator (VTX) consists of two oscillators that capture positive and negative trend movement.
/// A bullish signal triggers when the positive trend indicator crosses above the negative trend indicator or a key level.
/// A bearish signal triggers when the negative trend indicator crosses above the positive trend indicator or a key level.
/// The Vortex Indicator is either above or below these levels, which means it always has a clear bullish or bearish bias.
/// (StockCharts.com)
/// </summary>
[DisplayName("VTX")]
[Description("Vortex Indicator")]
[Category(IndicatorCategory.Trend)]
public sealed class Vortex : IPriceIndicator, IChartProvider
{
    private readonly int periods;
    private readonly AnalysisWindow tr, plusVM, minusVM;
    private IPrice? previous;

    /// <summary>
    /// Initializes a new instance of the <see cref="Vortex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public Vortex(IndicatorCapacity capacity)
        : this(capacity, new VortexSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Vortex"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public Vortex(IndicatorCapacity capacity, VortexSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;

        tr = new AnalysisWindow(periods, true, false);
        plusVM = new AnalysisWindow(periods, true, false);
        minusVM = new AnalysisWindow(periods, true, false);

        Up = capacity.CreateList<double>();
        Down = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the up-trend indicator.
    /// </summary>
    public IReadOnlyList<double> Up { get; }

    /// <summary>
    /// Gets the values for the down-trend indicator.
    /// </summary>
    public IReadOnlyList<double> Down { get; }

    /// <inheritdoc/>
    public bool IsReady => Up.Count > 0 && Down.Count > 0;

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

        // Calculate +VM1
        var plusVM1 = Math.Abs(price.High - previous.Low);
        plusVM.Add(plusVM1);

        // Calculate -VM1
        var minusVM1 = Math.Abs(price.Low - previous.High);
        minusVM.Add(minusVM1);

        if (tr.IsFilled && plusVM.IsFilled && minusVM.IsFilled)
        {
            // Caculate TR14, +VM14, -VM14
            var tr14 = tr.Sum;
            var plusVM14 = plusVM.Sum;
            var minusVM14 = minusVM.Sum;

            // Calculate +VI14, -VI14
            Up.Add(plusVM14 / tr14);
            Down.Add(minusVM14 / tr14);
        }

        previous = price;
    }

    /// <inheritdoc/>
    public Chart CreateChart()
    {
        return new Chart
        {
            Title = $"VTX ({periods})",
            ValueFormat = "N2",
            ValueSeries =
            [
                new ChartValueSeries("Up", Up, ChartValueSeriesStyle.Line, ChartColor.PositiveValue),
                new ChartValueSeries("Down", Down, ChartValueSeriesStyle.Line, ChartColor.NegativeValue),
            ]
        };
    }
}