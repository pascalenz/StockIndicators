using StockIndicators.Internal;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Indicators;

/// <summary>
/// Settings for the <see cref="DonchianChannel"/> indicator.
/// </summary>
public sealed class DonchianChannelSettings
{
    /// <summary>
    /// Gets or sets the number of look-back periods.
    /// </summary>
    [DisplayName("Periods")]
    [Description("The number of look-back periods.")]
    [Range(1, 200)]
    [DefaultValue(20)]
    public int Periods { get; init; } = 20;
}

/// <summary>
/// Donchian channels are price channel studies. Although the application was intended mostly for the commodity futures market,
/// these channels can also be widely used in the FX market to capture short-term bursts or longer-term trends.
/// Created by Richard Donchian, considered to be the father of successful trend following, the study contains the underlying currency
/// fluctuations and aims to place profitable entries upon the start of a new trend through penetration of either the lower or upper band.
/// (barchart.com)
/// </summary>
[DisplayName("Donchian Channel")]
[Category(IndicatorCategory.Trend)]
public sealed class DonchianChannel : IPriceIndicator, IChartValueSeriesProvider
{
    private readonly int periods;
    private readonly AnalysisWindow highs;
    private readonly AnalysisWindow lows;

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianChannel"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    public DonchianChannel(IndicatorCapacity capacity)
        : this(capacity, new DonchianChannelSettings())
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DonchianChannel"/> class.
    /// </summary>
    /// <param name="capacity">The maximum number of values to keep in the output list.</param>
    /// <param name="settings">The indicator settings.</param>
    public DonchianChannel(IndicatorCapacity capacity, DonchianChannelSettings settings)
    {
        IndicatorValidator.Verify(capacity, settings);

        periods = settings.Periods;

        highs = new AnalysisWindow(periods, false, true);
        lows = new AnalysisWindow(periods, false, true);

        UpperLine = capacity.CreateList<double>();
        CenterLine = capacity.CreateList<double>();
        LowerLine = capacity.CreateList<double>();
    }

    /// <summary>
    /// Gets the values for the upper envelope.
    /// </summary>
    public IReadOnlyList<double> UpperLine { get; }

    /// <summary>
    /// Gets the values for the average.
    /// </summary>
    public IReadOnlyList<double> CenterLine { get; }

    /// <summary>
    /// Gets the values for the lower envelope.
    /// </summary>
    public IReadOnlyList<double> LowerLine { get; }

    /// <inheritdoc/>
    public bool IsReady => CenterLine.Count > 0;

    /// <inheritdoc/>
    public void Add(IPrice price)
    {
        highs.Add(price.High);
        lows.Add(price.Low);

        if (highs.IsFilled && lows.IsFilled)
        {
            var high = highs.Max;
            var low = lows.Min;
            CenterLine.Add((high + low) / 2);
            UpperLine.Add(high);
            LowerLine.Add(low);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<ChartValueSeries> CreateChartValueSeries()
    {
        var title = $"Donchian Channel ({periods})";

        return
        [
            new ChartValueSeries(null, UpperLine, ChartValueSeriesStyle.Line),
            new ChartValueSeries(title, CenterLine, ChartValueSeriesStyle.Line),
            new ChartValueSeries(null, LowerLine, ChartValueSeriesStyle.Line)
        ];
    }
}