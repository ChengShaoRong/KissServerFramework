
using CSharpLike;
using KissFramework;
using System;
using System.Collections.Generic;

namespace KissServerFramework
{
    /// <summary>
    /// All our logic running in main thread, 
    /// You don't need to use 'lock' syntax!
    /// </summary>
    public class Framework : FrameworkBase
    {
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
        /// <summary>
        /// When the KISS framework initialized will call this function, you can call your initialize here.
        /// </summary>
        protected override void OnStart()
        {
            Logger.LogInfo("Framework:OnStart");

            AircraftBattleManager.Instance.Initialize();
            //Sample for using event timer
            RaiseEvent(ChatRoomManager.Instance.Update, 60f);//we call 'ChatRoomManager.Instance.Update' in every 60 seconds.

            //Bind the HTTP URL into different class to handle it.
            //It's recommend using this way handle HTTP.
            //That make your code look more simple and tidy than handle all HTTP URLs in
            //'protected override string OnHttpMessage(string url, JSONData jsonData, string ip, Action<string> delayCallback)'.
            //That run in main thread.
            BindHttpMsg("/TestHttp", TestHttp.OnHttpMessage);//the url is case-insensitive
            BindHttpMsg("/ReqGateway", ReqGateway.OnHttpMessage);//Client requet config setting
            BindHttpMsg("/TestThirdPartyAccount", (jsonData, ip, delayCallback) =>
            {
                //e.g. GET : url = 'http://ip[:port]/TestThirdPartyAccount?uid=123456789&token=xxxxxx&sign=yyyyyy'
                //e.g. POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = 'uid=123456789&token=xxxxxx&sign=yyyyyy'
                //e.g. POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = '{"uid":"123456789","token":"xxxxxx","sign":"yyyyyy"}'
                //We are both got jsonData = {"uid":"123456789","token":"xxxxxx","sign":"yyyyyy"}
                //If post the JSON string, we will get the JSON you send.
                JSONData jsonReturn = JSONData.NewDictionary();
                //int uid=jsonData["uid"];//You will got the int number 123456789 both the JSON {"uid":"123456789"} and {"uid":123456789}
                //sign = md5(uid+token+key)
                string sign = GetMD5((string)jsonData["uid"] + jsonData["token"] + "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
                if (sign != jsonData["sign"])
                {
                    jsonReturn["state"] = 1;
                    jsonReturn["msg"] = "sign not math";
                }
                else
                    jsonReturn["state"] = 0;
                //We just check the sign and return
                return jsonReturn.ToJson();//If success return {"state":0}, otherwise return {"state":1,"msg":"sign not math"}
            });
            BindHttpMsg("/TestDelayCallback", (jsonData, ip, delayCallback) =>
            {
                //We request HTTP for example
                new ThreadPoolHttp("http://www.google.com",
                    (msg) =>//The HTTP request running in background thread, and this callback run in main thread after the HTTP request done.
                    {
                        JSONData jsonReturn = JSONData.NewDictionary();
                        jsonReturn["state"] = 0;
                        jsonReturn["msg"] = msg;
                        delayCallback(jsonReturn);//Here are the real return string to the client after we request HTTP done.
                    });
                return "";//return empty string mean you will callback later.
            });

            //Bind the console command into different class to handle it.
            //It's recommend using this way console command.
            //That make your code look more simple and tidy than handle all console command in
            //'protected virtual void OnCommand(string cmd, string[] args)'.
            //That run in main thread.
            //Input 'TestCommand aa "bb ""Cc" 1 1.5' in console, will received TestCommand.OnCommand({"aa","bb \"Cc","1","1.5"}) in main thread
            BindCommand("TestCommand", TestCommand.OnCommand);//The command is case-insensitive, but args is case-sensitive
            BindCommand("ReloadCsv", (args) => { InitializeCSV(); });//Register command 'reloadcsv' for reload CSV file in console
            BindCommand("quit", (args) => { Running = false; });//Register command 'quit' in console
            BindCommand("exit", (args) => { Running = false; });//Register command 'exit' in console
            BindCommand("Reload", (args) =>
            {
                // Register command 'reload' for reload JSON config file in console
                // Reload the config value from your config JSON file.
                // You can consider as you had reload the value in Config, but exclude the value in ConfigBase (The value in ConfigBase changed but not take effect.).
                FrameworkBase.config = CreateConfig(Environment.CurrentDirectory
                    + "/" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".json");

                //Add your code here for make your config value take effect.
                ReqGateway.Clear();//Make sure the gateway server infomation reload from the config JSON.
            });
            BindCommand("TestThread", (args)=>
            {
                //We test read a file in thread, and then process the result.
                string strFile = "./KissFramework.dll";
                string result = "";
                new ThreadPoolEvent(() =>//This function run in thread. You can do some heavy work in thread here, such as IO operation.
                {
                    Logger.LogInfo($"start read file {strFile}", false);//Print log in thread you must set the param 'useInMainThread' as false.
                    byte[] buff = System.IO.File.ReadAllBytes(strFile);
                    System.Threading.Thread.Sleep(1000);//We simulate that work take long time.
                    result = $"{strFile} file length = {buff.Length}";
                },
                () =>//This function run in main thread. You can do some work after your work done.
                        {
                    Logger.LogInfo(result);
                });
            });
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
