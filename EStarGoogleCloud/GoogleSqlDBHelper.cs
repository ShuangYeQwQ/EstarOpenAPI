
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace EStarGoogleCloud
{
    public static class GoogleSqlDBHelper
    {
        private static string instance_host = ConfigurationManager.AppSettings["INSTANCE_HOST"] + "";
        private static string db_user = ConfigurationManager.AppSettings["DB_USER"] + "";
        private static string db_pass = ConfigurationManager.AppSettings["DB_PASS"] + "";
        private static string db_name = ConfigurationManager.AppSettings["DB_NAME"] + "";



        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(SqlCommand sqlCommand, ref string msg)
        {
            ExecuteNonQuery(sqlCommand, out int result, ref msg);
            sqlCommand.Dispose();
            return result;
        }
        public static int ExecuteNonQuery(SqlCommand sqlCommand, out int result, ref string msg)
        {
            result = 0;
            var connectionString = GetSqlServerConnectionString();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.Open();
                    sqlCommand.Connection = connection;
                    result = sqlCommand.ExecuteNonQuery();
                    sqlCommand.Dispose();
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            
            return result;
        }
        /// <summary>
        /// 修改表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string sql, DbCommand dbcom, ref string msg)
        {
            int num = 0;
            try
            {
                var connectionString = GetSqlServerConnectionString();
                using (DbConnection connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.OpenWithRetry();
                    using (var createTableCommand = connection.CreateCommand())
                    {
                        // Create the 'votes' table if it does not already exist.
                        createTableCommand.CommandText = sql;
                        foreach (DbParameter param in dbcom.Parameters)
                        {
                            // 创建新的参数副本
                            var newParam = createTableCommand.CreateParameter();
                            newParam.ParameterName = param.ParameterName;
                            newParam.Value = param.Value;
                            newParam.DbType = param.DbType; // 根据需要设置其他属性
                                                            // 添加新的参数
                            createTableCommand.Parameters.Add(newParam);
                        }
                        num = createTableCommand.ExecuteNonQuery();
                    }
                }
                return num;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return 0;
            }
            
        }
       
        
        
        
        
        
        /// <summary>
        /// 查询表记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataTable ExecuteReader(string sql, DbCommand dbcom, ref string msg)
        {
            DataTable dt = new DataTable();
            try
            {
                var connectionString = GetSqlServerConnectionString();
                using (DbConnection connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.OpenWithRetry();
                    using (var createTableCommand = connection.CreateCommand())
                    {
                        // Create the 'votes' table if it does not already exist.
                        createTableCommand.CommandText = sql;
                        foreach (DbParameter param in dbcom.Parameters)
                        {
                            // 创建新的参数副本
                            var newParam = createTableCommand.CreateParameter();
                            newParam.ParameterName = param.ParameterName;
                            newParam.Value = param.Value;
                            newParam.DbType = param.DbType; // 根据需要设置其他属性
                                                            // 添加新的参数
                            createTableCommand.Parameters.Add(newParam);
                        }
                        DbDataReader reader = createTableCommand.ExecuteReader();
                        dt.Load(reader);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return dt;
            }
        }
        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string ExecuteScalar(string sql, SqlCommand dbcom, ref string msg)
        {
            string result = null;
            try
            {
                var connectionString = GetSqlServerConnectionString();
                using (DbConnection connection = new SqlConnection(connectionString.ConnectionString))
                {
                    connection.OpenWithRetry();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sql;
                        foreach (DbParameter param in dbcom.Parameters)
                        {
                            // 创建新的参数副本
                            var newParam = command.CreateParameter();
                            newParam.ParameterName = param.ParameterName;
                            newParam.Value = param.Value;
                            newParam.DbType = param.DbType; // 根据需要设置其他属性
                                                            // 添加新的参数
                            command.Parameters.Add(newParam);
                        }
                        // 使用 ExecuteScalar 方法获取单个值
                        result = command.ExecuteScalar()?.ToString(); // 将结果转为字符串
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return null;
            }
        }
        public static int ExecuteNonQueryTransaction(List<DbCommand> dbcom, ref string msg)
        {
            int num = 0;
            try
            {
                // 创建事务范围
                using (var scope = new TransactionScope())
                {
                    try
                    {
                        var connectionString = GetSqlServerConnectionString();
                        using (var connection = new SqlConnection(connectionString.ConnectionString))
                        {
                            connection.Open();
                            // 执行传入的多个数据库命令
                            foreach (var command in dbcom)
                            {
                                command.Connection = connection; // 设置连接
                                num += command.ExecuteNonQuery(); // 执行命令并累加受影响的行数
                            }
                        }

                        // 提交事务
                        scope.Complete();
                        return num; // 返回受影响的行数
                    }
                    catch (Exception ex)
                    {
                        // 如果发生异常，事务将被回滚
                        msg = $"发生错误: {ex.Message}";
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message; // 捕获外层异常
                return 0;
            }

        }
        public static void OpenWithRetry(this DbConnection connection) =>
             // [START cloud_sql_sqlserver_dotnet_ado_backoff]
             Policy
                 .Handle<SqlException>()
                 .WaitAndRetry(new[]
                 {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5)
                 })
                 .Execute(() => connection.Open());
        // [END cloud_sql_sqlserver_dotnet_ado_backoff]
        public static SqlConnectionStringBuilder GetSqlServerConnectionString()
        {
            SqlConnectionStringBuilder connectionString;
            connectionString = NewSqlServerTCPConnectionString();
            // The values set here are for demonstration purposes only. You 
            // should set these values to what works best for your application.
            // [START cloud_sql_sqlserver_dotnet_ado_limit]
            // MaximumPoolSize sets maximum number of connections allowed in the pool.
            connectionString.MaxPoolSize = 5;
            // MinimumPoolSize sets the minimum number of connections in the pool.
            connectionString.MinPoolSize = 0;
            // [END cloud_sql_sqlserver_dotnet_ado_limit]
            // [START cloud_sql_sqlserver_dotnet_ado_timeout]
            // ConnectionTimeout sets the time to wait (in seconds) while
            // trying to establish a connection before terminating the attempt.
            connectionString.ConnectTimeout = 15;
            // [END cloud_sql_sqlserver_dotnet_ado_timeout]
            // [START cloud_sql_sqlserver_dotnet_ado_lifetime]
            // ADO.NET connection pooler removes a connection
            // from the pool after it's been idle for approximately
            // 4-8 minutes, or if the pooler detects that the
            // connection with the server no longer exists.
            // [END cloud_sql_sqlserver_dotnet_ado_lifetime]
            connectionString.TrustServerCertificate = true;
            return connectionString;
        }
        public static SqlConnectionStringBuilder NewSqlServerTCPConnectionString()
        {
            // Equivalent connection string:
            // "User Id=<DB_USER>;Password=<DB_PASS>;Server=<INSTANCE_HOST>;Database=<DB_NAME>;"
            var connectionString = new SqlConnectionStringBuilder()
            {
                // Note: Saving credentials in environment variables is convenient, but not
                // secure - consider a more secure solution such as
                // Cloud Secret Manager (https://cloud.google.com/secret-manager) to help
                // keep secrets safe.
                DataSource = Environment.GetEnvironmentVariable("INSTANCE_HOST"), // e.g. '127.0.0.1'
                // Set Host to 'cloudsql' when deploying to App Engine Flexible environment
                UserID = Environment.GetEnvironmentVariable("DB_USER"),         // e.g. 'my-db-user'
                Password = Environment.GetEnvironmentVariable("DB_PASS"),       // e.g. 'my-db-password'
                InitialCatalog = Environment.GetEnvironmentVariable("DB_NAME"), // e.g. 'my-database'

                // [END cloud_sql_sqlserver_dotnet_ado_connect_tcp_sslcerts]
                // The Cloud SQL proxy provides encryption between the proxy and instance
                Encrypt = false,
                // [START cloud_sql_sqlserver_dotnet_ado_connect_tcp_sslcerts]
            };
            // [END cloud_sql_sqlserver_dotnet_ado_connect_tcp]
            // For deployments that connect directly to a Cloud SQL for SQL Server instance
            // that has "Allow only SSL connections" enabled, without using the Cloud SQL Proxy,
            // configure encryption as Mandatory.
            if (!string.IsNullOrEmpty(instance_host))
            {
                connectionString.Encrypt = SqlConnectionEncryptOption.Mandatory;
            }
            // [START cloud_sql_sqlserver_dotnet_ado_connect_tcp]
            connectionString.Pooling = true;
            // Specify additional properties here.
            return connectionString;
        }

    }

}
