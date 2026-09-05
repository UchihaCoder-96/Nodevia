namespace Nodevia.Nodes;

public static class SliderPortOptions
{
    public static IReadOnlyDictionary<string, object> Create(double min, double max, double step = 1.0) =>
        new Dictionary<string, object>
        {
            ["Slider.Min"] = min,
            ["Slider.Max"] = max,
            ["Slider.Step"] = step,
        };
}

