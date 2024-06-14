using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class PositiveVolumeIndexTests
{
    private readonly Price[] prices =
    [
        new() { Close = 1355.69, Volume = 2739550000 },
        new() { Close = 1325.51, Volume = 3119460000 },
        new() { Close = 1335.02, Volume = 3466880000 },
        new() { Close = 1313.72, Volume = 2577120000 },
        new() { Close = 1319.99, Volume = 2480450000 },
        new() { Close = 1331.85, Volume = 2329790000 },
        new() { Close = 1329.04, Volume = 2793070000 },
        new() { Close = 1362.16, Volume = 3378780000 },
        new() { Close = 1365.51, Volume = 2417590000 },
        new() { Close = 1374.02, Volume = 1442810000 },
        new() { Close = 1367.58, Volume = 2122560000 },
        new() { Close = 1354.68, Volume = 2083560000 },
        new() { Close = 1352.46, Volume = 2103850000 },
        new() { Close = 1341.47, Volume = 2526190000 },
        new() { Close = 1341.45, Volume = 2491890000 },
        new() { Close = 1334.76, Volume = 2730320000 },
        new() { Close = 1356.78, Volume = 2311590000 },
        new() { Close = 1353.64, Volume = 2135420000 },
        new() { Close = 1363.67, Volume = 2642720000 },
        new() { Close = 1372.78, Volume = 2701350000 },
        new() { Close = 1376.51, Volume = 3122260000 },
        new() { Close = 1362.66, Volume = 3125070000 },
        new() { Close = 1350.52, Volume = 2728040000 },
        new() { Close = 1338.31, Volume = 2844190000 },
        new() { Close = 1337.89, Volume = 2810240000 },
        new() { Close = 1360.02, Volume = 3256420000 },
        new() { Close = 1385.97, Volume = 3261500000 },
        new() { Close = 1385.30, Volume = 2296260000 },
        new() { Close = 1379.32, Volume = 2701690000 },
        new() { Close = 1375.32, Volume = 2935850000 }
    ];

    [TestMethod]
    public void PositiveVolumeIndex()
    {
        var settings = new PositiveVolumeIndexSettings { SignalPeriods = 20 };
        var indicator = new PositiveVolumeIndex(IndicatorCapacity.Infinite, settings);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("1013.37", indicator.Values.Last().ToString("F2"));
    }
}
