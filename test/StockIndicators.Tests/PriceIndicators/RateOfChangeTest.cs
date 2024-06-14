using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class RateOfChangeTests
{
    private static readonly double[] prices =
    [
        11045.27, 11167.32, 11008.61, 11151.83, 10926.77, 10868.12, 10520.32, 10380.43, 10785.14, 10748.26,
        10896.91, 10782.95, 10620.16, 10625.83, 10510.95, 10444.37, 10068.01, 10193.39, 10066.57, 10043.75
    ];

    [TestMethod]
    public void RateOfChange()
    {
        var indicator = new RateOfChange(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(new Price { Close = price });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("-3.24", indicator.Values.Last().ToString("F2"));
    }
}
