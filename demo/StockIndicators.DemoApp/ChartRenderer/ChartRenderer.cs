using StockIndicators.DemoApp.ChartRenderer;

namespace StockIndicators.ChartRenderer;

internal sealed class ChartRenderer
{
    private readonly Chart chart;
    private readonly int periods;
    private readonly double minValue;
    private readonly double maxValue;
    private readonly double chartAreaLeft;
    private readonly double chartAreaTop;
    private readonly double chartAreaWidth;
    private readonly double chartAreaHeight;
    private readonly double valueWidth;

    private readonly SvgPlotter plotter;
    private double legendOffset = 0;

    public ChartRenderer(Chart chart)
    {
        this.chart = chart;
        plotter = new SvgPlotter();

        periods = chart.PriceSeries.Count > 0
            ? chart.PriceSeries.Select(i => i.Values.Count).Min()
            : chart.ValueSeries.Select(i => i.Values.Count).Min();

        minValue = Math.Min(
            chart.PriceSeries.Count > 0 ? chart.PriceSeries.SelectMany(i => i.Values).Select(i => i.Low).Min() : int.MaxValue,
            chart.ValueSeries.Count > 0 ? chart.ValueSeries.SelectMany(i => i.Values).Min() : int.MaxValue);

        maxValue = Math.Max(
            chart.PriceSeries.Count > 0 ? chart.PriceSeries.SelectMany(i => i.Values).Select(i => i.High).Max() : int.MinValue,
            chart.ValueSeries.Count > 0 ? chart.ValueSeries.SelectMany(i => i.Values).Max() : int.MinValue);

        chartAreaLeft = 20;
        chartAreaTop = 40;
        chartAreaWidth = chart.Width - 40;
        chartAreaHeight = chart.Height - 80;

        valueWidth = Math.Floor((double)chartAreaWidth / (double)periods);
    }

    public void SaveAsScalableVectorGraphics(string fileName)
    {
        using var stream = File.Create(fileName);

        legendOffset = 0;

        DrawHighlights(chart.Highlights);
        DrawLines(chart.GridLines);
        DrawTitle(chart.Title);
        DrawLegend(chart.ValueSeries);
        DrawPriceSeries(chart.PriceSeries);
        DrawValueSeries(chart.ValueSeries);
        plotter.SaveToStream(stream);
    }

    private void DrawTitle(string title)
    {
        plotter.DrawText(chartAreaLeft, chartAreaTop - 10, title, ChartColor.Black, true);
        legendOffset += SvgPlotter.GetTextWidth(title, true) + 20;
    }

    private void DrawLegend(IEnumerable<ChartValueSeries> series)
    {
        foreach (var item in series)
        {
            if (!string.IsNullOrEmpty(item.Title))
            {
                plotter.DrawRectangle(chartAreaLeft + legendOffset, chartAreaTop - 19, 6, 6, item.Color);
                plotter.DrawText(chartAreaLeft + legendOffset + 10, chartAreaTop - 10, item.Title, ChartColor.Black, false);

                legendOffset += SvgPlotter.GetTextWidth(item.Title, false) + 20;
            }
        }
    }

    private void DrawHighlights(IEnumerable<ChartHighlight> highlights)
    {
        foreach (var highlight in highlights)
        {
            var start = TranslateY(highlight.StartPosition);
            var end = TranslateY(highlight.EndPosition);

            plotter.DrawRectangle(
                chartAreaLeft,
                Math.Min(start, end),
                chartAreaWidth,
                Math.Abs(end - start),
                ChartColor.LightGray);
        }
    }

    private void DrawLines(IEnumerable<ChartGridLine> lines)
    {
        foreach (var line in lines)
        {
            var y = TranslateY(line.Position);

            plotter.DrawLine(
                chartAreaLeft,
                y,
                chartAreaLeft + chartAreaWidth,
                y,
                ChartColor.StrongGray);
        }
    }

