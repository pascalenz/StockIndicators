using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class ExponentialMovingAverageTests
{
    private static readonly double[] prices =
    [
        22.27, 22.19, 22.08, 22.17, 22.18, 22.13, 22.23, 22.43, 22.24, 22.29,
        22.15, 22.39, 22.38, 22.61, 23.36, 24.05, 23.75, 23.83, 23.95, 23.63,
        23.82, 23.87, 23.65, 23.19, 23.10, 23.33, 22.68, 23.10, 22.40, 22.17
    ];

    [TestMethod]
    public void ExponentialMovingAverage()
    {
        var settings = new ExponentialMovingAverageSettings { Periods = 10 };
        var indicator = new ExponentialMovingAverage(IndicatorCapacity.Infinite, settings);
        indicator.Add(prices);

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("22.92", indicator.Last!.Value.ToString("F2"));
    }
}
