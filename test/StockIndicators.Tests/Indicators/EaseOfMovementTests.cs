using StockIndicators.Indicators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Indicators;

[TestClass]
public class EaseOfMovementTests
{
    private readonly TestPrice[] prices =
    [
        new() { High = 63.74, Low = 62.63, Volume = 32178836 },
        new() { High = 64.51, Low = 63.85, Volume = 36461672 },
        new() { High = 64.57, Low = 63.81, Volume = 51372680 },
        new() { High = 64.31, Low = 62.62, Volume = 42476356 },
        new() { High = 63.43, Low = 62.73, Volume = 29504176 },
        new() { High = 62.85, Low = 61.95, Volume = 33098600 },
        new() { High = 62.70, Low = 62.06, Volume = 30577960 },
        new() { High = 63.18, Low = 62.69, Volume = 35693928 },
        new() { High = 62.47, Low = 61.54, Volume = 49768136 },
        new() { High = 64.16, Low = 63.21, Volume = 44759968 },
        new() { High = 64.38, Low = 63.87, Volume = 33425504 },
        new() { High = 64.89, Low = 64.29, Volume = 15895085 },
        new() { High = 65.25, Low = 64.48, Volume = 37015388 },
        new() { High = 64.69, Low = 63.65, Volume = 40672116 },
        new() { High = 64.26, Low = 63.68, Volume = 35627200 },
        new() { High = 64.51, Low = 63.12, Volume = 47337336 },
        new() { High = 63.46, Low = 62.50, Volume = 43373576 },
        new() { High = 62.69, Low = 61.86, Volume = 57651752 },
        new() { High = 63.52, Low = 62.56, Volume = 32357184 },
        new() { High = 63.52, Low = 62.95, Volume = 27620876 },
        new() { High = 63.74, Low = 62.63, Volume = 42467704 },
        new() { High = 64.58, Low = 63.39, Volume = 44460240 },
        new() { High = 65.31, Low = 64.72, Volume = 52992592 },
        new() { High = 65.10, Low = 64.21, Volume = 40561552 },
        new() { High = 63.68, Low = 62.51, Volume = 48636228 },
        new() { High = 63.66, Low = 62.55, Volume = 57230032 },
        new() { High = 62.95, Low = 62.15, Volume = 46260856 },
        new() { High = 63.73, Low = 62.97, Volume = 41926492 },
        new() { High = 64.99, Low = 63.64, Volume = 42620976 },
        new() { High = 65.31, Low = 64.60, Volume = 37809176 }
    ];

    [TestMethod]
    public void EaseOfMovement()
    {
        var indicator = new EaseOfMovement(IndicatorCapacity.Infinite);

        foreach (var price in prices)
        {
            indicator.Add(price);
        }

        Assert.IsTrue(indicator.IsReady);
        Assert.AreEqual("0.23", indicator.Values.Last().ToString("F2"));
    }
}
