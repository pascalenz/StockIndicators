using StockIndicators.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StockIndicators.Tests.Internal;

[TestClass]
public class AnalysisListTests
{
    private readonly Random random = new();
    private List<double> values = null!;
    private AnalysisWindow list = null!;

    [TestMethod]
    public void AnalysisList_LessThanSize_ByIndex()
    {
        SetupLessThanSize();

        for (int i = 0; i < values.Count; i++)
        {
            Assert.AreEqual(values[i], list[i]);
        }
    }

    [TestMethod]
    public void AnalysisList_LessThanSize_Enumerable()
    {
        SetupLessThanSize();

        var index = 0;

        foreach (var item in list)
        {
            Assert.AreEqual(values[index], item);
            index++;
        }
    }

    [TestMethod]
    public void AnalysisList_LessThanSize_Linq()
    {
        SetupLessThanSize();

        Assert.AreEqual(values.First(), list.First);
        Assert.AreEqual(values.Last(), list.Last);
        Assert.AreEqual(values.Sum().ToString("N4"), list.Sum.ToString("N4"));
        Assert.AreEqual(values.Min().ToString("N4"), list.Min.ToString("N4"));
        Assert.AreEqual(values.Max().ToString("N4"), list.Max.ToString("N4"));
        Assert.AreEqual(values.Average().ToString("N4"), list.Average.ToString("N4"));
    }

    [TestMethod]
    public void AnalysisList_MoreThanSize_ByIndex()
    {
        SetupMoreThanSize();

        for (int i = 0; i < values.Count; i++)
        {
            Assert.AreEqual(values[i], list[i]);
        }
    }

    [TestMethod]
    public void AnalysisList_MoreThanSize_Enumerable()
    {
        SetupMoreThanSize();

        var index = 0;

        foreach (var item in list)
        {
            Assert.AreEqual(values[index], item);
            index++;
        }
    }

    [TestMethod]
    public void AnalysisList_MoreThanSize_Linq()
    {
        SetupMoreThanSize();

        Assert.AreEqual(values.First(), list.First);
        Assert.AreEqual(values.Last(), list.Last);
        Assert.AreEqual(values.Sum().ToString("N4"), list.Sum.ToString("N4"));
        Assert.AreEqual(values.Min().ToString("N4"), list.Min.ToString("N4"));
        Assert.AreEqual(values.Max().ToString("N4"), list.Max.ToString("N4"));
        Assert.AreEqual(values.Average().ToString("N4"), list.Average.ToString("N4"));
    }

    [TestMethod]
    public void AnalysisList_EqualToSize_ByIndex()
    {
        SetupEqualToSize();

        for (int i = 0; i < values.Count; i++)
        {
            Assert.AreEqual(values[i], list[i]);
        }
    }

    [TestMethod]
    public void AnalysisList_EqualToSize_Enumerable()
    {
        SetupEqualToSize();

        var index = 0;

        foreach (var item in list)
        {
            Assert.AreEqual(values[index], item);
            index++;
        }
    }

    [TestMethod]
    public void AnalysisList_EqualToSize_Linq()
    {
        SetupEqualToSize();

        Assert.AreEqual(values.First(), list.First);
        Assert.AreEqual(values.Last(), list.Last);
        Assert.AreEqual(values.Sum().ToString("N4"), list.Sum.ToString("N4"));
        Assert.AreEqual(values.Min().ToString("N4"), list.Min.ToString("N4"));
        Assert.AreEqual(values.Max().ToString("N4"), list.Max.ToString("N4"));
        Assert.AreEqual(values.Average().ToString("N4"), list.Average.ToString("N4"));
    }

    private void SetupLessThanSize()
    {
        list = new AnalysisWindow(23, true, true);

        values = Enumerable.Range(0, 20).Select(i => random.NextDouble()).ToList();
        values.ForEach(v => list.Add(v));

        Assert.AreEqual(values.Count, list.Count);
    }

    private void SetupMoreThanSize()
    {
        list = new AnalysisWindow(23, true, true);

        values = Enumerable.Range(0, 100).Select(i => random.NextDouble()).ToList();
        values.ForEach(v => list.Add(v));
        values = values.Skip(77).ToList();

        Assert.AreEqual(values.Count, list.Count);
    }

    private void SetupEqualToSize()
    {
        list = new AnalysisWindow(23, true, true);

        values = Enumerable.Range(0, 23).Select(i => random.NextDouble()).ToList();
        values.ForEach(v => list.Add(v));

        Assert.AreEqual(values.Count, list.Count);
    }
}
