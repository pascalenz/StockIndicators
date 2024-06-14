namespace StockIndicators;

/// <summary>
/// Describes the properties of a chart section where the background is highlighted.
/// </summary>
/// <param name="StartPosition">The start position value on the vertical axis. </param>
/// <param name="EndPosition">The end position value on the vertical axis.</param>
public sealed record ChartHighlight(double StartPosition, double EndPosition);
