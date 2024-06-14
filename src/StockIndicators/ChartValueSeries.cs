namespace StockIndicators;

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
/// Describes the properties of a chart value series.
/// </summary>
/// <param name="Title">The series title.</param>
/// <param name="Values">The series values.</param>
/// <param name="Style">The series style.</param>
/// <param name="Color">The series color.</param>
public sealed record ChartValueSeries(string? Title, IReadOnlyList<double> Values, ChartValueSeriesStyle Style, ChartColor Color = ChartColor.Random);
