using MonsterConverterInterface;
using System;
using System.Globalization;
using System.Windows.Data;

namespace OTMonsterConverter
{
    [ValueConversion(typeof(ConvertError), typeof(string))]
    public sealed class ConvertErrorImageToImageFileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ConvertError)value)
            {
                case ConvertError.Error:
                    return "/Images/error.png";
                case ConvertError.Success:
                    return "/Images/success.png";
                case ConvertError.Warning:
                    return "/Images/warning.png";
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