    private void DrawPriceSeries(IEnumerable<ChartPriceSeries> series)
    {
        foreach (var item in series)
        {
            DrawOhlcChart(item.Values);
        }
    }

    private void DrawValueSeries(IEnumerable<ChartValueSeries> series)
    {
        foreach (var item in series)
        {
            switch (item.Style)
            {
                case ChartValueSeriesStyle.Bar:
                    DrawBarChart(item.Values, item.Color);
                    break;
                case ChartValueSeriesStyle.Dot:
                    DrawDotChart(item.Values, item.Color);
                    break;
                case ChartValueSeriesStyle.Line:
                    DrawLineChart(item.Values, item.Color);
                    break;
                case ChartValueSeriesStyle.Area:
                    DrawAreaChart(item.Values, item.Color);
                    break;
            }
        }
    }

    private void DrawOhlcChart(IReadOnlyList<IPrice> values)
    {
        for (int i = 0; i < values.Count; i++)
        {
            var x = TranslateX(periods - values.Count + i);
            var boxWidth = Math.Max(valueWidth * 0.3, 1);

            var high = TranslateY(values[i].High);
            var low = TranslateY(values[i].Low);
            var open = TranslateY(values[i].Open);
            var close = TranslateY(values[i].Close);

            var brush = values[i].Open <= values[i].Close ? ChartColor.PositiveValue : ChartColor.NegativeValue;

            plotter.DrawLine(x, high, x, low, brush);
            plotter.DrawLine(x - boxWidth, open, x, open, brush);
            plotter.DrawLine(x, close, x + boxWidth, close, brush);
        }
    }

    private void DrawBarChart(IReadOnlyList<double> values, ChartColor color)
    {
        for (int i = 0; i < values.Count; i++)
        {
            var x = TranslateX(periods - values.Count + i);
            var boxWidth = Math.Max(valueWidth * 0.3, 1);
            var boxTop = TranslateY(Math.Max(0, values[i]));
            var boxBottom = TranslateY(Math.Min(0, values[i]));

            plotter.DrawRectangle(x - boxWidth, boxTop, boxWidth * 2 + 1, boxBottom - boxTop, color);
        }
    }

    private void DrawDotChart(IReadOnlyList<double> values, ChartColor color)
    {
        for (int i = 0; i < values.Count; i++)
        {
            var x = TranslateX(periods - values.Count + i);
            var y = TranslateY(values[i]);

            plotter.DrawCircle(x, y, 2, color);
        }
    }

    private void DrawLineChart(IReadOnlyList<double> values, ChartColor color)
    {
        if (values.Count <= 1)
            return;

        var points = values.Select((value, index) => new SvgPlotterPoint(TranslateX(periods - values.Count + index), TranslateY(value)));
        plotter.DrawPath(points, color);
    }

    private void DrawAreaChart(IReadOnlyList<double> values, ChartColor color)
    {
        if (values.Count <= 1)
            return;

        var offset = periods - values.Count;
        var linePoints = values.Select((value, index) => new SvgPlotterPoint(TranslateX(offset + index), TranslateY(value)));

        var areaPoints = new List<SvgPlotterPoint>
        {
            new(TranslateX(offset), TranslateY(minValue))
        };

        areaPoints.AddRange(linePoints);
        areaPoints.Add(new SvgPlotterPoint(TranslateX(offset + values.Count - 1), TranslateY(minValue)));

        plotter.DrawPolygon(areaPoints, color);
        plotter.DrawPath(linePoints, color);
    }

    private double TranslateX(int index)
    {
        double valuePercent = (double)index * 100 / periods;
        return chartAreaLeft + (valueWidth / 2) + ((chartAreaWidth - valueWidth) * valuePercent / 100);
    }

    private double TranslateY(double value)
    {
        double valuePercent = (double)((value - minValue) / (maxValue - minValue) * 100);
        double pixelOffset = chartAreaHeight * valuePercent / 100;
        return chartAreaTop + chartAreaHeight - pixelOffset;
    }
}
