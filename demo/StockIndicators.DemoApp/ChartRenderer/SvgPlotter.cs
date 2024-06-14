using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Xml.Linq;

namespace StockIndicators.DemoApp.ChartRenderer;

internal record SvgPlotterPoint(double X, double Y);

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "Demo app only runs on Windows")]
internal sealed class SvgPlotter
{
    private readonly XNamespace ns = "http://www.w3.org/2000/svg";
    private readonly XElement svg;

    public SvgPlotter()
    {
        svg = new XElement(ns + "svg", new XAttribute("version", "1.1"));
    }

    public void DrawLine(double x1, double y1, double x2, double y2, ChartColor color)
    {
        svg.Add(new XElement(ns + "line",
            new XAttribute("x1", x1.ToString("F2")),
            new XAttribute("y1", y1.ToString("F2")),
            new XAttribute("x2", x2.ToString("F2")),
            new XAttribute("y2", y2.ToString("F2")),
            new XAttribute("style", GetStrokeStyle(color))));
    }

    public void DrawRectangle(double x, double y, double width, double height, ChartColor fillColor)
    {
        svg.Add(new XElement(ns + "rect",
            new XAttribute("x", x.ToString("F2")),
            new XAttribute("y", y.ToString("F2")),
            new XAttribute("width", width.ToString("F2")),
            new XAttribute("height", height.ToString("F2")),
            new XAttribute("style", GetStyle(fillColor))));
    }

    public void DrawCircle(double x, double y, double radius, ChartColor fillColor)
    {
        svg.Add(new XElement(ns + "circle",
            new XAttribute("cx", x.ToString("F2")),
            new XAttribute("cy", y.ToString("F2")),
            new XAttribute("r", radius),
            new XAttribute("style", GetStyle(fillColor))));
    }

    public void DrawCircle(double x, double y, double radius, ChartColor fillColor, ChartColor lineColor)
    {
        svg.Add(new XElement(ns + "circle",
            new XAttribute("cx", x.ToString("F2")),
            new XAttribute("cy", y.ToString("F2")),
            new XAttribute("r", radius),
            new XAttribute("style", GetStyle(fillColor, lineColor))));
    }

    public void DrawPolygon(IEnumerable<SvgPlotterPoint> points, ChartColor fillColor)
    {
        svg.Add(new XElement(ns + "polygon",
            new XAttribute("points", string.Join(" ", points.Select(p => p.X.ToString("F2") + "," + p.Y.ToString("F2")))),
            new XAttribute("style", GetStyle(fillColor))));
    }

    public void DrawPath(IEnumerable<SvgPlotterPoint> points, ChartColor lineColor)
    {
        var path = string.Format("M {0:F2} {1:F2} ", points.First().X, points.First().Y) + string.Join(" ", points.Skip(1).Select(p => string.Format("L {0:F2} {1:F2} ", p.X, p.Y)));

        svg.Add(new XElement(ns + "path",
            new XAttribute("d", path),
            new XAttribute("style", "fill:none;" + GetStrokeStyle(lineColor))));
    }

    public void DrawText(double x, double y, string text, ChartColor color, bool bold)
    {
        svg.Add(new XElement(ns + "text",
            new XAttribute("x", x.ToString("F2")),
            new XAttribute("y", y.ToString("F2")),
            new XAttribute("text-anchor", "start"),
            new XAttribute("fill", GetRgbValue(color)),
            new XAttribute("style", GetTextStyle(bold)),
            new XText(text)));
    }

    public void SaveToStream(Stream stream)
    {
        svg.Save(stream, SaveOptions.OmitDuplicateNamespaces);
    }

    public static double GetTextWidth(string text, bool bold)
    {
        using Bitmap bmp = new(1, 1);
        using Graphics g = Graphics.FromImage(bmp);
        return g.MeasureString(text, new Font("Verdana", 12, bold ? FontStyle.Bold : FontStyle.Regular)).Width;
    }

    private static string GetRgbValue(ChartColor color)
    {
        return color switch
        {
            ChartColor.Black => "rgb(0, 0, 0)",
            ChartColor.LightGray => "rgb(248, 248, 248)",
            ChartColor.StrongGray => "rgb(192, 192, 192)",
            ChartColor.Neutral => "rgb(0, 0, 0)",
            ChartColor.PositiveValue => "rgb(0, 128, 0)",
            ChartColor.NegativeValue => "rgb(255, 0, 0)",
            ChartColor.Red => "rgb(255, 0, 0)",
            ChartColor.Random => "rgb(0, 128, 128)",
            _ => throw new NotImplementedException(),
        };
    }

    private static string GetTextStyle(bool bold) => bold ? "font-family:Verdana,Arial;font-weight:bold" : "font-family:Verdana,Arial";

    private static string GetStrokeStyle(ChartColor color) => $"stroke:{GetRgbValue(color)};stroke-width:1";

    private static string GetStyle(ChartColor fillColor) => "fill:" + GetRgbValue(fillColor);

    private static string GetStyle(ChartColor fillColor, ChartColor lineColor) => GetStyle(fillColor) + ";" + GetStrokeStyle(lineColor);
}
