using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class UltimateOscillatorTests
{
    private readonly TestPrice[] prices =
    [
        new() { High = 57.93, Low = 56.52, Close = 57.57 },
        new() { High = 58.46, Low = 57.07, Close = 57.67 },
        new() { High = 57.76, Low = 56.44, Close = 56.92 },
        new() { High = 59.88, Low = 57.53, Close = 58.47 },
        new() { High = 59.02, Low = 57.58, Close = 58.74 },
        new() { High = 60.18, Low = 57.89, Close = 60.01 },
        new() { High = 60.29, Low = 58.01, Close = 58.45 },
        new() { High = 59.86, Low = 58.43, Close = 59.18 },
        new() { High = 59.78, Low = 58.45, Close = 58.67 },
        new() { High = 59.73, Low = 58.58, Close = 58.87 },
        new() { High = 59.60, Low = 58.54, Close = 59.30 },
        new() { High = 62.96, Low = 59.62, Close = 62.57 },
        new() { High = 62.27, Low = 61.36, Close = 62.02 },
        new() { High = 63.06, Low = 61.25, Close = 62.05 },
        new() { High = 63.74, Low = 62.19, Close = 62.52 },
        new() { High = 62.74, Low = 61.02, Close = 62.37 },
        new() { High = 63.48, Low = 61.57, Close = 63.40 },
        new() { High = 63.23, Low = 60.79, Close = 61.90 },
        new() { High = 62.14, Low = 60.34, Close = 60.54 },
        new() { High = 60.50, Low = 58.20, Close = 59.09 },
        new() { High = 59.89, Low = 58.91, Close = 59.01 },
        new() { High = 60.32, Low = 59.09, Close = 59.39 },
        new() { High = 59.71, Low = 58.59, Close = 59.21 },
        new() { High = 62.22, Low = 59.44, Close = 59.66 },
        new() { High = 59.74, Low = 57.33, Close = 59.07 },
        new() { High = 59.94, Low = 59.11, Close = 59.90 },
        new() { High = 59.65, Low = 58.87, Close = 59.29 },
        new() { High = 59.37, Low = 58.24, Close = 59.12 },
        new() { High = 60.21, Low = 58.26, Close = 59.68 },
        new() { High = 61.70, Low = 60.58, Close = 61.48 }
    ];

    [TestMethod]
    public void UltimateOscillator()
    {
        var indicator = new UltimateOscillator(IndicatorCapacity.Infinite);
        indicator.Add(prices);

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("57.21", indicator.Values.Last().ToString("F2"));
    }
}
