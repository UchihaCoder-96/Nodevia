namespace Nodevia.Nodes;

public static class NumericPortOptions
{
    public static IReadOnlyDictionary<string, object> Create(
        double min = double.NegativeInfinity,
        double max = double.PositiveInfinity,
        double step = 1.0,
        bool isInteger = false) =>
        new Dictionary<string, object>
        {
            ["Numeric.Min"] = min,
            ["Numeric.Max"] = max,
            ["Numeric.Step"] = step,
            ["Numeric.IsInteger"] = isInteger,
        };
}

