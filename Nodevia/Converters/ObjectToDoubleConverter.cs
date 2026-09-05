using System.Globalization;
using System.Windows.Data;

namespace Nodevia.Converters;

public class ObjectToDoubleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
            return 0.0;

        try
        {
            return System.Convert.ToDouble(value, culture);
        }
        catch
        {
            return 0.0;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
}

