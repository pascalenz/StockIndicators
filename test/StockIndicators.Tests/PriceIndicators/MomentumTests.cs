using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class MomentumTests
{
    private static readonly double[] prices =
    [
        52.00, 51.00, 51.50, 48.50, 53.00, 53.50, 53.50, 54.00, 54.00, 55.00
    ];

    [TestMethod]
    public void Momentum()
    {
        var settings = new MomentumSettings { Periods = 5 };
        var indicator = new Momentum(IndicatorCapacity.Infinite, settings);

        foreach (var price in prices)
        {
            indicator.Add(new Price { Close = price });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("2.00", indicator.Values.Last().ToString("F2"));
    }
}
