using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace StockIndicators.Catalog;

/// <summary>
/// Provides semantic access to all available indicators.
/// </summary>
public sealed class IndicatorCatalog
{
    private readonly IList<Assembly> assemblies;
    private Dictionary<string, IndicatorDescription>? indicatorDescriptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndicatorCatalog"/> class.
    /// </summary>
    public IndicatorCatalog()
    {
        assemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly()
        };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndicatorCatalog"/> class.
    /// </summary>
    /// <param name="assembliyNames">The names of the assemblies to scan for indicators.</param>
    public IndicatorCatalog(IEnumerable<string> assembliyNames)
    {
        ArgumentNullException.ThrowIfNull(assembliyNames);

        assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a =>
                assembliyNames.Contains(a.GetName().Name) ||
                a.GetName().Name == Assembly.GetExecutingAssembly().GetName().Name)
            .ToList();
    }

    /// <summary>
    /// Creates a new instance of the indicator with the supplied description.
    /// </summary>
    /// <param name="description">The description of the instance.</param>
    /// <param name="parameters">The parameter values.</param>
    /// <returns>The new instance of the indicator.</returns>
    public object CreateIndicator(IndicatorDescription description, IReadOnlyDictionary<string, double> parameters)
    {
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(parameters);

        var arguments = new List<object>();

        var hasCapacityParameter = description.IndicatorType
            .GetConstructors()
            .SelectMany(c => c.GetParameters())
            .Any(p => p.ParameterType == typeof(IndicatorCapacity));

        if (hasCapacityParameter)
            arguments.Add(IndicatorCapacity.Infinite);

        if (description.IndicatorSettingsType != null)
        {
            var settings = Activator.CreateInstance(description.IndicatorSettingsType)!;
            arguments.Add(settings);

            foreach (var parameter in parameters)
            {
                var property = settings.GetType().GetProperty(parameter.Key);
                if (property == null)
                    throw new InvalidOperationException($"Indicator '{description.Name}' does not have a parameter with name '{parameter.Key}'.");

                if (property.PropertyType == typeof(int) || property.PropertyType.IsEnum)
                    property.SetValue(settings, (int)parameter.Value);
                else
                    property.SetValue(settings, parameter.Value);
            }
        }

        var constructor = description.IndicatorType
            .GetConstructors()
            .OrderByDescending(c => c.GetParameters().Length)
            .First();

        return constructor.Invoke([.. arguments]);
    }

    /// <summary>
    /// Finds the description of the indicator with the supplied name.
    /// </summary>
    /// <param name="indicatorName">The name of the indicator to find.</param>
    /// <returns>The indicator's description or null.</returns>
    public IndicatorDescription? FindIndicatorDescription(string indicatorName)
    {
        if (indicatorDescriptions == null)
            GetIndicatorDescriptions();

        indicatorDescriptions!.TryGetValue(indicatorName, out IndicatorDescription? description);
        return description;
    }

    /// <summary>
    /// Gets the description of the indicator with the supplied name.
    /// </summary>
    /// <param name="indicatorName">The name of the indicator to return.</param>
    /// <returns>The indicator's description.</returns>
    public IndicatorDescription GetIndicatorDescription(string indicatorName)
    {
        var description = FindIndicatorDescription(indicatorName);
        return description ?? throw new InvalidOperationException($"indicator with name '{indicatorName}' not found.");
    }

    /// <summary>
    /// Gets the descriptions of all available indicators.
    /// </summary>
    /// <returns>A list of all available indicators.</returns>
    public IList<IndicatorDescription> GetIndicatorDescriptions()
    {
        indicatorDescriptions ??= assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass)
            .Where(t => typeof(IAverageIndicator).IsAssignableFrom(t) ||
                        typeof(IPriceIndicator).IsAssignableFrom(t) ||
                        typeof(IPriceComparisonIndicator).IsAssignableFrom(t))
            .Select(t => GetIndicatorDescription(t))
            .ToDictionary(i => i.Name, i => i);

        return indicatorDescriptions.Values
            .OrderBy(i => i.Name)
            .ToList();
    }

    private IndicatorDescription GetIndicatorDescription(Type indicatorType)
    {
        var displayName = indicatorType.GetCustomAttributes<DisplayNameAttribute>().Select(a => a.DisplayName).First();
        var description = indicatorType.GetCustomAttributes<DescriptionAttribute>().Select(a => a.Description).FirstOrDefault();
        var categories = indicatorType.GetCustomAttributes<CategoryAttribute>().Select(a => a.Category).ToList();

        var settingsType = indicatorType
            .GetConstructors()
            .SelectMany(c => c.GetParameters())
            .Select(p => p.ParameterType)
            .Where(p => p != typeof(IndicatorCapacity))
            .Distinct()
            .SingleOrDefault();

        var parameters = settingsType?.GetProperties().Select(GetIndicatorParameter).ToList() ?? [];

        return new IndicatorDescription(indicatorType, settingsType, displayName, description, categories, parameters);
    }

    private IndicatorParameter GetIndicatorParameter(PropertyInfo property)
    {
        var displayName = property.GetCustomAttributes<DisplayNameAttribute>().Select(a => a.DisplayName).First();
        var description = property.GetCustomAttributes<DescriptionAttribute>().Select(a => a.Description).FirstOrDefault();
        var defaultValue = property.GetCustomAttributes<DefaultValueAttribute>().Select(a => a.Value).FirstOrDefault();
        var range = property.GetCustomAttributes<RangeAttribute>().Select(a => new { a.Minimum, a.Maximum }).FirstOrDefault();

        return new IndicatorParameter(
            property.Name,
            displayName,
            description,
            (defaultValue is int) ? 0 : 3,
            range != null ? Convert.ToDouble(range.Minimum) : 0.0,
            range != null ? Convert.ToDouble(range.Maximum) : 0.0,
            Convert.ToDouble(defaultValue),
            property.PropertyType.IsEnum ? property.PropertyType.GetEnumNames().ToList() : []);
    }
}
