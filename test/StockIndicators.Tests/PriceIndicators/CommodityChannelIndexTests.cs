using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class CommodityChannelIndexTests
{
    private readonly Price[] prices =
    [
        new() { Open = 23.94, High = 24.20, Low = 23.85, Close = 23.89 },
        new() { Open = 23.85, High = 24.07, Low = 23.72, Close = 23.95 },
        new() { Open = 23.94, High = 24.04, Low = 23.64, Close = 23.67 },
        new() { Open = 23.73, High = 23.87, Low = 23.37, Close = 23.78 },
        new() { Open = 23.60, High = 23.67, Low = 23.46, Close = 23.50 },
        new() { Open = 23.46, High = 23.59, Low = 23.18, Close = 23.32 },
        new() { Open = 23.53, High = 23.80, Low = 23.40, Close = 23.75 },
        new() { Open = 23.73, High = 23.80, Low = 23.57, Close = 23.79 },
        new() { Open = 24.09, High = 24.30, Low = 24.05, Close = 24.14 },
        new() { Open = 23.95, High = 24.15, Low = 23.77, Close = 23.81 },
        new() { Open = 23.92, High = 24.05, Low = 23.60, Close = 23.78 },
        new() { Open = 24.04, High = 24.06, Low = 23.84, Close = 23.86 },
        new() { Open = 23.83, High = 23.88, Low = 23.64, Close = 23.70 },
        new() { Open = 24.05, High = 25.14, Low = 23.94, Close = 24.96 },
        new() { Open = 24.89, High = 25.20, Low = 24.74, Close = 24.88 },
        new() { Open = 24.95, High = 25.07, Low = 24.77, Close = 24.96 },
        new() { Open = 24.91, High = 25.22, Low = 24.90, Close = 25.18 },
        new() { Open = 25.24, High = 25.37, Low = 24.93, Close = 25.07 },
        new() { Open = 25.13, High = 25.36, Low = 24.96, Close = 25.27 },
        new() { Open = 25.26, High = 25.26, Low = 24.93, Close = 25.00 },
        new() { Open = 24.74, High = 24.82, Low = 24.21, Close = 24.46 },
        new() { Open = 24.36, High = 24.44, Low = 24.21, Close = 24.28 },
        new() { Open = 24.49, High = 24.65, Low = 24.43, Close = 24.62 },
        new() { Open = 24.70, High = 24.84, Low = 24.44, Close = 24.58 },
        new() { Open = 24.65, High = 24.75, Low = 24.20, Close = 24.53 },
        new() { Open = 24.48, High = 24.51, Low = 24.25, Close = 24.35 },
        new() { Open = 24.46, High = 24.68, Low = 24.21, Close = 24.34 },
        new() { Open = 24.62, High = 24.67, Low = 24.15, Close = 24.23 },
        new() { Open = 23.81, High = 23.84, Low = 23.63, Close = 23.76 },
        new() { Open = 23.91, High = 24.30, Low = 23.76, Close = 24.20 }
    ];

    [TestMethod]
    public void CommodityChannelIndex()
    {
        var indicator = new CommodityChannelIndex(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("-73.18", indicator.Values.Last().ToString("F2"));
    }
}
