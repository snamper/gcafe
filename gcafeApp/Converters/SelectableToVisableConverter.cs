﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using gcafeApp.gcafeSvc;

namespace gcafeApp.Converters
{
    public class SelectableToVisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (((SetmealItem)value).OptionItems != null)
            {
                if (((SetmealItem)value).OptionItems.Count > 0)
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
