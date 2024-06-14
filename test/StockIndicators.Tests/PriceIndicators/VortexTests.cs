using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class VortexTests
{
    private readonly Price[] prices =
    [
        new() { High = 1380.39, Low = 1371.21, Close = 1376.51 },
        new() { High = 1376.51, Low = 1362.19, Close = 1362.66 },
        new() { High = 1362.34, Low = 1337.56, Close = 1350.52 },
        new() { High = 1351.53, Low = 1329.24, Close = 1338.31 },
        new() { High = 1343.98, Low = 1331.50, Close = 1337.89 },
        new() { High = 1363.13, Low = 1338.17, Close = 1360.02 },
        new() { High = 1389.19, Low = 1360.05, Close = 1385.97 },
        new() { High = 1391.74, Low = 1381.37, Close = 1385.30 },
        new() { High = 1387.16, Low = 1379.17, Close = 1379.32 },
        new() { High = 1385.03, Low = 1373.35, Close = 1375.32 },
        new() { High = 1375.13, Low = 1354.65, Close = 1365.00 },
        new() { High = 1394.16, Low = 1365.45, Close = 1390.99 },
        new() { High = 1399.63, Low = 1391.04, Close = 1394.23 },
        new() { High = 1407.14, Low = 1394.46, Close = 1401.35 },
        new() { High = 1404.14, Low = 1396.13, Close = 1402.22 },
        new() { High = 1405.95, Low = 1398.80, Close = 1402.80 },
        new() { High = 1405.98, Low = 1395.62, Close = 1405.87 },
        new() { High = 1405.87, Low = 1397.32, Close = 1404.11 },
        new() { High = 1410.03, Low = 1400.60, Close = 1403.93 },
        new() { High = 1407.73, Low = 1401.83, Close = 1405.53 },
        new() { High = 1417.44, Low = 1404.15, Close = 1415.51 },
        new() { High = 1418.71, Low = 1414.67, Close = 1418.16 },
        new() { High = 1418.13, Low = 1412.12, Close = 1418.13 },
        new() { High = 1426.68, Low = 1410.86, Close = 1413.17 },
        new() { High = 1416.12, Low = 1406.78, Close = 1413.49 },
        new() { High = 1413.49, Low = 1400.50, Close = 1402.08 },
        new() { High = 1413.46, Low = 1398.04, Close = 1411.13 },
        new() { High = 1416.17, Low = 1409.11, Close = 1410.44 },
        new() { High = 1413.63, Low = 1405.59, Close = 1409.30 },
        new() { High = 1413.95, Low = 1406.57, Close = 1410.49 }
    ];

    [TestMethod]
    public void VortexIndicator()
    {
        var indicator = new Vortex(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("1.06", indicator.Up.Last().ToString("F2"));
        Assert.AreEqual("0.94", indicator.Down.Last().ToString("F2"));
    }
}
