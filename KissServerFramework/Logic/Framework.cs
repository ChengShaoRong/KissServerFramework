
using CSharpLike;
using KissFramework;
using System;
using System.Collections.Generic;
using System.IO;

namespace KissServerFramework
{
    /// <summary>
    /// All our logic running in main thread, 
    /// You don't need to use 'lock' syntax!
    /// </summary>
    public class Framework : FrameworkBase
    {
        static Framework instance;
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static Framework Instance
        {
            get
            {
                if (instance == null)
                    instance = new Framework();
                return instance;
            }
        }
        public static string NewJSONMsg(string msg)
        {
            JSONData jsonData = JSONData.NewDictionary();
            jsonData["msg"] = msg;
            return jsonData.ToJson();
        }
        /// <summary>
        /// Initialize CSV file, that run before OnStart.
        /// You can call this function by yourself to reload them if your CSV file was moidfied and need to be reload.
        /// </summary>
        public override void InitializeCSV()
        {
            Logger.LogInfo("Framework:InitializeCSV");
            //We recommend read data from CSV file with class(NOT struct!).
            KissCSV.Load(typeof(TestCsv), "TestCsv.csv", "id");
            //Sample for read a row data as class object
            TestCsv testCsv = KissCSV.Get("TestCsv.csv", 1) as TestCsv;
            if (testCsv != null)//If not exist "1" in columnName "id" will return null.
            {
                Logger.LogInfo($"TestCsv id={testCsv.id}");//output id=1
                Logger.LogInfo($"TestCsv name={testCsv.name}");//output name=test name
            }

            //If you don't want to define a class ,you can read data from CSV file by SimpleKissCSV.
            SimpleKissCSV.Load("Item.csv", "id");
            //Sample for how to read value from CSV file.
            Logger.LogInfo($"test CSV maxStack={SimpleKissCSV.GetInt("Item.csv", "100", "maxStack")}");
            Logger.LogInfo($"test CSV name={SimpleKissCSV.GetString("Item.csv", "100", "name")}");
        }

        public override void CheckDatabaseFilePermission(int uid, string token, Action<int> callback)
        {
            if (uid == 0 || string.IsNullOrEmpty(token))
            {
                callback(int.MaxValue);//Mean only get the public database file.
                return;//Not need check the database for permission
            }
            HttpAccountManager.Instance.GetAccountByUidAndToken(uid, token, (account, error) =>
            {
                if (string.IsNullOrEmpty(error))
                {
                    callback(account.uid);//The user uid mean just can get the public database file And the file owner is this account.
                    //callback(0);//Mean can get all database file, is administrator mode.
                }
                else
                    callback(int.MaxValue);//Mean only get the public database file.
            });
        }

        /// <summary>
        /// When the KISS framework initialized will call this function, you can call your initialize here.
        /// </summary>
        protected override void OnStart()
        {
            Logger.LogInfo("Framework:OnStart");

            AircraftBattleManager.Instance.Initialize();
            //Sample for using event timer
            RaiseEvent(ChatRoomManager.Instance.Update, 60f);//we call 'ChatRoomManager.Instance.Update' in every 60 seconds.
            RaiseEvent(HttpAccountManager.Instance.UpdateEveryHour, 3600f);
            //RaiseEvent(ForceCheckCacheFile, 1f);
            HttpMessageManager.InitMessage();
        }
        [CommandMethod]
        public static void Reload()
        {
            // Register command 'reload' for reload JSON config file in console
            // Reload the config value from your config JSON file.
            // You can consider as you had reload the value in Config, but exclude the value in ConfigBase (The value in ConfigBase changed but not take effect.).
            FrameworkBase.config = Framework.Instance.CreateConfig(Environment.CurrentDirectory
                + "/" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".json");

            //Add your code here for make your config value take effect.
            ReqGateway.Clear();//Make sure the gateway server infomation reload from the config JSON.

            instance.ForceCheckCacheFile();
        }

        /// <summary>
        /// When you input 'exit' or 'quit' in console will call this function.
        /// But if you use 'Ctrl+C' or click 'X' close button won't reach here,
        /// it's not a good way to exit which cause Update/Insert database failed.
        /// Make sure your work done before exit.
        /// When all your work done, call FrameworkBase.MarkAllDoneForSafeExit() 
        /// to notify the framework exit.
        /// </summary>
        protected override void OnNormalExit()
        {
            Logger.LogInfo("Framework:OnNormalExit");
            //Do your work in main thread here.

            //And then do your work in thread, e.g. save data into database.
            //And call FrameworkBase.MarkAllDoneForSafeExit() in that thread.

            FrameworkBase.MarkAllDoneForSafeExit();//You should call this while your work all done.
        }
        /// <summary>
        /// return client ip to client
        /// </summary>
        [WebMethod]
        static string GetSelfIP(string ip)
        {
            return ip;
        }
        #region don't modify it
        /// <summary>
        /// The config instance.
        /// Normally you don't need to modify it.
        /// </summary>
        public static new Config config
        {
            get
            {
                return (Config)FrameworkBase.config;
            }
        }
        /// <summary>
        /// Custom config instance, this function call by FrameworkBase only.
        /// Generally, you don't need to modify it.
        /// </summary>
        /// <param name="strJSON">The config JSON string load from the file
        /// (Environment.CurrentDirectory + "/" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".json").
        /// If that file not exist will receive a empty string</param>
        /// <returns>The Config instance</returns>
        protected override ConfigBase CreateConfig(string strJSON)
        {
            return string.IsNullOrEmpty(strJSON) ? new Config() : (Config)KissJson.ToObject(typeof(Config), strJSON);
        }
        /// <summary>
        /// Custom Player instance, this function call by FrameworkBase only.
        /// Normally you don't need to modify it.
        /// </summary>
        protected override PlayerBase CreatePlayer()
        {
            return new Player();
        }
        #endregion
    }
}
