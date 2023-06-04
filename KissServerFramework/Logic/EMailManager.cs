using CSharpLike;
using KissFramework;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace KissServerFramework
{
    public class EMailManager
    {
        class EMail
        {
            public EMail(int uid, string email, string token, ulong time)
            {
                this.uid = uid;
                this.email = email;
                this.token = token;
                this.time = time;
                emailsByUid[uid] = this;
                emailsByToken[token] = this;
            }
            public string token;
            public string email;
            public int uid;
            public ulong time;
            public void RaiseTimeout(bool immediately)
            {
                Framework.RaiseUniqueEvent(OnTimeout, "SendComfirmMail_"+uid, immediately ? 0f : 3600f, 1);
            }
            public void OnTimeout()
            {
                emailsByUid.Remove(uid);
                emailsByToken.Remove(token);
            }
        }
        static Dictionary<string, EMail> emailsByToken = new Dictionary<string, EMail>();
        static Dictionary<int, EMail> emailsByUid = new Dictionary<int, EMail>();
        static DateTime dtStart = new DateTime(1970, 1, 1, 8, 0, 0);
        public static void SendComfirmMail(int uid, string mailTo, string lang, Action<string> callback)
        {
            ulong time = (ulong)(DateTime.Now - dtStart).TotalSeconds;
            EMail email;
            if (emailsByUid.TryGetValue(uid, out email))
            {
                if (time - email.time < 30)
                {
                    callback("Send email rather frequently.");
                    return;
                }
                else
                {
                    email.time = time;
                    email.email = mailTo;
                    emailsByToken.Remove(email.token);
                    email.token = Guid.NewGuid().ToString("N");
                    emailsByToken[email.token] = email;
                }
            }
            else
            {
                email = new EMail(uid, mailTo, Guid.NewGuid().ToString("N"), time);
            }
            string url = $"{Framework.config.mailVerifyUrl}?token={HttpUtility.UrlEncode(email.token)}";
            SendMail(mailTo, string.Format(lang == "zh" ? Framework.config.mailVerifyContentZH : Framework.config.mailVerifyContent, url, url),
                lang == "zh" ? Framework.config.mailVerifyTitleZH : Framework.config.mailVerifyTitle, (error) =>
                {
                    email.RaiseTimeout(!string.IsNullOrEmpty(error));
                    callback(error);
                });
        }
        public static void VerifyComfirmEmail(string token, Action<int, string> callback)
        {
            if (emailsByToken.TryGetValue(token, out EMail email))
            {
                email.RaiseTimeout(true);
                callback(email.uid, email.email);
            }
            else
                callback(0, "");
        }
        public static void SendResetPasswordEmail(string email, string token, string name, string lang, Action<string> callback)
        {
            string url = $"{Framework.config.mailResetUrl}?name={HttpUtility.UrlEncode(name)}&token={HttpUtility.UrlEncode(token)}";
            if (lang == "zh")
                SendMail(email, string.Format(Framework.config.mailResetContentZH, url, url), Framework.config.mailResetTitleZH, callback);
            else
                SendMail(email, string.Format(Framework.config.mailResetContent, url, url), Framework.config.mailResetTitle, callback);
        }
        public static void SendMail(string to, string body, string subject, Action<string> callback)
        {
            string error = "";
            if (Framework.config.mailDontSend)
            {
                Logger.LogInfo($"SendMail: to={to}, body={body}, subject={subject}");
                callback(error);
                return;
            }
            new ThreadPoolEvent(() =>//This function run in thread. You can do some heavy work in thread here, such as IO operation.
            {
                try
                {
                    int nContain = 0;
                    string from = Framework.config.mailName;
                    MailMessage mailMsg = new MailMessage();
                    mailMsg.From = new MailAddress(from);
                    nContain += mailMsg.From.Address.Length;
                    mailMsg.To.Add(to);
                    nContain += mailMsg.To.ToString().Length;
                    mailMsg.Subject = subject;
                    mailMsg.SubjectEncoding = Encoding.UTF8;
                    nContain += mailMsg.Subject.Length;
                    mailMsg.Body = body;
                    mailMsg.BodyEncoding = Encoding.UTF8;
                    mailMsg.IsBodyHtml = true;
                    nContain += mailMsg.Body.Length;
                    if (mailMsg.IsBodyHtml == true)
                    {
                        nContain += 100;
                    }
                    SmtpClient client = new SmtpClient();
                    //client.UseDefaultCredentials = true;
                    client.Credentials = new System.Net.NetworkCredential(Framework.config.mailName, Framework.config.mailPassword);
                    client.Host = Framework.config.mailHost;
                    client.Port = 25;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    mailMsg.Priority = System.Net.Mail.MailPriority.Normal;
                    client.Send(mailMsg);
                }
                catch (Exception e)
                {
                    Logger.LogError($"{e.Message} {e.StackTrace}");
                    error = e.StackTrace;
                }
            },
            () =>//This function run in main thread. You can do some work after your work done.
            {
                callback(error);
            });
        }
    }
}
