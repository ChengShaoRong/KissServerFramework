/*
* C#Like
* Copyright © 2022 RongRong
* It's automatic generate by Account.ridl, don't modify this file.
*/

using KissFramework;
using System;
using CSharpLike;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace KissServerFramework
{
	/// <summary>
	/// This class is automatic generate by 'Account.ridl', for easy to interact with database. Don't modify this file.
	/// </summary>
	public abstract class AccountBase : IAsyncUpdateDB
	{
		#region Update
		/// <summary>
		/// For internal call only.
		/// </summary>
		[KissJsonDontSerialize]
		public bool _waitingUpdate_ { get; set; }
		/// <summary>
		/// For internal call only. You can override it if you want custom it.
		/// </summary>
		public virtual void Update(ref string _strSQL_, ref MySqlParameter[] _mySqlParameters_)
		{
			StringBuilder _sb_ = new StringBuilder();
			_sb_.Append("UPDATE `Account` SET ");
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			if (IsUpdate(UpdateMask.baseInfoMask))
			{
				_sb_.Append("`acctType` = @acctType,");
				_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
				_param_.Value = acctType;
				_ps_.Add(_param_);
			}
			if (IsUpdate(UpdateMask.loginInfoMask))
			{
				_sb_.Append("`name` = @name,");
				_param_ = new MySqlParameter("@name", MySqlDbType.VarChar,64);
				_param_.Value = name;
				_ps_.Add(_param_);
				_sb_.Append("`password` = @password,");
				_param_ = new MySqlParameter("@password", MySqlDbType.VarChar,64);
				_param_.Value = password;
				_ps_.Add(_param_);
			}
			if (IsUpdate(UpdateMask.nicknameMask))
			{
				_sb_.Append("`nickname` = @nickname,");
				_param_ = new MySqlParameter("@nickname", MySqlDbType.VarChar,64);
				_param_.Value = nickname;
				_ps_.Add(_param_);
			}
			if (IsUpdate(UpdateMask.moneyMask))
			{
				_sb_.Append("`money` = @money,");
				_param_ = new MySqlParameter("@money", MySqlDbType.Int32);
				_param_.Value = money;
				_ps_.Add(_param_);
			}
			if (IsUpdate(UpdateMask.scoreMask))
			{
				_sb_.Append("`score` = @score,");
				_param_ = new MySqlParameter("@score", MySqlDbType.Int32);
				_param_.Value = score;
				_ps_.Add(_param_);
				_sb_.Append("`scoreTime` = @scoreTime,");
				_param_ = new MySqlParameter("@scoreTime", MySqlDbType.DateTime);
				_param_.Value = scoreTime;
				_ps_.Add(_param_);
			}
			_waitingUpdate_ = false;
			ClearUpdate();
			if (_ps_.Count > 0)
			{
				_sb_.Remove(_sb_.Length - 1, 1);
				_sb_.Append(" WHERE `uid` = @uid");
				_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
				_param_.Value = uid;
				_ps_.Add(_param_);
				_strSQL_ = _sb_.ToString();
				_mySqlParameters_ = _ps_.ToArray();
			}
			else
			{
				Logger.LogWarning("No need update 'Account', you should call 'Account.Update()' or 'Account.UpdateImmediately()' after change something need to update to database.");
			}
		}
		/// <summary>
		/// Just notify some attributes of this object was modified, and the background thread will auto save it into database after several seconds (config by JOSN "updateDelaySeconds":1800). You should call this after change some attributes that you want save into database.
		/// </summary>
		public void Update()
		{
			AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
		}
		/// <summary>
		/// Notify some attributes of this object was modified, and the background thread will save it into database immediately.
		/// </summary>
		public void UpdateImmediately()
		{
			AsyncDatabaseManager.UpdateImmediatelyInBackgroundThread(this);
		}
		#endregion //Update

		#region Select
		private static void _Select_(string strSQL, List<MySqlParameter> ps, Action<List<Account>, string> callback)
		{
			List<Account> _accounts_ = new List<Account>();
			string error = "";
			new ThreadPoolMySql(
				(connection) =>
				{
					try
					{
						MySqlCommand cmd = new MySqlCommand(strSQL, connection);
						cmd.CommandType = CommandType.Text;
						foreach (MySqlParameter p in ps)
							cmd.Parameters.Add(p);
						using (MySqlDataAdapter msda = new MySqlDataAdapter())
						{
							msda.SelectCommand = cmd;
							DataTable dt = new DataTable();
							msda.Fill(dt);
							var raws = dt.Rows;
							for (int i = 0; i < raws.Count; i++)
							{
								var data = raws[i];
								Account _account_ = new Account();
								_account_.uid = (int)data["uid"];
								_account_.acctType = (int)data["acctType"];
								_account_.createTime = (DateTime)data["createTime"];
								_account_.name = (string)data["name"];
								_account_.password = (string)data["password"];
								_account_.nickname = (string)data["nickname"];
								_account_.money = (int)data["money"];
								_account_.score = (int)data["score"];
								_account_.scoreTime = (DateTime)data["scoreTime"];
								_account_.ClearUpdate();
								_accounts_.Add(_account_);
							}
						}
					}
					catch (Exception e)
					{
						error = e.Message;
					}
				},
				() =>
				{
					callback(_accounts_, error);
				});
		}
		/// <summary>
		/// Select all data from database. The select operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectAll(Action<List<Account>, string> callback)
		{
			_Select_("SELECT * FROM `Account`", new List<MySqlParameter>(), callback);
		}
		/// <summary>
		/// Select data from database by uid. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectByUid(int uid, Action<List<Account>, string> callback)
		{
			List<MySqlParameter> ps = new List<MySqlParameter>();
			MySqlParameter param;
			param = new MySqlParameter("@uid", MySqlDbType.Int32);
			param.Value = uid;
			ps.Add(param);
			_Select_("SELECT * FROM `Account` WHERE `uid` = @uid", ps, callback);
		}
		/// <summary>
		/// Select data from database by uid and acctType. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectByUidAndAcctType(int uid, int acctType, Action<List<Account>, string> callback)
		{
			List<MySqlParameter> ps = new List<MySqlParameter>();
			MySqlParameter param;
			param = new MySqlParameter("@uid", MySqlDbType.Int32);
			param.Value = uid;
			ps.Add(param);
			param = new MySqlParameter("@acctType", MySqlDbType.Int32);
			param.Value = acctType;
			ps.Add(param);
			_Select_("SELECT * FROM `Account` WHERE `uid` = @uid AND `acctType` = @acctType", ps, callback);
		}
		/// <summary>
		/// Select data from database by name and acctType. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectByNameAndAcctType(string name, int acctType, Action<List<Account>, string> callback)
		{
			List<MySqlParameter> ps = new List<MySqlParameter>();
			MySqlParameter param;
			param = new MySqlParameter("@name", MySqlDbType.VarChar,64);
			param.Value = name;
			ps.Add(param);
			param = new MySqlParameter("@acctType", MySqlDbType.Int32);
			param.Value = acctType;
			ps.Add(param);
			_Select_("SELECT * FROM `Account` WHERE `name` = @name AND `acctType` = @acctType", ps, callback);
		}
		#endregion //Select

		#region Delete
		private static void _Delete_(string strSQL, List<MySqlParameter> ps, Action<int, string> callback)
		{
			string error = "";
			int effectCount = 0;
			new ThreadPoolMySql(
				(connection) =>
				{
					try
					{
						MySqlCommand cmd = new MySqlCommand(strSQL, connection);
						cmd.CommandType = CommandType.Text;
						foreach (MySqlParameter p in ps)
							cmd.Parameters.Add(p);
						effectCount = cmd.ExecuteNonQuery();
					}
					catch (Exception e)
					{
						error = e.Message;
					}
				},
				() =>
				{
					if (callback != null)
						callback(effectCount, error);
				});
		}
		/// <summary>
		/// Delete self from database. The delete operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public void Delete(Action<int, string> _callback_ = null)
		{
			StringBuilder _sb_ = new StringBuilder();
			MySqlParameter _param_;
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			_sb_.Append("DELETE FROM `Account`");
			_sb_.Append(" WHERE `uid` = @uid");
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			_Delete_(_sb_.ToString(), _ps_, _callback_);
		}
		/// <summary>
		/// Delete all data from database. The delete operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void DeleteAll(Action<int, string> callback)
		{
			_Delete_("DELETE FROM `Account`", new List<MySqlParameter>(), callback);
		}
		/// <summary>
		/// Delete data from database by uid. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void DeleteByUid(int uid, Action<int, string> callback)
		{
			List<MySqlParameter> ps = new List<MySqlParameter>();
			MySqlParameter param;
			param = new MySqlParameter("@uid", MySqlDbType.Int32);
			param.Value = uid;
			ps.Add(param);
			_Delete_("DELETE FROM `Account` WHERE `uid` = @uid", ps, callback);
		}
		/// <summary>
		/// Delete data from database by uid and acctType. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void DeleteByUidAndAcctType(int uid, int acctType, Action<int, string> callback)
		{
			List<MySqlParameter> ps = new List<MySqlParameter>();
			MySqlParameter param;
			param = new MySqlParameter("@uid", MySqlDbType.Int32);
			param.Value = uid;
			ps.Add(param);
			param = new MySqlParameter("@acctType", MySqlDbType.Int32);
			param.Value = acctType;
			ps.Add(param);
			_Delete_("DELETE FROM `Account` WHERE `uid` = @uid AND `acctType` = @acctType", ps, callback);
		}
		/// <summary>
		/// Delete data from database by name and acctType. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void DeleteByNameAndAcctType(string name, int acctType, Action<int, string> callback)
		{
			List<MySqlParameter> ps = new List<MySqlParameter>();
			MySqlParameter param;
			param = new MySqlParameter("@name", MySqlDbType.VarChar,64);
			param.Value = name;
			ps.Add(param);
			param = new MySqlParameter("@acctType", MySqlDbType.Int32);
			param.Value = acctType;
			ps.Add(param);
			_Delete_("DELETE FROM `Account` WHERE `name` = @name AND `acctType` = @acctType", ps, callback);
		}
		#endregion //Delete

		#region Insert
		/// <summary>
		/// Insert into database. The insert operation run in background thread. The callback occur after insert into database.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done. You can ignore it if you don't care about the callback.</param>
		public static void Insert(int acctType, DateTime createTime, string name, string password, string nickname, int money, int score, DateTime scoreTime, Action<Account, string> _callback_ = null)
		{
			Account _account_ = null;
			string _error_ = "";
			new ThreadPoolMySql(
				(_connection_) =>
				{
					try
					{
						MySqlCommand _cmd_ = new MySqlCommand("INSERT INTO `Account` (`acctType`,`createTime`,`name`,`password`,`nickname`,`money`,`score`,`scoreTime`) VALUES (@acctType,@createTime,@name,@password,@nickname,@money,@score,@scoreTime)", _connection_);
						_cmd_.CommandType = CommandType.Text;
						MySqlParameter _param_;
						_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
						_param_.Value = acctType;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@createTime", MySqlDbType.Timestamp);
						_param_.Value = createTime;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@name", MySqlDbType.VarChar,64);
						_param_.Value = name;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@password", MySqlDbType.VarChar,64);
						_param_.Value = password;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@nickname", MySqlDbType.VarChar,64);
						_param_.Value = nickname;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@money", MySqlDbType.Int32);
						_param_.Value = money;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@score", MySqlDbType.Int32);
						_param_.Value = score;
						_cmd_.Parameters.Add(_param_);
						_param_ = new MySqlParameter("@scoreTime", MySqlDbType.DateTime);
						_param_.Value = scoreTime;
						_cmd_.Parameters.Add(_param_);
						if (_cmd_.ExecuteNonQuery() > 0)
						{
							_account_ = new Account();
							_account_.uid = (int)_cmd_.LastInsertedId;
							_account_.acctType = acctType;
							_account_.createTime = createTime;
							_account_.name = name;
							_account_.password = password;
							_account_.nickname = nickname;
							_account_.money = money;
							_account_.score = score;
							_account_.scoreTime = scoreTime;
							_account_.ClearUpdate();
						}
					}
					catch (Exception _e_)
					{
						_error_ = _e_.Message;
					}
				},
				() =>
				{
					if (_callback_ != null)
						_callback_(_account_, _error_);
				});
		}

		#endregion //Insert

		#region Property
		public enum UpdateMask : ulong
		{
			baseInfoMask = 1ul << 0,
			loginInfoMask = 1ul << 1,
			nicknameMask = 1ul << 2,
			moneyMask = 1ul << 3,
			scoreMask = 1ul << 4,
			AllMask_ = ulong.MaxValue
		};

		[KissJsonSerializeProperty]
		public int uid
		{
			get { return _attribute_.uid; }
			set
			{
				_attribute_.uid = value;
				_updateMask_ |= (ulong)UpdateMask.baseInfoMask;
			}
		}

		[KissJsonSerializeProperty]
		public int acctType
		{
			get { return _attribute_.acctType; }
			set
			{
				_attribute_.acctType = value;
				_updateMask_ |= (ulong)UpdateMask.baseInfoMask;
			}
		}

		[KissJsonSerializeProperty]
		public DateTime createTime
		{
			get { return _attribute_.createTime; }
			set
			{
				_attribute_.createTime = value;
				_updateMask_ |= (ulong)UpdateMask.baseInfoMask;
			}
		}

		[KissJsonSerializeProperty]
		public string name
		{
			get { return _attribute_.name; }
			set
			{
				_attribute_.name = value;
				_updateMask_ |= (ulong)UpdateMask.loginInfoMask;
			}
		}

		[KissJsonSerializeProperty]
		public string password
		{
			get { return _attribute_.password; }
			set
			{
				_attribute_.password = value;
				_updateMask_ |= (ulong)UpdateMask.loginInfoMask;
			}
		}

		[KissJsonSerializeProperty]
		public string nickname
		{
			get { return _attribute_.nickname; }
			set
			{
				_attribute_.nickname = value;
				_updateMask_ |= (ulong)UpdateMask.nicknameMask;
			}
		}

		[KissJsonSerializeProperty]
		public int money
		{
			get { return _attribute_.money; }
			set
			{
				_attribute_.money = value;
				_updateMask_ |= (ulong)UpdateMask.moneyMask;
			}
		}

		[KissJsonSerializeProperty]
		public int score
		{
			get { return _attribute_.score; }
			set
			{
				_attribute_.score = value;
				_updateMask_ |= (ulong)UpdateMask.scoreMask;
			}
		}

		[KissJsonSerializeProperty]
		public DateTime scoreTime
		{
			get { return _attribute_.scoreTime; }
			set
			{
				_attribute_.scoreTime = value;
				_updateMask_ |= (ulong)UpdateMask.scoreMask;
			}
		}

		#endregion //Property

		#region UpdateMark
		public void SetUpdate(UpdateMask updateMask)
		{
			_updateMask_ = (ulong)updateMask;
		}
		public void MarkUpdateAll()
		{
			_updateMask_ = ulong.MaxValue;
		}
		public void MarkUpdate(UpdateMask updateMask)
		{
			_updateMask_ |= (ulong)updateMask;
		}
		public void UnmarkUpdate(UpdateMask updateMask)
		{
			_updateMask_ |= ~(ulong)updateMask;
		}
		public void ClearUpdate()
		{
			_updateMask_ = ulong.MinValue;
		}
		public bool IsUpdate(UpdateMask updateMask)
		{
			return (_updateMask_ & (ulong)updateMask) > 0;
		}
		#endregion //UpdateMark

		#region JSON
		/// <summary>
		/// Convert this object to JSON string.
		/// </summary>
		public override string ToString()
		{
			return KissJson.ToJSONData(this).ToJson();
		}
		/// <summary>
		/// Convert this object to JSONData with custom mask.
		/// </summary>
		/// <param name="mask">Which property you want to be included, default is all property.</param>
		/// <param name="includePrimaryKey">Whether include primary key, default is true.</param>
		public JSONData ToJSONData(UpdateMask mask = UpdateMask.AllMask_, bool includePrimaryKey = true)
		{
			JSONData _jsonData_ = JSONData.NewDictionary();
			if ((mask & UpdateMask.baseInfoMask) > 0)
			{
				_jsonData_["uid"] = _attribute_.uid;
				_jsonData_["acctType"] = _attribute_.acctType;
				_jsonData_["createTime"] = _attribute_.createTime;
			}
			if ((mask & UpdateMask.loginInfoMask) > 0)
			{
				_jsonData_["name"] = _attribute_.name;
				_jsonData_["password"] = _attribute_.password;
			}
			if ((mask & UpdateMask.nicknameMask) > 0)
				_jsonData_["nickname"] = _attribute_.nickname;
			if ((mask & UpdateMask.moneyMask) > 0)
				_jsonData_["money"] = _attribute_.money;
			if ((mask & UpdateMask.scoreMask) > 0)
			{
				_jsonData_["score"] = _attribute_.score;
				_jsonData_["scoreTime"] = _attribute_.scoreTime;
			}
			if (includePrimaryKey)
			{
				if (!_jsonData_.ContainsKey("uid"))
					_jsonData_["uid"] = _attribute_.uid;
			}
			return _jsonData_;
		}
		/// <summary>
		/// Convert this object to JSON string with custom mask.
		/// </summary>
		/// <param name="mask">Which property you want to be included, default is all property.</param>
		/// <param name="includePrimaryKey">Whether include primary key, default is true.</param>
		public string ToJson(UpdateMask mask = UpdateMask.AllMask_, bool includePrimaryKey = true)
		{
			return ToJSONData(mask, includePrimaryKey).ToJson();
		}
		/// <summary>
		/// New a Account instance from JSON string.
		/// </summary>
		public static Account FromJson(string strJson)
		{
			return (Account)KissJson.ToObject(typeof(Account), strJson);
		}
		/// <summary>
		/// New a Account instance from JSONData.
		/// </summary>
		public static Account FromJson(JSONData jsonData)
		{
			return (Account)KissJson.ToObject(typeof(Account), jsonData);
		}
		/// <summary>
		/// Clone data from the Account instance with custom mask.
		/// </summary>
		public void Clone(Account _source_, UpdateMask _mask_ = UpdateMask.AllMask_)
		{
			if ((_mask_ & UpdateMask.baseInfoMask) > 0)
			{
				uid = _source_.uid;
				acctType = _source_.acctType;
				createTime = _source_.createTime;
			}
			if ((_mask_ & UpdateMask.loginInfoMask) > 0)
			{
				name = _source_.name;
				password = _source_.password;
			}
			if ((_mask_ & UpdateMask.nicknameMask) > 0)
				nickname = _source_.nickname;
			if ((_mask_ & UpdateMask.moneyMask) > 0)
				money = _source_.money;
			if ((_mask_ & UpdateMask.scoreMask) > 0)
			{
				score = _source_.score;
				scoreTime = _source_.scoreTime;
			}
		}

		#endregion //JSON

		#region PrivateFields
		[KissJsonDontSerialize]
		private struct _fields_
		{
			// baseInfo
			public int uid;
			public int acctType;
			public DateTime createTime;
		
			// loginInfo
			public string name;
			public string password;
		
			// nickname
			public string nickname;
		
			// money
			public int money;
		
			// score
			public int score;
			public DateTime scoreTime;
		
		}
		private _fields_ _attribute_ = new _fields_();
		[KissJsonDontSerialize]
		private ulong _updateMask_ = 0;
		#endregion //PrivateFields

	}
}
