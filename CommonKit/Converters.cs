using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CommonKit
{
    public class LambdaConverter : IValueConverter
    {
        readonly Func<object, Type, object, CultureInfo, object?> _convert;
        readonly Func<object, Type, object, CultureInfo, object?>? _convertBack;

        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            _convert.Invoke(value, targetType, parameter, culture);

        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            _convertBack?.Invoke(value, targetType, parameter, culture);

        public LambdaConverter(
            Func<object, Type, object, CultureInfo, object?> convert,
            Func<object, Type, object, CultureInfo, object?>? convertBack = null)
        {
            _convert = convert;
            _convertBack = convertBack;
        }
    }

    public static class Converters
    {
        public static IValueConverter DurationToSecondsConverter { get; } = new LambdaConverter(
            (duration, _, _, _) => ((Duration)duration).HasTimeSpan ? ((Duration)duration).TimeSpan.TotalSeconds : 0,
            (totalSeconds, _, _, _) => new Duration(TimeSpan.FromSeconds((double)totalSeconds)));

        public static IValueConverter TimespanToSecondsConverter { get; } = new LambdaConverter(
            (timeSpan, _, _, _) => ((TimeSpan)timeSpan).TotalSeconds,
            (totalSeconds, _, _, _) => TimeSpan.FromSeconds((double)totalSeconds));

        public static IValueConverter SecondsToTimespanConverter { get; } = new LambdaConverter(
            (totalSeconds, _, _, _) => TimeSpan.FromSeconds((double)totalSeconds),
            (timeSpan, _, _, _) => ((TimeSpan)timeSpan).TotalSeconds);
    }
}
