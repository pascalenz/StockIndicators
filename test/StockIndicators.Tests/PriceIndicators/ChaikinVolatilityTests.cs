using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class ChaikinVolatilityTests
{
    private readonly Price[] prices =
    [
        new() { High = 62.34, Low = 61.37 },
        new() { High = 62.05, Low = 60.69 },
        new() { High = 62.27, Low = 60.10 },
        new() { High = 60.79, Low = 58.61 },
        new() { High = 59.93, Low = 58.71 },
        new() { High = 61.75, Low = 59.86 },
        new() { High = 60.00, Low = 57.97 },
        new() { High = 59.00, Low = 58.02 },
        new() { High = 59.07, Low = 57.48 },
        new() { High = 59.22, Low = 58.30 },
        new() { High = 58.75, Low = 57.83 },
        new() { High = 58.65, Low = 57.86 },
        new() { High = 58.47, Low = 57.91 },
        new() { High = 58.25, Low = 57.83 },
        new() { High = 58.35, Low = 57.53 },
        new() { High = 59.86, Low = 58.58 },
        new() { High = 59.53, Low = 58.30 },
        new() { High = 62.10, Low = 58.53 },
        new() { High = 62.16, Low = 59.80 },
        new() { High = 62.67, Low = 60.93 },
        new() { High = 62.38, Low = 60.15 },
        new() { High = 63.73, Low = 62.26 },
        new() { High = 63.85, Low = 63.00 },
        new() { High = 66.15, Low = 63.58 },
        new() { High = 65.34, Low = 64.07 },
        new() { High = 66.48, Low = 65.20 },
        new() { High = 65.23, Low = 63.21 },
        new() { High = 63.40, Low = 61.88 },
        new() { High = 63.18, Low = 61.11 },
        new() { High = 62.70, Low = 61.25 }
    ];

    [TestMethod]
    public void ChaikinVolatility()
    {
        var indicator = new ChaikinVolatility(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("-0.224", indicator.Values.Last().ToString("F3"));
    }
}
