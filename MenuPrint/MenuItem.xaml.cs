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
using System.Data;
using System.Data.SqlClient;

namespace MenuPrint
{
    /// <summary>
    /// Interaction logic for MenuItem.xaml
    /// </summary>
    public partial class MenuItem : UserControl
    {
        public MenuItem()
        {
            InitializeComponent();

            using (var conn = new SqlConnection(@"Data Source=(localdb)\Projects;Initial Catalog=gcafe;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework"))
            {
                conn.Open();

                DataTable table = new DataTable();
                using (var adapter = new SqlDataAdapter("SELECT * FROM menu", conn))
                {
                    adapter.Fill(table);

                    int start = 500;
                    int count = 200;
                    int i = 0;
                    foreach (DataRow r in table.Rows)
                    {
                        if (i++ > start)
                        {
                            if (i > (start + count))
                                break;

                            MenuItemDetail mi = new MenuItemDetail();
                            mi.Code = (string)r["number"];
                            mi.Desc = (string)r["name"];
                            mi.Padding = new Thickness(25);

                            wp.Children.Add(mi);
                        }
                    }
                }

                conn.Close();
            }
        }

        public int WPWidth
        {
            set
            {
                wp.Width = value;
            }
        }
    }
}
