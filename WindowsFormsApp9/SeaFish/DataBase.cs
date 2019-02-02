using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace SeaFish {
        class  DataBase {

        #region 全局变量和构造函数
        static OleDbConnection conn;
        static readonly string basePaht = string.Format(@"{0}\{1}", Application.StartupPath, "dnscache.accdb");
        public bool isInit;

        public DataBase() {
            try {
                try {
                    conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Mode=12;Data Source=" + basePaht);
                    conn.Open();
                } catch {

                    MessageBox.Show("锁定数据库失败", "锁定数据库失败",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                    conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + basePaht);
                    conn.Open();
                }
                isInit = true;
            } catch {
                isInit = false;
                MessageBox.Show("打开数据库失败，无法使用取缓存数据", "打开数据库失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }            
        }
        #endregion

        #region 数据库操作

 
        /// <summary>
        /// 获取缓存数据
        /// </summary>
        /// <returns></returns>
        public bool ReadCache(DNSProxy.DnsPack dnsPack,out byte[] cache) {
            try {
                if (!isInit) throw new Exception();
                OleDbCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = "select QueryDns from dnscache where QueryName = ? and QueryType = ?";
                cmd.Parameters.Add("@QueryName", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryName;
                cmd.Parameters.Add("@QueryType", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryType;
                byte[] bytes = (byte[])cmd.ExecuteScalar();
                cache = bytes;
                return cache != null;
            } catch {
                cache = new byte[0];
                return false;
            }
        }

        /// <summary>
        /// 读取host表
        /// </summary>
        /// <param name="dnsPack"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        public bool ReadHost(DNSProxy.DnsPack dnsPack, out byte[] cache) {
            try {
                if (!isInit) throw new Exception();
                OleDbCommand cmd;
                cmd = conn.CreateCommand();
                cmd.CommandText = "select QueryDns from dnsHost where QueryName = ? and QueryType = ?";
                cmd.Parameters.Add("@QueryName", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryName;
                cmd.Parameters.Add("@QueryType", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryType;
                string Value = (string)cmd.ExecuteScalar();

                if (Value != null && Value != string.Empty ) {
                    switch (dnsPack.QueryRecords[0].QueryType) {
                        case DNSProxy.QueryType.A:
                            IPAddress iP;
                            if (!IPAddress.TryParse(Value, out iP)) break;
                            string[] ips = iP.ToString().Split('.');
                            byte[] ipBtye = new byte[] { byte.Parse(ips[0]), byte.Parse(ips[1]), byte.Parse(ips[2]), byte.Parse(ips[3]) };
                            dnsPack.ResouceRecords.Add(new DNSProxy.ResouceRecord {
                                QueryClass = 1,
                                QueryType = DNSProxy.QueryType.A,
                                QueryName = dnsPack.QueryRecords[0].QueryName,
                                Datas = ipBtye,
                                TTL = 0
                            });
                                break;
                        default:
                            break;
                    }
                    cache = dnsPack.ToBytes();
                    return cache != null;
                }
                throw new Exception();
            } catch (Exception) {
                cache = new byte[0];
                return false;
            }
        }


        /// <summary>
        /// 写入缓存到数据库
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public bool WriteCache(DNSProxy.DnsPack dnsPack) {
            try {
                if (!isInit) throw new Exception();
                bool result;
                OleDbCommand cmd = null;
                OleDbTransaction transaction = null;
                try {

                    #region 保存总账单
                    // 开始事务
                    transaction = conn.BeginTransaction();
                    cmd = conn.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.Connection = conn;

                    cmd.CommandText = "insert into dnscache(QueryName, QueryType, QueryClass, QueryDns) values(?, ?, ?, ?)";
                    cmd.Parameters.Add("@QueryName", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryName;
                    cmd.Parameters.Add("@QueryType", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryType;
                    cmd.Parameters.Add("@QueryClass", OleDbType.VarChar).Value = dnsPack.QueryRecords[0].QueryClass;
                    cmd.Parameters.Add("@ImageBinary", OleDbType.Binary, dnsPack.ToBytes().Length).Value = dnsPack.ToBytes();

                    if (cmd.ExecuteNonQuery() < 0)
                        throw new Exception();
                    #endregion

                    //提交事务
                    transaction.Commit();
                    result = true;
                } catch {
                    //异常时回滚事务
                    result = false;
                    transaction.Rollback();
                } finally {
                    cmd.Dispose();
                    transaction.Dispose();
                }
                return result;
            } catch  {
                return false;
            }

        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        public bool ClearCache() {
            try {
                if (!isInit) throw new Exception();
                bool result;
                OleDbTransaction transaction = null;
                OleDbCommand cmd = null;
                try {

                    // 开始事务
                    transaction = conn.BeginTransaction();
                    cmd = conn.CreateCommand();
                    cmd.Transaction = transaction;
                    cmd.Connection = conn;
                    cmd.CommandText = String.Format("delete from dnscache");
                    cmd.CommandType = CommandType.Text;

                    if (cmd.ExecuteNonQuery() < 0)
                        throw new Exception();

                    //提交事务
                    transaction.Commit();
                    result = true;
                } catch {
                    //异常时 回滚事务
                    result = false;
                    transaction.Rollback();
                } finally {
                    cmd.Dispose();
                    transaction.Dispose();
                }
                return result;
            } catch {
                return false;
            }

        }

        #endregion
    }
}
