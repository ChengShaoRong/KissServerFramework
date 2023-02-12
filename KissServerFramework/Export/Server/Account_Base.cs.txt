/*
* C#Like
* Copyright © 2022-2023 RongRong
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
	public abstract class Account_Base : MainNetObject<Account>
	{
		#region Update
		/// <summary>
		/// For internal call only. You can override it if you want custom it.
		/// </summary>
		public override void Update(ref string _strSQL_, ref MySqlParameter[] _mySqlParameters_)
		{
			StringBuilder _sb_ = new StringBuilder();
			_sb_.Append("UPDATE `Account` SET ");
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			if (HasUpdate(2ul))//UpdateMask.loginInfoMask)
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
			if (HasUpdate(4ul))//UpdateMask.nicknameMask)
			{
				_sb_.Append("`nickname` = @nickname,");
				_param_ = new MySqlParameter("@nickname", MySqlDbType.VarChar,64);
				_param_.Value = nickname;
				_ps_.Add(_param_);
			}
			if (HasUpdate(8ul))//UpdateMask.moneyMask)
			{
				_sb_.Append("`money` = @money,");
				_param_ = new MySqlParameter("@money", MySqlDbType.Int32);
				_param_.Value = money;
				_ps_.Add(_param_);
			}
			if (HasUpdate(16ul))//UpdateMask.scoreMask)
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
			if (HasUpdate(32ul))//UpdateMask.lastLoginTimeMask)
			{
				_sb_.Append("`lastLoginTime` = @lastLoginTime,");
				_param_ = new MySqlParameter("@lastLoginTime", MySqlDbType.DateTime);
				_param_.Value = lastLoginTime;
				_ps_.Add(_param_);
			}
			_waitingUpdate_ = false;
			ClearUpdateMask();
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
		#endregion //Update

		#region Select
		protected override void SetData(DataRow data)
		{
			_attribute_.uid = Convert.ToInt32(data["uid"]);
			_attribute_.acctType = Convert.ToInt32(data["acctType"]);
			_attribute_.createTime = Convert2DateTime(data["createTime"]);
			_attribute_.name = Convert.ToString(data["name"]);
			_attribute_.password = Convert.ToString(data["password"]);
			_attribute_.nickname = Convert.ToString(data["nickname"]);
			_attribute_.money = Convert.ToInt32(data["money"]);
			_attribute_.score = Convert.ToInt32(data["score"]);
			_attribute_.scoreTime = Convert2DateTime(data["scoreTime"]);
			_attribute_.lastLoginTime = Convert2DateTime(data["lastLoginTime"]);
			RegisterSync();
		}
		/// <summary>
		/// Select all data from database. The select operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectAll(Action<List<Account>, string> callback)
		{
			Select("SELECT * FROM `Account`", new List<MySqlParameter>(), callback);
		}
		/// <summary>
		/// Select data from database by uid. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void SelectByUid(int uid, Action<List<Account>, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			Select("SELECT * FROM `Account` WHERE `uid` = @uid", _ps_, _callback_);
		}
		/// <summary>
		/// Select data from database by uid and acctType. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void SelectByUidAndAcctType(int uid, int acctType, Action<List<Account>, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
			_param_.Value = acctType;
			_ps_.Add(_param_);
			Select("SELECT * FROM `Account` WHERE `uid` = @uid AND `acctType` = @acctType", _ps_, _callback_);
		}
		/// <summary>
		/// Select data from database by name and acctType. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void SelectByNameAndAcctType(string name, int acctType, Action<List<Account>, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@name", MySqlDbType.VarChar,64);
			_param_.Value = name;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
			_param_.Value = acctType;
			_ps_.Add(_param_);
			Select("SELECT * FROM `Account` WHERE `name` = @name AND `acctType` = @acctType", _ps_, _callback_);
		}
		#endregion //Select

		#region Delete
		/// <summary>
		/// Delete self from database. The delete operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
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
			Delete(_sb_.ToString(), _ps_, _callback_);
		}
		/// <summary>
		/// Delete all data from database. The delete operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void DeleteAll(Action<int, string> _callback_)
		{
			Delete("DELETE FROM `Account`", new List<MySqlParameter>(), _callback_);
		}
		/// <summary>
		/// Delete data from database by uid. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void DeleteByUid(int uid, Action<int, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			Delete("DELETE FROM `Account` WHERE `uid` = @uid", _ps_, _callback_);
		}
		/// <summary>
		/// Delete data from database by uid and acctType. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void DeleteByUidAndAcctType(int uid, int acctType, Action<int, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
			_param_.Value = acctType;
			_ps_.Add(_param_);
			Delete("DELETE FROM `Account` WHERE `uid` = @uid AND `acctType` = @acctType", _ps_, _callback_);
		}
		/// <summary>
		/// Delete data from database by name and acctType. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void DeleteByNameAndAcctType(string name, int acctType, Action<int, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@name", MySqlDbType.VarChar,64);
			_param_.Value = name;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
			_param_.Value = acctType;
			_ps_.Add(_param_);
			Delete("DELETE FROM `Account` WHERE `name` = @name AND `acctType` = @acctType", _ps_, _callback_);
		}
		#endregion //Delete

		#region Insert
		/// <summary>
		/// Insert into database. The insert operation run in background thread. The callback occur after insert into database.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done. You can ignore it if you don't care about the callback.</param>
		public static void Insert(int acctType, DateTime createTime, string name, string password, string nickname, int money, int score, DateTime scoreTime, DateTime lastLoginTime, Action<Account, string> _callback_ = null)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@acctType", MySqlDbType.Int32);
			_param_.Value = acctType;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@createTime", MySqlDbType.Timestamp);
			_param_.Value = createTime;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@name", MySqlDbType.VarChar,64);
			_param_.Value = name;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@password", MySqlDbType.VarChar,64);
			_param_.Value = password;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@nickname", MySqlDbType.VarChar,64);
			_param_.Value = nickname;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@money", MySqlDbType.Int32);
			_param_.Value = money;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@score", MySqlDbType.Int32);
			_param_.Value = score;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@scoreTime", MySqlDbType.DateTime);
			_param_.Value = scoreTime;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@lastLoginTime", MySqlDbType.DateTime);
			_param_.Value = lastLoginTime;
			_ps_.Add(_param_);
			Insert("INSERT INTO `Account` (`acctType`,`createTime`,`name`,`password`,`nickname`,`money`,`score`,`scoreTime`,`lastLoginTime`) VALUES (@acctType,@createTime,@name,@password,@nickname,@money,@score,@scoreTime,@lastLoginTime)",
				_ps_,
				(_lastInsertedId_, _error_) =>
				{
					Account _account_ = null;
					if (string.IsNullOrEmpty(_error_))
					{
						_account_ = new Account();
						_account_._attribute_.uid = (int)_lastInsertedId_;
						_account_.RegisterSync();
						_account_._attribute_.acctType = acctType;
						_account_._attribute_.createTime = createTime;
						_account_._attribute_.name = name;
						_account_._attribute_.password = password;
						_account_._attribute_.nickname = nickname;
						_account_._attribute_.money = money;
						_account_._attribute_.score = score;
						_account_._attribute_.scoreTime = scoreTime;
						_account_._attribute_.lastLoginTime = lastLoginTime;
					}
					if (_callback_ != null)
						_callback_(_account_, _error_);
				});
		}

		#endregion //Insert

		#region Sync
		public void RegisterSync()
		{
			int _uid_ = _attribute_.uid;
			Func<int, PlayerBase> _getPlayer_ = GetPlayer();
			RegisterSync("items", (player) => { Item.Sync(items.Values, player, "items", 100); }, _getPlayer_, _uid_, 0.1f);
			RegisterSync("mails", (player) => { Mail.Sync(mails.Values, player, "mails", 50); }, _getPlayer_, _uid_, 0.1f);
			RegisterSync("signIn", (player) => { signIn.Sync(player, "signIn"); }, _getPlayer_, _uid_, 0.1f);
			RegisterSync("account", (player) => { Sync(player, "account"); }, _getPlayer_, _uid_, 0.1f);
		}
		[KissJsonDontSerialize]
		public Dictionary<int, Item> items = new Dictionary<int, Item>();
		public void SetItems(List<Item> items)
		{
			if (items.Count == 0)
				return;
			if (this.items.Count == 0)
			{
				foreach (Item one in items)
				{
					one._mainObject_ = this;
					one.MarkModifyMaskAll();
					this.items[one.itemId] = one;
				}
			}
			else
			{
				foreach (Item one in items)
				{
					if (!this.items.ContainsKey(one.itemId))
					{
						one._mainObject_ = this;
						one.MarkModifyMaskAll();
						this.items[one.itemId] = one;
					}
					else
					{
						Item old = this.items[one.itemId];
						if (old.uid != one.uid)
						{
							old.count += one.count;
							one.Delete();
						}
					}
				}
			}
			SyncToClient("items");
		}
		public Item GetItem(int itemId)
		{
			if (items.TryGetValue(itemId, out Item _item_))
				return _item_;
			return null;
		}
		public bool RemoveItem(int itemId)
		{
			Item _item_ = GetItem(itemId);
			if (_item_ != null)
			{
				items.Remove(itemId);
				_item_.Delete();
				AddToDelete("items", itemId);
				return true;
			}
			return false;
		}
		[KissJsonDontSerialize]
		public Dictionary<int, Mail> mails = new Dictionary<int, Mail>();
		public void SetMails(List<Mail> mails)
		{
			if (mails.Count == 0)
				return;
			if (this.mails.Count == 0)
			{
				foreach (Mail one in mails)
				{
					one._mainObject_ = this;
					one.MarkModifyMaskAll();
					this.mails[one.uid] = one;
				}
			}
			else
			{
				foreach (Mail one in mails)
				{
					if (!this.mails.ContainsKey(one.uid))
					{
						one._mainObject_ = this;
						one.MarkModifyMaskAll();
						this.mails[one.uid] = one;
					}
				}
			}
			SyncToClient("mails");
		}
		public Mail GetMail(int uid)
		{
			if (mails.TryGetValue(uid, out Mail _mail_))
				return _mail_;
			return null;
		}
		public bool RemoveMail(int uid)
		{
			Mail _mail_ = GetMail(uid);
			if (_mail_ != null)
			{
				mails.Remove(uid);
				_mail_.Delete();
				AddToDelete("mails", uid);
				return true;
			}
			return false;
		}
		[KissJsonDontSerialize]
		public SignIn signIn = null;
		public SignIn GetSignIn()
		{
			return signIn;
		}
		public void SetSignIn(SignIn signIn)
		{
			this.signIn = signIn;
			signIn.MarkModifyMaskAll();
			signIn._mainObject_ = this;
			SyncToClient("signIn");
		}
		public bool RemoveSignIn()
		{
			if (signIn != null)
			{
				signIn.Delete();
				AddToDelete("signIn", 0);
				return true;
			}
			return false;
		}
		#endregion //Sync
		#region Property
		public enum UpdateMask : ulong
		{
			UseSendMask_ = 0ul,
			baseInfoMask = 1ul,
			loginInfoMask = 2ul,
			nicknameMask = 4ul,
			moneyMask = 8ul,
			scoreMask = 16ul,
			lastLoginTimeMask = 32ul,
			AllMask_ = ulong.MaxValue
		};

		[KissJsonSerializeProperty]
		public int uid
		{
			get { return _attribute_.uid; }
			set
			{
				_attribute_.uid = value;
			}
		}

		[KissJsonSerializeProperty]
		public int acctType
		{
			get { return _attribute_.acctType; }
			set
			{
				_attribute_.acctType = value;
			}
		}

		[KissJsonSerializeProperty]
		public DateTime createTime
		{
			get { return _attribute_.createTime; }
			set
			{
				_attribute_.createTime = value;
			}
		}

		[KissJsonSerializeProperty]
		public string name
		{
			get { return _attribute_.name; }
			set
			{
				_attribute_.name = value;
				MarkUpdateAndModifyMask(2ul);//UpdateMask.loginInfoMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		[KissJsonSerializeProperty]
		public string password
		{
			get { return _attribute_.password; }
			set
			{
				_attribute_.password = value;
				MarkUpdateAndModifyMask(2ul);//UpdateMask.loginInfoMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		[KissJsonSerializeProperty]
		public string nickname
		{
			get { return _attribute_.nickname; }
			set
			{
				_attribute_.nickname = value;
				MarkUpdateAndModifyMask(4ul);//UpdateMask.nicknameMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		[KissJsonSerializeProperty]
		public int money
		{
			get { return _attribute_.money; }
			set
			{
				_attribute_.money = value;
				MarkUpdateAndModifyMask(8ul);//UpdateMask.moneyMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		[KissJsonSerializeProperty]
		public int score
		{
			get { return _attribute_.score; }
			set
			{
				_attribute_.score = value;
				MarkUpdateAndModifyMask(16ul);//UpdateMask.scoreMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		[KissJsonSerializeProperty]
		public DateTime scoreTime
		{
			get { return _attribute_.scoreTime; }
			set
			{
				_attribute_.scoreTime = value;
				MarkUpdateAndModifyMask(16ul);//UpdateMask.scoreMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		[KissJsonSerializeProperty]
		public DateTime lastLoginTime
		{
			get { return _attribute_.lastLoginTime; }
			set
			{
				_attribute_.lastLoginTime = value;
				MarkUpdateAndModifyMask(32ul);//UpdateMask.lastLoginTimeMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				SyncToClient("account");
			}
		}

		#endregion //Property

		#region JSON
		/// <summary>
		/// Convert this object to JSONData with custom mask.
		/// </summary>
		/// <param name="mask">Which property you want to be included, default is send mask and the send mask default is all proterty.</param>
		public override JSONData ToJSONData(ulong mask = 0ul)
		{
			JSONData _jsonData_ = JSONData.NewDictionary();
			if (mask == 0ul) mask = GetSendMask();
			if ((mask & 1ul) > 0)//UpdateMask.baseInfoMask
			{
				_jsonData_["uid"] = _attribute_.uid;
				_jsonData_["acctType"] = _attribute_.acctType;
				_jsonData_["createTime"] = _attribute_.createTime;
			}
			if ((mask & 2ul) > 0)//UpdateMask.loginInfoMask
			{
				_jsonData_["name"] = _attribute_.name;
				_jsonData_["password"] = _attribute_.password;
			}
			if ((mask & 4ul) > 0)//UpdateMask.nicknameMask
				_jsonData_["nickname"] = _attribute_.nickname;
			if ((mask & 8ul) > 0)//UpdateMask.moneyMask
				_jsonData_["money"] = _attribute_.money;
			if ((mask & 16ul) > 0)//UpdateMask.scoreMask
			{
				_jsonData_["score"] = _attribute_.score;
				_jsonData_["scoreTime"] = _attribute_.scoreTime;
			}
			if ((mask & 32ul) > 0)//UpdateMask.lastLoginTimeMask
				_jsonData_["lastLoginTime"] = _attribute_.lastLoginTime;
			_jsonData_["_uid_"] = _uid_;
			_jsonData_["_sendMask_"] = mask;
			return _jsonData_;
		}
		/// <summary>
		/// Clone data from the Account instance with custom mask.
		/// <param name="_source_">The source Account</param>
		/// <param name="_mask_">Which part you want to clone, default is all.</param>
		/// </summary>
		public void Clone(Account _source_, ulong _mask_ = ulong.MaxValue)
			{
			if ((_mask_ & 1ul) > 0)//UpdateMask.baseInfoMask
			{
				uid = _source_.uid;
				acctType = _source_.acctType;
				createTime = _source_.createTime;
			}
			if ((_mask_ & 2ul) > 0)//UpdateMask.loginInfoMask
			{
				name = _source_.name;
				password = _source_.password;
			}
			if ((_mask_ & 4ul) > 0)//UpdateMask.nicknameMask
				nickname = _source_.nickname;
			if ((_mask_ & 8ul) > 0)//UpdateMask.moneyMask
				money = _source_.money;
			if ((_mask_ & 16ul) > 0)//UpdateMask.scoreMask
			{
				score = _source_.score;
				scoreTime = _source_.scoreTime;
			}
			if ((_mask_ & 32ul) > 0)//UpdateMask.lastLoginTimeMask
				lastLoginTime = _source_.lastLoginTime;
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
		
			// lastLoginTime
			public DateTime lastLoginTime;
		
		}
		private _fields_ _attribute_ = new _fields_();
		#endregion //PrivateFields

	}
}
