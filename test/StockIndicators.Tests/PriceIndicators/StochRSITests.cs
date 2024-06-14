using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class StochRSITests
{
    private static readonly double[] prices =
    [
        54.09, 59.90, 58.20, 59.76, 52.35, 52.82, 56.94, 57.47, 55.26, 57.51,
        54.80, 51.47, 56.16, 58.34, 56.02, 60.22, 56.75, 57.38, 50.23, 57.06,
        61.51, 63.69, 66.22, 69.16, 70.73, 67.79, 68.82, 62.38, 67.59, 67.59
    ];

    [TestMethod]
    public void StochRSI()
    {
        var indicator = new StochRSI(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(new Price { Close = price });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("0.66", indicator.Values.Last().ToString("F2"));
    }
}
