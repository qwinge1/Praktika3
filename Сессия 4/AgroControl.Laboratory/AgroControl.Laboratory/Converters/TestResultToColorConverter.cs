using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AgroControl.Laboratory.Converters
{
    public class TestResultToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string result = value as string;
            if (string.IsNullOrEmpty(result)) return Brushes.Black;
            return result.ToLower() == "pass" ? Brushes.Green : Brushes.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}