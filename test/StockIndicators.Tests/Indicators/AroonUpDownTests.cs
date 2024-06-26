﻿using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class AroonUpDownTests
{
    private static readonly double[] prices =
    [
        54.09, 59.90, 58.20, 59.76, 52.35, 52.82, 56.94, 57.47, 55.26, 57.51,
        54.80, 51.47, 56.16, 58.34, 56.02, 60.22, 56.75, 57.38, 50.23, 57.06,
        61.51, 63.69, 66.22, 69.16, 70.73, 67.79, 68.82, 62.38, 67.59, 67.59
    ];

    [TestMethod]
    public void AroonUpDown()
    {
        var indicator = new AroonUpDown(IndicatorCapacity.Infinite);
        indicator.Add(prices.Select(price => new TestPrice { Close = price }));

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("80.00", indicator.Up.Last().ToString("F2"));
        Assert.AreEqual("56.00", indicator.Down.Last().ToString("F2"));
    }
}
