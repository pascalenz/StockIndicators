using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class MoneyFlowIndexTests
{
    private readonly Price[] prices =
    [
        new() { High = 24.83, Low = 24.32, Close = 24.75, Volume = 18730 },
        new() { High = 24.76, Low = 24.60, Close = 24.71, Volume = 12272 },
        new() { High = 25.16, Low = 24.78, Close = 25.04, Volume = 24691 },
        new() { High = 25.58, Low = 24.95, Close = 25.55, Volume = 18358 },
        new() { High = 25.68, Low = 24.81, Close = 25.07, Volume = 22964 },
        new() { High = 25.34, Low = 25.06, Close = 25.11, Volume = 15919 },
        new() { High = 25.29, Low = 24.85, Close = 24.89, Volume = 16067 },
        new() { High = 25.13, Low = 24.75, Close = 25.00, Volume = 16568 },
        new() { High = 25.28, Low = 24.93, Close = 25.05, Volume = 16019 },
        new() { High = 25.39, Low = 25.03, Close = 25.34, Volume = 9774 },
        new() { High = 25.54, Low = 25.05, Close = 25.06, Volume = 22573 },
        new() { High = 25.60, Low = 25.06, Close = 25.45, Volume = 12987 },
        new() { High = 25.74, Low = 25.54, Close = 25.56, Volume = 10907 },
        new() { High = 25.72, Low = 25.46, Close = 25.56, Volume = 5799 },
        new() { High = 25.67, Low = 25.29, Close = 25.41, Volume = 7395 },
        new() { High = 25.45, Low = 25.17, Close = 25.37, Volume = 5818 },
        new() { High = 25.32, Low = 24.92, Close = 25.04, Volume = 7165 },
        new() { High = 25.26, Low = 24.91, Close = 24.92, Volume = 5673 },
        new() { High = 25.04, Low = 24.83, Close = 24.88, Volume = 5625 },
        new() { High = 25.01, Low = 24.71, Close = 24.97, Volume = 5023 },
        new() { High = 25.31, Low = 25.03, Close = 25.05, Volume = 7457 },
        new() { High = 25.12, Low = 24.34, Close = 24.45, Volume = 11798 },
        new() { High = 24.69, Low = 24.27, Close = 24.57, Volume = 12366 },
        new() { High = 24.55, Low = 23.89, Close = 24.02, Volume = 13295 },
        new() { High = 24.27, Low = 23.78, Close = 23.88, Volume = 9257 },
        new() { High = 24.27, Low = 23.72, Close = 24.20, Volume = 9691 },
        new() { High = 24.60, Low = 24.20, Close = 24.28, Volume = 8870 },
        new() { High = 24.48, Low = 24.24, Close = 24.33, Volume = 7169 },
        new() { High = 24.56, Low = 23.43, Close = 24.44, Volume = 11356 },
        new() { High = 25.16, Low = 24.27, Close = 25.00, Volume = 13379 }
    ];

    [TestMethod]
    public void MoneyFlowIndex()
    {
        var indicator = new MoneyFlowIndex(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("30.84", indicator.Values.Last().ToString("F2"));
    }
}
