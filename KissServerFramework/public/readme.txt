搭建C#Like(Free)示例的详细流程:

1 搭建KissServerFramework服务器
	1.1 下载服务器源码,使用Visual Studio(我自己使用VS2019在.NET5)编译发布最终单个exe(如果嫌弃太麻烦,可以直接使用我编译的KissServerFramework.exe),例如放C盘,目录结构为:
		C:\KissServerFramework
			|
			|--CSV
			|  |--Item.csv						//测试用的CSV文件
			|
			|--KissServerFramework.exe			//主程序,你选择自行编译或直接使用我编译好的
			|--KissServerFramework.json			//配置JSON文件
			|--kiss.sql							//这个是对应数据库SQL文件,仅在步骤3里使用

	1.2 如果目标机器没有安装.NET5运行时库的需要去微软官网下载且安装:https://download.visualstudio.microsoft.com/download/pr/a0832b5a-6900-442b-af79-6ffddddd6ba4/e2df0b25dd851ee0b38a86947dd0e42e/dotnet-runtime-5.0.17-win-x64.exe
	1.3 如有需要则修改KissServerFramework.json里的WebSocket端口9000,Socket端口9001,HTTP端口9002,请确保这些端口没有已经被占用
	1.4 如果存在防火墙,则需要对外开放TCP端口9000~9002

2 搭建XAMPP
	2.1 XAMPP的官网下载 https://www.apachefriends.org/download.html
	2.2 一直"下一步"地安装XAMPP直至安装完毕(默认安装到C:/xampp)
	2.3 确认MySQL和Apache开着的, 你可以在XAMPP控制面板查看.
	2.4 如果存在防火墙,则需要对外开放TCP端口80

3 创建"kiss"数据库且导入"kiss.sql"
	3.1 执行批处理文件files/CreateDatabase.bat(默认没密码,要求输入密码直接按回车键), 如果XAMPP安装路径有变,请修改bat文件
	3.2 执行批处理文件files/ImportDatabase.bat(默认没密码,要求输入密码直接按回车键), 如果XAMPP安装路径有变,请修改bat文件

