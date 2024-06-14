using System.ComponentModel.DataAnnotations;

namespace StockIndicators.Internal;

/// <summary>
/// A utility class to perform validation of indicator construction.
/// </summary>
public static class IndicatorValidator
{
    /// <summary>
    /// Checks that the capacity and settings are valid.
    /// </summary>
    /// <param name="capacity">The indicator capacity instance.</param>
    /// <param name="settings">The indicator settings instance.</param>
    public static void Verify(IndicatorCapacity capacity, object settings)
    {
        Verify(capacity);
        Verify(settings);
    }

    /// <summary>
    /// Checks that the capacity is valid.
    /// </summary>
    /// <param name="capacity">The indicator capacity instance.</param>
    public static void Verify(IndicatorCapacity capacity)
    {
        ArgumentNullException.ThrowIfNull(capacity);
    }

    /// <summary>
    /// Checks that the supplied indicator settings object is valid.
    /// </summary>
    /// <param name="settings">The indicator settings instance.</param>
    public static void Verify(object settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        Validator.ValidateObject(settings, new ValidationContext(settings), true);
    }
}
