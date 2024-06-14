namespace StockIndicators.Internal;

/// <summary>
/// A fixed-size collection to analyse values within a certain window.
/// </summary>
public sealed class AnalysisWindow : FixedSizeReadOnlyList<double>
{
    private readonly bool maintainSumAndAverage;
    private readonly bool maintainMinAndMax;
    private double sum = 0;
    private int minIndex = -1;
    private int maxIndex = -1;

    /// <summary>
    /// Initializes a new instance of the <see cref="AnalysisWindow"/> class.
    /// </summary>
    /// <param name="periods">The maximum number of items maintained by the list.</param>
    /// <param name="maintainSumAndAverage">A value indicating whether the list should calculate sum and avareage values.</param>
    /// <param name="maintainMinAndMax">A value indicating whether the list should calculate minimum and maximum values.</param>
    public AnalysisWindow(int periods, bool maintainSumAndAverage, bool maintainMinAndMax)
        : base(periods)
    {
        this.maintainSumAndAverage = maintainSumAndAverage;
        this.maintainMinAndMax = maintainMinAndMax;
    }

    /// <summary>
    /// Gets the sum of all values.
    /// </summary>
    public double Sum => sum;

    /// <summary>
    /// Gets the average of all values.
    /// </summary>
    public double Average => sum / Count;

    /// <summary>
    /// Gets the minimum of all values.
    /// </summary>
    public double Min => list[minIndex];

    /// <summary>
    /// Gets the maximim of all values.
    /// </summary>
    public double Max => list[maxIndex];

    /// <summary>
    /// Gets the first value.
    /// </summary>
    public double First => this[0];

    /// <summary>
    /// Gets the last value.
    /// </summary>
    public double Last => list[index];

    /// <summary>
    /// Adds a new value to the list and removes the first one if the list is full.
    /// </summary>
    /// <param name="value">The value to add.</param>
    public override void Add(double value)
    {
        var newIndex = (index + 1) % Size;

        if (maintainSumAndAverage)
        {
            if (IsFilled)
            {
                sum -= list[newIndex];
            }

            sum += value;
        }

        if (maintainMinAndMax)
        {
            if (IsFilled && (newIndex == minIndex))
                FindMinIndex(newIndex);

            if (IsFilled && (newIndex == maxIndex))
                FindMaxIndex(newIndex);

            if ((minIndex == -1) || (value < Min))
                minIndex = newIndex;

            if ((maxIndex == -1) || (value > Max))
                maxIndex = newIndex;
        }

        base.Add(value);
    }

    /// <summary>
    /// Find the index of the lowest value.
    /// </summary>
    /// <param name="newIndex">The index of the last added value.</param>
    private void FindMinIndex(int newIndex)
    {
        minIndex = -1;

        for (int i = 0; i < Size; i++)
        {
            if (i != newIndex)
            {
                if ((minIndex == -1) || (list[i] < list[minIndex]))
                    minIndex = i;
            }
        }
    }

    /// <summary>
    /// Find the index of the highest value.
    /// </summary>
    /// <param name="newIndex">The index of the last added value.</param>
    private void FindMaxIndex(int newIndex)
    {
        maxIndex = -1;

        for (int i = 0; i < Size; i++)
        {
            if (i != newIndex)
            {
                if ((maxIndex == -1) || (list[i] > list[maxIndex]))
                    maxIndex = i;
            }
        }
    }
}