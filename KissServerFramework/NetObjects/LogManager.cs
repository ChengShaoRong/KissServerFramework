/*
* C#Like 
* Copyright © 2022-2023 RongRong 
* It's automatic generate by KissEditor, don't modify this file. 
*/

using KissFramework;
using System;
using MySql.Data.MySqlClient;

namespace KissServerFramework
{
	/// <summary>
	/// The log system for saving log into database.
	/// You call this in main thread, but the real MySQL insert operation running in background thread,
	/// that won't block the main thread.
	/// And we don't care the result of the insert operation.
	/// </summary>
	public sealed class LogManager
	{
		public enum LogAccountType
        {
			Login,
			Logout,
			Register
        }
		public static void LogAccount(int acctId, int logType, string ip)
		{
			string strSQL = $"INSERT INTO `LogAccount` (`acctId`,`logType`,`ip`,`createTime`) VALUES (@acctId,@logType,@ip,@createTime)";
			MySqlParameter[] ps = new MySqlParameter[4]
			{
				new MySqlParameter("@acctId", MySqlDbType.Int32),
				new MySqlParameter("@logType", MySqlDbType.Int32),
				new MySqlParameter("@ip", MySqlDbType.String),
				new MySqlParameter("@createTime", MySqlDbType.Timestamp),
			};
			ps[0].Value = acctId;
			ps[1].Value = logType;
			ps[2].Value = ip;
			ps[3].Value = DateTime.Now;
			AsyncDatabaseManager.ExecuteSQLInBackgroundThread(strSQL, ps);
		}
		public static void LogItem(int acctId, int logType, int changeCount, int finalCount)
		{
			string strSQL = $"INSERT INTO `LogItem` (`acctId`,`logType`,`changeCount`,`finalCount`,`createTime`) VALUES (@acctId,@logType,@changeCount,@finalCount,@createTime)";
			MySqlParameter[] ps = new MySqlParameter[5]
			{
				new MySqlParameter("@acctId", MySqlDbType.Int32),
				new MySqlParameter("@logType", MySqlDbType.Int32),
				new MySqlParameter("@changeCount", MySqlDbType.Int32),
				new MySqlParameter("@finalCount", MySqlDbType.Int32),
				new MySqlParameter("@createTime", MySqlDbType.Timestamp),
			};
			ps[0].Value = acctId;
			ps[1].Value = logType;
			ps[2].Value = changeCount;
			ps[3].Value = finalCount;
			ps[4].Value = DateTime.Now;
			AsyncDatabaseManager.ExecuteSQLInBackgroundThread(strSQL, ps);
		}
		public static void LogMail(int acctId, int logType, string appendix, string content, string title)
		{
			string strSQL = $"INSERT INTO `LogMail` (`acctId`,`logType`,`appendix`,`content`,`title`,`createTime`) VALUES (@acctId,@logType,@appendix,@content,@title,@createTime)";
			MySqlParameter[] ps = new MySqlParameter[6]
			{
				new MySqlParameter("@acctId", MySqlDbType.Int32),
				new MySqlParameter("@logType", MySqlDbType.Int32),
				new MySqlParameter("@appendix", MySqlDbType.String),
				new MySqlParameter("@content", MySqlDbType.String),
				new MySqlParameter("@title", MySqlDbType.String),
				new MySqlParameter("@createTime", MySqlDbType.Timestamp),
			};
			ps[0].Value = acctId;
			ps[1].Value = logType;
			ps[2].Value = appendix;
			ps[3].Value = content;
			ps[4].Value = title;
			ps[5].Value = DateTime.Now;
			AsyncDatabaseManager.ExecuteSQLInBackgroundThread(strSQL, ps);
		}
	}
}
