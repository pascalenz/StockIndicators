using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class TRIXTests
{
    private static readonly double[] prices =
    [
        102.32, 105.54, 106.59, 107.39, 107.45, 109.08, 109.07, 109.10, 106.09, 106.72,
        107.90, 106.50, 108.88, 109.82, 110,97, 110.96, 110.24, 109.70, 109.68, 112.16,
        111.62, 112.37, 112.25, 111.70, 112.39, 111.78, 108.72, 108.05, 107.73, 107.68
    ];

    [TestMethod]
    public void TRIX()
    {
        var indicator = new TRIX(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(new TestPrice { Close = price });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("0.1909", indicator.Values.Last().ToString("F4"));
    }
}
