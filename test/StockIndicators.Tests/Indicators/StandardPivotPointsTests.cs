using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class StandardPivotPointsTests
{
    private readonly TestPrice[] prices =
    [
        new() { Timestamp = DateTimeOffset.Now.Date.AddDays(0), High = 62.34, Low = 61.37, Close = 62.15 },
        new() { Timestamp = DateTimeOffset.Now.Date.AddDays(1), High = 62.05, Low = 60.69, Close = 60.81 }
    ];

    [TestMethod]
    public void PivotPoints()
    {
        var indicator = new StandardPivotPoints(IndicatorCapacity.Infinite);
        indicator.Add(prices);

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("63.5067", indicator.Resistance3.Last().ToString("F4"));
        Assert.AreEqual("62.9233", indicator.Resistance2.Last().ToString("F4"));
        Assert.AreEqual("62.5367", indicator.Resistance1.Last().ToString("F4"));
        Assert.AreEqual("61.9533", indicator.Values.Last().ToString("F4"));
        Assert.AreEqual("61.5667", indicator.Support1.Last().ToString("F4"));
        Assert.AreEqual("60.9833", indicator.Support2.Last().ToString("F4"));
        Assert.AreEqual("60.5967", indicator.Support3.Last().ToString("F4"));
    }
}
