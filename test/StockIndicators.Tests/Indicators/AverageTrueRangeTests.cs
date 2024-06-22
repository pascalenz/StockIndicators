using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class AverageTrueRangeTests
{
    private readonly TestPrice[] prices =
    [
        new() { High = 48.70, Low = 47.79, Close = 48.16 },
        new() { High = 48.72, Low = 48.14, Close = 48.61 },
        new() { High = 48.90, Low = 48.39, Close = 48.75 },
        new() { High = 48.87, Low = 48.37, Close = 48.63 },
        new() { High = 48.82, Low = 48.24, Close = 48.74 },
        new() { High = 49.05, Low = 48.64, Close = 49.03 },
        new() { High = 49.20, Low = 48.94, Close = 49.07 },
        new() { High = 49.35, Low = 48.86, Close = 49.32 },
        new() { High = 49.92, Low = 49.50, Close = 49.91 },
        new() { High = 50.19, Low = 49.87, Close = 50.13 },
        new() { High = 50.12, Low = 49.20, Close = 49.53 },
        new() { High = 49.66, Low = 48.90, Close = 49.50 },
        new() { High = 49.88, Low = 49.43, Close = 49.75 },
        new() { High = 50.19, Low = 49.73, Close = 50.03 },
        new() { High = 50.36, Low = 49.26, Close = 50.31 },
        new() { High = 50.57, Low = 50.09, Close = 50.52 },
        new() { High = 50.65, Low = 50.30, Close = 50.41 },
        new() { High = 50.43, Low = 49.21, Close = 49.34 },
        new() { High = 49.63, Low = 48.98, Close = 49.37 },
        new() { High = 50.33, Low = 49.61, Close = 50.23 },
        new() { High = 50.29, Low = 49.20, Close = 49.24 },
        new() { High = 50.17, Low = 49.43, Close = 49.93 },
        new() { High = 49.32, Low = 48.08, Close = 48.43 },
        new() { High = 48.50, Low = 47.64, Close = 48.18 },
        new() { High = 48.32, Low = 41.55, Close = 46.57 },
        new() { High = 46.80, Low = 44.28, Close = 45.41 },
        new() { High = 47.80, Low = 47.31, Close = 47.77 },
        new() { High = 48.39, Low = 47.20, Close = 47.72 },
        new() { High = 48.66, Low = 47.90, Close = 48.62 },
        new() { High = 48.79, Low = 47.73, Close = 47.85 }
    ];

    [TestMethod]
    public void AverageTrueRange()
    {
        var indicator = new AverageTrueRange(IndicatorCapacity.Infinite);
        indicator.Add(prices);

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("1.3163", indicator.Values.Last().ToString("F4"));
    }
}
