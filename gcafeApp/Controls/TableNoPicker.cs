using System;
using System.Globalization;

namespace gcafeApp.Controls
{
    public class TableNoPicker : DataPickerBase
    {
        public TableNoPicker()
        {
            DefaultStyleKey = typeof(TableNoPicker);
            Value = "请点击这里选择台号";
        }
    }
}
