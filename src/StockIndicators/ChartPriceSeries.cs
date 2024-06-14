namespace StockIndicators;

/// <summary>
/// Describes the properties of a chart price series.
/// </summary>
/// <param name="Title">The series title.</param>
/// <param name="Values">The series values.</param>
/// <param name="Color">The series color.</param>
public sealed record ChartPriceSeries(string? Title, IReadOnlyList<IPrice> Values, ChartColor Color = ChartColor.Random);
