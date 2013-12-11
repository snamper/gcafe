using System;
using System.Windows;

namespace gcafeApp.Controls
{
    public interface IDataPickerPage
    {
        /// <summary>
        /// 用来记录台号，菜品和数量，全都统一用string类型
        /// </summary>
        string Value { get; set; }

        /// <summary>
        /// Sets the flow direction on any children controls that need it.
        /// </summary>
        /// <param name="flowDirection">Flow direction to set.</param>
        void SetFlowDirection(FlowDirection flowDirection);
    }
}
