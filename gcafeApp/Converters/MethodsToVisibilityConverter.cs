using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace gcafeApp.Converters
{
    public class MethodsToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility rtn = Visibility.Collapsed;

            if (value != null)
            {
                ObservableCollection<gcafeSvc.Method> methods = (ObservableCollection<gcafeSvc.Method>)value;
                if (methods.Count > 0)
                    rtn = Visibility.Visible;
            }

            return rtn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
