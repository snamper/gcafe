using System;
using System.Globalization;

namespace gcafeApp.Controls
{
    public class MenuPicker : DataPickerBase
    {
        public MenuPicker()
        {
            DefaultStyleKey = typeof(MenuPicker);
            Value = "请点击这里添加菜品";
        }
    }
}
