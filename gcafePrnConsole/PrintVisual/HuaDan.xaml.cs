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
    /// Interaction logic for HuaDan.xaml
    /// </summary>
    public partial class HuaDan : UserControl
    {
        public HuaDan()
        {
            InitializeComponent();

            Time = string.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString());
        }

        public string Department
        {
            set
            {
                tbDepart.Text = value;
            }
        }

        public string TableNum
        {
            set
            {
                tbTableNum.Text = value;
            }
        }

        public string SerialNum
        {
            set
            {
                tbSerialNum.Text = value;
            }
        }

        public string StaffName
        {
            set
            {
                tbWaiter.Text = value;
            }
        }

        public string OrderNum
        {
            set
            {
                tbOrderNum.Text = value;
            }
        }

        public string Time
        {
            set
            {
                tbTime.Text = value;
            }
        }

        public void AddItem(order_detail orderDetail)
        {
            Size size;

            // 不打印酒吧
            if (orderDetail.menu.number.Substring(0, 2) == "22")
                return;

            // 套餐品名
            TextBlock tb = new TextBlock()
            {
                FontSize = 22.00,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = System.Windows.VerticalAlignment.Center,
                Text = orderDetail.menu.name,
            };
            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            size = tb.DesiredSize;

            // 加一行
            RowDefinition rd = new RowDefinition();
            rd.Height = new GridLength(((int)size.Width / (int)187 + 1) * 35);
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
                Text = orderDetail.quantity.ToString("f0"),
            };
            Grid.SetColumn(tb, 1);
            Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
            _gridItems.Children.Add(tb);

            // 做法
            if (orderDetail.order_detail_method.Count > 0)
            {
                string zuofa = string.Empty;
                foreach (var method in orderDetail.order_detail_method)
                {
                    if (string.IsNullOrEmpty(zuofa))
                        zuofa = method.method.name;
                    else
                        zuofa += "，" + method.method.name;
                }

                tb = new TextBlock()
                {
                    FontSize = 22.00,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Text = zuofa,
                };
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                size = tb.DesiredSize;

                // 加一行
                rd = new RowDefinition();
                rd.Height = new GridLength(((int)size.Width / (int)187 + 1) * 35);
                _gridItems.RowDefinitions.Add(rd);

                // 将做法加入到grid中
                Grid.SetColumn(tb, 0);
                Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
                _gridItems.Children.Add(tb);
            }


            if (orderDetail.order_detail_setmeal.Count > 0)
            {
                foreach (var setmeal in orderDetail.order_detail_setmeal)
                {
                    // 不打印酒吧
                    if (setmeal.menu.number.Substring(0, 2) == "22")
                        return;

                    // 套餐内容
                    tb = new TextBlock()
                    {
                        FontSize = 22.00,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center,
                        Text = ">>" + setmeal.menu.name,
                    };
                    tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    size = tb.DesiredSize;

                    // 加一行
                    rd = new RowDefinition();
                    //var ss = _gridItems.ColumnDefinitions[0].Width;
                    rd.Height = new GridLength(((int)size.Width / (int)187 + 1) * 35);
                    _gridItems.RowDefinitions.Add(rd);

                    // 将套餐内容加入到grid中
                    Grid.SetColumn(tb, 0);
                    Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
                    _gridItems.Children.Add(tb);

                    // 做法
                    if (setmeal.order_detail_method.Count > 0)
                    {
                        string zuofa = string.Empty;
                        foreach (var method in setmeal.order_detail_method)
                        {
                            if (string.IsNullOrEmpty(zuofa))
                                zuofa = method.method.name;
                            else
                                zuofa += "，" + method.method.name;
                        }

                        tb = new TextBlock()
                        {
                            FontSize = 22.00,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center,
                            Text = zuofa,
                        };
                        tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                        size = tb.DesiredSize;

                        // 加一行
                        rd = new RowDefinition();
                        rd.Height = new GridLength(((int)size.Width / (int)187 + 1) * 35);
                        _gridItems.RowDefinitions.Add(rd);

                        // 将做法加入到grid中
                        Grid.SetColumn(tb, 0);
                        Grid.SetRow(tb, _gridItems.RowDefinitions.Count - 1);
                        _gridItems.Children.Add(tb);
                    }
                }
            }
        }

    }
}
