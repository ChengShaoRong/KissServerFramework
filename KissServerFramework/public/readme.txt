1 搭建KissServerFramework服务器
	1.1 下载服务器源码,使用Visual Studio(我自己使用VS2019在.NET5)编译发布最终exe(如果嫌弃太麻烦,可以直接使用我编译的KissServerFramework.exe),例如放C盘,目录结构为:
C:\KissServerFramework
	|
	|--CSV
	|  |--Item.csv
	|
	|--KissServerFramework.exe
	|
	|--KissServerFramework.json
	|
	|--kiss.sql

	1.2 如果目标机器没有安装.NET5运行时库的需要去微软官网下载且安装:https://download.visualstudio.microsoft.com/download/pr/a0832b5a-6900-442b-af79-6ffddddd6ba4/e2df0b25dd851ee0b38a86947dd0e42e/dotnet-runtime-5.0.17-win-x64.exe
	1.3 如有需要则修改KissServerFramework.json里的WebSocket端口9000,Socket端口9001,HTTP端口9002,请确保这些端口没有已经被占用
	1.4 如果存在防火墙,则需要对外开放TCP端口9000~9002

2 搭建XAMPP
	2.1 XAMPP的官网下载 https://www.apachefriends.org/download.html
	2.2 一直"下一步"地安装XAMPP直至安装完毕(默认安装到C:/xampp)
	2.3 确认MySQL和Apache开着的, 你可以在XAMPP控制面板查看.
	2.4 如果存在防火墙,则需要对外开放TCP端口80

3 创建"kiss"数据库且导入"kiss.sql"
	3.1 命令行执行以下命令创建数据库(默认没密码,要求输入密码直接按回车键)
		C:\xampp\mysql\bin\mysqladmin -u root -p create kiss
	3.1 命令行执行以下命令导入"kiss.sql"(默认没密码,要求输入密码直接按回车键)
		C:\xampp\mysql\bin\mysql kiss < C:/KissServerFramework/kiss.sql -u root -p

4 可选步骤:升级HTTP到HTTPS,升级WS到WSS
	4.1 准备SSL证书,可以到腾讯云(https://console.cloud.tencent.com/ssl)或"Let's Encrypt"(https://letsencrypt.org/)申请免费的SSL证书或购买.我自己是腾讯云申请的免费SSL证书,下载Apache版证书和Nginx版证书备用.
	4.2 安装Nginx代理
		4.2.1 Nginx官网下载 http://nginx.org/en/download.html 一般我们下载里面的稳定版本,我们这里选nginx.Windows-1.22.1
		4.2.2 下载的回来的nginx-1.22.1.zip解压在C盘中,且复制4个bat文件和替换C:/nginx-1.22.1/conf/nginx.conf,把前面申请的Nginx版的证书解压到C:\nginx-1.22.1\conf\xxxx.com\目录内
文件结构如下
C:\nginx-1.22.1
	|
	|--conf
	|  |--xxxx.com
	|  |   |--xxxx.com_bundle.crt
	|  |   |--xxxx.com.key
	|  |
	|  |--nginx.conf
	|
	|--contrib
	|--docs
	|--html
	|--logs
	|--temp
	|--nginx.exe
	|
	|--QuitNginx.bat			//双击这个是退出Nginx
	|
	|--RetartNginx.bat			//双击这个是重启Nginx,例如修改配置后用
	|
	|--StartNginx.bat			//双击这个是启动Nginx,例如首次打开时候用
	|
	|--StopNginx.bat			//双击这个是停止Nginx
		4.2.3 设置C:/nginx-1.22.1/conf/nginx.conf文件,把里面的xxxx.com替换成你实际域名,例如我自己的是csharplike.com替换xxxx.com
		4.2.4 如有需要修改nginx.conf里面的端口,里面设置WSS端口10000代理到WS端口9000,HTTPS端口10002代理到HTTP端口9002,如果有防火墙,请开启对于TCP端口(10000和10002)
		4.2.5 因为采用了WSS和HTTPS,请修改C:\KissServerFramework\KissServerFramework.json的网关端口:
			"serverInfos"节点内的"WebSocketURI": "wss://www.xxxx.com:10000"
			"serverInfos"节点内的"HttpURI": "https://www.xxxx.com:10002"
		4.2.6 最后记得执行StartNginx.bat启动Nginx,如果前面已经启动过可以执行RetartNginx.bat

	4.3 配置Apache的SSL证书
		4.2.1 把下载的Apache版的证书放到C:\xampp\apache\conf\xxxx.com\目录内
		4.2.2 如下修改C:\xampp\apache\conf\extra\httpd-ssl.conf
			SSLCertificateFile "C:/xampp/apache/conf/xxxx.com/xxxx.com.crt"
			SSLCertificateKeyFile "C:/xampp/apache/conf/xxxx.com/xxxx.com.key"
			SSLCertificateChainFile "C:/xampp/apache/conf/xxxx.com/root_bundle.crt"
			把上面的xxxx.com替换成你实际域名,例如我自己的是csharplike.com替换xxxx.com
		4.2.3 如果存在防火墙,则需要对外开放TCP端口443

	4.4 配置完毕记得在XAMPP控制面板里点Stop按钮,然后点Start按钮,来重启Apache令配置生效

5 双击运行C:/KissServerFramework/KissServerFramework.exe
	5.1 如果exe闪掉没开起了,可能是因为没有对应.NET运行时库
	5.2 如果exe的console显示log仅为"KissFramework version : 1.0.x.x"表示端口已经被占用,请核实一下端口.

6 把C#Like和C#LikeFree导出的WebGL放入到C:\xampp\htdocs目录内
	6.1 C#Like导出的CSharpLikeDemo放入C:\xampp\htdocs目录内
	6.2 C#LikeFree导出的CSharpLikeFreeDemo放入C:\xampp\htdocs目录内