4 可选步骤:升级HTTP到HTTPS,升级WS到WSS
	4.1 准备SSL证书,可以到腾讯云(https://console.cloud.tencent.com/ssl)或"Let's Encrypt"(https://letsencrypt.org/)申请免费的SSL证书或购买.我自己是腾讯云申请的免费SSL证书,下载Apache版证书和Nginx版证书备用.
	4.2 安装Nginx代理
		4.2.1 Nginx官网下载 http://nginx.org/en/download.html 一般我们下载里面的稳定版本,我们这里示范选nginx.Windows-1.22.1
		4.2.2 下载的回来的nginx-1.22.1.zip解压在C盘中
		4.2.3 复制"files/QuitNginx.bat" "files/RetartNginx.bat" "files/StartNginx.bat" "files/StopNginx.bat" 4个文件到Nginx目录下
		4.2.4 把"files/nginx.conf"替换"C:\nginx-1.22.1\conf\nginx.conf"
		4.2.5 把前面申请的Nginx版的证书解压到C:\nginx-1.22.1\conf\xxxx.com\目录内
			最后文件结构如下
			C:\nginx-1.22.1
				|
				|--conf
				|  |--xxxx.com				//Nginx版的证书
				|  |   |--xxxx.com_bundle.crt
				|  |   |--xxxx.com.key
				|  |
				|  |--nginx.conf			//配置文件
				|
				|--contrib
				|--docs
				|--html
				|--logs
				|--temp
				|--nginx.exe
				|
				|--QuitNginx.bat			//双击这个是退出Nginx
				|--RetartNginx.bat			//双击这个是重启Nginx,例如修改配置后用
				|--StartNginx.bat			//双击这个是启动Nginx,例如首次打开时候用
				|--StopNginx.bat			//双击这个是停止Nginx

		4.2.6 设置C:/nginx-1.22.1/conf/nginx.conf文件,把里面的xxxx.com替换成你实际域名,例如我自己的是csharplike.com替换xxxx.com
		4.2.7 如有需要修改nginx.conf里面的端口,里面设置WSS端口10000代理到WS端口9000,HTTPS端口10002代理到HTTP端口9002,如果有防火墙,请开启对于TCP端口(10000和10002)
		4.2.8 因为采用了WSS和HTTPS,请修改C:\KissServerFramework\KissServerFramework.json的网关端口:
			"serverInfos"节点内的"WebSocketURI": "wss://www.xxxx.com:10000"
			"serverInfos"节点内的"HttpURI": "https://www.xxxx.com:10002"
		4.2.9 最后记得执行StartNginx.bat启动Nginx,如果前面已经启动过可以执行RetartNginx.bat

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
	5.1 如果exe闪掉没开起了,可能是因为没有对应.NET运行时库,可以根据事件查看器的提示到微软官网下载运行时库.
	5.2 如果exe的console显示log仅为"KissFramework version : 1.0.x.x"表示端口已经被占用,可通过XAMPP的"NetStat"查看端口使用情况.

6 导出C#Like或C#LikeFree,这里选取WebGL平台
	6.1 创建一个空白的2D的Unity项目
	6.2 导入C#Like或C#LikeFree插件
	6.3 修改非热更代码的配置"Assets\C#Like\Runtime\Sample\SampleCSharpLike.cs"连接的域名和端口
		6.3.1 例如HTTP下:
			https://www.csharplike.com:10002/ReqGateway => http://127.0.0.1:9002/ReqGateway
		6.3.2 例如HTTPS下:
			https://www.csharplike.com:10002/ReqGateway => https://[Your domain]:[Your port]/ReqGateway
	6.4 在'Menu/Window/C#Like'打开C#Like设置面板,点"Rebuild Scripts"按钮,将把'Assets\C#Like\HotUpdateScripts'的脚本编译成二进制文件"Assets\StreamingAssets\output.bytes"
	6.5 可选步骤:把'Assets\C#Like\HotUpdateScripts'整个目录删掉(请自行备份哦),以验证我们的热更新脚本是真的可以热更新的!
	6.6 Unity编辑器内双击/Assets/C#Like/Scenes/SampleScene.unity打开演示场景
	6.7 在'Menu/File/Build Settings...'菜单打开'Build Settings'设置面板
		6.7.1 点击'Add Open Scenes',令'C#Like/Scenes/SampleScene'成为'Scenes In Build'里面首个场景(而非默认的空白场景'Scenes/SampleScene')
		6.7.2 选中"WebGL"后点击"Switch Platform"按钮切换当前Platform到WebGL Player Setting..."
		6.7.3 点击"Player Setting..."按钮,然后按需修改
			6.7.3.1 "Resolution and Presentation"
				"Default Canvas Width" 600
				"Default Canvas Height" 960
				"Run In Background" "v"
			6.7.3.2 "Other Settings"
				"Api Compatibility Level" ".NET Standard 2.0"
				"Strip Engine Code" "v"
				"Managed Stripping Level" "Medium"
			6.7.3.3 "Publishing Settings"
				"Enable Exceptions" "Explicitly Thrown Exceptions Only"
				"WebAssembly Arithmetic Exceptions" "Throw"
				"Compression Format" "Gzip"
				"Data Caching" "v"
				"Decompression Fallback" "v"
		6.7.4 点击"Build"按钮,导出最终的WebGL目录
			6.7.4.1 例如C#Like导出成CSharpLikeDemo
			6.7.4.2 例如C#LikeFree导出成CSharpLikeFreeDemo

7 把C#Like或C#LikeFree导出的WebGL放入到C:\xampp\htdocs目录内
	7.1 把导出的CSharpLikeDemo目录放入C:\xampp\htdocs目录内
		7.1.1 例如我自己的配置了SSL的可以通过 https://www.csharplike.com/CSharpLikeDemo/index.html 来访问
		7.1.2 例如如果本地没有配置SSL的可以通过 http://127.0.0.1/CSharpLikeDemo/index.html 来访问
	7.2 把导出的CSharpLikeFreeDemo目录放入C:\xampp\htdocs目录内
		7.2.1 例如我自己的配置了SSL的可以通过 https://www.csharplike.com/CSharpLikeFreeDemo/index.html 来访问
		7.2.2 例如如果本地没有配置SSL的可以通过 http://127.0.0.1/CSharpLikeFreeDemo/index.html 来访问
