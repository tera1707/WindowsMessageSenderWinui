using Microsoft.UI.Xaml.Data;
using System;

namespace WindowMessageSender;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool v)
        {
            return v ? "可視" : "不可視";
        }
        return "不可視";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
