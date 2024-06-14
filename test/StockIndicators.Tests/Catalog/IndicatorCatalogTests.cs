using StockIndicators.AverageIndicators;
using StockIndicators.Catalog;
using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Catalog;

[TestClass]
public class IndicatorCatalogTests
{
    [TestMethod]
    public void GetIndicatorDescriptions()
    {
        var catalog = new IndicatorCatalog();
        var descriptions = catalog.GetIndicatorDescriptions();

        Assert.AreNotEqual(0, descriptions.Count);
    }

    [TestMethod]
    public void CreateAverageIndicator()
    {
        var catalog = new IndicatorCatalog();
        var description = catalog.GetIndicatorDescriptions().First(d => d.IndicatorType == typeof(SimpleMovingAverage));
        var arguments = new Dictionary<string, double> { ["Periods"] = 50 };

        var indicator = catalog.CreateIndicator(description, arguments);

        Assert.IsInstanceOfType(indicator, typeof(SimpleMovingAverage));
    }

    [TestMethod]
    public void CreatePriceIndicator()
    {
        var catalog = new IndicatorCatalog();
        var description = catalog.GetIndicatorDescriptions().First(d => d.IndicatorType == typeof(MACD));
        var arguments = new Dictionary<string, double>
        {
            ["FastPeriods"] = 12,
            ["SlowPeriods"] = 26,
            ["SignalPeriods"] = 9
        };

        var indicator = catalog.CreateIndicator(description, arguments);

        Assert.IsInstanceOfType(indicator, typeof(MACD));
    }
}
