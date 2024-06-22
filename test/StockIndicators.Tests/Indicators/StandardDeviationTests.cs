using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class StandardDeviationTests
{
    private static readonly double[] prices =
    [
        52.22, 52.78, 53.02, 53.67, 53.67, 53.74, 53.45, 53.72,
        53.39, 52.51, 52.32, 51.45, 51.60, 52.43, 52.47, 52.91,
        52.07, 53.12, 52.77, 52.73, 52.09, 53.19, 53.73, 53.87,
        53.85, 53.88, 54.08, 54.14, 54.50, 54.30, 54.40, 54.16
    ];

    [TestMethod]
    public void StandardDeviation()
    {
        var settings = new StandardDeviationSettings { Periods = 10 };
        var indicator = new StandardDeviation(IndicatorCapacity.Infinite, settings);
        indicator.Add(prices.Select(price => new TestPrice { Close = price }));

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("0.74", indicator.Values.Last().ToString("F2"));
    }
}
