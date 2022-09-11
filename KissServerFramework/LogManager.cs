using System;
using KissFramework;
using MySql.Data.MySqlClient;

namespace KissServerFramework
{
    /// <summary>
    /// The log system for saving log into MySQL database.
    /// You call this in main thread, but the real MySQL insert operation running in background thread, 
    /// that won't block the main thread.
    /// And we don't care the return value of the insert operation.
    /// </summary>
    public class LogManager : Singleton<LogManager>
    {
        /// <summary>
        /// Sample for log the account login and logout information into the MySQL database.
        /// </summary>
        /// <param name="acctUid">account uid</param>
        /// <param name="logType">log type: 0 mean login, 1 mean logout</param>
        /// <param name="ip">the player ip</param>
        public void LogAccount(int acctUid, int logType, string ip)
        {
            string strSQL = $"INSERT INTO `logAccount` (`acctUid`,`logType`,`ip`,`createTime`) VALUES (@acctUid,@logType,@ip,@createTime)";
            MySqlParameter[] ps = new MySqlParameter[4]
            {
                new MySqlParameter("@acctUid", MySqlDbType.Int32, 11),
                new MySqlParameter("@logType", MySqlDbType.Int32, 11),
                new MySqlParameter("@ip", MySqlDbType.VarChar, 32),
                new MySqlParameter("@createTime", MySqlDbType.Timestamp),
            };
            ps[0].Value = acctUid;
            ps[1].Value = logType;
            ps[2].Value = ip;
            ps[3].Value = DateTime.Now;
            AsyncDatabaseManager.ExecuteSQLInBackgroundThread(strSQL, ps);//Notify the AsyncDatabaseManager to do insert operation in background thread.
        }
    }
}