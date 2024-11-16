using Microsoft.UI.Xaml.Data;
using System;

namespace WindowMessageSender;

public class IntToHexStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is int v && parameter is string keta)
        {
            return v.ToString($"X{keta}");
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
