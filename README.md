# Introduction
This is a most simple and stupid IOCP server framework, base on KISS rule. It demo server  of  [C#Like](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) and [C#LikeFree](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) that I uploaded to the Unity Asset Store. You can get the [C#LikeFree](https://github.com/ChengShaoRong/CSharpLikeFree) from GitHub too.    
**Compared with other server framework, what's special?**
>* **Support both WebSocket and Socket, the server don't care how the client connects**.  
>* **Minimalism HTTP**.   
>* **The user logic is all single-threaded, without paying attention to multi-threading (background multi-threaded processing, such as database, network, log)**.  
>* **Object-oriented design. The client and server transmit JSONData objects or user-defined class objects**.  
>* **No need to use SQL knowledge, just define the database table structure, then you can use the automatically acquired database data. After modified the data,  will update it to the database and client automatically and asynchronously in the background**. 
***
最简洁易用的IOCP服务器框架,遵循KISS原则,它是我上传到Unity资源商店里的[C#Like](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) 和 [C#Like免费版](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) 的示范服务器, 你也可以在GitHub里下载到[C#Like免费版](https://github.com/ChengShaoRong/CSharpLikeFree).   
**跟其他众多的服务器框架比,有什么特别过人之处?**
> * **支持WebSocket/Socket同时连接, 服务器无需理会客户端以何种方式连接**
> * **极简HTTP**
> * **用户逻辑全单线程, 无需关注多线程(后台多线程处理,例如数据库,网络,日志)**  
> * **面向对象设计, 客户端和服务器端传输的是JSONData对象或自定义类对象**  
> * **无需用到SQL知识, 仅需定义数据库表结构,即可使用自动获取的数据库数据. 修改数据后, 后台会全自动异步更新至数据库和客户端**

# Install
Use directly to copy this solution for modification. Use Git or checkout with SVN using the web URL:  
```
https://github.com/ChengShaoRong/KissServerFramework.git
```
***
直接使用复制本项目修改. 使用Git或SVN下面链接:   
```
https://github.com/ChengShaoRong/KissServerFramework.git
```

# Usage



* **Support both WebSocket and Socket, the server don't care how the client connects**
```
    /// <summary>
    /// The class for transfer JSON object between server and client, whatever the client using WebSocket or Socket.
    /// 1 Received JSON object from client by 'void OnMessage(JSONData jsonData)'. 
    /// 2 Send JSON object to client by 'void Send(JSONData jsonData)'.
    /// 3 Accept player info by account, that include all data from database.
    /// </summary>
    public sealed class Player : PlayerBase
    {
        /// <summary>
        /// Loaded account instance, that include all data from database.
        /// </summary>
        public Account account;
        /// <summary>
        /// Received JSON object from client, that run in main thread.
        /// </summary>
        /// <param name="jsonData">The JSON object from client</param>
        public override void OnMessage(JSONData jsonData)
        {
        }
        /// <summary>
        /// When the player disconnect, that run in main thread.
        /// </summary>
        public override void OnDisconnect()
        {
            Logger.LogInfo("Player:OnDisconnect");
        }
        /// <summary>
        /// When player connected, that run in main thread
        /// </summary>
        public override void OnConnect()
        {
            Logger.LogInfo("Player:OnConnect");
        }
        /// <summary>
        /// The WebSocket/Socket occur error, that run in main thread
        /// </summary>
        public override void OnError(string msg)
        {
            Logger.LogInfo("Player:OnError:"+ msg);
        }
    }
```
* **(chinese) 支持WebSocket/Socket同时连接,服务器无需理会客户端以何种方式连接**
```
    /// <summary>
    /// 这个类是用于客户端和服务器之间传输JSON对象, 无论客户端使用WebSocket还是Socket.
    /// 1 通过'void OnMessage(JSONData jsonData)'接收客户端发来的JSON对象. 
    /// 2 通过'void Send(JSONData jsonData)'发送JSON对象.
    /// 3 玩家对应的主对象为account, 它包含所有与玩家相关的数据库对象.
    /// </summary>
    public sealed class Player : PlayerBase
    {
        /// <summary>
        /// 已加载的玩家数据,它包含所有与玩家相关的数据库对象
        /// </summary>
        public Account account;
        /// <summary>
        /// 接收客户端发来的JSON对象, 本函数在主线程中运行.
        /// </summary>
        /// <param name="jsonData">客户端发来的JSON对象</param>
        public override void OnMessage(JSONData jsonData)
        {
        }
        /// <summary>
        /// 玩家断线事件, 本函数在主线程中运行.
        /// </summary>
        public override void OnDisconnect()
        {
            Logger.LogInfo("Player:OnDisconnect");
        }
        /// <summary>
        /// 玩家连接事件, 本函数在主线程中运行.
        /// </summary>
        public override void OnConnect()
        {
            Logger.LogInfo("Player:OnConnect");
        }
        /// <summary>
        /// 玩家连接发生错误事件, 本函数在主线程中运行.
        /// </summary>
        public override void OnError(string msg)
        {
            Logger.LogInfo("Player:OnError:"+ msg);
        }
    }
```
***

* **Minimalism HTTP**
```
	//Sample for return immediately (synchronization)
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
	//Sample for return delay (asynchronous)
	BindHttpMsg("/TestDelayCallback", (jsonData, ip, delayCallback) =>
	{
	    //We request a new async HTTP for example
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
```
* **(chinese) 极简HTTP**
```
	//立即返回的示范(同步)
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
	//延迟返回的示范(异步)
	BindHttpMsg("/TestDelayCallback", (jsonData, ip, delayCallback) =>
	{
	    //我们收到请求后,请求一个异步HTTP作为例子
	    new ThreadPoolHttp("http://www.google.com",
	        (msg) =>//该HTTP请求在后台线程里异步运行, 它完成后将加入到主线程里的本回调里运行.
	        {
	            JSONData jsonReturn = JSONData.NewDictionary();
	            jsonReturn["state"] = 0;
	            jsonReturn["msg"] = msg;
	            delayCallback(jsonReturn);//这里真正最终返回客户端请求.
	        });
	    return "";//这里固定返回空白字符串表示我们将延迟异步返回
	});
```
