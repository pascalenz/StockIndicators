using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class TrueStrengthIndexTests
{
    private static readonly double[] prices =
    [
        1390.69, 1399.98, 1403.36, 1397.91, 1405.82, 1402.31, 1391.57, 1369.10,
        1369.58, 1363.72, 1354.58, 1357.99, 1353.39, 1338.35, 1330.66, 1324.80,
        1304.86, 1295.22, 1315.99, 1316.63, 1318.86, 1320.68, 1317.82, 1332.42,
        1313.32, 1310.33, 1278.04, 1278.18, 1285.50, 1315.13, 1314.99, 1325.66,
        1308.93, 1324.18, 1314.88, 1329.10, 1342.84, 1344.78, 1357.98, 1355.69,
        1325.51, 1335.02, 1313.72, 1319.99, 1331.85, 1329.04, 1362.16, 1365.51,
        1374.02, 1367.58, 1354.68, 1352.46, 1341.47, 1341.45, 1334.76, 1356.78,
        1353.64, 1363.67, 1372.78, 1376.51, 1362.66, 1350.52, 1338.31, 1337.89,
        1360.02, 1385.97, 1385.30, 1379.32, 1375.32, 1365.00, 1390.99, 1394.23,
        1401.35, 1402.22, 1402.80, 1405.87, 1404.11, 1403.93, 1405.53, 1415.51,
        1418.16, 1418.13, 1413.17, 1413.49, 1402.08, 1411.13, 1410.44, 1409.30
    ];

    [TestMethod]
    public void TrueStrengthIndex()
    {
        var indicator = new TrueStrengthIndex(IndicatorCapacity.Infinite);
        indicator.Add(prices.Select(price => new TestPrice { Close = price }));

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("24.83", indicator.Values.Last().ToString("F2"));
    }
}
