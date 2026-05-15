using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AgroControl.Operator.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (string.IsNullOrEmpty(status)) return Brushes.Black;
            switch (status.ToLower())
            {
                case "одобрена":
                case "pass":
                    return Brushes.Green;
                case "заблокирована":
                case "blocked":
                    return Brushes.DarkRed;
                case "выполняется":
                    return Brushes.DarkOrange;
                case "критическое":
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}