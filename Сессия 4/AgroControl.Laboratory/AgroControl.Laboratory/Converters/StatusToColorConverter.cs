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
            if (string.IsNullOrEmpty(status)) return Brushes.White;
            switch (status.ToLower())
            {
                case "ожидает": return Brushes.LightGray;
                case "в работе": return Brushes.LightYellow;
                case "одобрена": return Brushes.LightGreen;
                case "заблокирована": return Brushes.LightCoral;
                default: return Brushes.White;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}