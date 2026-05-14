using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AgroControl.Laboratory.Converters
{
    public class StatusToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (string.IsNullOrEmpty(status)) return Brushes.Black;

            switch (status.ToLower())
            {
                case "одобрена":
                    return Brushes.Green;
                case "заблокирована":
                    return Brushes.DarkRed;
                case "в работе":
                    return Brushes.DarkOrange;
                case "ожидает":
                    return Brushes.Gray;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}