namespace StockIndicators;

/// <summary>
/// Defines the available colors for chart elements.
/// </summary>
public enum ChartColor
{
    /// <summary>
    /// Random color
    /// </summary>
    Random,

    /// <summary>
    /// Positive value color (usually green)
    /// </summary>
    PositiveValue,

    /// <summary>
    /// Negative value color (usually red)
    /// </summary>
    NegativeValue,

    /// <summary>
    /// Light Gray
    /// </summary>
    LightGray,

    /// <summary>
    /// Strong Gray
    /// </summary>
    StrongGray,

    /// <summary>
    /// Black
    /// </summary>
    Black,

    /// <summary>
    /// Red
    /// </summary>
    Red,

    /// <summary>
    /// Neutral
    /// </summary>
    Neutral
}

/// <summary>
/// Defines the value series drawing style.
/// </summary>
public enum ChartValueSeriesStyle
{
    /// <summary>
    /// The series should be presented as a straight line.
    /// </summary>
    Line,

    /// <summary>
    /// The series should be presented as a dots.
    /// </summary>
    Dot,

    /// <summary>
    /// The series should be presented as bars.
    /// </summary>
    Bar,

    /// <summary>
    /// The series should be presented as an area .
    /// </summary>
    Area
}

/// <summary>
/// Describes the properties of a chart grid line.
/// </summary>
/// <param name="Position">The line position value on the vertical axis.</param>
public sealed record ChartGridLine(double Position);

/// <summary>
/// Describes the properties of a chart section where the background is highlighted.
/// </summary>
/// <param name="StartPosition">The start position value on the vertical axis. </param>
/// <param name="EndPosition">The end position value on the vertical axis.</param>
public sealed record ChartHighlight(double StartPosition, double EndPosition);

/// <summary>
/// Describes the properties of a chart price series.
/// </summary>
/// <param name="Title">The series title.</param>
/// <param name="Values">The series values.</param>
/// <param name="Color">The series color.</param>
public sealed record ChartPriceSeries(string? Title, IReadOnlyList<IPrice> Values, ChartColor Color = ChartColor.Random);

/// <summary>
/// Describes the properties of a chart value series.
/// </summary>
/// <param name="Title">The series title.</param>
/// <param name="Values">The series values.</param>
/// <param name="Style">The series style.</param>
/// <param name="Color">The series color.</param>
public sealed record ChartValueSeries(string? Title, IReadOnlyList<double> Values, ChartValueSeriesStyle Style, ChartColor Color = ChartColor.Random);

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
