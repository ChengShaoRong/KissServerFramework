# 简介
最简洁易用的IOCP服务器框架,遵循KISS原则,它是我上传到Unity资源商店里的[C#Like](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) 和 [C#Like免费版](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) 的示范服务器.   
**跟其他众多的服务器框架比,有什么特别过人之处?**
> * **支持WebSocket/Socket同时连接, 服务器无需理会客户端以何种方式连接**
> * **极简HTTP**
> * **用户逻辑全单线程, 无需关注多线程(后台多线程处理,例如数据库,网络,日志)**  
> * **面向对象设计, 客户端和服务器端传输的是JSONData对象或自定义类对象**  
> * **无需用到SQL知识, 仅需定义数据库表结构,即可使用自动获取的数据库数据. 修改数据后, 后台会全自动异步更新至数据库和客户端**
***
本文档的英文版: [English](https://github.com/ChengShaoRong/KissServerFramework/). 

# 安装
***
直接使用复制本项目修改. 使用Git或SVN下面链接:   
```
https://github.com/ChengShaoRong/KissServerFramework.git
```

# 用法
* **支持WebSocket/Socket同时连接,服务器无需理会客户端以何种方式连接**
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

* **极简HTTP**
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


* **无需用到SQL知识, 仅需定义数据库表结构,即可使用自动获取的数据库数据. 修改数据后, 后台会全自动异步更新至数据库和客户端**
以示范项目为例:  
>* 客户端(Player类),通过账号登录管理器(AccountManager类)获取账号(Account类)信息,成功登录后,全自动加载所包含的子系统且全自动同步到客户端. 这里的示范子系统分别是邮件系统(Mail类)和物品系统(Item类)和签到系统(SignIn类).  
>* 使用KissEditor编辑里面用到的类.  
![Image text](https://raw.githubusercontent.com/ChengShaoRong/KissServerFramework/main/image/editor.png)
>* 每个编辑器生成的类将生成2个服务器用的XXXX类(用于自定义修改)和XXXX_Base类(禁止手动修改), 2个C#Like完整版用的XXXX类(用于自定义修改)和XXXX_Base类(禁止手动修改), 1个C#Like免费版用的XXXX类(用于自定义修改,为何没有XXXX_Base类?因为C#Like免费版热更新脚本不支持继承.).  
>* 代码里修改自动生成的类属性时候, 将会自动触发**后台线程**的保存数据库和同步客户端的操作.   
![Image text](https://raw.githubusercontent.com/ChengShaoRong/KissServerFramework/main/image/classDiagram.png)