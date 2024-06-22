using StockIndicators;
using StockIndicators.Catalog;
using StockIndicators.ChartRenderer;
using System.Diagnostics;
using System.Globalization;

// Load sample data.
var sampleFilePath = Path.Combine(Environment.CurrentDirectory, "Data", "MSFT.csv");

var prices = File.ReadAllLines(sampleFilePath)
    .Select(line => line.Split(','))
    .Select(values => (IPrice)new Price(
        DateTimeOffset.ParseExact(values[0], "yyyymmdd", CultureInfo.InstalledUICulture),
        double.Parse(values[1]),
        double.Parse(values[2]),
        double.Parse(values[3]),
        double.Parse(values[4]),
        long.Parse(values[5])))
    .ToList();

// Load indicator catalog.
var catalog = new IndicatorCatalog();
var indicatorDescriptions = catalog.GetIndicatorDescriptions()
    .Where(i => !i.Categories.Contains(IndicatorCategory.Comparison))
    .ToArray();

while (true)
{
    // Select an indicator from the catalog.
    IndicatorDescription? selectedIndicatorDescription = null;
    while (selectedIndicatorDescription == null)
    {
        Console.WriteLine("Select an indicator:");

        for (var i = 0; i < indicatorDescriptions.Length; i++)
        {
            var name = string.IsNullOrEmpty(indicatorDescriptions[i].Description)
                ? indicatorDescriptions[i].Name
                : $"{indicatorDescriptions[i].Description} ({indicatorDescriptions[i].Name})";

            Console.WriteLine($"[{i + 1}] {name}");
        }

        var option = Console.ReadLine();
        if (int.TryParse(option, out var index) && index <= indicatorDescriptions.Length)
        {
            selectedIndicatorDescription = indicatorDescriptions[index - 1];
        }
    }

    // Create the indicator instance and write the test data to it.
    var parameters = selectedIndicatorDescription.Parameters.ToDictionary(p => p.Name, p => p.DefaultValue);
    var indicator = catalog.CreateIndicator(selectedIndicatorDescription, parameters);

    if (indicator is IAverageIndicator averageIndicator)
        prices.ForEach(p => averageIndicator.Add(p.Close));
    else if (indicator is IPriceIndicator priceIndicator)
        prices.ForEach(p => priceIndicator.Add(p));
    else
        throw new InvalidOperationException("Unexpected chart type");

    // Generate the chart description from the indicator.
    Chart? chart;

    if (indicator is IChartProvider chartProvider)
    {
        chart = chartProvider.CreateChart();
    }
    else if (indicator is IChartValueSeriesProvider chartValueSeriesProvider)
    {
        chart = new Chart
        {
            Title = "Price",
            PriceSeries = [new ChartPriceSeries(null, prices)],
            ValueSeries = chartValueSeriesProvider.CreateChartValueSeries().ToArray()
        };
    }
    else
    {
        throw new InvalidOperationException("Unexpected chart type");
    }

    // Generate the chart.
    var outputFilePath = Path.Combine(Path.GetTempPath(), "chart.svg");

    var renderer = new ChartRenderer(chart);
    renderer.SaveAsScalableVectorGraphics(outputFilePath);

    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "explorer",
            Arguments = $"\"{outputFilePath}\""
        }
    };

    process.Start();

    Console.WriteLine("Chart generated.");
}
