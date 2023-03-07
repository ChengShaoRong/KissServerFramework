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
	    //例如客户端GET : url = 'http://ip[:port]/TestThirdPartyAccount?uid=123456789&token=xxxxxx&sign=yyyyyy'
	    //例如客户端POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = 'uid=123456789&token=xxxxxx&sign=yyyyyy'
	    //例如客户端POST : url = 'http://ip[:port]/TestThirdPartyAccount'  post = '{"uid":"123456789","token":"xxxxxx","sign":"yyyyyy"}'
	    //我们都会获取到JSON对象 = {"uid":"123456789","token":"xxxxxx","sign":"yyyyyy"}
	    JSONData jsonReturn = JSONData.NewDictionary();
	    //int uid=jsonData["uid"];//JSON {"uid":"123456789"} and {"uid":123456789}都可以自动变成数字123456789
	    //sign = md5(uid+token+key)
	    string sign = GetMD5((string)jsonData["uid"] + jsonData["token"] + "zzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
	    if (sign != jsonData["sign"])
	    {
	        jsonReturn["state"] = 1;
	        jsonReturn["msg"] = "验签失败";
	    }
	    else
	        jsonReturn["state"] = 0;
	    //We just check the sign and return
	    return jsonReturn.ToJson();//如果成功则返回{"state":0}, 失败则返回 {"state":1,"msg":"验签失败"}
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

***

* **No need to use SQL knowledge, just define the database table structure, then you can use the automatically acquired database data. After modified the data,  will update it to the database and client automatically and asynchronously in the background**
e. g. Demo server:  
>* Client(class Player) get account infomation(class Account) from the account manager(class AccountManager). After login success, will automatically load all subsystem and sync to client.  The demo project include 3 subsystem : mail system(class Mail), item system(class Item) and sign in system(class SignIn).  
>* You need define 4 RIDL file, include 'account.ridl', 'item.ridl', 'mail.ridl', 'signIn.ridl'.  
>* By KissGennerateRIDL project, each 'XXXX.ridl' file will automatically generate 2 classes(class XXXX can be customized or modified, but XXXX_Base can't be modified and will be covered while 'XXXX.ridl' file was modified) for the server and 2 classes(class XXXX can be customized or modified, but XXXX_Base can't be modified and will be covered while 'XXXX.ridl' file was modified) for C#Like and 1 class(Why not has class XXXX_Base? Because not support inherit class in free version.) for C#LikeFree.  
>* Modify the attribute in class XXXX_Base will active save into database and sync to client action in **background thread**. Spme attributes may be no need to be update, you can modify RIDL file exclude it,  e. g. itemId and acctId in class Item will never change in logic.  
```mermaid
classDiagram
class AccountManager{
	+Dictionary~int, Account~ accounts
	+Dictionary~Account, Player~ playersByAccount
	+Dictionary~string, Account~ accountsByName
	+Login(JSONData jsonData, Player player)
	-LoginStep2(JSONData jsonData, Player player)
	-LoginStep3()
	+GetAccount(ref Account account)
	+GetAccount(int uid)
	+GetAccount(string name, int acctType)
	+GetAccountByNickname(string nickname)
	+ClearAccountCache(Account account)
	+GetPlayer(Account account)
	+GetPlayer(int uid)
	+ChangeNameAndPassword()
	+BroadcastToAllPlayer(JSONData msg)
}
class PlayerBase{
	+string SessionID
	+string IP
	+bool printSendAndReceived$
	+Send(JSONData jsonData)
	+Send(string msg)
	+Disconnect()
}
class Player{
	+Account account
	+OnMessage(JSONData jsonData)
	+OnDisconnect(JSONData jsonData)
	+OnConnect(JSONData jsonData)
	+OnError(JSONData jsonData)
}
class Account{
	
}
class Account_Base{
	+int uid
	+int acctType
	+DateTime createTime
	+string name
	+string password
	+string nickname
	+int money
	+int score
	+DateTime scoreTime
	+DateTime lastLoginTime
	+Dictionary~int,Item~ items
	+Dictionary~int,Mail~ mails
	+SignIn signIn
	+LoadAllSubSystem(PlayerBase player)
	+SelectByNameAndAcctType(string name, int acctType, Action<List<Account>, string> _callback_)
}
class Item{
	
}
class Item_Base{
	+int uid
	+int itemId
	+int acctId
	+int count
}
class Mail{
	
}
class Mail_Base{
	+int uid
	+int acctId
	+int senderId
	+int acctType
	+string senderName
	+string title
	+string content
	+string appendix
	+DateTime createTime
	+byte wasRead
	+byte received
}
class SignIn{
	
}
class SignIn_Base{
	+int acctId
	+int month
	+string signInList
	+string vipSignInList
}
Player --|> PlayerBase
Player *-- Account
Account_Base *-- "0..n" Item
Account_Base *-- "0..n" Mail
Account_Base *-- "1" SignIn
Account --|> Account_Base
Mail --|> Mail_Base
Item --|> Item_Base
SignIn --|> SignIn_Base
```

* **(chinese) 无需用到SQL知识, 仅需定义数据库表结构,即可使用自动获取的数据库数据. 修改数据后, 后台会全自动异步更新至数据库和客户端**
以示范项目为例:  
>* 客户端(Player类),通过账号登录管理器(AccountManager类)获取账号(Account类)信息,成功登录后,全自动加载所包含的子系统且全自动同步到客户端. 这里的示范子系统分别是邮件系统(Mail类)和物品系统(Item类)和签到系统(SignIn类).  
>* 你需要定义account.ridl, item.ridl, mail.ridl, signIn.ridl这4个文件.  
>* 每个'XXXX.ridl'文件将生成2个服务器用的XXXX类(用于自定义修改)和XXXX_Base类(禁止修改且修改ridl会自动覆盖), 2个C#Like完整版用的XXXX类(用于自定义修改)和XXXX_Base类(禁止修改且修改ridl会自动覆盖), 1个C#Like免费版用的XXXX类(用于自定义修改,为何没有XXXX_Base类?因为C#Like免费版热更新脚本不支持继承.).  
>* 修改自动生成的类属性时候, 将会自动触发**后台线程**的保存数据库和同步客户端的操作. 某些无需更新的属性可以通过配置ridl文件排除, 例如Item里的itemId和acctId是绝对不会改变的.  
```mermaid
classDiagram
class AccountManager{
	+Dictionary~int, Account~ accounts
	+Dictionary~Account, Player~ playersByAccount
	+Dictionary~string, Account~ accountsByName
	+Login(JSONData jsonData, Player player)
	-LoginStep2(JSONData jsonData, Player player)
	-LoginStep3()
	+GetAccount(ref Account account)
	+GetAccount(int uid)
	+GetAccount(string name, int acctType)
	+GetAccountByNickname(string nickname)
	+ClearAccountCache(Account account)
	+GetPlayer(Account account)
	+GetPlayer(int uid)
	+ChangeNameAndPassword()
	+BroadcastToAllPlayer(JSONData msg)
}
class PlayerBase{
	+string SessionID
	+string IP
	+bool printSendAndReceived$
	+Send(JSONData jsonData)
	+Send(string msg)
	+Disconnect()
}
class Player{
	+Account account
	+OnMessage(JSONData jsonData)
	+OnDisconnect(JSONData jsonData)
	+OnConnect(JSONData jsonData)
	+OnError(JSONData jsonData)
}
class Account{
	
}
class Account_Base{
	+int uid
	+int acctType
	+DateTime createTime
	+string name
	+string password
	+string nickname
	+int money
	+int score
	+DateTime scoreTime
	+DateTime lastLoginTime
	+Dictionary~int,Item~ items
	+Dictionary~int,Mail~ mails
	+SignIn signIn
	+LoadAllSubSystem(PlayerBase player)
	+SelectByNameAndAcctType(string name, int acctType, Action<List<Account>, string> _callback_)
}
class Item{
	
}
class Item_Base{
	+int uid
	+int itemId
	+int acctId
	+int count
}
class Mail{
	
}
class Mail_Base{
	+int uid
	+int acctId
	+int senderId
	+int acctType
	+string senderName
	+string title
	+string content
	+string appendix
	+DateTime createTime
	+byte wasRead
	+byte received
}
class SignIn{
	
}
class SignIn_Base{
	+int acctId
	+int month
	+string signInList
	+string vipSignInList
}
Player --|> PlayerBase
Player *-- Account
Account_Base *-- "0..n" Item
Account_Base *-- "0..n" Mail
Account_Base *-- "1" SignIn
Account --|> Account_Base
Mail --|> Mail_Base
Item --|> Item_Base
SignIn --|> SignIn_Base
```
