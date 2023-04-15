/*
* C#Like 
* Copyright © 2022-2023 RongRong 
* It's automatic generate by KissEditor, don't modify this file. 
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
	/// This class is automatic generate by 'KissEditor', for easy to interact with database. Don't modify this file.
	/// </summary>
	public abstract class Mail_Base : NetObject<Mail, Account>
	{
		#region Update
		/// <summary>
		/// For internal call only. You can override it if you want custom it.
		/// </summary>
		public override void Update(ref string _strSQL_, ref MySqlParameter[] _mySqlParameters_)
		{
			StringBuilder _sb_ = new StringBuilder();
			_sb_.Append("UPDATE `Mail` SET ");
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			if (HasUpdate(2ul))//UpdateMask.wasReadMask)
			{
				_sb_.Append("`wasRead` = @wasRead,");
				_param_ = new MySqlParameter("@wasRead", MySqlDbType.Byte);
				_param_.Value = wasRead;
				_ps_.Add(_param_);
				_sb_.Append("`received` = @received,");
				_param_ = new MySqlParameter("@received", MySqlDbType.Byte);
				_param_.Value = received;
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
				Logger.LogWarning("No need update 'Mail', you should call 'Mail.Update()' or 'Mail.UpdateImmediately()' after change something need to update to database.");
			}
		}
		#endregion //Update

		#region Select
		protected override void SetData(DataRow data)
		{
			_attribute_.uid = Convert.ToInt32(data["uid"]);
			_attribute_.acctId = Convert.ToInt32(data["acctId"]);
			_attribute_.senderId = Convert.ToInt32(data["senderId"]);
			_attribute_.senderName = Convert.ToString(data["senderName"]);
			_attribute_.title = Convert.ToString(data["title"]);
			_attribute_.content = Convert.ToString(data["content"]);
			_attribute_.appendix = Convert.ToString(data["appendix"]);
			_attribute_.createTime = Convert2DateTime(data["createTime"]);
			_attribute_.wasRead = Convert.ToByte(data["wasRead"]);
			_attribute_.received = Convert.ToByte(data["received"]);
		}
		/// <summary>
		/// Select all data from database. The select operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectAll(Action<List<Mail>, string> callback)
		{
			Select("SELECT * FROM `Mail`", new List<MySqlParameter>(), callback);
		}
		/// <summary>
		/// Select data from database by uid. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void SelectByUid(int uid, Action<List<Mail>, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			Select("SELECT * FROM `Mail` WHERE `uid` = @uid", _ps_, _callback_);
		}
		/// <summary>
		/// Select data from database by acctId. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void SelectByAcctId(int acctId, Action<List<Mail>, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
			_param_.Value = acctId;
			_ps_.Add(_param_);
			Select("SELECT * FROM `Mail` WHERE `acctId` = @acctId", _ps_, _callback_);
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
			_sb_.Append("DELETE FROM `Mail`");
			_sb_.Append(" WHERE `uid` = @uid");
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			Delete(_sb_.ToString(), _ps_, _callback_);
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
			Delete("DELETE FROM `Mail` WHERE `uid` = @uid", _ps_, _callback_);
		}
		/// <summary>
		/// Delete data from database by acctId. The delete operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void DeleteByAcctId(int acctId, Action<int, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
			_param_.Value = acctId;
			_ps_.Add(_param_);
			Delete("DELETE FROM `Mail` WHERE `acctId` = @acctId", _ps_, _callback_);
		}
		#endregion //Delete

		#region Insert
		/// <summary>
		/// Insert into database. The insert operation run in background thread. The callback occur after insert into database. (All params)
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done. You can ignore it if you don't care about the callback.</param>
		public static void Insert(int acctId, int senderId, string senderName, string title, string content, string appendix, DateTime createTime, byte wasRead, byte received, Action<Mail, string> _callback_ = null)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
			_param_.Value = acctId;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@senderId", MySqlDbType.Int32);
			_param_.Value = senderId;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@senderName", MySqlDbType.String);
			_param_.Value = senderName;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@title", MySqlDbType.String);
			_param_.Value = title;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@content", MySqlDbType.String);
			_param_.Value = content;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@appendix", MySqlDbType.String);
			_param_.Value = appendix;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@createTime", MySqlDbType.DateTime);
			_param_.Value = createTime;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@wasRead", MySqlDbType.Byte);
			_param_.Value = wasRead;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@received", MySqlDbType.Byte);
			_param_.Value = received;
			_ps_.Add(_param_);
			Insert("INSERT INTO `Mail` (`acctId`,`senderId`,`senderName`,`title`,`content`,`appendix`,`createTime`,`wasRead`,`received`) VALUES (@acctId,@senderId,@senderName,@title,@content,@appendix,@createTime,@wasRead,@received)",
				_ps_,
				(_lastInsertedId_, _error_) =>
				{
					Mail _mail_ = null;
					if (string.IsNullOrEmpty(_error_))
					{
						_mail_ = new Mail();
						_mail_._attribute_.uid = (int)_lastInsertedId_;
						_mail_._attribute_.acctId = acctId;
						_mail_._attribute_.senderId = senderId;
						_mail_._attribute_.senderName = senderName;
						_mail_._attribute_.title = title;
						_mail_._attribute_.content = content;
						_mail_._attribute_.appendix = appendix;
						_mail_._attribute_.createTime = createTime;
						_mail_._attribute_.wasRead = wasRead;
						_mail_._attribute_.received = received;
					}
					if (_callback_ != null)
						_callback_(_mail_, _error_);
				});
		}

		/// <summary>
		/// Insert into database. The insert operation run in background thread. The callback occur after insert into database. (Selected param only.)
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done. You can ignore it if you don't care about the callback.</param>
		public static void Insert(int acctId, int senderId, string senderName, string title, string content, string appendix, DateTime createTime, Action<Mail, string> _callback_ = null)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
			_param_.Value = acctId;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@senderId", MySqlDbType.Int32);
			_param_.Value = senderId;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@senderName", MySqlDbType.String);
			_param_.Value = senderName;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@title", MySqlDbType.String);
			_param_.Value = title;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@content", MySqlDbType.String);
			_param_.Value = content;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@appendix", MySqlDbType.String);
			_param_.Value = appendix;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@createTime", MySqlDbType.DateTime);
			_param_.Value = createTime;
			_ps_.Add(_param_);
			Insert("INSERT INTO `Mail` (`acctId`,`senderId`,`senderName`,`title`,`content`,`appendix`,`createTime`) VALUES (@acctId,@senderId,@senderName,@title,@content,@appendix,@createTime)",
				_ps_,
				(_lastInsertedId_, _error_) =>
				{
					Mail _mail_ = null;
					if (string.IsNullOrEmpty(_error_))
					{
						_mail_ = new Mail();
						_mail_._attribute_.uid = (int)_lastInsertedId_;
						_mail_._attribute_.acctId = acctId;
						_mail_._attribute_.senderId = senderId;
						_mail_._attribute_.senderName = senderName;
						_mail_._attribute_.title = title;
						_mail_._attribute_.content = content;
						_mail_._attribute_.appendix = appendix;
						_mail_._attribute_.createTime = createTime;
					}
					if (_callback_ != null)
						_callback_(_mail_, _error_);
				});
		}

		#endregion //Insert

		#region Property
		public enum UpdateMask : ulong
		{
			UseSendMask_ = 0ul,
			uidMask = 1ul,
			wasReadMask = 2ul,
			AllMask_ = 3ul
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
		public int acctId
		{
			get { return _attribute_.acctId; }
			set
			{
				_attribute_.acctId = value;
			}
		}

		[KissJsonSerializeProperty]
		public int senderId
		{
			get { return _attribute_.senderId; }
			set
			{
				_attribute_.senderId = value;
			}
		}

		[KissJsonSerializeProperty]
		public string senderName
		{
			get { return _attribute_.senderName; }
			set
			{
				_attribute_.senderName = value;
			}
		}

		[KissJsonSerializeProperty]
		public string title
		{
			get { return _attribute_.title; }
			set
			{
				_attribute_.title = value;
			}
		}

		[KissJsonSerializeProperty]
		public string content
		{
			get { return _attribute_.content; }
			set
			{
				_attribute_.content = value;
			}
		}

		[KissJsonSerializeProperty]
		public string appendix
		{
			get { return _attribute_.appendix; }
			set
			{
				_attribute_.appendix = value;
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
		public byte wasRead
		{
			get { return _attribute_.wasRead; }
			set
			{
				_attribute_.wasRead = value;
				MarkUpdateAndModifyMask(2ul);//UpdateMask.wasReadMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				if (_mainObject_ != null)
					_mainObject_.SyncToClient("mails");
			}
		}

		[KissJsonSerializeProperty]
		public byte received
		{
			get { return _attribute_.received; }
			set
			{
				_attribute_.received = value;
				MarkUpdateAndModifyMask(2ul);//UpdateMask.wasReadMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				if (_mainObject_ != null)
					_mainObject_.SyncToClient("mails");
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
			if ((mask & 1ul) > 0)//UpdateMask.uidMask
			{
				_jsonData_["uid"] = _attribute_.uid;
				_jsonData_["acctId"] = _attribute_.acctId;
				_jsonData_["senderId"] = _attribute_.senderId;
				_jsonData_["senderName"] = _attribute_.senderName;
				_jsonData_["title"] = _attribute_.title;
				_jsonData_["content"] = _attribute_.content;
				_jsonData_["appendix"] = _attribute_.appendix;
				_jsonData_["createTime"] = _attribute_.createTime;
			}
			if ((mask & 2ul) > 0)//UpdateMask.wasReadMask
			{
				_jsonData_["wasRead"] = _attribute_.wasRead;
				_jsonData_["received"] = _attribute_.received;
			}
			_jsonData_["_uid_"] = _uid_;
			_jsonData_["_sendMask_"] = mask;
			return _jsonData_;
		}
		/// <summary>
		/// Clone data from the Mail instance with custom mask.
		/// <param name="_source_">The source Mail</param>
		/// <param name="_mask_">Which part you want to clone, default is all.</param>
		/// </summary>
		public void Clone(Mail _source_, ulong _mask_ = ulong.MaxValue)
			{
			if ((_mask_ & 1ul) > 0)//UpdateMask.uidMask
			{
				uid = _source_.uid;
				acctId = _source_.acctId;
				senderId = _source_.senderId;
				senderName = _source_.senderName;
				title = _source_.title;
				content = _source_.content;
				appendix = _source_.appendix;
				createTime = _source_.createTime;
			}
			if ((_mask_ & 2ul) > 0)//UpdateMask.wasReadMask
			{
				wasRead = _source_.wasRead;
				received = _source_.received;
			}
		}

		#endregion //JSON

		#region PrivateFields
		[KissJsonDontSerialize]
		private struct _fields_
		{
			// uid
			public int uid;
			public int acctId;
			public int senderId;
			public string senderName;
			public string title;
			public string content;
			public string appendix;
			public DateTime createTime;
		
			// wasRead
			public byte wasRead;
			public byte received;
		
		}
		private _fields_ _attribute_ = new _fields_();
		#endregion //PrivateFields

	}
}
