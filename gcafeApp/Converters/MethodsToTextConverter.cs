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
    public class MethodsToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string rtn = string.Empty;

            if (value != null)
            {
                rtn = "做法: ";

                ObservableCollection<gcafeSvc.Method> methods = (ObservableCollection<gcafeSvc.Method>)value;
                foreach (var method in methods)
                    rtn += method.Name + " ";
            }

            //if (value.GetType() == typeof(gcafeSvc.MenuItem))
            //{
            //    gcafeSvc.MenuItem mi = (gcafeSvc.MenuItem)value;
            //    if (mi.Methods != null)
            //    {
            //        foreach (var method in mi.Methods)
            //            rtn += method.name + " ";
            //    }
            //}
            //else if (value.GetType() == typeof(gcafeSvc.SetmealItem))
            //{

            //}

            return rtn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
