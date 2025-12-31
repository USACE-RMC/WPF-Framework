using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace FrameworkUI
{
    public class CutoffConverter : IValueConverter
    {

        public double Cutoff { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(double))
            {
                return (double)value < Cutoff;
            }
            if (value.GetType() == typeof(GridLength))
            {
                return ((GridLength)value).Value < Cutoff;
            }
            return false;
                   
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
