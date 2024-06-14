namespace StockIndicators.Catalog;

/// <summary>
/// Provides a description of an indicator parameter.
/// </summary>
/// <param name="Name">The name of the parameter.</param>
/// <param name="DisplayName">The display name of the parameter.</param>
/// <param name="Description">The description of the parameter.</param>
/// <param name="Scale">The number of decimal places.</param>
/// <param name="MinValue">The minimum value.</param>
/// <param name="MaxValue">The maximum value.</param>
/// <param name="DefaultValue">The default value.</param>
/// <param name="AllowedValueNames">The allowed values.</param>
public sealed record IndicatorParameter(
    string Name,
    string DisplayName,
    string? Description,
    int Scale,
    double MinValue,
    double MaxValue,
    double DefaultValue,
    IReadOnlyCollection<string> AllowedValueNames);
