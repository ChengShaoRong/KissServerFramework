using KissFramework;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace KissServerFramework
{
    public class AccountManager : Singleton<AccountManager>
    {
        /// <summary>
        /// All account unique key by uid
        /// </summary>
        Dictionary<int, Account> accounts = new Dictionary<int, Account>();
        /// <summary>
        /// All account unique key by player instance
        /// </summary>
        Dictionary<Account, Player> playersByAccount = new Dictionary<Account, Player>();
        /// <summary>
        /// All account unique key by 'name+"_"+acctType'
        /// </summary>
        Dictionary<string, Account> accountsByName = new Dictionary<string, Account>();
        /// <summary>
        /// Get account by uid
        /// </summary>
        /// <param name="uid">unique id of account</param>
        public Account GetAccount(int uid)
        {
            Account account;
            if (accounts.TryGetValue(uid, out account))
                return account;
            return null;
        }
        /// <summary>
        /// Get account by login name and account type.
        /// </summary>
        /// <param name="name">login name, not nickname</param>
        /// <param name="acctType">account type</param>
        public Account GetAccount(string name, int acctType)
        {
            if (accountsByName.TryGetValue(name + "_" + acctType, out Account account))
                return account;
            return null;
        }
        /// <summary>
        /// Get player by account.
        /// </summary>
        /// <param name="account">The account instance</param>
        public Player GetPlayer(Account account)
        {
            if (playersByAccount.TryGetValue(account, out Player player))
                return player;
            return null;
        }
        /// <summary>
        /// Check the account whether exist in cache.
        /// If exist use the data in cache, otherwise use the current data and cache it.
        /// You should check it because you can't make sure other request whether
        /// had loaded it or modified it while you loading it from database in multi-threading.
        /// </summary>
        /// <param name="account">The account just load from database</param>
        public void GetAccount(ref Account account)
        {
            if (accounts.TryGetValue(account.uid, out Account accountCache))
                account = accountCache;
            else//if not exist we cache it
            {
                accounts[account.uid] = account;
                accountsByName[account.name + "_" + account.acctType] = account;
            }
        }
        public Account GetAccountByNickname(string nickname)
        {
            foreach(var account in accounts.Values)
            {
                if (account.nickname == nickname)
                    return account;
            }
            return null;
        }

        /// <summary>
        /// Login server.
        /// This step check the third party SDK.
        /// If OK will go to next step.
        /// </summary>
        /// <param name="jsonData">JSONData from client(name/password)</param>
        /// <param name="player">current player</param>
        public void Login(JSONData jsonData, Player player)
        {
            //You may process re login, we send back the account instance to client
            if (player.account != null && player.account.acctType == jsonData["acctType"])
            {
                Player oldPlayer = GetPlayer(player.account);
                if (oldPlayer != player)
                {
                    oldPlayer.Replace(player);//will disconnect the old player
                    player = oldPlayer;
                }
                jsonData = JSONData.NewPacket(PacketType.CB_AccountLogin);
                jsonData["account"] = player.account.ToJSONData();
                player.Send(jsonData);

                //And other data you should send back to client too, if had.
                return;
            }

            Account.AccountType acctType = (Account.AccountType)(int)jsonData["acctType"];
            switch (acctType)
            {
                case Account.AccountType.BuildIn:
                    LoginStep2(jsonData, player);
                    return;
                case Account.AccountType.ThirdParty_Test:
                    {
                        //Sample of using POST
                        string strURL = $"http://127.0.0.1:{Framework.config.httpServerPort}/TestThirdPartyAccount";
                        JSONData jsonPost = JSONData.NewDictionary();
                        jsonPost["uid"] = jsonData["name"];
                        jsonPost["token"] = jsonData["password"];
                        jsonPost["sign"] = FrameworkBase.GetMD5((string)jsonData["uid"] + jsonData["token"] + "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
                        //You can post JSONData or string like below.
                        //string strPost = $"uid={jsonData["name"]}&token={jsonData["password"]}&sign={FrameworkBase.GetMD5((string)jsonData["uid"] + jsonData["token"] + "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz")}";
                        new ThreadPoolHttp(strURL, jsonPost.ToString(),
                            (callback) =>//The HTTP request running in background thread, and this callback run in main thread.
                            {
                                JSONData jsonCallback = KissJson.ToJSONData(callback);
                                if (jsonCallback["state"] == 0)//success
                                {
                                    if (player == null)
                                    {
                                        Logger.LogWarning("player offline, ignore request in Login");
                                        return;
                                    }
                                    LoginStep2(jsonData, player);
                                }
                                else
                                {
                                    player.CallbackError($"url={strURL} post={jsonPost} callback={callback}");
                                }
                            });
                        /*
                        //Sample of using GET
                        string strURL = $"http://127.0.0.1:{Framework.config.httpServerPort}/TestThirdPartyAccount?uid={jsonData["name"]}&token={jsonData["password"]}&sign={FrameworkBase.GetMD5((string)jsonData["uid"] + jsonData["token"] + "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz")}";
                        new ThreadPoolHttp(strURL, 
                            (callback) =>//The HTTP request running in background thread, and this callback run in main thread.
                            {
                                JSONData jsonCallback = KissJson.ToJSONData(callback);
                                if (jsonCallback["state"] == 0)//success
                                {
                                    if (player == null)
                                    {
                                        Logger.LogWarning("player offline, ignore request in LoginStep2");
                                        return;
                                    }
                                    LoginStep2(jsonData, player);
                                }
                                else
                                {
                                    player.CallbackError($"url={strURL} callback={callback}");
                                }
                            });
                        */
                        /*
                        //If you have to custom the WebRequest by yourself.
                        string strURL = $"http://127.0.0.1:{Framework.config.httpServerPort}/TestThirdPartyAccount";
                        var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(strURL);
                        //To do your special process to the WebRequest.
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.Method = "GET";

                        new ThreadPoolHttp(request, 
                            (callback) =>
                            {
                                JSONData jsonCallback = KissJson.ToJSONData(callback);
                                if (jsonCallback["state"] == 0)//success
                                {
                                    if (player == null)
                                    {
                                        Logger.LogWarning("player offline, ignore request in LoginStep2");
                                        return;
                                    }
                                    LoginStep2(jsonData, player);
                                }
                                else
                                {
                                    player.CallbackError($"url={strURL} callback={callback}");
                                }
                            });
                        */
                    }
                    break;
                default:
                    player.CallbackError("Not support acctType:" + jsonData["acctType"]);
                    return;
            }
        }
        /// <summary>
        /// Select data from Database if not in cache, and insert into database if not exist.
        /// And then go to next step.
        /// </summary>
        /// <param name="jsonData">JSONData from client(name/password)</param>
        /// <param name="player">current player</param>
        void LoginStep2(JSONData jsonData, Player player)
        {
            string name = ThreadPoolMySql.ReplaceInjectString(jsonData["name"]);
            jsonData["name"] = name;
            int acctType = jsonData["acctType"];
            //check the cache
            Account account;
            if (!accountsByName.TryGetValue(name + "_" + acctType, out account))//no cache
            {
                Account.SelectByNameAndAcctType(jsonData["name"], jsonData["acctType"], (accounts, error) =>
                {
                    //select account occur error
                    if (!string.IsNullOrEmpty(error))
                    {
                        player.CallbackError($"Select account occur error : {error}.");
                        return;
                    }
                    //New account if not exist account
                    if (accounts.Count == 0)
                    {
                        Account.Insert(jsonData["acctType"], DateTime.Now, jsonData["name"], jsonData["password"],
                            "Guest" + FrameworkBase.GetRand(100000), 0, 0, DateTime.Now,
                            (newAccount, error) =>
                            {
                                //select account occur error
                                if (!string.IsNullOrEmpty(error))
                                {
                                    player.CallbackError($"Insert database error : {error}.");
                                    return;
                                }
                                //Using the account that just inserted into database
                                LoginStep3(jsonData, newAccount, player);
                            });
                        return;
                    }
                    //Using the account select from database
                    LoginStep3(jsonData, accounts[0], player);
                });
                return;
            }
            //Using the account in cache
            LoginStep3(jsonData, account, player);
        }
        /// <summary>
        /// Check account info whether valid, and then send to client.
        /// </summary>
        /// <param name="jsonData">JSONData from client(name/password)</param>
        /// <param name="account">the account instance</param>
        /// <param name="player">current player</param>
        void LoginStep3(JSONData jsonData, Account account, Player player)
        {
            //cache account
            GetAccount(ref account);

            if (account.acctType == (int)Account.AccountType.BuildIn && account.password != jsonData["password"])
                player.CallbackError("password not match");//build-in account check password here.
            else
            {
                //whether exist the old player instance
                Player oldPlayer = GetPlayer(account);
                if (oldPlayer != null)
                {
                    oldPlayer.Replace(player);//will disconnect the old player
                    player = oldPlayer;
                }
                else
                {
                    player.account = account;
                    playersByAccount[account] = player;
                }
                //callback to client
                jsonData = JSONData.NewPacket(PacketType.CB_AccountLogin);
                jsonData["account"] = account.ToJSONData();
                player.Send(jsonData);

                //Log account
                LogManager.Instance.LogAccount(account.uid, 0, player.IP);
            }
        }
        public void ChangeNameAndPassword(JSONData jsonData, Player player)
        {
            if (player.account == null)
            {
                player.CallbackError("Not login yet!");
                return;
            }
            if (player.account.acctType != (int)Account.AccountType.BuildIn)
            {
                player.CallbackError("Only can change the password of build-in account!");
                return;
            }
            string oldPW = ThreadPoolMySql.ReplaceInjectString(jsonData["oldPassword"]).Trim();
            if (!string.IsNullOrEmpty(player.account.password) && oldPW != player.account.password)
            {
                player.CallbackError("Password not match!");
                return;
            }
            string newPW = ThreadPoolMySql.ReplaceInjectString(jsonData["newPassword"]).Trim();
            if (newPW.Length < 6 && newPW.Length > 64)
            {
                player.CallbackError("Password length must less than 65 and more then 5!");
                return;
            }
            string newName = jsonData["newName"];
            if (!string.IsNullOrEmpty(newName) && player.account.name != newName)//change name too
            {
                //check cache
                if (accountsByName.ContainsKey(newName + "_0"))
                {
                    player.CallbackError("That name use by other player, can't use this name:" + newName);
                    return;
                }
                //check database
                Account.SelectByNameAndAcctType(newName, (int)Account.AccountType.BuildIn, (accounts, error) =>
                {
                    if (player == null)
                    {
                        Logger.LogWarning("player offline, ignore request");
                        return;
                    }
                    if (!string.IsNullOrEmpty(error))
                    {
                        player.CallbackError("That name use by other player, can't use this name:" + newName);
                        return;
                    }
                    if (accountsByName.ContainsKey(newName + "_0"))//check cache again, may be used by other player while query database
                    {
                        player.CallbackError("That name use by other player, can't use this name:" + newName);
                        return;
                    }
                    if (accounts.Count > 0)//database exist
                    {
                        //cache this account
                        Account accountNew = accounts[0];
                        GetAccount(ref accountNew);
                        player.CallbackError("That name use by other player, can't use this name:" + newName);
                    }
                    else
                    {
                        //update cache
                        accountsByName.Remove(player.account.name);
                        accountsByName[newName] = player.account;
                        player.account.name = newName;
                        player.account.password = newPW;
                        player.account.Update();
                        jsonData = JSONData.NewPacket(PacketType.CB_AccountChangeNameAndPassword);
                        jsonData["password"] = newPW;
                        jsonData["name"] = newName;
                        player.Send(jsonData);
                    }
                });
                return;
            }
            player.account.password = newPW;
            player.account.Update();
            jsonData = JSONData.NewPacket(PacketType.CB_AccountChangeNameAndPassword);
            jsonData["password"] = newPW;
            jsonData["name"] = player.account.name;
            player.Send(jsonData);
        }
    }
}
