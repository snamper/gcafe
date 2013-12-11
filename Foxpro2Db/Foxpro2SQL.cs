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
        public override void Convert()
        {
            DbMaintain();

            SqlTransaction trans = null;

            try
            {
                using (var conn = new OleDbConnection(FoxproConnStr))
                {
                    conn.Open();

                    using (var adapter = new OleDbDataAdapter("SELECT * FROM product", conn))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);

                        using (var dbConn = new SqlConnection(DbConnStr))
                        {
                            dbConn.Open();

                            trans = dbConn.BeginTransaction("trans");
                            List<string> listSetmeal = new List<string>();
                            string sql;

                            #region 插入非套餐，非33开头在菜品
                            foreach (DataRow row in table.Rows)
                            {
                                string prodNo = ((string)row["productno"]).Trim();
                                string prodName = ((string)row["prodname"]).Trim();
                                decimal price = (decimal)row["price"];
                                bool isSetmeal = IsSetmeal(prodNo);

                                if (prodNo.Substring(0, 2) != "33")
                                {
                                    sql = string.Format("INSERT INTO menu(branch_id, name, number, unit, price, printer_group_id, is_setmeal) VALUES(1, '{0}', '{1}', '份', {2}, 1, {3})",
                                        prodName, prodNo, price, isSetmeal ? 1 : 0);

                                    using (var dbCmd = new SqlCommand(sql, dbConn, trans))
                                    {
                                        dbCmd.ExecuteNonQuery();
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
    }
}
