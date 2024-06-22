using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class CorrelationCoefficientTests
{
    private static readonly double[] prices1 =
    [
        21.40, 21.71, 21.20, 21.34, 21.49, 21.39, 22.16, 22.53, 22.44, 22.75,
        23.23, 23.09, 22.85, 22.45, 22.48, 22.27, 22.37, 22.28, 23.06, 22.99
    ];

    private static readonly double[] prices2 =
    [
        54.83, 55.34, 54.38, 55.25, 56.07, 56.30, 57.05, 57.91, 58.20, 58.39,
        59.19, 59.03, 57.96, 57.52, 57.76, 57.09, 57.85, 57.54, 58.85, 58.60
    ];

    [TestMethod]
    public void CorrelationCoefficient()
    {
        var settings = new CorrelationCoefficientSettings { Periods = 20 };
        var indicator = new CorrelationCoefficient(IndicatorCapacity.Infinite, settings);

        for (var i = 0; i < 20; i++)
        {
            indicator.Add(new TestPrice { Close = prices1[i] }, new TestPrice { Close = prices2[i] });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("0.96", indicator.Values[indicator.Values.Count - 1].ToString("F2"));
    }
}
