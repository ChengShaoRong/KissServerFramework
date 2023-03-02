# KissServerFramework
This is a most simple and stupid server framework commponent include WebSocket/Socket/HTTP/MySQL, base on rule of 'Keep It Simple,Stupid'. All your logic work in A single main thread, you don't need to worry about multi-threading problem. All the heavy work process by framework in background threads. Easy to use database even never hear about SQL. You won't use the SQL knowledge, just need define the struct of database table, and then can use that data and it will automatically synchronize data with client and database.
这是一个最简洁易用的服务器框架,包含WebSocket/Socket/HTTP,基于'Keep It Simple,Stupid'设计原则.最简洁易用的IOCP服务器框架,用户逻辑单线程,后台数据库多线程,面向对象,操作极简,包含WebSocket/Socket/HTTP/MySQL,你不会用到SQL的,只需定义数据库表结构,即可使用数据且自动和客户端和数据库三者间同步数据

# [C#Like](https://assetstore.unity.com/packages/tools/integration/c-like-hot-update-framework-222256) and [C#LikeFree](https://assetstore.unity.com/packages/tools/integration/c-likefree-hot-update-framework-222880) 
They are the link to Unity asset store that the corresponding client of this server code.
上面两个链接是对应的C#客户端

# KissGennerateRIDL project
This is a tool for convert *.ridl files into *.cs files, both client code and server code.
Usage: Automatic be called when compile project KissServerFramework, 
这是一个把当前目录下所有*.ridl文件转化成*.cs文件的工具, 会同时生成客户端代码和服务器代码.
用法:当编译KissServerFramework的时候会自动调用,把当前项目下所有的*.ridl编译成服务器和客户端的NetObject代码,请配置KissGennerateRIDL.ini确保可以直接复制到指定的客户端位置

# KissServerFramework project
This's the sample of KissFramework. Server corresponding to C#Like and C#LikeFree.
这是KissFramework的使用例子. 对应C#Like和C#Like免费版的服务器
