using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class BollingerBandTests
{
    private static readonly double[] prices =
    [
        86.16, 89.09, 88.78, 90.32, 89.07, 91.15, 89.44, 89.18, 86.93, 87.68,
        86.96, 89.43, 89.32, 88.72, 87.45, 87.26, 89.50, 87.90, 89.13, 90.70,
        92.90, 92.98, 91.80, 92.66, 92.68, 92.30, 92.77, 92.54, 92.95, 93.20,
        91.07, 89.83, 89.74, 90.40, 90.74, 88.02, 88.09, 88.84, 90.78, 90.54,
        91.39, 90.65
    ];

    [TestMethod]
    public void BollingerBand()
    {
        var indicator = new BollingerBand(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(new TestPrice { Close = price });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("91.05", indicator.MiddleBand.Last().ToString("F2"));
        Assert.AreEqual("95.22", indicator.UpperBand.Last().ToString("F2"));
        Assert.AreEqual("86.88", indicator.LowerBand.Last().ToString("F2"));
    }
}
