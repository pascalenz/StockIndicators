using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class StochasticTests
{
    private readonly Price[] prices =
    [
        new() { High = 127.01, Low = 125.36 },
        new() { High = 127.62, Low = 126.16 },
        new() { High = 126.59, Low = 124.93 },
        new() { High = 127.35, Low = 126.09 },
        new() { High = 128.17, Low = 126.82 },
        new() { High = 128.43, Low = 126.48 },
        new() { High = 127.37, Low = 126.03 },
        new() { High = 126.42, Low = 124.83 },
        new() { High = 126.90, Low = 126.39 },
        new() { High = 126.85, Low = 125.72 },
        new() { High = 125.65, Low = 124.56 },
        new() { High = 125.72, Low = 124.57 },
        new() { High = 127.16, Low = 125.07 },
        new() { High = 127.72, Low = 126.86, Close = 127.29 },
        new() { High = 127.69, Low = 126.63, Close = 127.18 },
        new() { High = 128.22, Low = 126.80, Close = 128.01 },
        new() { High = 128.27, Low = 126.71, Close = 127.11 },
        new() { High = 128.09, Low = 126.80, Close = 127.73 },
        new() { High = 128.27, Low = 126.13, Close = 127.06 },
        new() { High = 127.74, Low = 125.92, Close = 127.33 },
        new() { High = 128.77, Low = 126.99, Close = 128.71 },
        new() { High = 129.29, Low = 127.81, Close = 127.87 },
        new() { High = 130.06, Low = 128.47, Close = 128.58 },
        new() { High = 129.12, Low = 128.06, Close = 128.60 },
        new() { High = 129.29, Low = 127.61, Close = 127.93 },
        new() { High = 128.47, Low = 127.60, Close = 128.11 },
        new() { High = 128.09, Low = 127.00, Close = 127.60 },
        new() { High = 128.65, Low = 126.90, Close = 127.60 },
        new() { High = 129.14, Low = 127.49, Close = 128.69 },
        new() { High = 128.64, Low = 127.40, Close = 128.27 }
    ];

    [TestMethod]
    public void Stochastic()
    {
        var indicator = new Stochastic(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("54.75", indicator.KLine.Last().ToString("F2"));
    }
}
