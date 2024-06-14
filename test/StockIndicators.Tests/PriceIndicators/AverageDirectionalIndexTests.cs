using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class AverageDirectionalIndexTests
{
    private readonly Price[] prices =
    [
        new() { High = 30.20, Low = 29.41, Close = 29.87 },
        new() { High = 30.28, Low = 29.32, Close = 30.24 },
        new() { High = 30.45, Low = 29.96, Close = 30.10 },
        new() { High = 29.35, Low = 28.74, Close = 28.90 },
        new() { High = 29.35, Low = 28.56, Close = 28.92 },
        new() { High = 29.29, Low = 28.41, Close = 28.48 },
        new() { High = 28.83, Low = 28.08, Close = 28.56 },
        new() { High = 28.73, Low = 27.43, Close = 27.56 },
        new() { High = 28.67, Low = 27.66, Close = 28.47 },
        new() { High = 28.85, Low = 27.83, Close = 28.28 },
        new() { High = 28.64, Low = 27.40, Close = 27.49 },
        new() { High = 27.68, Low = 27.09, Close = 27.23 },
        new() { High = 27.21, Low = 26.18, Close = 26.35 },
        new() { High = 26.87, Low = 26.13, Close = 26.33 },
        new() { High = 27.41, Low = 26.63, Close = 27.03 },
        new() { High = 26.94, Low = 26.13, Close = 26.22 },
        new() { High = 26.52, Low = 25.43, Close = 26.01 },
        new() { High = 26.52, Low = 25.35, Close = 25.46 },
        new() { High = 27.09, Low = 25.88, Close = 27.03 },
        new() { High = 27.69, Low = 26.96, Close = 27.45 },
        new() { High = 28.45, Low = 27.14, Close = 28.36 },
        new() { High = 28.53, Low = 28.01, Close = 28.43 },
        new() { High = 28.67, Low = 27.88, Close = 27.95 },
        new() { High = 29.01, Low = 27.99, Close = 29.01 },
        new() { High = 29.87, Low = 28.76, Close = 29.38 },
        new() { High = 29.80, Low = 29.14, Close = 29.36 },
        new() { High = 29.75, Low = 28.71, Close = 28.91 },
        new() { High = 30.65, Low = 28.93, Close = 30.61 },
        new() { High = 30.60, Low = 30.03, Close = 30.05 },
        new() { High = 30.76, Low = 29.39, Close = 30.19 },
        new() { High = 31.17, Low = 30.14, Close = 31.12 },
        new() { High = 30.89, Low = 30.43, Close = 30.54 },
        new() { High = 30.04, Low = 29.35, Close = 29.78 },
        new() { High = 30.66, Low = 29.99, Close = 30.04 },
        new() { High = 30.60, Low = 29.52, Close = 30.49 },
        new() { High = 31.97, Low = 30.94, Close = 31.47 },
        new() { High = 32.10, Low = 31.54, Close = 32.05 },
        new() { High = 32.03, Low = 31.36, Close = 31.97 },
        new() { High = 31.63, Low = 30.92, Close = 31.13 },
        new() { High = 31.85, Low = 31.20, Close = 31.66 }
    ];

    [TestMethod]
    public void AverageDirectionalIndex()
    {
        var indicator = new AverageDirectionalIndex(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("18.73", indicator.Values.Last().ToString("F2"));
    }
}
