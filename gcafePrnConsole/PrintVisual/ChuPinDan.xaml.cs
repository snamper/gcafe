using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace gcafePrnConsole.PrintVisual
{
    /// <summary>
    /// Interaction logic for ChuPinDan.xaml
    /// </summary>
    public partial class ChuPinDan : UserControl, IDisposable
    {
        public ChuPinDan()
        {
            InitializeComponent();

            Time = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
        }

        public string Department
        {
            set
            {
                Depart.Text = value;
            }
        }

        public string TableNum
        {
            set
            {
                TableNo.Text = value;
            }
        }

        public string StaffName
        {
            set
            {
                tbWaiter.Text = value;
            }
        }

        public string SerialNum
        {
            set
            {
                _serialNum.Text = value;
            }
        }

        public string PageCnt
        {
            set
            {
                _pageCnt.Text = value;
            }
        }

        public string OrderNum
        {
            set
            {
                _orderNum.Text = value;
            }
        }

        public string Time
        {
            set
            {
                tbTime.Text = value;
            }
        }

        /// <summary>
        /// 增加一条记录到列表中
        /// </summary>
        /// <param name="item"></param>
        /// <param name="showRealQuantity">true表示显示真正数量，false时数量总是1</param>
        public void AddItem(order_detail_setmeal item, bool showRealQuantity = false)
        {
            //StaffName = item.order_detail.staff.name;
            //TableNum = item.order_detail.order.table_no;
            //OrderNum = item.order_detail.order.order_num;

            // 套餐品名
            TextBlock tb = new TextBlock() { 
                FontSize = 22.00, 
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = item.order_detail.menu.name,
            };

            // 加一行
            RowDefinition rd = new RowDefinition();
            rd.Height = GridLength.Auto;
            _gridItems.RowDefinitions.Add(rd);

            // 将品名加入到grid中
            Grid.SetColumn(tb, 0);
            Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
            _gridItems.Children.Add(tb);

            // 套餐内容
            tb = new TextBlock()
            {
                FontSize = 22.00,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = ">>" + item.menu.name,
            };

            // 加一行
            rd = new RowDefinition();
            //var ss = _gridItems.ColumnDefinitions[0].Width;
            rd.Height = GridLength.Auto;
            _gridItems.RowDefinitions.Add(rd);

            // 将套餐内容加入到grid中
            Grid.SetColumn(tb, 0);
            Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
            _gridItems.Children.Add(tb);

            // 数量
            tb = new TextBlock() {
                FontSize = 22.00,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = showRealQuantity ? item.order_detail.quantity.ToString() : "1",
            };
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
            _gridItems.Children.Add(tb);

            // 做法
            if (item.order_detail_method != null && item.order_detail_method.Count > 0)
            {
                string zuofa = string.Empty;
                foreach (var method in item.order_detail_method)
                {
                    if (string.IsNullOrEmpty(zuofa))
                        zuofa = method.method.name;
                    else
                        zuofa += "，" + method.method.name;
                }

                // 做法
                tb = new TextBlock()
                {
                    FontSize = 22.00,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Text = zuofa,
                };

                // 加一行
                rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                _gridItems.RowDefinitions.Add(rd);

                // 将做法加入到grid中
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
                _gridItems.Children.Add(tb);
            }
        }

        public void AddItem(order_detail item, bool showRealQuantity = false)
        {
            //StaffName = item.staff.name;
            //TableNum = item.order.table_no;
            //OrderNum = item.order.order_num;

            // 品名
            TextBlock tb = new TextBlock()
            {
                FontSize = 22.00,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = item.menu.name,
            };

            // 加一行
            RowDefinition rd = new RowDefinition();
            rd.Height = GridLength.Auto;
            _gridItems.RowDefinitions.Add(rd);

            // 将品名加入到grid中
            Grid.SetColumn(tb, 0);
            Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
            _gridItems.Children.Add(tb);

            // 数量
            tb = new TextBlock()
            {
                FontSize = 22.00,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = showRealQuantity ? item.quantity.ToString() : "1",
            };
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
            _gridItems.Children.Add(tb);

            // 做法
            if (item.order_detail_method != null && item.order_detail_method.Count > 0)
            {
                string zuofa = string.Empty;
                foreach (var method in item.order_detail_method)
                {
                    if (string.IsNullOrEmpty(zuofa))
                        zuofa = method.method.name;
                    else
                        zuofa += "，" + method.method.name;
                }

                // 做法
                tb = new TextBlock()
                {
                    FontSize = 22.00,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Text = zuofa,
                };

                // 加一行
                rd = new RowDefinition();
                rd.Height = GridLength.Auto;
                _gridItems.RowDefinitions.Add(rd);

                // 将做法加入到grid中
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
                _gridItems.Children.Add(tb);
            }
        }

        public void Dispose()
        {
        }
    }
}
