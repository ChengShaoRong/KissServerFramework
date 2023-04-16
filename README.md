# Introduction
This is a most simple and stupid IOCP server framework, base on KISS rule. It demo server  of  [C#Like](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) and [C#LikeFree](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) that I uploaded to the Unity Asset Store.    
**Compared with other server framework, what's special?**
>* **Support both WebSocket and Socket, the server don't care how the client connects**.  
>* **Minimalism HTTP**.   
>* **The user logic is all single-threaded, without paying attention to multi-threading (background multi-threaded processing, such as database, network, log)**.  
>* **Object-oriented design. The client and server transmit JSONData objects or user-defined class objects**.  
>* **No need to use SQL knowledge, just define the database table structure, then you can use the automatically acquired database data. After modified the data,  will update it to the database and client automatically and asynchronously in the background**. 
***
Read this in other languages: [中文](https://github.com/ChengShaoRong/KissServerFramework/README_Chinese.md). 

# Install
Use directly to copy this solution for modification. Use Git or checkout with SVN using the web URL:  
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

***

* **No need to use SQL knowledge, just define the database table structure, then you can use the automatically acquired database data. After modified the data,  will update it to the database and client automatically and asynchronously in the background**
e. g. Demo server:  
>* Client(class Player) get account infomation(class Account) from the account manager(class AccountManager). After login success, will automatically load all subsystem and sync to client.  The demo project include 3 subsystem : mail system(class Mail), item system(class Item) and sign in system(class SignIn).  
>* You need define 4 RIDL file, include 'account.ridl', 'item.ridl', 'mail.ridl', 'signIn.ridl'.  
>* By KissGennerateRIDL project, each 'XXXX.ridl' file will automatically generate 2 classes(class XXXX can be customized or modified, but XXXX_Base can't be modified and will be covered while 'XXXX.ridl' file was modified) for the server and 2 classes(class XXXX can be customized or modified, but XXXX_Base can't be modified and will be covered while 'XXXX.ridl' file was modified) for C#Like and 1 class(Why not has class XXXX_Base? Because not support inherit class in free version.) for C#LikeFree.  
>* Modify the attribute in class XXXX_Base will active save into database and sync to client action in **background thread**. Spme attributes may be no need to be update, you can modify RIDL file exclude it,  e. g. itemId and acctId in class Item will never change in logic.  
![Image text](https://github.com/ChengShaoRong/KissServerFramework/image/classDiagram.png)



