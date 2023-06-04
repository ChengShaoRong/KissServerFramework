using CSharpLike;
using KissFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace KissServerFramework
{
    public class HttpAccountManager : Singleton<HttpAccountManager>
    {
        Dictionary<int, Account> accountsByUid = new Dictionary<int, Account>();
        Dictionary<string, Account> accountsByNameAndAcctType = new Dictionary<string, Account>();
        Dictionary<string, Account> accountsByEMail = new Dictionary<string, Account>();
        void AddAccount(Account account)
        {
            accountsByUid[account.uid] = account;
            accountsByNameAndAcctType[account.name + "_" + account.acctType] = account;
            if (account.email.Length > 0)
                accountsByEMail[account.email] = account;
            Mail.SelectByAcctId(account.uid, (mails, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    account.SetMails(mails);
                    account.OnMailLoaded();
                }
                else
                    Logger.LogError(error);
            });
        }
        void RemoveAccount(Account account)
        {
            accountsByUid.Remove(account.uid);
            accountsByNameAndAcctType.Remove(account.name + "_" + account.acctType);
            if (account.email.Length > 0)
                accountsByEMail.Remove(account.email);
        }
        public void GetAccountByUidAndToken(int uid, string token, Action<Account, string> callback)
        {
            GetAccountByUid(uid, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    if (token != account.token || (account.tokenExpireTime - DateTime.Now).TotalSeconds < 0)
                        callback(null, "token not match or expire, please relogin.");
                    else
                    {
                        callback(account, error);
                    }
                }
                else
                    callback(account, error);
            });
        }
        public void GetAccountByUid(int uid, Action<Account, string> callback)
        {
            if (accountsByUid.TryGetValue(uid, out Account account))
                callback(account, "");
            else
            {
                Account.SelectByUid(uid, (accounts, error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        if (accounts.Count > 0)
                        {
                            if (accountsByUid.ContainsKey(uid))
                                callback(accountsByUid[uid], "");
                            else
                            {
                                account = accounts[0];
                                AddAccount(account);
                                callback(account, "");
                            }
                        }
                        else
                            callback(null, "not exist");
                    }
                    else
                        callback(null, error);
                });
            }
        }
        public void GetAccountByEmail(string email, Action<Account, string> callback)
        {
            if (accountsByEMail.TryGetValue(email, out Account account))
                callback(account, "");
            else
            {
                Account.SelectByEmail(email, (accounts, error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        if (accounts.Count > 0)
                        {
                            if (accountsByEMail.ContainsKey(email))
                                callback(accountsByEMail[email], "");
                            else
                            {
                                account = accounts[0];
                                AddAccount(account);
                                callback(account, "");
                            }
                        }
                        else
                            callback(null, "not exist");
                    }
                    else
                        callback(null, error);
                });
            }
        }
        public void GetAccountByNameAndAcctType(string name, int acctType, Action<Account, string> callback)
        {
            if (accountsByNameAndAcctType.TryGetValue(name+"_"+ acctType, out Account account))
            {
                callback(account, "");
            }
            else
            {
                Account.SelectByNameAndAcctType(name, acctType, (accounts, error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        if (accounts.Count > 0)
                        {
                            account = accounts[0];
                            AddAccount(account);
                            callback(account, "");
                        }
                        else
                            callback(null, "not exist");
                    }
                    else
                        callback(null, error);
                });
            }
        }
        public void UpdateEveryHour()
        {
            List<Account> deletes = new List<Account>();
            DateTime now = DateTime.Now;
            foreach(Account account in accountsByUid.Values)
            {
                if ((account.tokenExpireTime - now).TotalSeconds < 0)
                    deletes.Add(account);
            }
            foreach(Account account in deletes)
                RemoveAccount(account);
        }
        [WebMethod]
        public static string HttpGetLogAccount(int uid, string token, Action<string> delayCallback)
        {
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    JSONData jsonData = JSONData.NewDictionary();
                    jsonData["msg"] = "";
                    JSONData list = JSONData.NewList();
                    jsonData["data"] = list;
                    new ThreadPoolMySql(
                        (connection) =>
                        {
                            try
                            {
                                MySqlCommand cmd = new MySqlCommand("SELECT * FROM `LogAccount` WHERE `acctId` = @acctId", connection);
                                cmd.CommandType = CommandType.Text;
                                MySqlParameter _param_;
                                _param_ = new MySqlParameter("@acctId", MySqlDbType.Int32);
                                _param_.Value = uid;
                                cmd.Parameters.Add(_param_);
                                using (MySqlDataAdapter msda = new MySqlDataAdapter())
                                {
                                    msda.SelectCommand = cmd;
                                    DataTable dt = new DataTable();
                                    msda.Fill(dt);
                                    var raws = dt.Rows;
                                    for (int i = 0; i < raws.Count; i++)
                                    {
                                        JSONData one = JSONData.NewDictionary();
                                        list.Add(one);
                                        var data = raws[i];
                                        one["acctId"] = Convert.ToInt32(data["acctId"]);
                                        one["logType"] = Convert.ToInt32(data["logType"]);
                                        one["ip"] = Convert.ToString(data["ip"]);
                                        one["createTime"] = ((MySql.Data.Types.MySqlDateTime)data["createTime"]).GetDateTime();
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                jsonData["msg"] = e.Message;
                            }
                        },
                        () =>
                        {
                            delayCallback(jsonData.ToJson());
                        });
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpGetLogAccountStat(int uid, string token, int day, Action<string> delayCallback)
        {
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    JSONData jsonData = JSONData.NewDictionary();
                    jsonData["msg"] = "";
                    JSONData days = JSONData.NewList();
                    jsonData["days"] = days;
                    JSONData logins = JSONData.NewList();
                    jsonData["logins"] = logins;
                    JSONData registers = JSONData.NewList();
                    jsonData["registers"] = registers;
                    Dictionary<string, int> ls = new Dictionary<string, int>();
                    Dictionary<string, int> rs = new Dictionary<string, int>();
                    DateTime dtNow = DateTime.Now;
                    new ThreadPoolMySql(
                        (connection) =>
                        {
                            try
                            {
                                MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) AS C, DATE_FORMAT(createTime, \"%Y-%m-%d\") AS D FROM `LogAccount` WHERE `logType`=0 AND `createTime` >= DATE_ADD(NOW(),INTERVAL @day day) GROUP BY D ORDER BY D", connection);
                                cmd.CommandType = CommandType.Text;
                                MySqlParameter _param_;
                                _param_ = new MySqlParameter("@day", MySqlDbType.Int32);
                                _param_.Value = day;
                                cmd.Parameters.Add(_param_);
                                using (MySqlDataAdapter msda = new MySqlDataAdapter())
                                {
                                    msda.SelectCommand = cmd;
                                    DataTable dt = new DataTable();
                                    msda.Fill(dt);
                                    var raws = dt.Rows;
                                    for (int i = 0; i < raws.Count; i++)
                                    {
                                        JSONData one = JSONData.NewDictionary();
                                        var data = raws[i];
                                        ls.Add(Convert.ToString(data["D"]), Convert.ToInt32(data["C"]));
                                    }
                                }
                                cmd = new MySqlCommand("SELECT COUNT(*) AS C, DATE_FORMAT(createTime, \"%Y-%m-%d\") AS D FROM `LogAccount` WHERE `logType`=1 AND `createTime` >= DATE_ADD(NOW(),INTERVAL @day day) GROUP BY D ORDER BY D", connection);
                                cmd.CommandType = CommandType.Text;
                                _param_ = new MySqlParameter("@day", MySqlDbType.Int32);
                                _param_.Value = day;
                                cmd.Parameters.Add(_param_);
                                using (MySqlDataAdapter msda = new MySqlDataAdapter())
                                {
                                    msda.SelectCommand = cmd;
                                    DataTable dt = new DataTable();
                                    msda.Fill(dt);
                                    var raws = dt.Rows;
                                    for (int i = 0; i < raws.Count; i++)
                                    {
                                        JSONData one = JSONData.NewDictionary();
                                        var data = raws[i];
                                        rs.Add(Convert.ToString(data["D"]), Convert.ToInt32(data["C"]));
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                jsonData["msg"] = e.Message;
                            }
                        },
                        () =>
                        {
                            for(int i=day; i<=0; i++)
                            {
                                string key = dtNow.AddDays(i).ToString("yyyy-MM-dd");
                                days.Add(key);
                                logins.Add(ls.ContainsKey(key) ? ls[key] : 0);
                                registers.Add(rs.ContainsKey(key) ? rs[key] : 0);
                            }
                            delayCallback(jsonData.ToJson());
                        });
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpGetMail(int uid, string token, Action<string> delayCallback)
        {
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    JSONData jsonData = JSONData.NewDictionary();
                    jsonData["msg"] = "";
                    JSONData list = JSONData.NewList();
                    jsonData["data"] = list;
                    foreach (Mail mail in account.mails.Values)
                    {
                        JSONData data = mail.ToJSONData();
                        data.RemoveKey("_uid_");
                        data.RemoveKey("_sendMask_");
                        list.Add(data);
                    }
                    delayCallback(jsonData.ToJson());
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpAccountToken(int uid, string token, string ip, Action<string> delayCallback)
        {
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    account.tokenExpireTime = DateTime.Now.AddHours(Framework.config.tokenExpireTime);
                    delayCallback(account.jsonDataHTTP.ToJson());
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpAccountLogin(string name, string password, string token, string validCode, string ip, Action<string> delayCallback, int acctType = (int)Account.AccountType.BuildInHTTP)
        {
            Logger.LogInfo($"HttpAccountLogin : {name}, {acctType}, {password}, {token}, {validCode} from {ip}");
            if (!ValidCode.CheckCode(token, validCode.ToUpper()))
                return Framework.NewJSONMsg($"code invalid or expired");
            name = name.Trim();
            if (!Utils.ValidName(name))
                return Framework.NewJSONMsg($"invalid name");
            password = password.Trim();
            if (!Utils.ValidPassword(password))
                return Framework.NewJSONMsg($"invalid password");
            Instance.GetAccountByNameAndAcctType(name, acctType, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    if (Utils.HashPassword(password) != account.password)
                        delayCallback(Framework.NewJSONMsg("name or password not match"));
                    else
                    {
                        account.token = Guid.NewGuid().ToString("N");
                        account.tokenExpireTime = DateTime.Now.AddHours(Framework.config.tokenExpireTime);
                        delayCallback(account.jsonDataHTTP.ToJson());
                        LogManager.LogAccount(account.uid, (int)LogManager.LogAccountType.Login, ip);
                    }
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpAccountLogout(int uid, string token, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"HttpAccountLogout : uid = {uid} token = {token} from {ip}");
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    account.tokenExpireTime = DateTime.Now;
                    delayCallback(Framework.NewJSONMsg(""));
                    LogManager.LogAccount(account.uid, (int)LogManager.LogAccountType.Logout, ip);
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpAccountChangePassword(int uid, string token, string passwordOld, string passwordNew, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"HttpAccountChangePassword : uid = {uid} passwordOld = {passwordOld} passwordNew = {passwordNew} from {ip}");
            passwordOld = passwordOld.Trim();
            passwordNew = passwordNew.Trim();
            if (passwordNew.Length < 6 || passwordNew.Length > 64)
                return Framework.NewJSONMsg($"invalid new password");
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    if (!string.IsNullOrEmpty(account.password) && account.password != Utils.HashPassword(passwordOld))
                        delayCallback(Framework.NewJSONMsg($"invalid old password"));
                    else
                    {
                        account.password = Utils.HashPassword(passwordNew);
                        account.token = Guid.NewGuid().ToString("N");
                        account.tokenExpireTime = DateTime.Now.AddHours(Framework.config.tokenExpireTime);
                        JSONData jsonReturn = JSONData.NewDictionary();
                        jsonReturn["msg"] = "";
                        jsonReturn["token"] = account.token;
                        jsonReturn["uid"] = account.uid;
                        delayCallback(jsonReturn.ToJson());
                    }
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpAccountRegister(string name, string password, string email, string token, string validCode, string lang, string ip, Action<string> delayCallback, int acctType = (int)Account.AccountType.BuildInHTTP)
        {
            Logger.LogInfo($"HttpAccountRegister : {name}, {acctType}, {password}, {email}, {token}, {validCode} from {ip}");
            if (!ValidCode.CheckCode(token, validCode.ToUpper()))
                return Framework.NewJSONMsg($"code invalid or expired");
            name = name.Trim();
            if (!Utils.ValidName(name))
                return Framework.NewJSONMsg($"invalid name");
            password = password.Trim();
            if (!Utils.ValidPassword(password))
                return Framework.NewJSONMsg($"invalid password");
            email = email.Trim();
            if (email.Length > 0 && !Utils.ValidMail(email))
                return Framework.NewJSONMsg($"invalid email");
            Instance.GetAccountByNameAndAcctType(name, acctType, (account, error) =>
            {
                if (error == "not exist")
                {
                    if (email.Length > 0)
                    {
                        Instance.GetAccountByEmail(email, (account2, error2) =>
                        {
                            if (error2 == "not exist")
                                Account.Insert(acctType, name, Utils.HashPassword(password), name, Guid.NewGuid().ToString("N"),
                                    DateTime.Now.AddHours(Framework.config.tokenExpireTime), "", (account, error) =>
                                    {
                                        if (!string.IsNullOrEmpty(error))
                                            delayCallback(Framework.NewJSONMsg(error));
                                        else if (Instance.accountsByNameAndAcctType.ContainsKey(name + "_" + acctType))
                                            delayCallback(Framework.NewJSONMsg($"name '{name}' already exist."));
                                        else
                                        {
                                            Instance.AddAccount(account);
                                            delayCallback(account.jsonDataHTTP.ToJson());
                                            EMailManager.SendComfirmMail(account.uid, email, lang, (error) =>
                                            {
                                                Logger.LogInfo("HttpAccountRegister:SendComfirmMail:" + error);
                                            });
                                            LogManager.LogAccount(account.uid, (int)LogManager.LogAccountType.Register, ip);
                                            account.SendMail("Welcome", "You register account success.");
                                        }
                                    });
                            else if (account2 != null)
                                delayCallback(Framework.NewJSONMsg("Email exist, use other email or using reset password by email."));
                            else
                                delayCallback(Framework.NewJSONMsg(error2));
                        });
                    }
                    else
                        Account.Insert(acctType, name, Utils.HashPassword(password), name, Guid.NewGuid().ToString("N"),
                            DateTime.Now.AddHours(Framework.config.tokenExpireTime), email, (account, error) =>
                            {
                                if (!string.IsNullOrEmpty(error))
                                    delayCallback(Framework.NewJSONMsg(error));
                                else if (Instance.accountsByNameAndAcctType.ContainsKey(name + "_" + acctType))
                                    delayCallback(Framework.NewJSONMsg($"name '{name}' already exist."));
                                else
                                {
                                    Instance.AddAccount(account);
                                    delayCallback(account.jsonDataHTTP.ToJson());
                                    LogManager.LogAccount(account.uid, (int)LogManager.LogAccountType.Register, ip);
                                    account.SendMail("Welcome", "You register account success.");
                                }
                            });
                }
                else if (account != null)
                    delayCallback(Framework.NewJSONMsg($"name '{name}' already exist."));
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        class ResetPassword
        {
            public ResetPassword(int uid)
            {
                this.uid = uid;
                token = Guid.NewGuid().ToString("N");
                tokenExpire = DateTime.Now.AddMinutes(10);
                Framework.RaiseUniqueEvent(OnTimeout, "ResetPassword_" + uid, 60 * 10, 1);
                waitingResetPasswords[token] = this;
                waitingResetPasswordsByUID[uid] = this;
            }
            public void Release()
            {
                Logger.LogInfo("ResetPassword:Release:" + uid);
                Framework.CancelEvent("ResetPassword_" + uid);
                waitingResetPasswords.Remove(token);
                waitingResetPasswordsByUID.Remove(uid);
                Framework.CancelEvent("ResetPassword_" + uid);
            }
            public int uid;
            public string token;
            public DateTime tokenExpire;
            void OnTimeout()
            {
                Logger.LogInfo("ResetPassword:OnTimeout:"+ uid);
                Release();
            }
        }
        static Dictionary<string, ResetPassword> waitingResetPasswords = new Dictionary<string, ResetPassword>();
        static Dictionary<int, ResetPassword> waitingResetPasswordsByUID = new Dictionary<int, ResetPassword>();
        [WebMethod]
        public static string HttpAccountConfirmResetPassword(string token, string passwordNew, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"HttpAccountConfirmResetPassword : token = {token} passwordNew = {passwordNew} from {ip}");
            passwordNew = passwordNew.Trim();
            if (passwordNew.Length < 6 || passwordNew.Length > 64)
                return Framework.NewJSONMsg($"invalid new password");
            if (!waitingResetPasswords.ContainsKey(token))
                return Framework.NewJSONMsg($"invalid token or token expire");
            else if (waitingResetPasswords[token].tokenExpire < DateTime.Now)
                return Framework.NewJSONMsg($"token expire");
            else
            {
                Instance.GetAccountByUid(waitingResetPasswords[token].uid, (account, error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        account.password = Utils.HashPassword(passwordNew);
                        account.token = Guid.NewGuid().ToString("N");
                        account.tokenExpireTime = DateTime.Now.AddHours(Framework.config.tokenExpireTime);
                        JSONData jsonReturn = JSONData.NewDictionary();
                        jsonReturn["msg"] = "";
                        jsonReturn["token"] = account.token;
                        jsonReturn["uid"] = account.uid;
                        delayCallback(jsonReturn.ToJson());
                    }
                    else
                        delayCallback(Framework.NewJSONMsg(error));
                });
            }
            return "";
        }
        [WebMethod]
        public static string HttpAccountRequestResetPassword(string email, string token, string validCode, string lang, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"HttpAccountResetPassword : {email},{token},{validCode} from {ip}");
            if (!ValidCode.CheckCode(token, validCode.ToUpper()))
                return Framework.NewJSONMsg($"code invalid or expired");
            email = email.Trim();
            if (!Utils.ValidMail(email))
                return Framework.NewJSONMsg($"Invalid email format");
            else
            {
                Instance.GetAccountByEmail(email, (account, error) =>
                {
                    if (string.IsNullOrEmpty(error))
                    {
                        if (waitingResetPasswordsByUID.ContainsKey(account.uid))
                        {
                            waitingResetPasswordsByUID[account.uid].Release();
                        }
                        ResetPassword resetPassword = new ResetPassword(account.uid);
                        EMailManager.SendResetPasswordEmail(email, resetPassword.token, account.name, lang, (error) =>
                        {
                            if (string.IsNullOrEmpty(error))
                                delayCallback(Framework.NewJSONMsg("Reset password web link had send to your email. Please check out your email and then modify your password."));
                            else
                            {
                                resetPassword.Release();
                                delayCallback(Framework.NewJSONMsg(error));
                            }
                        });
                    }
                    else
                        delayCallback(Framework.NewJSONMsg(error));
                });
            }
            return "";
        }
        [WebMethod]
        public static string HttpAccountModifyEmail(int uid, string token, string email, string lang, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"HttpAccountModifyEmail : uid = {uid} email = {email}  from {ip}");
            email = email.Trim();
            if (Utils.ValidMail(email))
                return Framework.NewJSONMsg($"invalid email");
            Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    if (email == account.email)
                        delayCallback(Framework.NewJSONMsg($"same email"));
                    else
                    {
                        Instance.GetAccountByEmail(email, (account2, error2) =>
                        {
                            if (account2 == null)
                            {
                                EMailManager.SendComfirmMail(uid, email, lang, (error) =>
                                {
                                    if (string.IsNullOrEmpty(error))
                                    {
                                        JSONData jsonReturn = JSONData.NewDictionary();
                                        jsonReturn["msg"] = "";
                                        jsonReturn["desc"] = "A verify email had been send to your email address, please check your email.";
                                        delayCallback(jsonReturn.ToJson());
                                    }
                                    else
                                        delayCallback(Framework.NewJSONMsg(error));
                                });
                            }
                            else
                                delayCallback(Framework.NewJSONMsg($"This email was bound, please change to other email. If your forget your login name or password, your can reset your password by this email."));
                        });
                    }
                }
                else
                    delayCallback(Framework.NewJSONMsg(error));
            });
            return "";
        }
        [WebMethod]
        public static string HttpAccountConfirmEmail(string token, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"HttpAccountConfirmEmail : uid = {token} from {ip}");
            EMailManager.VerifyComfirmEmail(token, (uid, email) =>
            {
                if (uid == 0)
                    delayCallback(Framework.NewJSONMsg("Comfirm email fail for token was expired, please resend email."));
                else
                    Instance.GetAccountByUid(uid, (account, error) =>
                    {
                        if (string.IsNullOrEmpty(error))
                        {
                            Instance.GetAccountByEmail(email, (account2, error2) =>
                            {
                                if (string.IsNullOrEmpty(error2))
                                    delayCallback(Framework.NewJSONMsg("Comfirm email fail for this email was used."));
                                else
                                {
                                    account.email = email;
                                    Instance.accountsByEMail[account.email] = account;
                                    delayCallback(Framework.NewJSONMsg(""));
                                }
                            });
                        }
                        else
                            delayCallback(Framework.NewJSONMsg(error));
                    });
            });
            return "";
        }
    }
}
