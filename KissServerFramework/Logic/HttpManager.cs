using CSharpLike;
using KissFramework;
using System;

namespace KissServerFramework
{
    /// <summary>
    /// We process HTTP request here.
    /// You can add web mehtod ANYWHERE as you like.
    /// Is very easy to bind HTTP(s) request:
    /// 1: Define method as static.
    /// 2: Define method return type as 'string'.
    /// 3: Add [WebMethod] into your method.
    /// 4: Custom command name like '[WebMethod(UriName = "ReqGateway")]', if you don't set it, it will using current method name as URI, and is case-insensitive.
    /// 5: You can has 0 parameter or some parameters. Support param type 'byte/sbyte/short/ushort/int/uint/DateTime/string/float/double/bool'.
    /// 6: The parameter "string ip" is build-in parameter, you can get client ip from it, and you can choose not add this parameter if you don't care it.
    /// 7: You MUST add build-in parameter "Action&lt;string&gt; delayCallback" in your method if you not return JSON string immediately.
    /// </summary>
    public static class HttpManager
    {
        /// <summary>
        /// Sample for callback JSON sting immediately
        /// </summary>
        /// <param name="uid">uid request from client. if the uid is integer number, you can set this parameter as 'int uid'</param>
        /// <param name="token">token request from client</param>
        /// <param name="sign">sign request from client</param>
        [WebMethod]
        public static string TestThirdPartyAccount(string uid, string token, string sign)
        {
            //e.g. GET : url = 'http://ip[:port]/TestThirdPartyAccount?uid=123456789&token=xxxxxx&sign=yyyyyy'
            //e.g. POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = 'uid=123456789&token=xxxxxx&sign=yyyyyy'
            //e.g. POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = '{"uid":"123456789","token":"xxxxxx","sign":"yyyyyy"}'
            JSONData jsonReturn = JSONData.NewDictionary();
            //sign = md5(uid+token+key)
            string signCalc = Framework.GetMD5(uid + token + "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            if (signCalc != sign)
            {
                jsonReturn["state"] = 1;
                jsonReturn["msg"] = "sign not math";
            }
            else
                jsonReturn["state"] = 0;
            //We just check the sign and return
            return jsonReturn.ToJson();//If success return {"state":0}, otherwise return {"state":1,"msg":"sign not math"}
        }
        /// <summary>
        /// Sample for callback JSON sting NOT immediately, this method return "", and call action 'delayCallback' after your job done.
        /// e.g. GET : url = 'http://ip[:port]/TestDelayCallback
        /// </summary>
        [WebMethod]
        public static string TestDelayCallback(string ip, Action<string> delayCallback)
        {
            Logger.LogInfo($"TestDelayCallback:client ip = {ip}");
            //We send a HTTP request asynchronously for example
            new ThreadPoolHttp("http://www.google.com",
                (msg) =>//The HTTP request running in background thread, and this callback run in main thread after the HTTP request done.
                {
                    JSONData jsonReturn = JSONData.NewDictionary();
                    jsonReturn["state"] = 0;
                    jsonReturn["msg"] = msg;
                    delayCallback(jsonReturn);//HERE are the real return string to the client after we request HTTP done.
                });
            return "";//return empty string mean you will callback later.
        }
    }
}
