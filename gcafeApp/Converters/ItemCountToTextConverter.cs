using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Data;

namespace gcafeApp.Converters
{
    public class ItemCountToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if (value.GetType() != typeof(ICollection))
            //    throw new InvalidOperationException("必须是ICollection类型");

            //return string.Format("这个套餐有{0}项内容", ((ICollection)value).Count);

            return string.Format("这个套餐有{0}项内容", ((ObservableCollection<gcafeSvc.SetmealItem>)value).Count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
