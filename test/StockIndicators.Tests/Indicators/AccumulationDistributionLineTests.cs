using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class AccumulationDistributionLineTests
{
    private readonly TestPrice[] prices =
    [
        new() { High = 62.34, Low = 61.37, Close = 62.15, Volume = 7849 },
        new() { High = 62.05, Low = 60.69, Close = 60.81, Volume = 11692 },
        new() { High = 62.27, Low = 60.10, Close = 60.45, Volume = 10575 },
        new() { High = 60.79, Low = 58.61, Close = 59.18, Volume = 13059 },
        new() { High = 59.93, Low = 58.71, Close = 59.24, Volume = 20734 },
        new() { High = 61.75, Low = 59.86, Close = 60.20, Volume = 29630 },
        new() { High = 60.00, Low = 57.97, Close = 58.48, Volume = 17705 },
        new() { High = 59.00, Low = 58.02, Close = 58.24, Volume = 7259 },
        new() { High = 59.07, Low = 57.48, Close = 58.69, Volume = 10475 },
        new() { High = 59.22, Low = 58.30, Close = 58.65, Volume = 5204 },
        new() { High = 58.75, Low = 57.83, Close = 58.47, Volume = 3423 },
        new() { High = 58.65, Low = 57.86, Close = 58.02, Volume = 3962 },
        new() { High = 58.47, Low = 57.91, Close = 58.17, Volume = 4096 },
        new() { High = 58.25, Low = 57.83, Close = 58.07, Volume = 3766 },
        new() { High = 58.35, Low = 57.53, Close = 58.13, Volume = 4239 },
        new() { High = 59.86, Low = 58.58, Close = 58.94, Volume = 8040 },
        new() { High = 59.53, Low = 58.30, Close = 59.10, Volume = 6957 },
        new() { High = 62.10, Low = 58.53, Close = 61.92, Volume = 18172 },
        new() { High = 62.16, Low = 59.80, Close = 61.37, Volume = 22226 },
        new() { High = 62.67, Low = 60.93, Close = 61.68, Volume = 14614 },
        new() { High = 62.38, Low = 60.15, Close = 62.09, Volume = 12320 },
        new() { High = 63.73, Low = 62.26, Close = 62.89, Volume = 15008 },
        new() { High = 63.85, Low = 63.00, Close = 63.53, Volume = 8880 },
        new() { High = 66.15, Low = 63.58, Close = 64.01, Volume = 22694 },
        new() { High = 65.34, Low = 64.07, Close = 64.77, Volume = 10192 },
        new() { High = 66.48, Low = 65.20, Close = 65.22, Volume = 10074 },
        new() { High = 65.23, Low = 63.21, Close = 63.28, Volume = 9412 },
        new() { High = 63.40, Low = 61.88, Close = 62.40, Volume = 10392 },
        new() { High = 63.18, Low = 61.11, Close = 61.55, Volume = 8927 },
        new() { High = 62.70, Low = 61.25, Close = 62.69, Volume = 7460 }
    ];

    [TestMethod]
    public void AccumulationDistributionLine()
    {
        var indicator = new AccumulationDistributionLine(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("-51551", indicator.Values.Last().ToString("F0"));
    }
}
