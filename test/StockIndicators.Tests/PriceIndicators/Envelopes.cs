using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class EnvelopesTests
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
    public void Envelopes()
    {
        var indicator = new Envelopes(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(new Price { Close = price });
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("90.40", indicator.Average.Last().ToString("F2"));
        Assert.AreEqual("92.66", indicator.UpperEnvelope.Last().ToString("F2"));
        Assert.AreEqual("88.14", indicator.LowerEnvelope.Last().ToString("F2"));
    }
}
