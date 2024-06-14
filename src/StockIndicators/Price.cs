namespace StockIndicators;

/// <inheritdoc/>
public record Price(DateTimeOffset Timestamp, double Open, double High, double Low, double Close, long Volume) : IPrice;
