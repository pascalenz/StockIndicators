using StockIndicators.Indicators;

namespace StockIndicators.Internal;

/// <summary>
/// Creates instances of <see cref="IAverageIndicator"/>.
/// </summary>
internal static class MovingAverageFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IAverageIndicator"/>.
    /// </summary>
    /// <param name="type">The type of the average indicator to create.</param>
    /// <param name="periods">The number of look-back periods for the fast moving average.</param>
    /// <returns>The instance of <see cref="IAverageIndicator"/>.</returns>
    public static IAverageIndicator Create(MovingAverageType type, int periods) => type switch
    {
        MovingAverageType.Simple => new SimpleMovingAverage(IndicatorCapacity.Minimum, new() { Periods = periods }),
        MovingAverageType.Exponential => new ExponentialMovingAverage(IndicatorCapacity.Minimum, new() { Periods = periods }),
        MovingAverageType.DoubleExponential => new DoubleExponentialMovingAverage(IndicatorCapacity.Minimum, new() { Periods = periods }),
        MovingAverageType.TripleExponential => new TripleExponentialMovingAverage(IndicatorCapacity.Minimum, new() { Periods = periods }),
        MovingAverageType.Weighted => new WeightedMovingAverage(IndicatorCapacity.Minimum, new() { Periods = periods }),
        MovingAverageType.Hull => new HullMovingAverage(IndicatorCapacity.Minimum, new() { Periods = periods }),
        _ => throw new NotSupportedException("Unsupported average indicator type."),
    };
}
