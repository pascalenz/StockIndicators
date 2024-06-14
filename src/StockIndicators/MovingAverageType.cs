using System.ComponentModel.DataAnnotations;

namespace StockIndicators;

/// <summary>
/// Specified the type of <see cref="IAverageIndicator"/> to use in an indicator.
/// </summary>
public enum MovingAverageType
{
    /// <summary>
    /// Simple Moving Average.
    /// </summary>
    [Display(Name = "Simple")]
    Simple = 0,

    /// <summary>
    /// Exponential Moving Average.
    /// </summary>
    [Display(Name = "Exponential")]
    Exponential = 1,

    /// <summary>
    /// Double Exponential Moving Average.
    /// </summary>
    [Display(Name = "Double Exponential")]
    DoubleExponential = 2,

    /// <summary>
    /// Triple Exponential Moving Average.
    /// </summary>
    [Display(Name = "Triple Exponential")]
    TripleExponential = 3,

    /// <summary>
    /// Weighted Moving Average.
    /// </summary>
    [Display(Name = "Weighted")]
    Weighted = 4,

    /// <summary>
    /// Hull Moving Average.
    /// </summary>
    [Display(Name = "Hull")]
    Hull = 5
}
