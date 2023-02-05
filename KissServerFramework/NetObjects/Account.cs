
using CSharpLike;
using KissFramework;
using System;
using System.Collections.Generic;

namespace KissServerFramework
{
    /// <summary>
    /// This class include all the system belong to player, such as items/mails...
    /// </summary>
    public sealed class Account : Account_Base
    {
        public enum AccountType
        {
            BuildIn,//Build-in account
            ThirdParty_Test,//Sample for test third party account.
                            //Login flow :
                            //Client got uid and token from third party SDK. 
                            //-> send to our server.
                            //-> our server confirm that uid and token from third party server by HTTP(s).
                            //-> login success/fail
        }
        public override Func<int, PlayerBase> GetPlayer()
        {
            return (int uid) => { return AccountManager.Instance.GetPlayer(uid); };
        }
        /// <summary>
        /// Send message to client
        /// </summary>
        public void Send(string msg)
        {
            Player player = AccountManager.Instance.GetPlayer(uid);
            if (player != null)
                player.Send(msg);
        }
        /// <summary>
        /// Send message to client
        /// </summary>
        public void Send(JSONData jsonData)
        {
            Send(jsonData.ToString());
        }
        string offlineEventKey = "";
        /// <summary>
        /// Process your custom action when account online
        /// </summary>
        public void OnOnline()
        {
            //Cancel the event for clear the cache of this account in 1 hour.
            if (!string.IsNullOrEmpty(offlineEventKey))
            {
                Framework.CancelEvent(offlineEventKey);
                offlineEventKey = "";
            }
        }
        /// <summary>
        /// Process your custom action when account offline
        /// </summary>
        public void OnOffline()
        {
            //Raise the event for clear the cache of this account in 1 hour.
            if (string.IsNullOrEmpty(offlineEventKey))
                offlineEventKey = Framework.RaiseEvent((deltaTime) =>
                {
                    AccountManager.Instance.ClearAccountCache(this);

                }, Framework.config.accountCacheTime);
        }
        /// <summary>
        /// Change item count
        /// </summary>
        /// <param name="itemId">item id</param>
        /// <param name="count">item count</param>
        /// <param name="logType">type for log in database, default is 0</param>
        public bool ChangeItem(int itemId, int count, int logType = 0)
        {
            if (count == 0)
                return false;
            Item item = GetItem(itemId);
            if (count > 0)//AddItem
            {
                if (item != null)
                {
                    item.count += count;
                    LogManager.LogItem(uid, logType, count, item.count);
                }
                else
                {
                    Item.Insert(itemId, uid, count, (newItem, error) =>
                    {
                        item = GetItem(itemId);
                        if (item != null)//Has exist the item due to was add same item id while inserting into DB. It probably happen!!!
                        {
                            //We increase the count and remove the duplicate one
                            item.count += newItem.count;//Change the item count, and it will auto save to database.
                            newItem.Delete();//Delete the duplicate item from database.
                        }
                        else
                        {
                            SetItems(new List<Item>() { newItem });
                            item = newItem;
                        }
                        LogManager.LogItem(uid, logType, count, item.count);
                    });
                }
            }
            else//RemoveItem
            {
                if (item == null || item.count < count)//Not exist or not enough count
                {
                    return false;
                }
                else
                {
                    item.count -= count;
                    LogManager.LogItem(uid, logType, count, item.count);
                }
            }
            return true;
        }
        /// <summary>
        /// Send mail to player
        /// </summary>
        /// <param name="title">mail title</param>
        /// <param name="content">mail content</param>
        /// <param name="appendix">mail appendix, default is empty mean no appendix</param>
        /// <param name="senderId">sender uid, default is 0</param>
        /// <param name="senderName">sender name, default is 'System'</param>
        public void SendMail(string title, string content, string appendix = "", int senderId = 0, string senderName = "System")
        {
            //Insert into database
            Mail.Insert(uid, senderId, senderName, title, content, appendix, DateTime.Now, 0, 0,
                (mail, error) =>//callback from database
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        //Sync to client after inserted into database
                        SetMails(new List<Mail>() { mail });
                        LogManager.LogMail(uid, 0, appendix, content, title);//You may don't need log this.
                    }
                    else
                        Logger.LogError(error);
                });
        }
        /// <summary>
        /// Send mail to player
        /// </summary>
        /// <param name="title">mail title</param>
        /// <param name="content">mail content</param>
        /// <param name="appendixs">multiple item for mail appendix, the key is item id and the value is item count</param>
        /// <param name="senderId">sender uid, default is 0</param>
        /// <param name="senderName">sender name, default is 'System'</param>
        public void SendMail(string title, string content, Dictionary<int, int> appendixs, int senderId = 0, string senderName = "System")
        {
            string appendix = "";
            foreach(var item in appendixs)
            {
                appendix += " " + item.Key + "," + item.Value;
            }
            SendMail(title, content, appendix.Trim(), senderId, senderName);
        }
        /// <summary>
        /// Send mail to player
        /// </summary>
        /// <param name="title">mail title</param>
        /// <param name="content">mail content</param>
        /// <param name="itemId">One item id for mail appendix</param>
        /// <param name="itemCount">One item count for mail appendix</param>
        /// <param name="senderId">sender uid, default is 0</param>
        /// <param name="senderName">sender name, default is 'System'</param>
        public void SendMail(string title, string content, int itemId, int itemCount, int senderId = 0, string senderName = "System")
        {
            if (itemId != 0 && itemCount != 0)
                SendMail(title, content, itemId + "," + itemCount, senderId, senderName);
            else
                SendMail(title, content, "", senderId, senderName);
        }
        /// <summary>
        /// Mark mail was read
        /// </summary>
        /// <param name="mailUID">The mail UID, if value is 0 mean all mails.</param>
        public void ReadMail(int mailUID)
        {
            if (mailUID == 0)
            {
                foreach(Mail mail in mails.Values)
                {
                    if (mail.wasRead == 0)
                        mail.wasRead = 1;
                }
            }
            else
            {
                Mail mail = GetMail(mailUID);
                if (mail != null && mail.wasRead == 0)
                    mail.wasRead = 1;
            }
            //Callback to client if necessary.
            JSONData jsonData = JSONData.NewPacket(PacketType.CB_ReadMail);
            jsonData["mailUID"] = mailUID;
            Send(jsonData);
        }
        /// <summary>
        /// Take mail appendix
        /// </summary>
        /// <param name="mailUID">The mail UID, if value is 0 mean all mails.</param>
        public void TakeMailAppendix(int mailUID)
        {
            JSONData jsonData = JSONData.NewPacket(PacketType.CB_TakeMailAppendix);
            jsonData["mailUID"] = mailUID;
            if (mailUID == 0)
            {
                jsonData["msg"] = "No appendix.";
                foreach (Mail mail in mails.Values)
                {
                    if (!string.IsNullOrEmpty(mail.appendix) && mail.received == 0)
                    {
                        mail.wasRead = 0;
                        mail.received = 1;
                        jsonData["msg"] = "success";
                        foreach (var appendix in mail.AppendixItems)
                            ChangeItem(appendix.Key, appendix.Value, 1);
                    }
                }
            }
            else
            {
                Mail mail = GetMail(mailUID);
                if (mail == null)
                    jsonData["msg"] = "Mail null.";
                else if (string.IsNullOrEmpty(mail.appendix))
                    jsonData["msg"] = "No appendix.";
                else if (mail.received != 0)
                    jsonData["msg"] = "The appendix had token.";
                else
                {
                    mail.wasRead = 0;
                    mail.received = 1;
                    jsonData["msg"] = "success";
                    foreach(var appendix in mail.AppendixItems)
                        ChangeItem(appendix.Key, appendix.Value, 1);
                }
            }
            Send(jsonData);
        }
        /// <summary>
        /// Change the score value
        /// </summary>
        public void ChangeScroe(int valueChange)
        {
            //Check the valueChange whether valid
            //...

            //change value
            score += valueChange;
            scoreTime = DateTime.Now;

            //Notify rank of aircraft battle
            AircraftBattleManager.Instance.OnChangeScore(this);
        }
    }
}
