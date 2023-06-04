
using CSharpLike;
using KissFramework;
using System.Collections.Generic;

namespace KissServerFramework
{
    /// <summary>
    /// The config load from JSON file(Environment.CurrentDirectory + "/" + System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".json").
    /// The field of ConfigBase is internal use by FrameworkBase.
    /// You can custom this class and modify the JSON file.
    /// Here are the sample read the 'welcomeMsg' of this class from JSON file:
    /// string msg = Framework.config.welcomeMsg;
    /// </summary>
    public class Config : ConfigBase
    {
        /// <summary>
        /// sample for config JSON : {"welcomeMsg":"Welcome [{0}] to chat room."}
        /// </summary>
        public string welcomeMsg = "Welcome [{0}] to chat room.";
        /// <summary>
        /// sample for config JSON : {"goodbyeMsg":"[{0}] leave the chat room."}
        /// </summary>
        public string goodbyeMsg = "[{0}] leave the chat room.";
        /// <summary>
        /// sample for config JSON : {"aircraftBattleRankCount":100}
        /// </summary>
        public int aircraftBattleRankCount = 100;
        /// <summary>
        /// sample for config JSON : {"sampleInt":10}
        /// </summary>
        public int sampleInt = 123;
        /// <summary>
        /// sample for config JSON : {"sampleStringList":["a","abc","cba"]}
        /// </summary>
        public List<string> sampleStringList = new List<string>();
        /// <summary>
        /// sample for config JSON : {"sampleIntList":[1,2,3]}
        /// </summary>
        public List<int> sampleIntList = new List<int>();
        /// <summary>
        /// sample for config JSON : {"sampleStringIntDictionary":{"aa":1.2,"bb":3.2}}
        /// </summary>
        public Dictionary<string, float> sampleStringIntDictionary = new Dictionary<string, float>();
        /// <summary>
        /// sample for config JSON : {"sampleStringStringDictionary":{"aa":"xyz","bb":"abc"}}
        /// </summary>
        public Dictionary<string, string> sampleStringStringDictionary = new Dictionary<string, string>();
        /// <summary>
        /// How many seconds the account data keep in cache after account offline.
        /// </summary>
        public int accountCacheTime = 60;
        /// <summary>
        /// Server infomation for gateway
        /// </summary>
        public List<GatewayServerInfo> serverInfos = new List<GatewayServerInfo>();
        /// <summary>
        /// Hot udpate scripte binary file URI for C#Like full version
        /// </summary>
        public string hotUpdateScriptFile = "";
        /// <summary>
        /// Hot udpate scripte binary file URI for C#Like free version
        /// </summary>
        public string hotUpdateScriptFileFree = "";
        /// <summary>
        /// Token expire time, in hours.
        /// </summary>
        public int tokenExpireTime = 24;//1 day.
        /// <summary>
        /// MD5 hash salt
        /// </summary>
        public string passwordHashSalt = "K/SdftgfDJf~wusa#d(djk8@Mdm8_xT,SFwq]DSF!DFdwWqSFmbvIlO8^SdGsfg2{XC3@dfHL5Xs|dk;DS&%FS*FVrgd6fdg3";
        /// <summary>
        /// Mail don't send, just print log.
        /// </summary>
        public bool mailDontSend = true;
        /// <summary>
        /// Mail host
        /// </summary>
        public string mailHost = "smtp.163.com";
        /// <summary>
        /// Mail name
        /// </summary>
        public string mailName = "teachmeplay@163.com";
        /// <summary>
        /// Mail password
        /// </summary>
        public string mailPassword = "DLYYSGIUSCRZLPCG";
        /// <summary>
        /// Verify mail url
        /// </summary>
        public string mailVerifyUrl = "http://127.0.0.1:9002/confirm.html";
        /// <summary>
        /// Verify mail title
        /// </summary>
        public string mailVerifyTitle = "Email verification";
        /// <summary>
        /// Verify mail title(zh for chinese)
        /// </summary>
        public string mailVerifyTitleZH = "验证Email";
        /// <summary>
        /// Verify mail content(zh for chinese)
        /// </summary>
        public string mailVerifyContent = "<div><div style='margin-left:4%%;'><p>Hello：</p><p style='text-indent: 2em;'>You are currently binding the email function. Please click on the link below to complete the email binding.</p><p style='text-indent: 2em;display: block;word-break: break-all;'>Link Address：<a style='text-decoration: none;' href='{0}'>{1}</a></p><ul style='color: rgb(169, 169, 189);font-size: 18px;'><li>To ensure the security of your account, this link is valid for 1 hours.</li><li>If the link cannot be clicked, please directly copy the above URL to the browser address bar to access it.</li><li>There is no need to reply to this email.</li></ul></div>";
        /// <summary>
        /// Verify mail content
        /// </summary>
        public string mailVerifyContentZH = "<div><div style='margin-left:4%%;'><p>您好：</p><p style='text-indent: 2em;'>您正在使用绑定密保邮件功能. 请点下面链接完成密保邮箱的绑定.</p><p style='text-indent: 2em;display: block;word-break: break-all;'>链接地址：<a style='text-decoration: none;' href='{0}'>{1}</a></p><ul style='color: rgb(169, 169, 189);font-size: 18px;'><li>为了保证您的账号安全,该链接仅1小时内有效.</li><li>如果无法点击该链接,请复制上面的URL链接到浏览器里浏览.</li><li>本邮件请勿回复.</li></ul></div>";
        /// <summary>
        /// Reset password mail title
        /// </summary>
        public string mailResetTitle = "Reset password";
        /// <summary>
        /// Reset password mail title(zh for chinese)
        /// </summary>
        public string mailResetTitleZH = "重设密码";
        /// <summary>
        /// Verify mail url
        /// </summary>
        public string mailResetUrl = "http://127.0.0.1:9002/reset.html";
        /// <summary>
        /// Reset password mail content
        /// </summary>
        public string mailResetContent = "<div><div style='margin-left:4%%;'><p>Hello：</p><p style='text-indent: 2em;'>You are currently resetting password by email. Please click on the link below to complete the password resetting.</p><p style='text-indent: 2em;display: block;word-break: break-all;'>Link Address：<a style='text-decoration: none;' href='{0}'>{1}</a></p><ul style='color: rgb(169, 169, 189);font-size: 18px;'><li>To ensure the security of your account, this link is valid for 10 minutes.</li><li>If the link cannot be clicked, please directly copy the above URL to the browser address bar to access it.</li><li>There is no need to reply to this email.</li></ul></div>";
        /// <summary>
        /// Reset password mail content(zh for chinese)
        /// </summary>
        public string mailResetContentZH = "<div><div style='margin-left:4%%;'><p>您好：</p><p style='text-indent: 2em;'>您正在使用密保邮箱重置密码功能. 请点下面链接完成修改密码操作.</p><p style='text-indent: 2em;display: block;word-break: break-all;'>Link Address：<a style='text-decoration: none;' href='{0}'>{1}</a></p><ul style='color: rgb(169, 169, 189);font-size: 18px;'><li>为了保证你的账号安全,该链接仅在10分钟内有效.</li><li>如果无法点击该链接,请复制上面的URL链接到浏览器里浏览.</li><li>本邮件请勿回复.</li></ul></div>";
    }
}
