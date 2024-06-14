namespace StockIndicators.Catalog;

/// <summary>
/// Provides properties that describe an indicator.
/// </summary>
/// <param name="IndicatorType">The CLR type of the indicator.</param>
/// <param name="IndicatorSettingsType">The CLR type of the indicator settings.</param>
/// <param name="Name">The name.</param>
/// <param name="Description">The description.</param>
/// <param name="Categories">The categories.</param>
/// <param name="Parameters">The parameters defined by the indicator.</param>
public sealed record IndicatorDescription(
    Type IndicatorType,
    Type? IndicatorSettingsType,
    string Name,
    string? Description,
    IReadOnlyCollection<string> Categories,
    IReadOnlyCollection<IndicatorParameter> Parameters);
