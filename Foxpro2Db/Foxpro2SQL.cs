using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Sql;
using System.Data.SqlClient;


namespace Foxpro2Db
{
    class Foxpro2SQL : ConvertBase
    {
        public override void Convert(int branchId)
        {
            DbMaintain();
            return;

            MenuCata();

            SqlTransaction trans = null;
            List<string> _sellOuts = new List<string>();

            try
            {
                using (var conn = new OleDbConnection(FoxproConnStr))
                {
                    conn.Open();

                    var soCmd = new OleDbCommand("SELECT productno FROM sellout", conn);
                    var soReader = soCmd.ExecuteReader();
                    while (soReader.Read())
                    {
                        _sellOuts.Add(soReader.GetString(0));
                    }
                    soReader.Close();

                    DataTable prntrTbl = null;
                    using (var conn1 = new OleDbConnection(@"Provider=VFPOLEDB.1;Data Source=D:\gwr\DATA\printt.dbc"))
                    {
                        var soAdapter = new OleDbDataAdapter("SELECT * FROM prntr", conn1);
                        prntrTbl = new DataTable();
                        soAdapter.Fill(prntrTbl);
                    }

                    // 是否节日价
                    soCmd = new OleDbCommand("SELECT pricetype FROM sysinfo", conn);
                    int pType = Int32.Parse((string)soCmd.ExecuteScalar());

                    using (var adapter = new OleDbDataAdapter("SELECT * FROM product", conn))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);

                        using (var dbConn = new SqlConnection(DbConnStr))
                        {
                            dbConn.Open();

                            var cc = new SqlCommand(string.Format("UPDATE sys_info SET is_festival = {0} WHERE branch_id = {1}", pType == 2 ? 1 : 0, branchId), dbConn);
                            cc.ExecuteNonQuery();

                            #region
                            // 插入打印机
                            foreach (DataRow rr in prntrTbl.Rows)
                            {
                                string pgn = (string)rr["Printgroup"];
                                string prntr = ((string)rr["prntr"]).Trim();

                                cc = new SqlCommand(string.Format("SELECT id FROM printer_group WHERE name = '{0}' AND branch_id = {1}", pgn, branchId), dbConn);
                                int? id = (int?)cc.ExecuteScalar();
                                if (id != null)
                                {
                                    cc = new SqlCommand(string.Format("SELECT name FROM printer WHERE printer_group_id = {0}", id), dbConn);
                                    string pn = (string)cc.ExecuteScalar();
                                    if (pn == null)
                                    {
                                        cc = new SqlCommand(string.Format("INSERT INTO printer(name, printer_group_id, print_cnt, print_total_cnt) VALUES('{0}', {1}, 0, 0)", prntr, id), dbConn);
                                        cc.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        if (pn != prntr)
                                        {
                                            cc = new SqlCommand(string.Format("UPDATE printer SET name = '{0}' WHERE printer_group_id = {1}", prntr, id), dbConn);
                                            cc.ExecuteNonQuery();
                                        }
                                    }
                                }
                                else
                                {
                                    cc = new SqlCommand(string.Format("INSERT INTO printer_group(name, branch_id, print_cnt, print_total_cnt) VALUES('{0}', {1}, 0, 0)", pgn, branchId), dbConn);
                                    cc.ExecuteNonQuery();
                                    cc = new SqlCommand(string.Format("INSERT INTO printer(name, printer_group_id, print_cnt, print_total_cnt) VALUES('{0}', @@Identity, 0, 0)", prntr), dbConn);
                                    cc.ExecuteNonQuery();
                                }
                            }
                            #endregion

                            trans = dbConn.BeginTransaction("trans");
                            List<string> listSetmeal = new List<string>();
                            string sql;

                            #region 插入非套餐，非33开头在菜品
                            foreach (DataRow row in table.Rows)
                            {
                                string prodNo = ((string)row["productno"]).Trim();
                                string prodName = ((string)row["prodname"]).Trim();
                                decimal price = (decimal)row["price"];
                                decimal fprice = (decimal)row["fprice"];
                                bool locked = (bool)row["locked"];
                                bool isSetmeal = IsSetmeal(prodNo);

                                if ((prodNo.Substring(0, 2) != "33") &&
                                    (prodNo.Length > 4))
                                {
                                    int? pgId = GetPrinterGroupId(((string)row["printgroup"]).Trim());

                                    sql = string.Format("SELECT price, fprice FROM menu WHERE number = '{0}'", prodNo);
                                    var dbCmd = new SqlCommand(sql, dbConn, trans);
                                    var dbReader = dbCmd.ExecuteReader();
                                    if (dbReader.Read())
                                    {
                                        sql = string.Format("UPDATE menu SET price = {0}, fprice = {1}, is_locked = {2}, sold_out = {3}, printer_group_id = {6} WHERE number = '{4}' AND branch_id = {5}", price, fprice, locked ? 1 : 0, _sellOuts.Contains(prodNo) ? 1 : 0, prodNo, branchId, pgId == null ? "NULL" : pgId.ToString());
                                    }
                                    else
                                    {
                                        if (locked == true)
                                        {
                                            dbReader.Close();
                                            continue;
                                        }
                                        else
                                        {
                                            int? catId = GetMenuCataId(prodNo);

                                            if (catId != null)
                                                sql = string.Format("INSERT INTO menu(branch_id, name, number, unit, price, printer_group_id, is_setmeal, menu_catalog_id, fprice) VALUES({6}, '{0}', '{1}', '份', {2}, {7}, {3}, {4}, {5})",
                                                    prodName, prodNo, price, isSetmeal ? 1 : 0, catId, fprice, branchId, pgId == null ? "NULL" : pgId.ToString());
                                            else
                                                sql = string.Format("INSERT INTO menu(branch_id, name, number, unit, price, printer_group_id, is_setmeal, fprice) VALUES({5}, '{0}', '{1}', '份', {2}, {6}, {3}, {4})",
                                                    prodName, prodNo, price, isSetmeal ? 1 : 0, fprice, branchId, pgId == null ? "NULL" : pgId.ToString());
                                        }
                                    }
                                    dbReader.Close();

                                    using (var dbCmd1 = new SqlCommand(sql, dbConn, trans))
                                    {
                                        dbCmd1.ExecuteNonQuery();
                                    }

                                    System.Console.WriteLine(prodName);
                                }
                            }
                            #endregion 插入非套餐，非33开头在菜品

                            sql = string.Format("SELECT id, number FROM menu WHERE is_setmeal = 1");
                            var dbCmd2 = new SqlCommand(sql, dbConn, trans);
                            var dbAdapter = new SqlDataAdapter(dbCmd2);
                            var tbl1 = new DataTable();
                            dbAdapter.Fill(tbl1);

                            #region 插入套餐内容
                            foreach (DataRow row in tbl1.Rows)
                            {
                                string prodNo = (string)row["number"];
                                int id = (int)row["id"];

                                OleDbDataAdapter da1 = new OleDbDataAdapter(string.Format("SELECT * FROM subprod  WHERE productno = '{0}'", prodNo), conn);
                                var tbl2 = new DataTable();
                                da1.Fill(tbl2);

                                foreach (DataRow r in tbl2.Rows)
                                {
                                    string foxSubProd = ((string)r["productno"]).Trim();
                                    string foxSubProdName = ((string)r["spname"]).Trim();

                                    sql = string.Format("SELECT id FROM menu WHERE name = '{0}'", foxSubProdName);
                                    var dbCmd = new SqlCommand(sql, dbConn, trans);
                                    int subId = 0;
                                    object val = dbCmd.ExecuteScalar();
                                    if (val != null)
                                        subId = (int)val;
                                    else
                                        subId = 0;

                                    System.Console.WriteLine(string.Format("{0} - {1} - {2}", prodNo, foxSubProdName, subId));

                                    sql = string.Format("INSERT INTO setmeal_item(setmeal_menu_id, setmeal_item_menu_id) VALUES({0}, {1})",
                                        id, subId);

                                    dbCmd = new SqlCommand(sql, dbConn, trans);
                                    dbCmd.ExecuteNonQuery();
                                }
                            }
                            #endregion 插入套餐内容

                            sql = string.Format("SELECT productno, prodname FROM product WHERE productno LIKE '33%' ORDER BY productno");
                            var da2 = new OleDbDataAdapter(sql, conn);
                            var tbl3 = new DataTable();
                            da2.Fill(tbl3);

                            decimal idLast = 1;
                            foreach (DataRow r in tbl3.Rows)
                            {
                                string prodNo = ((string)r["productno"]).Trim();
                                string prodName = ((string)r["prodname"]).Trim();

                                SqlCommand dbCmd;
                                if (prodNo.Length == 4)
                                {
                                    sql = string.Format("INSERT INTO method_catalog(name) VALUES('{0}');SELECT @@IDENTITY AS 'Identity'", prodName);
                                    dbCmd = new SqlCommand(sql, dbConn, trans);
                                    object val = dbCmd.ExecuteScalar();
                                    Type t = val.GetType();
                                    idLast = (decimal)val;
                                    dbCmd.Dispose();
                                }
                                else if (prodNo.Length == 6)
                                {
                                    sql = string.Format("INSERT INTO method(method_catalog_id, name) VALUES({0}, '{1}')",
                                        idLast, prodName);

                                    dbCmd = new SqlCommand(sql, dbConn, trans);
                                    dbCmd.ExecuteNonQuery();
                                }
                            }

                            trans.Commit();

                            dbConn.Close();
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                trans.Rollback();

                System.Console.WriteLine(ex.Message);
            }
        }

        public override bool IsSetmeal(string prodNo)
        {
            bool isSetmeal = false;

            using (var conn = new OleDbConnection(FoxproConnStr))
            {
                conn.Open();

                using (var cmd = new OleDbCommand(string.Format("SELECT * FROM  subprod WHERE productno = '{0}'", prodNo), conn))
                {
                    OleDbDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                        isSetmeal = true;
                }

                conn.Close();
            }

            return isSetmeal;
        }

        private void DbMaintain()
        {
            using (var conn = new OleDbConnection(FoxproConnStr))
            {
                conn.Open();

                var adapter = new OleDbDataAdapter("SELECT * FROM subprod", conn);
                var table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow r in table.Rows)
                {
                    string prod2 = ((string)r["product2"]).Trim();

                    var cmd = new OleDbCommand(string.Format("SELECT prodname FROM product WHERE productno = '{0}'", prod2), conn);
                    string prodName = (string)cmd.ExecuteScalar();
                    prodName = prodName.Trim();

                    cmd = new OleDbCommand(string.Format("UPDATE subprod SET spname = '{0}' WHERE product2 = '{1}'", prodName, prod2), conn);
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }

        private void MenuCata()
        {
            using (var conn = new OleDbConnection(FoxproConnStr))
            {
                conn.Open();

                var adapter = new OleDbDataAdapter("SELECT productno, prodname, locked FROM product ORDER BY productno", conn);
                var table = new DataTable();
                adapter.Fill(table);
                foreach (DataRow r in table.Rows)
                {
                    string prodNo = ((string)r["productno"]).Trim();
                    string prodName = ((string)r["prodname"]).Trim();
                    bool locked = (bool)r["locked"];

                    if (prodNo.Substring(0, 2) != "33")
                    {
                        if ((prodNo.Length > 2) &&
                            (prodNo.Length < 5))
                        {
                            using (var sqlConn = new SqlConnection(DbConnStr))
                            {
                                sqlConn.Open();

                                string sql;

                                sql = string.Format("SELECT name FROM menu_catalog WHERE name = '{0}'", prodName);
                                var cmd = new SqlCommand(sql, sqlConn);
                                if (cmd.ExecuteScalar() == null)
                                {
                                    sql = string.Format("INSERT INTO menu_catalog(parent_id, name, locked) VALUES({0}, '{1}', {2})", prodNo.Substring(0, 2) == "11" ? 1 : 2, prodName, locked ? 1 : 0);

                                    cmd = new SqlCommand(sql, sqlConn);
                                    cmd.ExecuteNonQuery();
                                }

                                sqlConn.Close();
                            }
                        }
                    }
                }

                conn.Close();
            }
        }

        int? GetPrinterGroupId(string pgn)
        {
            int? rtn = null;

            if (!string.IsNullOrEmpty(pgn))
            {
                using (var conn = new SqlConnection(DbConnStr))
                {
                    conn.Open();

                    var cmd = new SqlCommand(string.Format("SELECT id FROM printer_group WHERE name = '{0}'", pgn), conn);
                    rtn = (int?)cmd.ExecuteScalar();

                    conn.Close();
                }
            }

            return rtn;
        }

        private int? GetMenuCataId(string prodNo)
        {
            int? id = null;

            using (var conn = new OleDbConnection(FoxproConnStr))
            {
                conn.Open();

                string sql = string.Empty;

                sql = string.Format("SELECT prodname FROM product WHERE productno = '{0}'", prodNo.Substring(0, 4));
                var cmd = new OleDbCommand(sql, conn);
                string cata = (string)cmd.ExecuteScalar();

                if (cata != null)
                {
                    cata = cata.Trim();

                    sql = string.Format("SELECT id FROM menu_catalog WHERE name = '{0}'", cata);
                    using (var sqlConn = new SqlConnection(DbConnStr))
                    {
                        sqlConn.Open();

                        var cmd1 = new SqlCommand(sql, sqlConn);
                        object val = cmd1.ExecuteScalar();
                        Type t = val.GetType();
                        id = (int)val;

                        sqlConn.Close();
                    }
                }

                conn.Close();
            }

            return id;
        }
    }
}
