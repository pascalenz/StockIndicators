namespace StockIndicators;

/// <summary>
/// Describes the properties of a chart.
/// </summary>
public sealed class Chart
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Chart"/> class.
    /// </summary>
    public Chart()
    {
        Title = string.Empty;
        ValueFormat = "N2";
        AutoGenerateGridLines = true;
        GridLines = [];
        Highlights = [];
        PriceSeries = [];
        ValueSeries = [];
        Width = 1000;
        Height = 300;
    }

    /// <summary>
    /// Gets or sets the chart title.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Gets or sets the string format for the values.
    /// </summary>
    public string ValueFormat { get; init; }

    /// <summary>
    /// Gets or sets the minimum value to display in the vertical axis.
    /// </summary>
    public double? MinValue { get; init; }

    /// <summary>
    /// Gets or sets the maximum value to display in the vertical axis.
    /// </summary>
    public double? MaxValue { get; init; }

    /// <summary>
    /// Gets or sets a value indicating whether grid lines should be automatically generated.
    /// </summary>
    public bool AutoGenerateGridLines { get; init; }

    /// <summary>
    /// Gets or sets the chart grid lines.
    /// </summary>
    public IReadOnlyCollection<ChartGridLine> GridLines { get; init; }

    /// <summary>
    /// Gets or sets the highlighted sections of the chart.
    /// </summary>
    public IReadOnlyCollection<ChartHighlight> Highlights { get; init; }

    /// <summary>
    /// Gets or sets the value serieses.
    /// </summary>
    public IReadOnlyCollection<ChartPriceSeries> PriceSeries { get; init; }

    /// <summary>
    /// Gets or sets the value serieses.
    /// </summary>
    public IReadOnlyCollection<ChartValueSeries> ValueSeries { get; init; }

    /// <summary>
    /// Gets or sets the width of the chart.
    /// </summary>
    public double Width { get; init; }

    /// <summary>
    /// Gets or sets the height of the chart.
    /// </summary>
    public double Height { get; init; }
}
