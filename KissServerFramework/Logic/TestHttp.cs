using CSharpLike;
using KissFramework;
using System;

namespace KissServerFramework
{
    public static class TestHttp
    {
        /// <summary>
        /// Process HTTP message, that run in main thread.
        /// </summary>
        /// <param name="jsonData">The request JSON data from client, support GET/POST data</param>
        /// <param name="url">The client ip</param>
        /// <param name="delayCallback">If you want to do something in thread or delay callback,
        /// such as get some data from database, you can return "" in this function 
        /// and call this action while your work done. 
        /// And the timeout is 'config.processTimeoutHTTP = 10;' seconds.
        /// You can ignore this if you return client immediately.</param>
        /// <returns>That string will callback to client immediately. If return empty string, you should call delayCallback later.</returns>
        public static string OnHttpMessage(JSONData jsonData, string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"TestHttp : jsonData = {jsonData} from {ip}");

            JSONData jsonReturn = JSONData.NewDictionary();
            jsonReturn["code"] = 1;
            jsonReturn["msg"] = "Hello world";
            return jsonReturn.ToString();
        }
    }
}
