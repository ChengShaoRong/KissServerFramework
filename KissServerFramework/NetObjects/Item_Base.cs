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
	public abstract class Item_Base : NetObject<Item, Account>
	{
		#region Update
		/// <summary>
		/// For internal call only. You can override it if you want custom it.
		/// </summary>
		public override void Update(ref string _strSQL_, ref MySqlParameter[] _mySqlParameters_)
		{
			StringBuilder _sb_ = new StringBuilder();
			_sb_.Append("UPDATE `Item` SET ");
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			if (HasUpdate(2ul))//UpdateMask.countMask)
			{
				_sb_.Append("`count` = @count,");
				_param_ = new MySqlParameter("@count", MySqlDbType.Int32);
				_param_.Value = count;
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
				Logger.LogWarning("No need update 'Item', you should call 'Item.Update()' or 'Item.UpdateImmediately()' after change something need to update to database.");
			}
		}
		#endregion //Update

		#region Select
		protected override void SetData(DataRow data)
		{
			_attribute_.uid = Convert.ToInt32(data["uid"]);
			_attribute_.itemId = Convert.ToInt32(data["itemId"]);
			_attribute_.acctId = Convert.ToInt32(data["acctId"]);
			_attribute_.count = Convert.ToInt32(data["count"]);
		}
		/// <summary>
		/// Select all data from database. The select operation run in background thread. The callback action occur after database operation done.
		/// </summary>
		/// <param name="callback">This callback occur after database operation done.</param>
		public static void SelectAll(Action<List<Item>, string> callback)
		{
			Select("SELECT * FROM `Item`", new List<MySqlParameter>(), callback);
		}
		/// <summary>
		/// Select data from database by acctId. The select operation run in background thread.The callback action occur after database operation done.
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done.</param>
		public static void SelectByAcctId(int acctId, Action<List<Item>, string> _callback_)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
			_param_.Value = acctId;
			_ps_.Add(_param_);
			Select("SELECT * FROM `Item` WHERE `acctId` = @acctId", _ps_, _callback_);
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
			_sb_.Append("DELETE FROM `Item`");
			_sb_.Append(" WHERE `uid` = @uid");
			_param_ = new MySqlParameter("@uid", MySqlDbType.Int32);
			_param_.Value = uid;
			_ps_.Add(_param_);
			Delete(_sb_.ToString(), _ps_, _callback_);
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
			Delete("DELETE FROM `Item` WHERE `acctId` = @acctId", _ps_, _callback_);
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
			Delete("DELETE FROM `Item` WHERE `uid` = @uid", _ps_, _callback_);
		}
		#endregion //Delete

		#region Insert
		/// <summary>
		/// Insert into database. The insert operation run in background thread. The callback occur after insert into database. (All params)
		/// </summary>
		/// <param name="_callback_">This callback occur after database operation done. You can ignore it if you don't care about the callback.</param>
		public static void Insert(int itemId, int acctId, int count, Action<Item, string> _callback_ = null)
		{
			List<MySqlParameter> _ps_ = new List<MySqlParameter>();
			MySqlParameter _param_;
			_param_ = new MySqlParameter("@itemId", MySqlDbType.Int32);
			_param_.Value = itemId;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
			_param_.Value = acctId;
			_ps_.Add(_param_);
			_param_ = new MySqlParameter("@count", MySqlDbType.Int32);
			_param_.Value = count;
			_ps_.Add(_param_);
			Insert("INSERT INTO `Item` (`itemId`,`acctId`,`count`) VALUES (@itemId,@acctId,@count)",
				_ps_,
				(_lastInsertedId_, _error_) =>
				{
					Item _item_ = null;
					if (string.IsNullOrEmpty(_error_))
					{
						_item_ = new Item();
						_item_._attribute_.uid = (int)_lastInsertedId_;
						_item_._attribute_.itemId = itemId;
						_item_._attribute_.acctId = acctId;
						_item_._attribute_.count = count;
					}
					if (_callback_ != null)
						_callback_(_item_, _error_);
				});
		}

		#endregion //Insert

		#region Property
		public enum UpdateMask : ulong
		{
			UseSendMask_ = 0ul,
			uidMask = 1ul,
			countMask = 2ul,
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
		public int itemId
		{
			get { return _attribute_.itemId; }
			set
			{
				_attribute_.itemId = value;
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
		public int count
		{
			get { return _attribute_.count; }
			set
			{
				_attribute_.count = value;
				MarkUpdateAndModifyMask(2ul);//UpdateMask.countMask
				AsyncDatabaseManager.UpdateDelayInBackgroundThread(this);
				if (_mainObject_ != null)
					_mainObject_.SyncToClient("items");
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
				_jsonData_["itemId"] = _attribute_.itemId;
				_jsonData_["acctId"] = _attribute_.acctId;
			}
			if ((mask & 2ul) > 0)//UpdateMask.countMask
				_jsonData_["count"] = _attribute_.count;
			_jsonData_["_uid_"] = _uid_;
			_jsonData_["_sendMask_"] = mask;
			return _jsonData_;
		}
		/// <summary>
		/// Clone data from the Item instance with custom mask.
		/// <param name="_source_">The source Item</param>
		/// <param name="_mask_">Which part you want to clone, default is all.</param>
		/// </summary>
		public void Clone(Item _source_, ulong _mask_ = ulong.MaxValue)
			{
			if ((_mask_ & 1ul) > 0)//UpdateMask.uidMask
			{
				uid = _source_.uid;
				itemId = _source_.itemId;
				acctId = _source_.acctId;
			}
			if ((_mask_ & 2ul) > 0)//UpdateMask.countMask
				count = _source_.count;
		}

		#endregion //JSON

		#region PrivateFields
		[KissJsonDontSerialize]
		private struct _fields_
		{
			// uid
			public int uid;
			public int itemId;
			public int acctId;
		
			// count
			public int count;
		
		}
		private _fields_ _attribute_ = new _fields_();
		#endregion //PrivateFields

	}
}
