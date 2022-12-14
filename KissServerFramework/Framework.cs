
using CSharpLike;
using KissFramework;
using System;

namespace KissServerFramework
{
    /// <summary>
    /// All our logic running in main thread, 
    /// except the database SELECT/INSERT/UPDATE/DELETE operation, 
    /// We write sample for operate database, it's easy way to use database in Multithreading.
    /// You don't need to use 'lock' syntax!
    /// </summary>
    public class Framework : FrameworkBase
    {
        /// <summary>
        /// When the KISS framework initialized will call this function, you can call your initialize here.
        /// </summary>
        protected override void OnStart()
        {
            Logger.LogInfo("Framework OnStart");
            AircraftBattleManager.Instance.Initialize();
            RaiseEvent(ChatRoomManager.Instance.Update, 60f);//we call 'ChatRoomManager.Instance.Update' in every 60 seconds.
        }
        /// <summary>
        /// The main game loop function, that run in main thread.
        /// You should call all other Update(float deltaTime) from here.
        /// We recommend using:
        /// 'public static string RaiseEvent(Action action, float intervalTime = 0, int repeatCount = int.MaxValue)'
        /// to do the loop timer.
        /// </summary>
        /// <param name="deltaTime">delta time since last update, in seconds</param>
        protected override void OnUpdate(float deltaTime)
        {
            //Add your custom function below if you need Update.
        }

        /// <summary>
        /// Process the command input from the console, that run in main thread.
        /// You can do some command to control your game logic, e.g. some cheat command.
        /// </summary>
        /// <param name="cmd">The command input from the console, was convert to lowercase letters.</param>
        protected override void OnCommand(string cmd)
        {
            switch (cmd)
            {
                //When input 'exit' or 'quit' will exit the game.
                case "exit":
                case "quit":
                    Running = false;
                    break;
                //Add your custom command below.
                case "test":
                    {
                        //The real request run in background thread.
                        new ThreadPoolHttp("http://sn.csharplike.com:11114/UpdateAnonymous", "ver=123&startTime=1970-01-01",
                            (cb) =>//The callback function run in main thread. You can do some work after your HTTP request done.
                            {
                                Logger.LogInfo(cb);
                            });
                    }
                    break;
                case "test2":
                    {
                        //The real request run in background thread.
                        new ThreadPoolHttp("http://sn.csharplike.com:11114/UpdateAnonymous", "{\"ver\":123,\"startTime\":\"1970-01-01\"}",
                            (cb) =>//The callback function run in main thread. You can do some work after your HTTP request done.
                            {
                                Logger.LogInfo(cb);
                            });
                    }
                    break;
                case "testthread":
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
                    }
                    break;

                //Default just print an error message.
                default:
                    Logger.LogError("Unrecognized command : " + cmd);
                    break;
            }
        }

        /// <summary>
        /// Process HTTP message, that run in main thread.
        /// </summary>
        /// <param name="url">The url string, that case sensitive.</param>
        /// <param name="jsonData">The request JSON data from client, support GET/POST data</param>
        /// <param name="url">The client ip</param>
        /// <param name="delayCallback">If you want to do something in thread or delay callback,
        /// such as get some data from database, you can return "" in this function 
        /// and call this action while your work done. 
        /// And the timeout is 'config.processTimeoutHTTP = 10;' seconds.
        /// You can ignore this if you return client immediately.</param>
        /// <returns>That string will callback to client immediately. If return empty string, you should call delayCallback later.</returns>
        protected override string OnHttpMessage(string url, JSONData jsonData, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"OnHttpMessage : url = {url}  jsonData = {jsonData} from {ip}");
            switch (url)
            {
                //e.g. GET : url = 'http://ip[:port]/TestThirdPartyAccount?uid=123456789&token=xxxxxx&sign=yyyyyy'
                //e.g. POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = 'uid=123456789&token=xxxxxx&sign=yyyyyy'
                //e.g. POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = '{"uid":"123456789","token":"xxxxxx","sign":"yyyyyy"}'
                //We are both got jsonData = {"uid":"123456789","token"="xxxxxx","sign":"yyyyyy"}
                //If post the JSON string, we will get the JSON you send.
                case "/TestThirdPartyAccount":
                    {
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
                        return jsonReturn.ToString();//If success return {"state":0}, otherwise return {"state":1,"msg":"sign not math"}
                    }
                case "/TestDelayCallback"://Sample for callback delay
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
                    }
                default:
                    return @"{""error"":""unknown url""}";
            }
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
