using StockIndicators.PriceIndicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.PriceIndicators;

[TestClass]
public class OnBalanceVolumeTests
{
    private readonly Price[] prices =
    [
        new() { Close = 24.75, Volume = 18730 },
        new() { Close = 24.71, Volume = 12272 },
        new() { Close = 25.04, Volume = 24691 },
        new() { Close = 25.55, Volume = 18358 },
        new() { Close = 25.07, Volume = 22964 },
        new() { Close = 25.11, Volume = 15919 },
        new() { Close = 24.89, Volume = 16067 },
        new() { Close = 25.00, Volume = 16568 },
        new() { Close = 25.05, Volume = 16019 },
        new() { Close = 25.34, Volume = 9774 },
        new() { Close = 25.06, Volume = 22573 },
        new() { Close = 25.45, Volume = 12987 },
        new() { Close = 25.56, Volume = 10907 },
        new() { Close = 25.56, Volume = 5799 },
        new() { Close = 25.41, Volume = 7395 },
        new() { Close = 25.37, Volume = 5818 },
        new() { Close = 25.04, Volume = 7165 },
        new() { Close = 24.92, Volume = 5673 },
        new() { Close = 24.88, Volume = 5625 },
        new() { Close = 24.97, Volume = 5023 },
        new() { Close = 25.05, Volume = 7457 },
        new() { Close = 24.45, Volume = 11798 },
        new() { Close = 24.57, Volume = 12366 },
        new() { Close = 24.02, Volume = 13295 },
        new() { Close = 23.88, Volume = 9257 },
        new() { Close = 24.20, Volume = 9691 },
        new() { Close = 24.28, Volume = 8870 },
        new() { Close = 24.33, Volume = 7169 },
        new() { Close = 24.44, Volume = 11356 },
        new() { Close = 25.00, Volume = 13379 }
    ];

    [TestMethod]
    public void OnBalanceVolume()
    {
        var indicator = new OnBalanceVolume(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("60632", indicator.Values.Last().ToString("F0"));
    }
}
