using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AgroControl.Laboratory.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null) return Brushes.White;

            switch (status.ToLower())
            {
                case "ожидает":
                case "создано":
                    return Brushes.LightGray;
                case "в работе":
                    return Brushes.LightYellow;
                case "одобрена":
                case "approved":
                    return Brushes.LightGreen;
                case "заблокирована":
                case "blocked":
                    return Brushes.LightCoral;
                default:
                    return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}