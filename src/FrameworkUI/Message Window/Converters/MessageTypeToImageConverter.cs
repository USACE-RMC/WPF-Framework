using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using FrameworkInterfaces;

namespace FrameworkUI.MessageWindow.Converters
{
    public class MessageTypeToImageConverter : IValueConverter
    {
        public ImageSource ErrorImageSource { get; set; }
        public ImageSource MessageImageSource { get; set; }
        public ImageSource WarningImageSource { get; set; }
        public ImageSource EventImageSource { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((MessageType) value)
            {
                case MessageType.Error: return ErrorImageSource;
                case MessageType.Warning: return WarningImageSource;
                case MessageType.Message: return MessageImageSource;
                case MessageType.Event: return EventImageSource;
                default: throw new ArgumentOutOfRangeException("value");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
