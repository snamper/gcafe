using System;
using System.Transactions;
using System.Configuration;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Runtime.CompilerServices;
using System.Data.OleDb;

namespace gcafeFoxproSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "gcafeSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select gcafeSvc.svc or gcafeSvc.svc.cs at the Solution Explorer and start debugging.
    public class gcafeSvc : IgcafeSvc, IDisposable
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        //private static gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient();

        private static string TraceMessage(
            string memberName = "",
            int lineNum = 0)
        {
            return string.Format("Call {0} at {1}", memberName, lineNum);
        }

        public List<MenuCatalog> GetMenuCatalogs(string DeviceId, string rootCata)
        {
            List<MenuCatalog> cataList = new List<MenuCatalog>();

            if (rootCata == "厨房")
                rootCata = "11";
            else
                rootCata = "22";

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT `TRIM`(productno) AS Expr1, prodname, len(`TRIM`(productno)) AS Expr1 FROM product WHERE (len(`TRIM`(productno)) = 4) AND (locked = 0) AND (LEFT(productno, 2) = '{0}') ORDER BY productno", rootCata);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            cataList.Add(new MenuCatalog() { ID = Int32.Parse(reader.GetString(0)), Name = reader.GetString(1).Trim() });
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return cataList;
        }

        public List<MenuItem> GetMenuItemsByCatalogId(string DeviceId, int cataId)
        {
            List<MenuItem> menuList = new List<MenuItem>();

            _log.Trace(TraceMessage());

            try
            {
                bool isFestival = false;

                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT pricetype FROM sysinfo");

                    // 看是否节日价
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        if (Int32.Parse((string)cmd.ExecuteScalar()) == 2)
                            isFestival = true;
                    }

                    if (cataId > 0)
                        sql = string.Format("SELECT `TRIM`(productno) AS Expr1, `TRIM`(prodname) AS Expr2, price, fprice FROM product WHERE (productno LIKE '{0}%') AND (len(`TRIM`(productno)) > 4) AND (locked = 0)", cataId);
                    else
                        sql = string.Format("SELECT `TRIM`(productno) AS Expr1, `TRIM`(prodname) AS Expr2, price, fprice FROM product WHERE (len(`TRIM`(productno)) > 4) AND (locked = 0)");

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string prodNo = reader.GetString(0).Trim();
                                string prodName = reader.GetString(1).Trim();
                                decimal price = reader.GetDecimal(2);
                                decimal fprice = reader.GetDecimal(3);
                                if (isFestival)
                                    if (fprice > price)
                                        price = fprice;

                                MenuItem menuItem = new MenuItem()
                                {
                                    ID = Int32.Parse(prodNo),
                                    Number = prodNo,
                                    Name = prodName,
                                    Price = price,
                                    Unit = "份",
                                    SetmealItems = new List<SetmealItem>(),
                                };

                                #region 看是否有套餐内容
                                // 看是否有套餐内容
                                sql = string.Format("SELECT `TRIM`(subprod.product2) AS Expr1, `TRIM`(product.prodname) As Expr2, subprod.real FROM product, subprod WHERE (product.productno = subprod.product2) AND (subprod.productno = '{0}')", prodNo);
                                using (var cmd1 = new OleDbCommand(sql, conn))
                                {
                                    using (var reader1 = cmd1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            string subProdNo = reader1.GetString(0).Trim();
                                            string subProdName = reader1.GetString(1).Trim();
                                            bool subReal = reader1.GetBoolean(2);

                                            if (!menuItem.IsSetmeal)
                                                menuItem.IsSetmeal = true;

                                            menuItem.SetmealItems.Add(new SetmealItem()
                                            {
                                                MenuID = Int32.Parse(subProdNo),
                                                Name = subProdName,
                                                Unit = "份",
                                                OptionItems = new List<SetmealItem>(),
                                            });

                                            #region 看是否有可选内容
                                            // 看是否有可选内容
                                            if (subReal)
                                            {
                                                menuItem.SetmealItems.Last().OptionItems.Add(new SetmealItem()
                                                    {
                                                        MenuID = Int32.Parse(subProdNo),
                                                        Name = subProdName,
                                                        Unit = "份",
                                                    });

                                                sql = string.Format("SELECT subprod2.realname, `TRIM`(product.prodname) As Expr2 FROM product, subprod2 WHERE (product.productno = subprod2.realname) AND (subprod2.productno = '{0}') AND (subprod2.subprod = '{1}')", prodNo, subProdNo);
                                                using (var cmd2 = new OleDbCommand(sql, conn))
                                                {
                                                    var reader2 = cmd2.ExecuteReader();
                                                    while (reader2.Read())
                                                    {
                                                        subProdNo = reader2.GetString(0).Trim();
                                                        subProdName = reader2.GetString(1).Trim();

                                                        menuItem.SetmealItems.Last().OptionItems.Add(new SetmealItem()
                                                        {
                                                            MenuID = Int32.Parse(subProdNo),
                                                            Name = subProdName,
                                                            Unit = "份",
                                                        });
                                                    }
                                                }
                                            }
                                            #endregion 看是否有可选内容
                                        }
                                    }
                                }
                                #endregion 看是否有套餐内容

                                menuList.Add(menuItem);
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return menuList;
        }

        public MenuItem GetMenuItemByNumber(string DeviceId, string number)
        {
            MenuItem menuItem = null;

            _log.Trace(TraceMessage());

            try
            {
                bool isFestival = false;

                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT pricetype FROM sysinfo");

                    // 看是否节日价
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        if (Int32.Parse((string)cmd.ExecuteScalar()) == 2)
                            isFestival = true;
                    }

                    sql = string.Format("SELECT `TRIM`(productno) AS Expr1, `TRIM`(prodname) AS Expr2, price, fprice FROM product WHERE (productno = '{0}') AND (len(`TRIM`(productno)) > 4) AND (locked = 0)", number);

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string prodNo = reader.GetString(0).Trim();
                                string prodName = reader.GetString(1).Trim();
                                decimal price = reader.GetDecimal(2);
                                decimal fprice = reader.GetDecimal(3);
                                if (isFestival)
                                    if (fprice > price)
                                        price = fprice;

                                menuItem = new MenuItem()
                                {
                                    ID = Int32.Parse(prodNo),
                                    Name = prodName,
                                    Price = price,
                                    Unit = "份",
                                    SetmealItems = new List<SetmealItem>(),
                                };

                                #region 看是否有套餐内容
                                // 看是否有套餐内容
                                sql = string.Format("SELECT `TRIM`(subprod.product2) AS Expr1, `TRIM`(product.prodname) As Expr2, subprod.real FROM product, subprod WHERE (product.productno = subprod.product2) AND (subprod.productno = '{0}')", prodNo);
                                using (var cmd1 = new OleDbCommand(sql, conn))
                                {
                                    using (var reader1 = cmd1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            string subProdNo = reader1.GetString(0).Trim();
                                            string subProdName = reader1.GetString(1).Trim();
                                            bool subReal = reader1.GetBoolean(2);

                                            if (!menuItem.IsSetmeal)
                                                menuItem.IsSetmeal = true;

                                            menuItem.SetmealItems.Add(new SetmealItem()
                                            {
                                                MenuID = Int32.Parse(subProdNo),
                                                Name = subProdName,
                                                Unit = "份",
                                                OptionItems = new List<SetmealItem>(),
                                            });

                                            #region 看是否有可选内容
                                            // 看是否有可选内容
                                            if (subReal)
                                            {
                                                menuItem.SetmealItems.Last().OptionItems.Add(new SetmealItem()
                                                {
                                                    MenuID = Int32.Parse(subProdNo),
                                                    Name = subProdName,
                                                    Unit = "份",
                                                });

                                                sql = string.Format("SELECT subprod2.realname, `TRIM`(product.prodname) As Expr2 FROM product, subprod2 WHERE (product.productno = subprod2.realname) AND (subprod2.productno = '{0}') AND (subprod2.subprod = '{1}')", prodNo, subProdNo);
                                                using (var cmd2 = new OleDbCommand(sql, conn))
                                                {
                                                    var reader2 = cmd2.ExecuteReader();
                                                    while (reader2.Read())
                                                    {
                                                        subProdNo = reader2.GetString(0).Trim();
                                                        subProdName = reader2.GetString(1).Trim();

                                                        menuItem.SetmealItems.Last().OptionItems.Add(new SetmealItem()
                                                            {
                                                                MenuID = Int32.Parse(subProdNo),
                                                                Name = subProdName,
                                                                Unit = "份",
                                                            });
                                                    }
                                                }
                                            }
                                            #endregion 看是否有可选内容
                                        }
                                    }
                                }
                                #endregion 看是否有套餐内容
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return menuItem;
        }

        public TableInfo TableOpr(string DeviceId, TableInfo tableInfo, string oldTableNum, TableOprType oprType)
        {
            TableInfo rtn = null;

            _log.Trace(TraceMessage());

            try
            {
                switch (oprType)
                {
                    case TableOprType.OpenTable:
                        rtn = OpenTable(DeviceId, tableInfo);
                        break;

                    case TableOprType.ChangeTable:
                        rtn = ChangeTable(DeviceId, tableInfo, oldTableNum);
                        break;

                    case TableOprType.ChangeCustomerNum:
                        break;
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        TableInfo ChangeCustomerNum(string DeviceId, TableInfo tableInfo)
        {
            TableInfo rtn = null;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("UPDATE orders SET personum = {0} WHERE (tableno = '{1}') AND (paid = 0)", tableInfo.CustomerNum, tableInfo.Num);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();

                        rtn = tableInfo;
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        TableInfo ChangeTable(string DeviceId, TableInfo tableInfo, string oldTableNum)
        {
            TableInfo rtn = null;
            string orderNo = null;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    #region 检查是否已开台
                    // 检查是否已开台
                    string sql = string.Format("SELECT orderno FROM orders WHERE (tableno = '{0}') AND (paid = 0)", tableInfo.Num);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        orderNo = (string)cmd.ExecuteScalar();
                    }
                    #endregion 检查是否已开台

                    if (orderNo == null)
                    {
                        sql = string.Format("UPDATE orders SET tableno = '{0}' WHERE (tableno = '{1}') AND (paid = 0)", tableInfo.Num, oldTableNum);
                        using (var cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();

                            rtn = tableInfo;
                        }

                        sql = string.Format("UPDATE poh SET tableno = '{0}' WHERE (orderno = '{1}')", tableInfo.Num, tableInfo.OrderNum);
                        using (var cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();

                            rtn = tableInfo;
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        TableInfo OpenTable(string DeviceId, TableInfo tableInfo)
        {
            TableInfo rtn = null;
            string orderNo = string.Empty;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    #region 检查是否已开台
                    // 检查是否已开台
                    string sql = string.Format("SELECT orderno FROM orders WHERE (tableno = '{0}') AND (paid = 0)", tableInfo.Num);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        orderNo = (string)cmd.ExecuteScalar();
                    }
                    #endregion 检查是否已开台

                    if (orderNo == null)
                    {
                        orderNo = GenerateOrderNo();
                        sql = string.Format("INSERT INTO orders(orderno, ordertime, tableno, custkind, personum, waiter, paid) VALUE('{0}', {1}, '{2}', ' ', {3}, '{4}', 0)",
                            orderNo, "{ fn NOW() }", tableInfo.Num, tableInfo.CustomerNum, tableInfo.OpenTableStaff.Number);

                        using (var cmd = new OleDbCommand(sql, conn))
                        {
                            cmd.ExecuteNonQuery();

                            rtn = tableInfo;
                            rtn.OrderNum = orderNo;
                        }
                    }


                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public List<TableInfo> GetTablesInfo(string DeviceId)
        {
            List<TableInfo> tableInfoList = new List<TableInfo>();

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT orders.orderno, orders.ordertime, orders.tableno, orders.personum, orders.waiter FROM orders WHERE (orders.paid = 0)");
                    //string sql = string.Format("SELECT orders.orderno, orders.ordertime, orders.tableno, orders.personum, orders.waiter, staff.name FROM orders, staff WHERE (orders.waiter = staff.idno) AND (orders.paid = 0)");
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string orderNo = reader.GetString(0).Trim();
                            DateTime orderTime = reader.GetDateTime(1);
                            string tableNo = reader.GetString(2).Trim();
                            int customerNum = reader.GetInt32(3);
                            string staffNum = reader.GetString(4).Trim();
                            Staff staff = GetStaffByNum("", staffNum);
                            string staffName = staff != null ? staff.Name : "未知姓名";
                            decimal amount = 0;


                            sql = string.Format("SELECT price, quantity FROM orditem WHERE (orderno = '{0}')", orderNo);
                            using (var cmd1 = new OleDbCommand(sql, conn))
                            {
                                OleDbDataReader reader1 = cmd1.ExecuteReader();
                                while (reader1.Read())
                                {
                                    decimal price = reader1.GetDecimal(0);
                                    int quantity = reader1.GetInt32(1);

                                    amount += price * quantity;
                                }
                            }

                            tableInfoList.Add(new TableInfo()
                            {
                                CustomerNum = customerNum,
                                Num = tableNo,
                                OrderNum = orderNo,
                                OpenTableTime = orderTime,
                                OpenTableStaff = new Staff() { Number = staffNum, Name = staffName },
                                Amount = amount,
                            });
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return tableInfoList;
        }

        public bool IsTableAvaliable(string tableNum)
        {
            bool rtn = false;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    #region 检查是否已开台
                    // 检查是否已开台
                    string sql = string.Format("SELECT orderno FROM orders WHERE (tableno = '{0}') AND (paid = 0)", tableNum);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        if (cmd.ExecuteScalar() == null)
                            rtn = true;
                    }
                    #endregion 检查是否已开台

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public List<Method> GetMethods()
        {
            List<Method> methodList = new List<Method>();

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = @"SELECT product.productno, product.prodname, tb1.productno AS Expr1, tb1.prodname AS Expr2 " +
                        "FROM product, product tb1 " +
                        "WHERE LEFT(product.productno, 4) = tb1.productno AND (product.productno LIKE '33%') AND (len(`TRIM`(product.productno)) > 4) " +
                        "ORDER BY product.productno";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            string prodNo = reader.GetString(0).Trim();
                            string prodName = reader.GetString(1).Trim();
                            string cataProdNo = reader.GetString(2).Trim();
                            string cataProdName = reader.GetString(3).Trim();

                            methodList.Add(new Method()
                            {
                                ID = Int32.Parse(prodNo),
                                Name = prodName,
                                MethodCatalog = new MethodCatalog()
                                {
                                    ID = Int32.Parse(cataProdNo),
                                    Name = cataProdName,
                                }
                            });
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return methodList;
        }

        public List<MethodCatalog> GetMethodCatalogs()
        {
            List<MethodCatalog> rtn = new List<MethodCatalog>();

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = @"SELECT product.productno, product.prodname, tb1.productno AS Expr1, tb1.prodname AS Expr2 " +
                        "FROM product, product tb1 " +
                        "WHERE LEFT(product.productno, 4) = tb1.productno AND (product.productno LIKE '33%') AND (len(`TRIM`(product.productno)) > 4) " +
                        "ORDER BY product.productno";

                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        string prevCataNo = string.Empty;
                        while (reader.Read())
                        {
                            string prodNo = reader.GetString(0).Trim();
                            string prodName = reader.GetString(1).Trim();
                            string cataProdNo = reader.GetString(2).Trim();
                            string cataProdName = reader.GetString(3).Trim();

                            if (prevCataNo != cataProdNo)
                            {
                                rtn.Add(new MethodCatalog()
                                {
                                    ID = Int32.Parse(cataProdNo),
                                    Name = cataProdName,
                                    Methods = new List<Method>(),
                                });

                                prevCataNo = cataProdNo;
                            }

                            rtn.Last().Methods.Add(new Method()
                            {
                                ID = Int32.Parse(prodNo),
                                Name = prodName,
                            });
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public string OrderMeal(string deviceId, int staffId, TableInfo tableInfo, List<MenuItem> meals)
        {
            string rtn = string.Empty;
            OleDbTransaction trans = null;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    trans = conn.BeginTransaction();

                    string strNow = string.Format("{0}^{1}{2}", "{", System.DateTime.Now.ToString("u"), "}");
                    string strMethod = string.Empty;
                    string strSetmealMethod = string.Empty;
                    string sql = string.Empty;

                    // 取orderNum, 应该是不需要这步的，但设计错误，只能这样，以后再改
                    sql = string.Format("SELECT orderno FROM orders WHERE (paid = 0) AND (tableno = '{0}')", tableInfo.Num);
                    using (var cmd = new OleDbCommand(sql, conn, trans))
                    {
                        tableInfo.OrderNum = (string)cmd.ExecuteScalar();
                        if (tableInfo.OrderNum == null)
                            throw new Exception("没有相应的台号");
                        else
                            tableInfo.OrderNum = tableInfo.OrderNum.Trim();
                    }

                    int cnt = 0;
                    foreach (MenuItem menuItem in meals)
                    {
                        #region 插入orditem表
                        // 从product表查出price, price2, fprice, productnn
                        sql = string.Format("SELECT prodname, price, price2, fprice, productnn FROM product WHERE (productno = '{0}')", menuItem.ID);
                        using (var cmd = new OleDbCommand(sql, conn, trans))
                        {
                            string prodName, prodNn;
                            decimal price, price2, fprice;

                            OleDbDataReader reader = cmd.ExecuteReader();
                            if (reader.Read())
                            {
                                prodName = reader.GetString(0).Trim();
                                price = reader.GetDecimal(1);
                                price2 = reader.GetDecimal(2);
                                fprice = reader.GetDecimal(3);
                                prodNn = reader.GetString(4).Trim();

                                sql = string.Format("INSERT INTO orditem(ordertime, orderno, productno, prodname, price, price2, quantity, note1name, note2name, note1no, price1, quantity1, add10, quantity2, discount, machineid, taiji, memberno, amt, productnn, note2no, waiter) VALUES({0}, '{1}', '{2}', '{3}', {4}, {5}, {6}, '11', '', '', 0, 0, 0, 0, 1, '{7}', 0, '', 0, '{8}', '', '{9:D4}')",
                                    strNow, tableInfo.OrderNum, menuItem.ID, menuItem.Name, menuItem.Price, price2, menuItem.Quantity, "A", prodNn, staffId);

                                using (var cmd1 = new OleDbCommand(sql, conn, trans))
                                {
                                    cmd1.ExecuteNonQuery();
                                }
                            }
                            else
                                throw new Exception(string.Format("{0} 没有对应的产品", 111));
                        }

                        #endregion 插入orditem表

                        // 做法
                        strMethod = string.Empty;
                        if (menuItem.Methods != null && menuItem.Methods.Count > 0)
                        {
                            foreach (Method method in menuItem.Methods)
                            {
                                if (string.IsNullOrEmpty(strMethod))
                                    strMethod += method.Name;
                                else
                                    strMethod += "," + method.Name;
                            }
                        }

                        #region 插入poh表
                        // 插入poh表
                        if (menuItem.SetmealItems != null && menuItem.SetmealItems.Count > 0)
                        {
                            // 这是套餐
                            foreach (SetmealItem setmeal in menuItem.SetmealItems)
                            {
                                strSetmealMethod = string.Empty;

                                if (setmeal.Methods != null && setmeal.Methods.Count > 0)
                                {
                                    foreach (Method method in setmeal.Methods)
                                    {
                                        if (string.IsNullOrEmpty(strSetmealMethod))
                                            strSetmealMethod += method.Name;
                                        else
                                            strSetmealMethod += "," + method.Name;
                                    }
                                }

                                string recMethod = string.Empty;
                                if (!string.IsNullOrEmpty(strMethod))
                                {
                                    recMethod = strMethod;
                                    if (!string.IsNullOrEmpty(strSetmealMethod))
                                        recMethod += ",";
                                }

                                recMethod = strSetmealMethod;

                                sql = string.Format("INSERT INTO poh(department, ordertime, orderno, serialno, prodname, machineid, quantity, tableno, itemno, printgroup, remark1, remark2, waiter, serial) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12:D4}', '{13}')",
                                    setmeal.MenuID.ToString().Substring(0, 2), strNow, tableInfo.OrderNum, GenerateSerialNo(setmeal.MenuID.ToString(), trans), setmeal.Name, "WP", menuItem.Quantity, tableInfo.Num, "0", GetPrintGroup(setmeal.MenuID.ToString(), trans), menuItem.Name, recMethod, staffId, cnt);

                                using (var cmd = new OleDbCommand(sql, conn, trans))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        else
                        {
                            // 这不是套餐
                            sql = string.Format("INSERT INTO poh(department, ordertime, orderno, serialno, prodname, machineid, quantity, tableno, itemno, printgroup, remark1, remark2, waiter, serial) VALUES('{0}', {1}, '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12:D4}', '{13}')",
                                menuItem.ID.ToString().Substring(0, 2), strNow, tableInfo.OrderNum, GenerateSerialNo(menuItem.ID.ToString(), trans), menuItem.Name, "WP", menuItem.Quantity, tableInfo.Num, "0", GetPrintGroup(menuItem.ID.ToString(), trans), "", strMethod, staffId, cnt);

                            using (var cmd = new OleDbCommand(sql, conn, trans))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        #endregion 插入poh表

                        cnt++;
                    }

                    trans.Commit();

                    conn.Close();
                }

                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintLiuTai, tableInfo.OrderNum.Length > 7 ? Int32.Parse(tableInfo.OrderNum.Substring(2)) : Int32.Parse(tableInfo.OrderNum), -1));
                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintChuPin, tableInfo.OrderNum.Length > 7 ? Int32.Parse(tableInfo.OrderNum.Substring(2)) : Int32.Parse(tableInfo.OrderNum), -1));
                Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintHuaDan, tableInfo.OrderNum.Length > 7 ? Int32.Parse(tableInfo.OrderNum.Substring(2)) : Int32.Parse(tableInfo.OrderNum), -1));
                //using (gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient())
                //{
                //    _gcafePrn.PrintChuPing(Int32.Parse(tableInfo.OrderNum.Substring(2)), -1, false);
                //    _gcafePrn.PrintHuaDan(Int32.Parse(tableInfo.OrderNum.Substring(2)), -1, false);
                //    _gcafePrn.PrintLiuTai(Int32.Parse(tableInfo.OrderNum.Substring(2)), -1, false);
                //}
            }
            catch (Exception ex)
            {
                try
                {
                    trans.Rollback();
                }
                catch (Exception e)
                {

                }

                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
                rtn = ex.Message;

                //if (_gcafePrn.State == CommunicationState.Faulted)
                //   _gcafePrn = new gcafePrnSvc.IgcafePrnClient();
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public List<MenuItem> GetOrderDetailByOrderNum(string orderNum)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            orderNum = GetFoxproOrderNo(Int32.Parse(orderNum));

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT ordertime, serial, prodname, quantity, remark1, remark2, waiter FROM poh WHERE orderno = '{0}' ORDER BY ordertime, serial", orderNum);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        MenuItem menuItem = null;

                        DateTime orderTimePrev = System.DateTime.Now;
                        string serialPrev = string.Empty;

                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            DateTime orderTime = reader.GetDateTime(0);
                            string serial = reader.GetString(1).Trim();
                            string prodName = reader.GetString(2).Trim();
                            int quantity = reader.GetInt32(3);
                            string remark1 = reader.GetString(4).Trim();
                            string remark2 = reader.GetString(5).Trim();
                            string waiter = reader.GetString(6).Trim();
                            if (string.IsNullOrEmpty(waiter))
                                waiter = "未知";

                            if (orderTimePrev != orderTime || serialPrev != serial)
                            {
                                if (menuItem != null)
                                    menuItems.Add(menuItem);

                                menuItem = new MenuItem()
                                {
                                    Quantity = quantity,
                                    OrderStaffName = waiter,
                                    OrderTime = orderTime,
                                    IsSetmeal = false,
                                    SetmealItems = new List<SetmealItem>(),
                                    Methods = new List<Method>(),
                                };

                                if (string.IsNullOrEmpty(remark1))
                                {
                                    // 不是套餐
                                    menuItem.Name = prodName;
                                    menuItem.Price = GetFoxproOrderitemPrice(orderNum, orderTime, prodName);
                                    
                                    if (!string.IsNullOrEmpty(remark2))
                                    {
                                        // 有做法
                                        string[] methods = remark2.Split(',');
                                        foreach (string method in methods)
                                            menuItem.Methods.Add(new Method() { Name = method, });
                                    }
                                }
                                else
                                {
                                    // 是套餐
                                    menuItem.IsSetmeal = true;
                                    menuItem.Name = remark1;
                                    menuItem.Price = GetFoxproOrderitemPrice(orderNum, orderTime, remark1);

                                    SetmealItem setmeal = new SetmealItem()
                                    {
                                        Name = prodName,
                                        Methods = new List<Method>(),
                                    };

                                    if (!string.IsNullOrEmpty(remark2))
                                    {
                                        // 有做法
                                        string[] methods = remark2.Split(',');
                                        foreach (string method in methods)
                                            setmeal.Methods.Add(new Method() { Name = method, });
                                    }

                                    menuItem.SetmealItems.Add(setmeal);
                                }

                                // update
                                orderTimePrev = orderTime;
                                serialPrev = serial;
                            }
                            else
                            {
                                // 这一定是套餐内容项
                                SetmealItem setmeal = new SetmealItem()
                                {
                                    Name = prodName,
                                    Methods = new List<Method>(),
                                };

                                if (!string.IsNullOrEmpty(remark2))
                                {
                                    // 有做法
                                    string[] methods = remark2.Split(',');
                                    foreach (string method in methods)
                                        setmeal.Methods.Add(new Method() { Name = method, });
                                }

                                menuItem.SetmealItems.Add(setmeal);
                            }
                        }

                        menuItems.Add(menuItem);
                    }

                    //string sql = string.Format("SELECT ordertime, productno, prodname, price, quantity, waiter FROM orditem WHERE orderno = '{0}' ORDER BY ordertime", orderNum);
                    //using (var cmd = new OleDbCommand(sql, conn))
                    //{
                    //    OleDbDataReader reader = cmd.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        DateTime orderTime = reader.GetDateTime(0);
                    //        string prodNo = reader.GetString(1).Trim();
                    //        string prodName = reader.GetString(2).Trim();
                    //        decimal price = reader.GetDecimal(3);
                    //        int quantity = reader.GetInt32(4);
                    //        string waiter = reader.GetString(5);
                    //        if (waiter != null)
                    //            waiter = waiter.Trim();
                    //        else
                    //            waiter = "未知";

                    //        Staff staff = GetStaffByNum("", waiter);
                    //        waiter = staff != null ? staff.Name : "未知名";

                    //        menuItems.Add(new MenuItem()
                    //        {
                    //            ID = Int32.Parse(prodNo),
                    //            Name = prodName,
                    //            OrderStaffName = waiter,
                    //            OrderTime = orderTime,
                    //            Price = price,
                    //            Quantity = quantity,
                    //            IsSetmeal = false,
                    //        });
                    //    }
                    //}

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return menuItems;
        }

        public int GetTableOrderCount(string orderNum)
        {
            int rtn = -1;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT ordertime FROM orditem WHERE orderno = '{0}' ORDER BY ordertime", orderNum);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        rtn = 0;
                        DateTime prevTime = DateTime.Now;
                        while (reader.Read())
                        {
                            DateTime ot = reader.GetDateTime(0);
                            if (prevTime != ot)
                            {
                                rtn++;
                                prevTime = ot;
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public Staff GetStaffByNum(string DeviceId, string Num)
        {
            Staff staff = null;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    string sql = string.Format("SELECT idno, name, pwdppc FROM staff WHERE idno = '{0}'", Num);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            staff = new Staff()
                            {
                                Number = reader.GetString(0).Trim(),
                                ID = Int32.Parse(reader.GetString(0).Trim()),
                                Name = reader.GetString(1).Trim(),
                                Password = reader.GetString(2).Trim(),
                            };
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return staff;
        }

        public void ReprintLiutaiDan(string orderNum, int prnType)
        {
            Global.PrintTaskMgr.AddTask(new PrintTask(PrintTask.PrintType.PrintLiuTai, orderNum.Length > 7 ? Int32.Parse(orderNum.Substring(2)) : Int32.Parse(orderNum), prnType));
        }

        public void ReprintChupinDan(string orderNum, List<MenuItem> menuItems)
        {

        }

        /// <summary>
        /// 生成poh.serialno
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        int GenerateSerialNo(string productNo, OleDbTransaction trans)
        {
            int serialNo = -1;

            try
            {
                string sql;

                if (productNo.Substring(0, 2) == "11")
                    sql = "SELECT kitchen FROM lastsn";
                else if (productNo.Substring(0, 2) == "22")
                    sql = "SELECT bar FROM lastsn";
                else
                    return -1;

                using (var cmd = new OleDbCommand(sql, trans.Connection, trans))
                {
                    serialNo = Int32.Parse(cmd.ExecuteScalar() as string);
                }

                if (productNo.Substring(0, 2) == "11")
                    sql = string.Format("UPDATE lastsn SET kitchen = '{0}'", serialNo + 1);
                else if (productNo.Substring(0, 2) == "22")
                    sql = string.Format("UPDATE lastsn SET bar = '{0}'", serialNo + 1);

                using (var cmd = new OleDbCommand(sql, trans.Connection, trans))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }

            return serialNo;
        }

        /// <summary>
        /// 取回打印组
        /// </summary>
        /// <param name="productNo"></param>
        /// <returns></returns>
        private string GetPrintGroup(string productNo, OleDbTransaction trans)
        {
            string pg = string.Empty;

            using (var cmd = new OleDbCommand(string.Format("SELECT printgroup FROM product WHERE productno = '{0}'",
                productNo), trans.Connection, trans))
            {
                pg = cmd.ExecuteScalar() as string;
                if (pg != null)
                    pg = pg.Trim();
                else
                    pg = string.Empty;
            }

            return pg;
        }

        /// <summary>
        /// 生成orderno, sysinfo.fax+日期+lastsn.orderno
        /// </summary>
        /// <returns></returns>
        string GenerateOrderNo()
        {
            string strRtn = string.Empty;
            string fax = string.Empty;
            int orderNo = 0;

            try
            {
                using (var conn = new OleDbConnection(Properties.Settings.Default.FoxproPath))
                {
                    conn.Open();

                    // 取 sysinfo.fax
                    using (var cmd = new OleDbCommand("SELECT fax FROM sysinfo", conn))
                    {
                        fax = cmd.ExecuteScalar() as string;
                        fax = fax.Trim();
                    }

                    // 取 lastsn.orderno
                    while (true)
                    {
                        using (var cmd = new OleDbCommand("SELECT orderno FROM lastsn", conn))
                        {
                            orderNo = Int32.Parse(cmd.ExecuteScalar() as string) + 1;

                            // 更新lastsn.orderno
                            using (var cmd1 = new OleDbCommand(string.Format("UPDATE lastsn SET orderno = '{0}'", orderNo), conn))
                            {
                                cmd1.ExecuteNonQuery();
                            }

                            using (var cmd1 = new OleDbCommand(string.Format("SELECT orderno FROM orders WHERE orderno = '{0}'", orderNo), conn))
                            {
                                if (cmd1.ExecuteScalar() == null)
                                    break;
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

            strRtn = string.Format("{0}{1}{2:D2}{3:D2}{4:D4}", fax, System.DateTime.Now.Year.ToString().Substring(2, 2),
                System.DateTime.Now.Month, System.DateTime.Now.Day, orderNo);

            return strRtn;
        }

        decimal GetFoxproOrderitemPrice(string orderNum, DateTime orderTime, string prodName)
        {
            decimal rtn = 0;

            using (var conn = new OleDbConnection(Global.FoxproPath))
            {
                conn.Open();

                string strOrderTime = string.Format("{0}^{1}{2}", "{", orderTime.ToString("u"), "}");
                string sql = string.Format("SELECT price FROM orditem WHERE (orderno = '{0}') AND (ordertime = {1}) AND (prodname = '{2}')", orderNum, strOrderTime, prodName);
                using (var cmd = new OleDbCommand(sql, conn))
                {
                    decimal? price = (decimal?)cmd.ExecuteScalar();
                    if (price != null)
                        rtn = (decimal)price;
                }

                conn.Close();
            }

            return rtn;
        }

        string GetFoxproOrderNo(int orderId)
        {
            string orderNo = string.Empty;

            if (orderId < 1000000000)
            {
                orderNo = string.Format("{0:D7}", orderId);
            }
            else
            {
                using (var conn = new OleDbConnection(Global.FoxproPath))
                {
                    conn.Open();

                    string sql = "SELECT fax FROM sysinfo";
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        string fax = (string)cmd.ExecuteScalar();
                        if (fax != null)
                        {
                            orderNo = fax.Trim() + orderId.ToString();
                        }
                    }

                    conn.Close();
                }
            }

            return orderNo;
        }

        public void Dispose()
        {
            int i = 0;
        }
    }
}
