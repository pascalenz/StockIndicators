[![Build & Test](https://github.com/pascalenz/FluentExceptions/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/pascalenz/FluentExceptions/actions/workflows/build-and-test.yml)

# Introduction

## Background
The project contains a library of technical analysis indicators. It was written several years ago for a research project, and I decided to extract it from the project, modernize the code, and share it on GitHub in case anyone finds it useful.

## Features
There are many similar open-source libraries available nowadays. However, this library has a few advantages that were the reason why it was built in the first place.

- Data can be streamed into the indicators value by value and the indicators incrementally update the results. This is useful for efficient analysis of large amounts of data or for real-time analysis, e.g. in automated trading scenarios.

- The indicators provide description for chart visualization. The library itself is not a charting library (although the demo application does generate very simple charts) but the descriptions can easily be translated to any charting library like Highcharts.

- The Indicator Catalog utility class uses reflection to provide a description of all available indicators and their parameters. This enables building dynamic and generic user interfaces and makes it easy to extend the library with additional indicators.

## Included Indicators
The following indicators are included in the library.

| Name | Category |
| --- | --- |
| Accumulation Distribution Line (ADL) | Momentum |
| Average Directional Index (ADX) | Trend |
| Aroon Oscillator | Trend |
| Aroon Up/Down | Trend |
| Average True Range (ATR) | Volatility |
| Bollinger Bands | Moving Average |
| Bollinger Bands - %B | Moving Average |
| Bollinger Bands - Bandwidth | Moving Average |
| Commodity Channel Index (CCI) | Momentum |
| Chaikin Oscillator | Momentum |
| Chaikin Volatility | Volatility |
| Chandelier Long Exit | Signal |
| Chandelier Short Exit | Signal |
| Chaikin Money Flow (CMF) | Momentum |
| Double Exponential Moving Average (DEMA) | Moving Average |
| Donchian Channel | Trend |
| Donchian Channel Width | Trend |
| Exponential Moving Average (EMA) | Moving Average |
| Ease of Movement (EMV) | Moving Average |
| Envelopes | Momentum |
| Fibonacci Pivot Points | Pivot |
| Hull Moving Average (HMA) | Moving Average |
| Keltner Channels | Momentum |
| Moving Average Convergence/Divergence (MACD) | Momentum |
| Mass Index | Trend |
| Money Flow Index (MFI) | Momentum |
| Momentum | Momentum |
| Negative Volume Index (NVI) | Volume |
| On Balance Volume (OBV) | Momentum |
| Percentage Price Oscillator (PPO) | Momentum |
| Positive Volume Index (PVI) | Volume |
| Percentage Volume Oscillator (PVO) | Volume |
| Rate-of-Change (ROC) | Momentum |
| Relative Strength Index (RSI) | Momentum |
| Simple Moving Average (SMA) | Moving Average |
| Standard Deviation | Moving Average |
| Standard Pivots Points | Pivot |
| Stochastic Oscillator | Momentum |
| StochRSI | Momentum |
| Triple Exponential Moving Average (TEMA) | Moving Average |
| TRIX | Moving Average |
| True Strength Index (TSI) | Momentum |
| Ultimate Oscillator | Moving Average |
| Vortex Indicator (VTX) | Trend |
| Volume-Weighted Average Price (VWAP) | Moving Average |
| Williams %R | Momentum |
| Weighted Moving Average (WMA) | Moving Average |

# Usage
## Simple Indicator Creation
Indicators typically consist of a settings class and the actual indicator class. To instantiate an indicator, simply pass the settings object to its constructor.

In addition, you can specify how many values the indicator should keep. This could be all values (`IndicatorCapacity.Infinite`), only the last value (`IndicatorCapacity.Minimum`), or a specific number of values.

```C#
var envelopesSettings = new EnvelopesSettings
{
    Periods = 20,
    Envelope = 0.025
}

var envelopes = new Envelopes(IndicatorCapacity.Infinite, envelopesSettings);
```

Once the indicator is instantiated, data can be streamed into it. There are three different types (interfaces) of indicators and their `Add` method exepts different types of arguments.

| Interface | Description | Add Value Type |
| --- | --- | --- |
| IAverageIndicator | Indicators that generate an average of a simple value series like a closing price or volume. | double |
| IPriceIndicator | A price indicator that might analyze multiple values like high, low, open, close, and volume. | IPrice |
| IPriceComparisonIndicator | An indicator that compares two price series. | IPrice, IPrice |

```C#
envelopes.Add(new Price(...));
```

Once the minimum number of required values has been added, the results can be read from the indicator.

```C#
if (envelopes.IsReady)
{
    var upperEnvelope = envelopes.UpperEnvelope;
    var lowerEnvelope = envelopes.LowerEnvelope;
    var average = envelopes.Average;
}
```

## Using the Indicator Catalog
The indicator catalog is a utility class that provide a list of all available indicators and their parameters and allows instantiating indicator objects.

```C#
// Create the catalog. This uses reflection to find all indicator
// classes in the library and optionally additional assemblies.
var catalog = new IndicatorCatalog();

// Get a specific indicator be name.
var indicatorDescription = catalog.GetIndicatorDescription("MACD");

// Define the parameter values. We just stick to the defaults here.
var parameters = indicatorDescription.Parameters
    .ToDictionary(p => p.Name, p => p.DefaultValue);

// Now instantiate the indicator.
var indicator = catalog.CreateIndicator(indicatorDescription, parameters);

// Finally we fill the indicator with data. To do this,
// we need to know what type of indicator it is.
if (indicator is IAverageIndicator averageIndicator)
    averageIndicator.Add(doubleValue);

if (indicator is IPriceIndicator priceIndicator)
    priceIndicator.Add(priceValue);

if (indicator is IPriceComparisonIndicator priceComparisonIndicator)
    priceComparisonIndicator.Add(priceValue1, priceValue2);
```

## Retrieving Chart Descriptions
Indicators can provide a semantic description of a chart to visualize the indicator values. There are two types of interfaces that indicators implement to provide chart descriptions.

| Interface | Description |
| --- | --- |
| IChartProvider | Implemented by indicators that are visualized in a standalone chart. Therefore, the interface returns a description of the entire chart including grid lines, highlights, and legend. |
| IChartValueSeriesProvider | Implemented by charts that add additional information to price charts, for example moving averages. The interface returns one or more value series collections that can be added to an existing chart. |

For indicators that implement `IChartProvider`, we can simply retrieve the description of the entire chart.

```C#
var chart = indicator.CreateChart();
```

For indicators that implement `IChartValueSeriesProvider`, we need to create the price chart first an then add the additional series to it.

```C#
var chart = new Chart
{
    Title = "Price",
    PriceSeries = [new ChartPriceSeries("Price", prices)],
    ValueSeries = envelopes.CreateChartValueSeries().ToArray()
};
```

## Extending the library
The Indicator Catalog uses reflection to find and describe indicators. To extend the library with additional indicators, simple write them in the same way as the existing ones using the same interfaces and attributes and then pass the name of the assembly that contains the indicators to the constructor of the `IndicatorCatalog` class.
