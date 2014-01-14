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
using gcafeWeb40;

namespace gcafeWebFox
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "gcafeSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select gcafeSvc.svc or gcafeSvc.svc.cs at the Solution Explorer and start debugging.
    public class gcafeSvc : IgcafeSvc
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

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
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

                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
                {
                    conn.Open();

                    string sql = string.Format("SELECT pricetype FROM sysinfo");

                    // 看是否节日价
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        if (Int32.Parse((string)cmd.ExecuteScalar()) == 2)
                            isFestival = true;
                    }

                    sql = string.Format("SELECT `TRIM`(productno) AS Expr1, `TRIM`(prodname) AS Expr2, price, fprice FROM product WHERE (productno LIKE '{0}%') AND (len(`TRIM`(productno)) > 4) AND (locked = 0)", cataId);

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
                                    Name = prodName,
                                    Price = price,
                                    Unit = "份",
                                    SetmealItems = new List<SetmealItem>(),
                                };

                                #region 看是否有套餐内容
                                // 看是否有套餐内容
                                sql = string.Format("SELECT `TRIM`(subprod.product2) AS Expr1, `TRIM`(product.prodname) As Expr2 FROM product, subprod WHERE (product.productno = subprod.product2) AND (subprod.productno = '{0}')", prodNo);
                                using (var cmd1 = new OleDbCommand(sql, conn))
                                {
                                    using (var reader1 = cmd1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            string subProdNo = reader1.GetString(0).Trim();
                                            string subProdName = reader1.GetString(1).Trim();

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

                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
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
                                sql = string.Format("SELECT `TRIM`(subprod.product2) AS Expr1, `TRIM`(product.prodname) As Expr2 FROM product, subprod WHERE (product.productno = subprod.product2) AND (subprod.productno = '{0}')", prodNo);
                                using (var cmd1 = new OleDbCommand(sql, conn))
                                {
                                    using (var reader1 = cmd1.ExecuteReader())
                                    {
                                        while (reader1.Read())
                                        {
                                            string subProdNo = reader1.GetString(0).Trim();
                                            string subProdName = reader1.GetString(1).Trim();

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
                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
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
                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
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
                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
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
            int branchId = Int32.Parse(ConfigurationManager.AppSettings.GetValues("BranchID")[0]);
            List<TableInfo> tableInfoList = new List<TableInfo>();

            _log.Trace(TraceMessage());

            try
            {
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
            _log.Trace(TraceMessage());

            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return true;
        }

        public List<Method> GetMethods()
        {
            _log.Trace(TraceMessage());

            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return new List<Method>();
        }

        public List<MethodCatalog> GetMethodCatalogs()
        {
            List<MethodCatalog> rtn = new List<MethodCatalog>();

            _log.Trace(TraceMessage());

            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return rtn;
        }

        public string OrderMeal(string deviceId, int staffId, string tableNum, List<MenuItem> meals)
        {
            string rtn = string.Empty;
            int orderId;

            _log.Trace(TraceMessage());

            try
            {
                //using (gcafePrnSvc.IgcafePrnClient _gcafePrn = new gcafePrnSvc.IgcafePrnClient())
                //{
                //    _gcafePrn.PrintChuPing(orderId, -1, false);
                //    _gcafePrn.PrintHuaDan(orderId, -1, false);
                //    _gcafePrn.PrintLiuTai(orderId, -1, false);
                //}
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));

                //if (_gcafePrn.State == CommunicationState.Faulted)
                //   _gcafePrn = new gcafePrnSvc.IgcafePrnClient();
            }

            _log.Trace(TraceMessage());

            return "点菜成功";
        }

        public List<MenuItem> GetOrderDetailByOrderNum(string orderNum)
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            //return menuItems;

            _log.Trace(TraceMessage());

            try
            {
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("{0}, msg:{1}", TraceMessage(), ex.Message));
            }

            _log.Trace(TraceMessage());

            return menuItems;
        }

        public Staff GetStaffByNum(string DeviceId, string Num)
        {
            Staff staff = null;

            _log.Trace(TraceMessage());

            try
            {
                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
                {
                    conn.Open();

                    string sql = string.Format("SELECT idno, name, pwdppc FROM staff WHERE idno = '{0}'", Num);
                    using (var cmd = new OleDbCommand(sql, conn))
                    {
                        OleDbDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            staff = new Staff() { 
                                Number = reader.GetString(0).Trim(),
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
                using (var conn = new OleDbConnection(ConfigurationManager.AppSettings.GetValues("foxproPath")[0]))
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

    }
}
