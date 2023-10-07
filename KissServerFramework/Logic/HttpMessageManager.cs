using CSharpLike;
using KissFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace KissServerFramework
{
    /// <summary>
    /// Manager the player message.
    /// 1 Save the player message leave from webpage.
    /// 2 Load the player message to webpage.
    /// 3 Answer the player message by GM.
    /// 4 Delete some meaningless messages by GM.
    /// </summary>
    public static class HttpMessageManager
    {
        class Message
        {
            public long uid;
            public string title;
            public string question;
            public string nickname;
            public string contact;
            public string ip;
            public bool hideIp;
            public DateTime createTime;

            public string answer = "";
            public DateTime answerTime = new DateTime(0);

            public override string ToString()
            {
                return ToJSONData().ToJson(true);
            }
            public JSONData ToJSONData()
            {
                return KissJson.ToJSONData(this);
            }
            public JSONData GetJSONStringForClient()
            {
                JSONData data = JSONData.NewDictionary();
                data["uid"] = uid;
                data["title"] = title;
                data["question"] = question;
                if (hideIp)
                {
                    string[] ips = ip.Split('.');
                    if (ips.Length == 4)
                        data["ip"] = ips[0] + "." + ips[1] + ".*.*";
                    else
                        data["ip"] = "*.*.*.*";
                }
                else
                    data["ip"] = ip;
                data["createTime"] = createTime;
                data["answer"] = answer;
                data["answerTime"] = answerTime;
                return data;
            }
        }
        static DateTime Convert2DateTime(object obj)
        {
            if (obj.GetType() == typeof(DBNull))
                return new DateTime();
            return ((MySql.Data.Types.MySqlDateTime)obj).GetDateTime();
        }
        [CommandMethod]
        public static void InitMessage()
        {
            SortedDictionary<long, Message> messages = new SortedDictionary<long, Message>();
            new ThreadPoolMySql(
                (connection) => //Run in background thread!!!!!!!!
                {
                    string sql = "SELECT * FROM `Message` ORDER BY UID DESC LIMIT 1000";
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.CommandType = CommandType.Text;
                        using (MySqlDataAdapter msda = new MySqlDataAdapter())
                        {
                            msda.SelectCommand = cmd;
                            DataTable dt = new DataTable();
                            msda.Fill(dt);
                            foreach(DataRow dataRow in dt.Rows)
                            {
                                Message message = new Message()
                                {
                                    uid = Convert.ToInt64(dataRow["uid"]),
                                    title = Convert.ToString(dataRow["title"]),
                                    question = Convert.ToString(dataRow["question"]),
                                    nickname = Convert.ToString(dataRow["nickname"]),
                                    ip = Convert.ToString(dataRow["ip"]),
                                    hideIp = Convert.ToSByte(dataRow["hideIp"]) > 0,
                                    contact = Convert.ToString(dataRow["contact"]),
                                    createTime = Convert2DateTime(dataRow["createTime"]),
                                    answer = Convert.ToString(dataRow["answer"]),
                                    answerTime = Convert2DateTime(dataRow["answerTime"])
                                };
                                messages.Add(10000000000 - message.uid, message);//Why not add it directly to mMessages? Because they are in different threads, or else will cause thread problem.
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError($"Execute SQL '{sql}' occur error {e.Message} {e.StackTrace}", false);
                    }
                },
                () => //Run in main thread
                {
                    mMessages.Clear();
                    foreach (var message in messages)
                        mMessages[message.Key] = message.Value;
                });
        }
        /// <summary>
        /// All messages that max count is 1000
        /// </summary>
        static SortedDictionary<long, Message> mMessages = new SortedDictionary<long, Message>();
        /// <summary>
        /// Sample for callback JSON sting immediately
        /// </summary>
        /// <param name="title">Player title</param>
        /// <param name="question">Player question</param>
        /// <param name="contact">Player contact infomation, default is empty</param>
        /// <param name="nickname">Player nickname, default is 'anonymous'</param>
        /// <param name="hideIp">Player may don't want to show his ip, default is true</param>
        [WebMethod]
        public static string SaveMessage(string title, string question, string ip, Action<string> delayCallback, string contact = "", string nickname = "anonymous", bool hideIp = true)
        {
            Logger.LogInfo($"SaveMessage({title},{question},{ip},{contact},{nickname},{hideIp})");
            JSONData ret = JSONData.NewDictionary();
            if (IsLimitIP(ip))
            {
                ret["error"] = "A single IP can leave up to 3 messages every half an hour";
                return ret.ToJson();
            }
            Add2LimitIP(ip);
            DateTime createTime = DateTime.Now;
            List<MySqlParameter> _ps_ = new List<MySqlParameter>();
            MySqlParameter _param_;
            _param_ = new MySqlParameter("@title", MySqlDbType.String);
            _param_.Value = title;
            _ps_.Add(_param_);
            _param_ = new MySqlParameter("@question", MySqlDbType.String);
            _param_.Value = question;
            _ps_.Add(_param_);
            _param_ = new MySqlParameter("@nickname", MySqlDbType.String);
            _param_.Value = nickname;
            _ps_.Add(_param_);
            _param_ = new MySqlParameter("@contact", MySqlDbType.String);
            _param_.Value = contact;
            _ps_.Add(_param_);
            _param_ = new MySqlParameter("@ip", MySqlDbType.String);
            _param_.Value = ip;
            _ps_.Add(_param_);
            _param_ = new MySqlParameter("@hideIp", MySqlDbType.Byte);
            _param_.Value = hideIp;
            _ps_.Add(_param_);
            _param_ = new MySqlParameter("@createTime", MySqlDbType.DateTime);
            _param_.Value = createTime;
            _ps_.Add(_param_);
            Item.Insert("INSERT INTO `Message` (`title`,`question`,`nickname`,`contact`,`ip`,`hideIp`,`createTime`) VALUES (@title,@question,@nickname,@contact,@ip,@hideIp,@createTime)", _ps_,
                (_lastInsertedId_, _error_) =>
                {
                    if (string.IsNullOrEmpty(_error_))
                    {
                        ret["error"] = "success";
                        Message message = new Message()
                        {
                            uid = _lastInsertedId_,
                            title = title,
                            question = question,
                            nickname = nickname,
                            ip = ip,
                            hideIp = hideIp,
                            contact = contact,
                            createTime = createTime
                        };
                        mMessages[10000000000 - _lastInsertedId_] = message;
                        ret["message"] = message.GetJSONStringForClient();
                    }
                    else
                        ret["error"] = _error_;
                    ret["gm"] = ip == Framework.config.gmIP;
                    delayCallback(ret.ToJson());
                });
            return "";
        }
        static Dictionary<string, int> limitSaveMessages = new Dictionary<string, int>();
        /// <summary>
        /// Every 30 minutes reset the count for prevent someone attack the server
        /// </summary>
        [EventMethod(IntervalTime=1800f)]
        static void LimitSaveMessageByIP()
        {
            limitSaveMessages.Clear();
        }
        static bool IsLimitIP(string ip)
        {
            if (limitSaveMessages.TryGetValue(ip, out int count))
            {
                return count >= Framework.config.saveMessageCount;
            }
            return false;
        }
        static void Add2LimitIP(string ip)
        {
            if (limitSaveMessages.ContainsKey(ip))
                limitSaveMessages[ip] += 1;
            else
                limitSaveMessages[ip] = 1;
        }
        /// <summary>
        /// Load 50 message
        /// </summary>
        /// <param name="fromUID">Start from the UID, exclude that UID, default is 10000000000</param>
        /// <param name="myUIDs">Must contain those UIDs, so they alway can see their own questions. Split by ',' and max count is 3.</param>
        [WebMethod]
        public static string LoadMessage(string ip, long fromUID = 10000000000, string myUIDs = "")
        {
            Logger.LogInfo($"LoadMessage({ip},{fromUID},{myUIDs}");
            JSONData ret = JSONData.NewDictionary();
            ret["error"] = "success";
            JSONData messages = JSONData.NewList();
            ret["messages"] = messages;
            bool bGM = ip == Framework.config.gmIP;
            ret["gm"] = bGM;
            Dictionary<long, Message> myMessages = new Dictionary<long, Message>();
            if (string.IsNullOrEmpty(myUIDs))
            {
                try
                {
                    foreach (string str in myUIDs.Split(','))
                    {
                        long uid = Convert.ToInt64(str);
                        if (uid > 0 && mMessages.TryGetValue(10000000000 - uid, out Message message))
                        {
                            messages.Add(bGM ? message.ToJSONData() : message.GetJSONStringForClient());
                            myMessages.Add(uid, message);
                            if (myMessages.Count > 3)
                                break;
                        }
                    }
                }
                catch
                {
                }
            }
            int maxCount = Framework.config.maxMessages;
            Logger.LogInfo($"maxCount={maxCount},mMessages.Count={mMessages.Count}");
            foreach (Message one in mMessages.Values)
            {
                long uid = one.uid;
                if (uid < fromUID)
                    continue;
                if (messages.Count >= maxCount)
                    break;
                if (!myMessages.ContainsKey(uid))
                    messages.Add(bGM ? one.ToJSONData() : one.GetJSONStringForClient());
            }
            return ret.ToJson();
        }
        [CommandMethod]
        public static void SetGMIP(string ip)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                Framework.config.gmIP = ip;
            }
        }

        [WebMethod]
        public static string AnswerMessage(long uid, string answer, string ip)
        {
            JSONData ret = JSONData.NewDictionary();
            ret["error"] = "success";
            do
            {
                if (ip != Framework.config.gmIP)
                {
                    ret["error"] = "Invalid IP";
                    break;
                }
                if (mMessages.TryGetValue(10000000000 - uid, out Message message))
                {
                    DateTime dtNow = DateTime.Now;
                    message.answer = answer;
                    message.answerTime = dtNow;
                    ret["message"] = message.ToJSONData();

                    new ThreadPoolMySql(
                        (connection) =>
                        {
                            string sql = "UPDATE `Message` SET `answer`=@answer,`answerTime`=@answerTime WHERE `uid`=@uid";
                            try
                            {
                                MySqlCommand cmd = new MySqlCommand(sql, connection);
                                cmd.CommandType = CommandType.Text;
                                MySqlParameter _param_;
                                _param_ = new MySqlParameter("@answer", MySqlDbType.String);
                                _param_.Value = answer;
                                cmd.Parameters.Add(_param_);
                                _param_ = new MySqlParameter("@answerTime", MySqlDbType.DateTime);
                                _param_.Value = dtNow;
                                cmd.Parameters.Add(_param_);
                                _param_ = new MySqlParameter("@uid", MySqlDbType.Int64);
                                _param_.Value = uid;
                                cmd.Parameters.Add(_param_);
                                cmd.ExecuteNonQuery();
                            }
                            catch (Exception e)
                            {
                                Logger.LogError($"Execute SQL '{sql}' occur error {e.Message} {e.StackTrace}");
                            }
                        },
                        () =>
                        {
                        });
                }
                else
                {
                    ret["error"] = "Not exist uid : " + uid;
                }
            } while (false);
            return ret.ToJson();
        }
        [WebMethod]
        public static string DeleteMessage(long uid, string ip)
        {
            JSONData ret = JSONData.NewDictionary();
            ret["error"] = "success";
            do
            {
                if (ip != Framework.config.gmIP)
                {
                    ret["error"] = "Invalid IP";
                    break;
                }
                if (!mMessages.Remove(10000000000 - uid))
                {
                    ret["error"] = "Not exist uid : " + uid;
                    break;
                }

                new ThreadPoolMySql(
                    (connection) =>
                    {
                        string sql = uid == 0 ? "DELETE FROM `Message`" : "DELETE FROM `Message` WHERE `uid`=@uid";
                        try
                        {
                            MySqlCommand cmd = new MySqlCommand(sql, connection);
                            cmd.CommandType = CommandType.Text;
                            if (uid > 0)
                            {
                                MySqlParameter _param_;
                                _param_ = new MySqlParameter("@uid", MySqlDbType.Int64);
                                _param_.Value = uid;
                                cmd.Parameters.Add(_param_);
                            }
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            Logger.LogError($"Execute SQL '{sql}' occur error {e.Message} {e.StackTrace}");
                        }
                    },
                    () =>
                    {
                    });
            } while (false);
            return ret.ToJson();
        }
    }
}
