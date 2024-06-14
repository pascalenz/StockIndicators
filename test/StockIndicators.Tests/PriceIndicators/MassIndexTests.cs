using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class MassIndexTests
{
    private readonly Price[] prices =
    [
        new() { High = 1081.58, Low = 1067.08 },
        new() { High = 1063.20, Low = 1046.68 },
        new() { High = 1059.38, Low = 1039.83 },
        new() { High = 1061.45, Low = 1045.40 },
        new() { High = 1065.21, Low = 1039.70 },
        new() { High = 1064.40, Low = 1048.79 },
        new() { High = 1055.14, Low = 1040.88 },
        new() { High = 1081.30, Low = 1049.72 },
        new() { High = 1090.10, Low = 1080.39 },
        new() { High = 1105.10, Low = 1093.61 },
        new() { High = 1102.60, Low = 1091.15 },
        new() { High = 1103.26, Low = 1092.36 },
        new() { High = 1110.27, Low = 1101.15 },
        new() { High = 1110.88, Low = 1103.92 },
        new() { High = 1123.87, Low = 1113.38 },
        new() { High = 1127.36, Low = 1115.58 },
        new() { High = 1126.46, Low = 1114.63 },
        new() { High = 1125.44, Low = 1118.88 },
        new() { High = 1131.47, Low = 1122.43 },
        new() { High = 1144.86, Low = 1126.57 },
        new() { High = 1148.59, Low = 1136.22 },
        new() { High = 1144.38, Low = 1131.58 },
        new() { High = 1136.77, Low = 1122.79 },
        new() { High = 1148.90, Low = 1131.69 },
        new() { High = 1149.92, Low = 1142.00 },
        new() { High = 1150.00, Low = 1132.09 },
        new() { High = 1148.63, Low = 1140.26 },
        new() { High = 1157.16, Low = 1136.08 },
        new() { High = 1150.30, Low = 1139.42 },
        new() { High = 1148.16, Low = 1131.87 },
        new() { High = 1162.76, Low = 1140.68 },
        new() { High = 1162.33, Low = 1154.85 },
        new() { High = 1163.87, Low = 1151.41 },
        new() { High = 1167.73, Low = 1155.58 },
        new() { High = 1168.68, Low = 1162.02 },
        new() { High = 1172.58, Low = 1155.71 },
        new() { High = 1184.38, Low = 1171.32 },
        new() { High = 1178.89, Low = 1166.71 },
        new() { High = 1181.20, Low = 1167.12 },
        new() { High = 1185.53, Low = 1174.55 },
        new() { High = 1178.64, Low = 1159.71 },
        new() { High = 1182.94, Low = 1166.74 },
        new() { High = 1189.43, Low = 1171.17 },
        new() { High = 1183.93, Low = 1178.99 },
        new() { High = 1196.14, Low = 1184.74 },
        new() { High = 1187.11, Low = 1177.72 }
    ];

    [TestMethod]
    public void MassIndex()
    {
        var settings = new MassIndexSettings { MovingAveragePeriods = 9, SumPeriods = 25 };
        var indicator = new MassIndex(IndicatorCapacity.Infinite, settings);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("25.30", indicator.Values.Last().ToString("F2"));
    }
}
