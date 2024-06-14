namespace StockIndicators.Tests;

internal sealed class Price : IPrice
{
    public DateTimeOffset Timestamp { get; init; }

    public double Open { get; init; }

    public double High { get; init; }

    public double Low { get; init; }

    public double Close { get; init; }

    public long Volume { get; init; }
}
