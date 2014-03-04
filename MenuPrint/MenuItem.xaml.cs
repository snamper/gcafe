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
using System.Data.OleDb;

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

            using (var conn = new OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=D:\\gwr\\DATA\\lygOrder.dbc"))
            //using (var conn = new SqlConnection(@"Data Source=(localdb)\Projects;Initial Catalog=gcafe;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework"))
            {
                conn.Open();

                DataTable table = new DataTable();
                //using (var adapter = new SqlDataAdapter("SELECT * FROM menu", conn))
                using (var adapter = new OleDbDataAdapter("SELECT productno, prodname FROM product WHERE locked = 0", conn))
                {
                    adapter.Fill(table);

                    int start = 500;
                    int count = 20;
                    int i = 0;
                    foreach (DataRow r in table.Rows)
                    {
                        if (i++ > start)
                        {
                            if (i > (start + count))
                                break;

                            MenuItemDetail mi = new MenuItemDetail();
                            mi.Code = ((string)r["productno"]).Trim();
                            mi.Desc = ((string)r["prodname"]).Trim();
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
